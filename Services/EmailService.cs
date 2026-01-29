using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MailKit.Security;
using MimeKit;
public enum MyFolder
{
    Inbox,
    Sent,
    All,
    Drafts,
    Trash,
    Junk
}


namespace TeamProject.Services
{
    public class EmailService
    {
        private readonly string _email;
        private readonly string _password;
        private readonly string _imapServer;
        private readonly int _imapPort;
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        public class EmailWithUid
        {
            public UniqueId Uid { get; set; }
            public MimeMessage Message { get; set; } = null!;
        }

        public EmailService(IConfiguration config)
        {
            _email = config["EmailSettings:Email"]
                ?? throw new Exception("Email is missing from configuration");

            _password = config["EmailSettings:Password"]
                ?? throw new Exception("Password is missing from configuration");

            _imapServer = config["EmailSettings:ImapServer"] ?? "imap.gmail.com";
            _imapPort = int.Parse(config["EmailSettings:ImapPort"] ?? "993");
            _smtpServer = config["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(config["EmailSettings:SmtpPort"] ?? "587");
        }

        public EmailService(string email, string password,
                        string imapServer = "imap.gmail.com", int imapPort = 993,
                        string smtpServer = "smtp.gmail.com", int smtpPort = 587)
        {
            _email = email;
            _password = password;
            _imapServer = imapServer;
            _imapPort = imapPort;
            _smtpServer = smtpServer;
            _smtpPort = smtpPort;
        }

        private async Task<IMailFolder> GetFolderAsync(ImapClient client, MyFolder folderType)
        {
            return folderType switch
            {
                MyFolder.Inbox => client.Inbox,
                MyFolder.Sent => await client.GetFolderAsync("[Gmail]/Sent Mail"),
                MyFolder.All => await client.GetFolderAsync("[Gmail]/All Mail"),
                MyFolder.Drafts => await client.GetFolderAsync("[Gmail]/Drafts"),
                MyFolder.Trash => await client.GetFolderAsync("[Gmail]/Trash"),
                MyFolder.Junk => await client.GetFolderAsync("[Gmail]/Spam"),
                _ => client.Inbox
            };
        }


        // ================= FETCH MESSAGES =================
        public async Task<List<MimeMessage>> GetMessagesAsync(MyFolder folderType, int maxCount = 50)
        {
            using var client = new ImapClient();
            await client.ConnectAsync(_imapServer, _imapPort, SecureSocketOptions.SslOnConnect);
            await client.AuthenticateAsync(_email, _password);

            var folder = await GetFolderAsync(client, folderType);
            await folder.OpenAsync(FolderAccess.ReadOnly);

            var messages = new List<MimeMessage>();
            int start = Math.Max(0, folder.Count - maxCount);

            for (int i = start; i < folder.Count; i++)
                messages.Add(await folder.GetMessageAsync(i));

            await client.DisconnectAsync(true);
            return messages;
        }


        // ================= SEND EMAIL =================
        public async Task SendEmailAsync(string fromEmail, string appPassword, string to, string subject, string body, bool isHtml, string? fromName = null)
        {
            var message = new MimeMessage();

            // Use display name if provided, otherwise default to email
            var senderName = string.IsNullOrWhiteSpace(fromName) ? fromEmail : fromName;
            message.From.Add(new MailboxAddress(senderName, fromEmail));

            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            message.Body = isHtml
                ? new TextPart("html") { Text = body }
                : new TextPart("plain") { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(fromEmail, appPassword);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }

        public async Task<int> GetUnreadCountAsync(MyFolder folderType = MyFolder.Inbox)
        {
            using var client = new ImapClient();
            await client.ConnectAsync(_imapServer, _imapPort, SecureSocketOptions.SslOnConnect);
            await client.AuthenticateAsync(_email, _password);

            var folder = await GetFolderAsync(client, folderType);
            await folder.OpenAsync(FolderAccess.ReadOnly);

            // Search for unread messages
            var unreadUids = await folder.SearchAsync(SearchQuery.NotSeen);

            await client.DisconnectAsync(true);
            return unreadUids.Count;
        }

        public async Task<List<MimeMessage>> GetInboxAsync(int maxCount = 50)
        {
            using var client = new ImapClient();
            try
            {
                Console.WriteLine($"Connecting to {_imapServer}:{_imapPort} as {_email}");
                await client.ConnectAsync(_imapServer, _imapPort, SecureSocketOptions.SslOnConnect);

                await client.AuthenticateAsync(_email, _password);
                Console.WriteLine("Authenticated successfully!");

                var inbox = client.Inbox;
                await inbox.OpenAsync(FolderAccess.ReadOnly);

                Console.WriteLine($"Inbox has {inbox.Count} messages.");

                var messages = new List<MimeMessage>();
                int start = Math.Max(0, inbox.Count - maxCount);

                for (int i = start; i < inbox.Count; i++)
                {
                    var message = await inbox.GetMessageAsync(i);
                    messages.Add(message);
                    Console.WriteLine($"Fetched: {message.Subject} from {message.From}");
                }

                await client.DisconnectAsync(true);
                return messages;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to fetch inbox: {ex.Message}");
                throw;
            }
        }
        public async Task<List<EmailWithUid>> GetInboxBatchAsync(int skip, int take)
        {
            using var client = new ImapClient();
            await client.ConnectAsync(_imapServer, _imapPort, MailKit.Security.SecureSocketOptions.SslOnConnect);
            await client.AuthenticateAsync(_email, _password);

            var inbox = client.Inbox;
            await inbox.OpenAsync(FolderAccess.ReadOnly);

            // Get all UIDs in the folder
            var uids = await inbox.SearchAsync(MailKit.Search.SearchQuery.All);

            // Order descending (latest first)
            var orderedUids = uids.OrderByDescending(u => u.Id).ToList();

            // Apply skip/take
            var batchUids = orderedUids.Skip(skip).Take(take).ToList();

            var messages = new List<EmailWithUid>();
            foreach (var uid in batchUids)
            {
                var message = await inbox.GetMessageAsync(uid);
                messages.Add(new EmailWithUid
                {
                    Uid = uid,
                    Message = message
                });
            }

            await client.DisconnectAsync(true);
            return messages;
        }

        public async Task<MimeMessage?> GetEmailByUidAsync(string uid)
        {
            if (string.IsNullOrEmpty(uid))
                return null;

            using var client = new ImapClient();

            try
            {
                // Connect to IMAP server with SSL (true)
                await client.ConnectAsync(_imapServer, _imapPort, true);

                // Authenticate
                await client.AuthenticateAsync(_email, _password);

                var inbox = client.Inbox;
                await inbox.OpenAsync(FolderAccess.ReadOnly);

                // Convert string UID to UniqueId
                if (!UniqueId.TryParse(uid, out UniqueId messageUid))
                    return null;

                // Get the email by UID
                var message = await inbox.GetMessageAsync(messageUid);

                return message;
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}
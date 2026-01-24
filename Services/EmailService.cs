using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
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
        public async Task<List<MimeMessage>> GetInboxAsync(int maxCount = 50)
        {
            return await GetMessagesAsync(MyFolder.Inbox, maxCount);
        }


        // ================= SEND EMAIL =================
        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_email));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            message.Body = isHtml
                ? new TextPart("html") { Text = body }
                : new TextPart("plain") { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_email, _password);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }
    }
}

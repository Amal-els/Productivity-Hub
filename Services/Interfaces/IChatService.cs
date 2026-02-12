namespace TeamProject.Services.Interfaces
{
    public interface IChatService
    {
        Task<string> SendMessageAsync(string message);
    }
}
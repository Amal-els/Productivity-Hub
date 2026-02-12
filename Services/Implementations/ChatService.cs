using System.Net.Http.Json;
using TeamProject.Models;
using TeamProject.Services.Interfaces;
using TeamProject.Models.Dtos;
namespace TeamProject.Services.Implementations
{
    public class ChatService : IChatService
    {
        private readonly HttpClient _httpClient;

        public ChatService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> SendMessageAsync(string message)
        {
            var request = new ChatRequest { Message = message };
            var response = await _httpClient.PostAsJsonAsync("/chat", request);

            if (!response.IsSuccessStatusCode)
                return "Error connecting to chatbot.";

            var chatResponse = await response.Content.ReadFromJsonAsync<ChatResponses>();
            return chatResponse?.Reply ?? "No reply.";
        }
    }
}
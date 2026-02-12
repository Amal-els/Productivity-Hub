using Microsoft.AspNetCore.Mvc;
using TeamProject.Services.Implementations;
using TeamProject.Models;
namespace TeamProject.Controllers
{
    public class ChatController : Controller
    {
        private readonly ChatService _chatService;

        public ChatController(ChatService chatService)
        {
            _chatService = chatService;
        }

        public IActionResult Index()
        {    ViewData["CurrentTab"] = "Assistant";
            return View();

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
                return Json(new { reply = "Please type a message." });

            var reply = await _chatService.SendMessageAsync(request.Message);
            return Json(new { reply });
        }

    }
}
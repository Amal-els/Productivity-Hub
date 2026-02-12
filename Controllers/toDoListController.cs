using Microsoft.AspNetCore.Mvc;
using TeamProject.Services.Interfaces;
using TeamProject.Models;
using System.Threading.Tasks;

namespace TeamProject.Controllers
{
    public class toDoListController : Controller
    {
        private readonly ItoDoListService _toDoListService;

        public toDoListController(ItoDoListService toDoListService)
        {
            _toDoListService = toDoListService;
        }

        public async Task<IActionResult> getList(string filter )
        {
            ViewData["CurrentTab"] = "ToDoList";
            var userId = "TEMP_USER_ID"; // later: get from Identity
            var vm = await _toDoListService.GetToDoViewModelForUser(userId, filter);
            return View(vm);
        }
    }
}
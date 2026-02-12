using Microsoft.AspNetCore.Mvc;
using TeamProject.Services.Interfaces;
using TeamProject.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace TeamProject.Controllers
{
    public class toDoListController : Controller
    {
        private readonly ItoDoListService _toDoListService;
        private readonly UserManager<ApplicationUser> _userManager;

        public toDoListController(ItoDoListService toDoListService, UserManager<ApplicationUser> userManager)
        {
            _toDoListService = toDoListService;
            _userManager = userManager;
        }

        public async Task<IActionResult> getList(string filter )
        {
            ViewData["CurrentTab"] = "ToDoList";
            var userId = _userManager.GetUserId(User); 
            var vm = await _toDoListService.GetToDoViewModelForUser(userId, filter);
            return View(vm);
        }
    }
}
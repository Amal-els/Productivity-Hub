using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TeamProject.Data;
using TeamProject.Models;

using TeamProject.Services.Interfaces;

namespace TeamProject.Controllers
{
    public class toDoTaskController : Controller
    {
        private readonly ItoDoTaskService _taskService;

        public toDoTaskController(ItoDoTaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public IActionResult Create()
        {
           
            var model = new toDoTask
            {
                Id = Guid.Empty,
                Priority = Priority.medium,
                dateOfCreation = DateTime.Now
            };
            return View(model); // ← Pass the model!
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // ← Add this back for security
        public async Task<IActionResult> Create(toDoTask task)
        {
            var userId = "TEMP_USER_ID"; // later: Identity

            await _taskService.AddTask(userId, task);

            return RedirectToAction("getList", "ToDoList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // ← Use this instead of IgnoreAntiforgeryToken
        public async Task<IActionResult> Delete(Guid taskId)
        {
            Console.WriteLine($"[CONTROLLER] Delete called with taskId: {taskId}");

            try
            {
                await _taskService.deleteTask(taskId);
                Console.WriteLine($"[CONTROLLER] Service completed successfully");

                TempData["Success"] = "Task deleted successfully!";
                return RedirectToAction("getList", "ToDoList");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CONTROLLER] ERROR: {ex.Message}");
                Console.WriteLine($"[CONTROLLER] Stack trace: {ex.StackTrace}");

                TempData["Error"] = $"Failed to delete: {ex.Message}";
                return RedirectToAction("getList", "ToDoList");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid taskId)
        {
            Console.WriteLine("AAAAAABBBBBB");
            var task = await _taskService.GetTaskById(taskId);
            if (task == null)
            {
                return NotFound();
            }
            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(toDoTask task)
        {
            if (!ModelState.IsValid)
            {
                foreach (var entry in ModelState)
                {
                    var fieldName = entry.Key;

                    foreach (var error in entry.Value.Errors)
                    {
                        var errorMessage = error.ErrorMessage;
                        var exception = error.Exception;

                        // Set a breakpoint here or log it
                        Console.WriteLine($"Field: {fieldName}, Error: {errorMessage}");
                    }
                }
            }
            if (ModelState.IsValid)
            {
                try
                { Console.WriteLine("Le contrôleur est bien atteint !");
                    await _taskService.UpdateTask(task);
                    // On redirige vers getList de la ToDoList parente après succès
                    return RedirectToAction("getList", "toDoList");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erreur lors de la mise à jour : " + ex.Message);
                }
            } else {
                    
            }

            // Si on arrive ici, c'est qu'il y a une erreur, on réaffiche le formulaire
            return View(task);
        }
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> ToggleComplete(Guid taskId)
{
    var task = await _taskService.GetTaskById(taskId);

    if (task == null)
        return NotFound();

    // 🔁 Toggle achieved state
    task.Acheived = !task.Acheived;

    await _taskService.UpdateTask(task);

    return RedirectToAction("getList", "toDoList");
}
        


}


        }
    

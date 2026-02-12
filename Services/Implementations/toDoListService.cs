using Microsoft.EntityFrameworkCore;
using TeamProject.Data;
using TeamProject.Models;
using TeamProject.Services.Interfaces;
namespace TeamProject.Services.Implementations;

public class toDoListService : ItoDoListService
{
    private readonly ApplicationDbContext _context;

    public toDoListService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ToDoViewModel> GetToDoViewModelForUser(string userId, string filter)
    {
        // Get the user's list (create if not exists)
        var list = await _context.todolist
            .Include(l => l.Tasks)
            .FirstOrDefaultAsync(l => l.UserId == userId);

        if (list == null)
        {
            list = new toDoList { UserId = userId, Name = "My Tasks" };
            _context.todolist.Add(list);
            await _context.SaveChangesAsync();
        }

        // Filter tasks
        var tasks = list.Tasks.AsQueryable();

        if (filter == "active")
            tasks = tasks.Where(t => t.Acheived != true);
        else if (filter == "completed")
            tasks = tasks.Where(t => t.Acheived == true);

        // Map to ViewModel
        var vm = new ToDoViewModel
        {
            AllLists = new List<toDoList> { list },
            AllTasks = tasks.OrderByDescending(t => t.dateOfCreation).ToList()
        };

        return vm;
    }
}

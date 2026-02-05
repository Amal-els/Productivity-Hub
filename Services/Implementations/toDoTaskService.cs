using TeamProject.Services.Interfaces;
using TeamProject.Data;
using TeamProject.Models;
using Microsoft.EntityFrameworkCore;

namespace TeamProject.Services.Implementations
{
    public class toDoTaskService : ItoDoTaskService
    {
        private readonly ApplicationDbContext _context;

        public toDoTaskService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddTask(string userId, toDoTask task)
        {
            var list = await _context.todolist
                .Include(l => l.Tasks)
                .FirstOrDefaultAsync(l => l.UserId == userId);

            if (list == null)
            {
                list = new toDoList { UserId = userId };
                _context.todolist.Add(list);
            }

            task.ToDoList = list;
            task.dateOfCreation = DateTime.Now;
            task.Acheived = false;

            _context.Task.Add(task);
            await _context.SaveChangesAsync();
        }

        public async Task deleteTask(Guid taskId)
        {

            Console.WriteLine($"[SERVICE] Starting delete for taskId: {taskId}");

            var task = await _context.Task.FindAsync(taskId);

            if (task != null)
            {
                Console.WriteLine($"[SERVICE] Task found: {task.Name ?? "unnamed"}");
                _context.Task.Remove(task);

                var changes = await _context.SaveChangesAsync();
                Console.WriteLine($"[SERVICE] SaveChanges returned: {changes} rows affected");
            }
            else
            {
                Console.WriteLine($"[SERVICE] Task NOT FOUND with ID: {taskId}");
            }
        }

        public async Task<toDoTask?> GetTaskById(Guid taskId)
        {
            return await _context.Task
                .Include(t => t.ToDoList)
                .FirstOrDefaultAsync(t => t.Id == taskId);
        }
        public async Task UpdateTask(toDoTask task)
        {
            _context.Attach(task);
            _context.Entry(task).State = EntityState.Modified;
        
            // On évite de modifier la date de création lors d'un Edit
            _context.Entry(task).Property(x => x.dateOfCreation).IsModified = false;
            _context.Entry(task).Property(x => x.ToDoListId).IsModified = false;

            await _context.SaveChangesAsync();
        }
    public async Task ToggleTaskStatusAsync(Guid id)
{
    var task = await _context.Task.FindAsync(id);
       
    
    if (task != null)
    {   task.Id=task.Id;
        task.Name=task.Name;
        task.dateOfCompletion=task.dateOfCompletion;
        task.dateOfCreation=task.dateOfCreation;
        task.Acheived = !task.Acheived;
        _context.Task.Update(task);
        await _context.SaveChangesAsync();
}}

public async Task<ToDoViewModel> GetCompletedTasks(string userId)
{
    var list = await _context.todolist
        .Include(l => l.Tasks)
        .FirstOrDefaultAsync(l => l.UserId == userId);

    var viewModel = new ToDoViewModel();

    if (list != null)
    {
        // Filtrer seulement les tâches complétées
        viewModel.AllTasks = list.Tasks.Where(t => t.Acheived == true).ToList();
    }

    return viewModel;
}

public async Task<ToDoViewModel> GetActiveTasks(string userId)
{
    var list = await _context.todolist
        .Include(l => l.Tasks)
        .FirstOrDefaultAsync(l => l.UserId == userId);

    var viewModel = new ToDoViewModel();

    if (list != null)
    {
        // Filtrer seulement les tâches actives (non complétées)
        viewModel.AllTasks = list.Tasks.Where(t => t.Acheived != true).ToList();
    }

    return viewModel;
}
    }   
}


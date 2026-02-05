using TeamProject.Models;
namespace TeamProject.Services.Interfaces;

public interface ItoDoTaskService
{
    Task AddTask(string userId, toDoTask task);
    Task deleteTask(Guid taskId);
   
    Task<toDoTask?> GetTaskById(Guid taskId);
    Task UpdateTask(toDoTask task);
    Task ToggleTaskStatusAsync(Guid id);
    Task<ToDoViewModel> GetCompletedTasks(string userId);
Task<ToDoViewModel> GetActiveTasks(string userId);
}
using TeamProject.Models;


namespace TeamProject.Services.Interfaces;

public interface ItoDoListService
{
    Task<ToDoViewModel> GetToDoViewModelForUser(string userId, string filter = "all");
  
}
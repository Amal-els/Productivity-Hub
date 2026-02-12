
namespace TeamProject.Models
{
    public class ToDoViewModel
    {
        public List<toDoList> AllLists { get; set; } = new();
        public List<toDoTask> AllTasks { get; set; } = new(); // Pour le tableau "All"
        
        // Stats pour les boîtes de résumé
        public int TotalCount => AllTasks.Count;
        public int ActiveCount => AllTasks.Count(t => t.Acheived != true);
        public int CompletedCount => AllTasks.Count(t => t.Acheived == true);
    }

}
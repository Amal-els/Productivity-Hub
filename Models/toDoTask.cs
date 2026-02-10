namespace TeamProject.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class toDoTask
{   
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime dateOfCreation{ get; set; }
    public DateTime? dateOfCompletion { get; set; }
    public bool? Acheived { get; set; }
    public Guid ToDoListId { get; set; }

    // Navigation
    public toDoList? ToDoList { get; set; }
    public Priority Priority { get; set; }
    
}
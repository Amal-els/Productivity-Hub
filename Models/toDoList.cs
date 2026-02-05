namespace TeamProject.Models;

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class toDoList
{   
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    // Foreign key to IdentityUser
    public string UserId { get; set; }   
    public ApplicationUser User { get; set; }

    // Collection of tasks
    public List<toDoTask> Tasks { get; set; } = new List<toDoTask>();

    // Optional: a title or name for the ToDoList
    public string Name { get; set; }

  
}

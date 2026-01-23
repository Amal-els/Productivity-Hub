namespace TeamProject.Models;

public class Activity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime dateOfCreation{ get; set; }
    public DateTime? dateOfCompletion { get; set; }
    public String? Acheived { get; set; }
}
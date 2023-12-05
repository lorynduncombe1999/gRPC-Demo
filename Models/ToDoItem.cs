namespace gRPC.Models;

/// <summary>
/// This class defines the properties of a ToDo Item
/// </summary>
public class ToDoItem
{
    
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string ToDoStatus { get; set; } = "NEW";
}
using gRPC.Models;
using Microsoft.EntityFrameworkCore;

namespace gRPC.Data;

/// <summary>
/// This class acts as the Db Context for this demo
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Empty constructor that allows for the implementation of DB context
    /// </summary>
    /// <param name="options"></param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    /// <summary>
    /// Constructor for unit testing
    /// </summary>

    public AppDbContext()
    {
        
    }

    public virtual DbSet<ToDoItem> ToDoItems => Set<ToDoItem>();
}
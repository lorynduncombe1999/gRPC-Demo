using Grpc.Core;
using gRPC.Data;
using gRPC.Models;
using Microsoft.EntityFrameworkCore;

namespace gRPC.Services;

public class ToDoService : ToDoIt.ToDoItBase
{
    /// <summary>
    /// instance of dbContext which is implemented via dependency injection
    /// </summary>
    private readonly AppDbContext _dbContext;

    /// <summary>
    /// Class constructor
    /// </summary>
    /// <param name="dbContext">dbContext</param>
    public ToDoService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Creates a to do item
    /// </summary>
    /// <param name="request">Type-CreateToDoRequest</param>
    /// <param name="context">behaves similarly to a cancellation token within Rest API</param>
    /// <returns>Task(CreateToDoResponse)</returns>
    /// <exception cref="RpcException">catches invalid object</exception>
    public override async Task<CreateToDoResponse> CreateToDo(CreateToDoRequest request, ServerCallContext context)
    {
        if (request.Title == string.Empty || request.Description == string.Empty)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply a valid object"));

        var toDoItem = new ToDoItem()
        {
            Title = request.Title,
            Description = request.Description
        };

        await _dbContext.AddAsync(toDoItem);
        await _dbContext.SaveChangesAsync();
        return await Task.FromResult(new CreateToDoResponse
        {
            Id = toDoItem.Id
        });
    }

    /// <summary>
    /// Reads a singular todo item (find by id)
    /// </summary>
    /// <param name="request">readToDoRequest</param>
    /// <param name="context">ServerCallContext</param>
    /// <returns>ReadToDoResponse</returns>
    /// <exception cref="RpcException">Id not found</exception>
    public override async Task<ReadToDoResponse> ReadToDo(ReadToDoRequest request, ServerCallContext context)
    {
        if (request.Id <= 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Resource index must be greater than 0"));
        var toDoItem = await _dbContext.ToDoItems.FirstOrDefaultAsync(t => t.Id == request.Id);

        if (toDoItem != null)
        {
            return await Task.FromResult(new ReadToDoResponse()
            {
                Id = toDoItem.Id,
                Title = toDoItem.Title,
                Description = toDoItem.Description,
                ToDoStatus = toDoItem.ToDoStatus
            });
        }

        throw new RpcException(new Status(StatusCode.NotFound, $"No Task with id {request.Id}"));
    }

    /// <summary>
    /// Retrieves a list of ALL todo list items
    /// </summary>
    /// <param name="request">GetAllRequest</param>
    /// <param name="context">ServerCallContext</param>
    /// <returns>Task(GetAllResponse)</returns>
    public override async Task<GetAllResponse> ListToDo(GetAllRequest request, ServerCallContext context)
    {
        var response = new GetAllResponse();
        var toDoItems = await _dbContext.ToDoItems.ToListAsync();

        foreach (var toDo in toDoItems)
        {
            response.ToDo.Add(new ReadToDoResponse()
            {
                Id = toDo.Id,
                Title = toDo.Title,
                Description = toDo.Description,
                ToDoStatus = toDo.ToDoStatus
            });
        }

        return await Task.FromResult(response);
    }

    /// <summary>
    /// Updates an entire object(put)
    /// </summary>
    /// <param name="request">UpdateToDoRequest</param>
    /// <param name="context">ServerCallContext</param>
    /// <returns>Task(UpdateToDoResponse)</returns>
    /// <exception cref="RpcException">request doesnt exist/not found</exception>
    public override async Task<UpdateToDoResponse> UpdateToDo(UpdateToDoRequest request, ServerCallContext context)
    {
        if (request.Id <= 0 || request.Title == string.Empty || request.Description == string.Empty)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply a valid object"));

        var toDoItem = await _dbContext.ToDoItems.FirstOrDefaultAsync(t => t.Id == request.Id);
        if (toDoItem == null)
            throw new RpcException(new Status(StatusCode.NotFound, $"$No Task with Id {request.Id}"));
        toDoItem.Title = request.Title;
        toDoItem.Description = request.Description;
        toDoItem.ToDoStatus = request.ToDoStatus;

        await _dbContext.SaveChangesAsync();

        return await Task.FromResult(new UpdateToDoResponse()
        {
            Id = toDoItem.Id
        });
    }

    /// <summary>
    /// Deletes todo item
    /// </summary>
    /// <param name="request">DeleteToDoRequest</param>
    /// <param name="context">ServerCallContext</param>
    /// <returns>Task(DeleteToDoResponse)-the id of the deleted item</returns>
    /// <exception cref="RpcException"></exception>
    public override async Task<DeleteToDoResponse> DeleteToDo(DeleteToDoRequest request, ServerCallContext context)
    {
        if (request.Id <= 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Resource index must be greater than 0"));
        var toDoItem = await _dbContext.ToDoItems.FirstOrDefaultAsync(t => t.Id == request.Id);

        if (toDoItem == null) throw new RpcException(new Status(StatusCode.NotFound, $"No Task with id {request.Id}"));
        _dbContext.Remove(toDoItem);
        await _dbContext.SaveChangesAsync();


        return await Task.FromResult(new DeleteToDoResponse()
        {
            Id = toDoItem.Id
        });
    }
}

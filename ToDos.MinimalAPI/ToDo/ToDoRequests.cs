using Microsoft.AspNetCore.Authorization;

namespace ToDos.MinimalAPI;

public static class ToDoRequests
{
    public static WebApplication RegisterEndpoints(this WebApplication app)
    {
        app.MapGet("/todos", ToDoRequests.GetAll)
            .Produces<List<ToDo>>()
            .WithTags("To Dos")
            .RequireAuthorization();   
        
        app.MapGet("/todos/{id}", ToDoRequests.GetById)
            .Produces<ToDo>()
            .Produces(StatusCodes.Status404NotFound)
            .WithTags("To Dos")
            .AllowAnonymous();

        app.MapPost("/todos", ToDoRequests.Create)
            .Produces<ToDo>(StatusCodes.Status201Created)
            .Accepts<ToDo>("application/json")
            .WithTags("To Dos")
            .WithValidator<ToDo>();

        app.MapPut("/todos/{id}", ToDoRequests.Update)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Accepts<ToDo>("application/json")
            .WithTags("To Dos")
            .WithValidator<ToDo>();

        app.MapDelete("/todos/{id}", ToDoRequests.Delete)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithTags("To Dos")
            .ExcludeFromDescription();

        return app;
    }

    public static IResult GetAll(IToDoService service)
    {
        var todos = service.GetAll();
        return Results.Ok(todos);
    }

    public static IResult GetById(IToDoService service, Guid id)
    {
        var todo = service.GetById(id);
        if (todo is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(todo);
    }

    [Authorize] //same authorization as in endpoint definition but it is a second way to done this.
    public static IResult Create(IToDoService service,ToDo toDo)
    {
        service.Create(toDo);
        
        return Results.Created($"/todos/{toDo.Id}", toDo);
    }

    public static IResult Update(IToDoService service, Guid id, ToDo toDo)
    {
        var todo = service.GetById(id);
        if (todo is null)
        {
            return Results.NotFound();
        }

        service.Update(toDo);

        return Results.NoContent();
    }

    public static IResult Delete(IToDoService service, Guid id)
    {
        var todo = service.GetById(id);
        if (todo is null)
        {
            return Results.NotFound();
        }

        service.Delete(id); 
        
        return Results.NoContent();
    }
}

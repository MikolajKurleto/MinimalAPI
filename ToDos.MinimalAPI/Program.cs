using Microsoft.AspNetCore.Mvc;
using ToDos.MinimalAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IToDoService, ToDoService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/todos", (IToDoService service) => service.GetAll());
app.MapGet("/todos/{id}", ([FromServices]IToDoService service, [FromRoute]Guid id) => service.GetById(id));
app.MapPost("/todos", (IToDoService service, [FromBody]ToDo toDo) => service.Create(toDo));
app.MapPut("/todos/{id}", (IToDoService service, Guid id, ToDo toDo) => service.Update(toDo));
app.MapDelete("/todos/{id}", (IToDoService service, Guid id) => service.Delete(id));

app.Run();

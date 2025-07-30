using ToDoList.Core.Entities;
using ToDoList.Application.DTOs;

namespace ToDoList.Application;

public interface IToDoService
{
    IEnumerable<ToDoItem> GetAll();
    ToDoItem? Get(int id);
    ToDoItem Add(ToDoItemDto item);
    ToDoItem? Update(ToDoItemDto item);
    void Delete(int id);
}
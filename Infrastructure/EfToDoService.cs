using Microsoft.EntityFrameworkCore;
using ToDoList.Application;
using ToDoList.Application.DTOs;
using ToDoList.Core.Entities;
using ToDoList.Infrastructure.Data;

public class EfToDoService : IToDoService
{
    private readonly AppDbContext _context;

    public EfToDoService(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<ToDoItem> GetAll()
    {
        return _context.ToDoItems.Include(t => t.User).ToList();
    }

    public ToDoItem? Get(int id)
    {
        return _context.ToDoItems.Include(t => t.User).FirstOrDefault(t => t.Id == id);
    }

    public ToDoItem Add(ToDoItemDto item)
    {
        var toDo = new ToDoItem
        {
            Title = item.Title,
            Description = item.Description,
            IsCompleted = item.IsCompleted,
            DueDate = item.DueDate,
            UserId = item.UserId,
        };

        _context.ToDoItems.Add(toDo);
        _context.SaveChanges();

        return toDo;
    }

    public ToDoItem Update(ToDoItemDto item)
    {
        var existing = _context.ToDoItems.FirstOrDefault(t => t.Id == item.Id);

        if (existing == null)
            return null;

        existing.Title = item.Title;
        existing.Description = item.Description;
        existing.IsCompleted = item.IsCompleted;
        existing.DueDate = item.DueDate;
        existing.UserId = item.UserId;
        existing.UpdateAt = DateTime.UtcNow;
      
        _context.SaveChanges();

        return existing;
    }

    public void Delete(int id)
    {
        var item = _context.ToDoItems.Find(id);
        if (item != null)
        {
            _context.ToDoItems.Remove(item);
            _context.SaveChanges();
        }
    }
}

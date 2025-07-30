using Microsoft.EntityFrameworkCore;
using ToDoList.Core.Entities;

namespace ToDoList.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<ToDoItem> ToDoItems => Set<ToDoItem>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}

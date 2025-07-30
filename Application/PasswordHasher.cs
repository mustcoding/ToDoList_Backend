namespace ToDoList.Application;

public interface IPasswordHasher
{
    string Hash(string password);
}
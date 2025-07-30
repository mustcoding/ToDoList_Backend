using ToDoList.Application.DTOs;
using ToDoList.Core.Entities;

namespace ToDoList.Application;

public interface IUserService
{
    IEnumerable<User> GetAll();
    User? Get(string ulid);
    void Add(User user);
    void Update(User user);
    void Delete(string ulid);
    User Register(UserRegisterDto dto);
    User Login(UserRegisterDto dto);
    User? GetById(int userId);
}
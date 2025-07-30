using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using ToDoList.Application;
using ToDoList.Application.DTOs;
using ToDoList.Core.Entities;
using ToDoList.Infrastructure.Data;

public class EfUserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher _hasher;

    public EfUserService(AppDbContext context, IPasswordHasher hasher)
    {
        _context = context;
        _hasher = hasher;
    }

    public IEnumerable<User> GetAll() => _context.Users.ToList();

    public User? Get(string ulid)
        => _context.Users.FirstOrDefault(u => u.Ulid == ulid);

    public void Add(User user)
    {
        user.Ulid = Guid.NewGuid().ToString("N");
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public void Update(User user)
    {
        _context.Users.Update(user);
        _context.SaveChanges();
    }

    public void Delete(string ulid)
    {
        var user = _context.Users.FirstOrDefault(u => u.Ulid == ulid);
        if (user != null)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
        }
    }

    public User Register(UserRegisterDto dto)
    {
        if (_context.Users.Any(u => u.Email.ToLower() == dto.Email.ToLower()))
            throw new InvalidOperationException("Email is already registered.");

        var emailValidator = new EmailAddressAttribute();
        if (!emailValidator.IsValid(dto.Email))
            throw new InvalidOperationException("Invalid email format.");

        var hashedPassword = _hasher.Hash(dto.Password);
        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            Password = hashedPassword,
            Ulid = Guid.NewGuid().ToString("N")
        };

        _context.Users.Add(user);
        _context.SaveChanges();
        return user;
    }

    public User Login(UserRegisterDto dto)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email.ToLower() == dto.Email.ToLower());
        if (user == null)
            throw new InvalidOperationException("User not found.");

        var hashedPassword = _hasher.Hash(dto.Password);
        if (user.Password != hashedPassword)
            throw new InvalidOperationException("Incorrect password.");

        return user;
    }

    public User? GetById(int userId)
        => _context.Users.FirstOrDefault(u => u.UserId == userId);
}

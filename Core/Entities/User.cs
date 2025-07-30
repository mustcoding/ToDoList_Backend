using System.Collections.ObjectModel;
using Microsoft.AspNetCore.SignalR;

namespace ToDoList.Core.Entities;

public class User
{
    public int UserId { get; set; }
    public string Ulid { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
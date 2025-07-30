using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Application;
using ToDoList.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using ToDoList.Application.DTOs;

namespace ToDoList.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]

public class TodoController : ControllerBase
{
    private readonly IToDoService _service;

    public TodoController(IToDoService service)
    {
        _service = service;
    }

    [HttpGet]
    public ActionResult GetAll() => Ok(_service.GetAll());

    [HttpGet("{id}")]
    public ActionResult Get(int id)
    {
        var item = _service.Get(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public ActionResult Create([FromBody] ToDoItemDto item)
    {
        if (string.IsNullOrWhiteSpace(item.Title))
            return BadRequest("Title is required.");
        if (string.IsNullOrWhiteSpace(item.Description))
            return BadRequest("Description is required");
        if (!item.DueDate.HasValue)
            return BadRequest("DueDate is required");

        try
        {
            var to_do = _service.Add(item);
            return Ok(new
            {
                to_do = new
                {
                    to_do.Title,
                    to_do.Description,
                    to_do.IsCompleted,
                    to_do.DueDate,
                    to_do.CreatedAt,
                    to_do.UpdateAt,
                    to_do.UserId,
                }
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }

    }

    [HttpPutAttribute("{id}")]
    public ActionResult Update(int id, [FromBody] ToDoItemDto item)
    {
        if (id != item.Id) return BadRequest("ID mismatch.");
        var to_do = _service.Update(item);

        if (to_do == null)
            return NotFound();

        return Ok(new
        {
            to_do.Title,
            to_do.Description,
            to_do.IsCompleted,
            to_do.DueDate,
            to_do.CreatedAt,
            to_do.UpdateAt,
            to_do.UserId,
        });
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        _service.Delete(id);
        return NoContent();
    }
}
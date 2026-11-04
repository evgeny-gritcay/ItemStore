using ItemStore.Api.Data;
using ItemStore.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ItemStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Item>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        return await db.Items
            .OrderByDescending(i => i.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Item>> GetById(int id)
    {
        var item = await db.Items.FindAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<Item>> Create(Item item)
    {
        if (string.IsNullOrWhiteSpace(item.Name))
            return BadRequest("Name is required");
        if (item.Price < 0)
            return BadRequest("Price must be non-negative");

        db.Items.Add(item);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Item input)
    {
        var item = await db.Items.FindAsync(id);
        if (item is null) return NotFound();

        item.Name = input.Name;
        item.Description = input.Description;
        item.Price = input.Price;
        item.Quantity = input.Quantity;

        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await db.Items.FindAsync(id);
        if (item is null) return NotFound();

        db.Items.Remove(item);
        await db.SaveChangesAsync();
        return NoContent();
    }
}

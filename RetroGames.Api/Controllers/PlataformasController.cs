using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetroGames.Api.Data;
using RetroGames.Api.Models;

namespace RetroGames.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlataformasController : ControllerBase
{
    private readonly AppDbContext _context;

    public PlataformasController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Plataforma>>> GetPlataformas()
    {
        return await _context.Plataformas.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Plataforma>> GetPlataforma(int id)
    {
        var plataforma = await _context.Plataformas.FindAsync(id);
        if (plataforma == null) return NotFound();
        return plataforma;
    }

    [HttpPost]
    public async Task<ActionResult<Plataforma>> PostPlataforma(Plataforma plataforma)
    {
        _context.Plataformas.Add(plataforma);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetPlataforma), new { id = plataforma.Id }, plataforma);
    }
}
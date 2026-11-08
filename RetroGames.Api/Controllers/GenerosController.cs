using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetroGames.Api.Data;
using RetroGames.Api.Models;

namespace RetroGames.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GenerosController : ControllerBase
{
    private readonly AppDbContext _context;

    public GenerosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Genero>>> GetGeneros()
    {
        return await _context.Generos.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Genero>> PostGenero(Genero genero)
    {
        _context.Generos.Add(genero);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetGeneros), new { id = genero.Id }, genero);
    }
}
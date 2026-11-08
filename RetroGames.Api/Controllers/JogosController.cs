using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetroGames.Api.Data;
using RetroGames.Api.Models;

namespace RetroGames.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JogosController : ControllerBase
{
    private readonly AppDbContext _context;

    public JogosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Jogo>>> GetJogos()
    {
        return await _context.Jogos
            .Include(j => j.Plataforma)
            .Include(j => j.Genero)
            .Include(j => j.Condicao)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Jogo>> GetJogo(int id)
    {
        var jogo = await _context.Jogos
            .Include(j => j.Plataforma)
            .Include(j => j.Genero)
            .Include(j => j.Condicao)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (jogo == null) return NotFound();
        return jogo;
    }

    [HttpPost]
    public async Task<ActionResult<Jogo>> PostJogo(Jogo jogo)
    {
        _context.Jogos.Add(jogo);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetJogo), new { id = jogo.Id }, jogo);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutJogo(int id, Jogo jogo)
    {
        if (id != jogo.Id) return BadRequest();

        _context.Entry(jogo).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Jogos.Any(e => e.Id == id)) return NotFound();
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteJogo(int id)
    {
        var jogo = await _context.Jogos.FindAsync(id);
        if (jogo == null) return NotFound();

        _context.Jogos.Remove(jogo);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
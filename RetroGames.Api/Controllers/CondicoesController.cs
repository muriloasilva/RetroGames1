using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetroGames.Api.Data;
using RetroGames.Api.Models;

namespace RetroGames.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CondicoesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CondicoesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Condicao>>> GetCondicoes()
    {
        return await _context.Condicoes.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Condicao>> PostCondicao(Condicao condicao)
    {
        _context.Condicoes.Add(condicao);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCondicoes), new { id = condicao.Id }, condicao);
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Shared.Models;
using BlazorPokemonCardSetViewer.Features.PokemonSet;

namespace Server.Controller;

[ApiController]
[Route("api/[controller]")]
public class PokemonSetController : ControllerBase
{
    private readonly PokemonDbContext _context;
    private readonly ILogger<PokemonSetController> _logger;

    public PokemonSetController(PokemonDbContext context, ILogger<PokemonSetController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("sets")] // Example: https://localhost:7240/api/PokemonSet/sets?pageSize=12&pageNumber=1
    public async Task<ActionResult<PagedList<PokemonSetDataResponse>>> GetAllSets(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 12)
    {
        try
        {
            _logger.LogInformation("Retrieving all Pokemon Sets");

            var setDto = await _context.PokemonSets
                .Select(s => new PokemonSetDataResponse
                {
                    Id = s.Id,
                    Name = s.Name,
                    Series = s.Series,
                    PrintedTotal = s.PrintedTotal,
                    Total = s.Total,
                    PtcgoCode = s.PtcgoCode,
                    ReleaseDate = DateTime.Parse(s.ReleaseDate),
                    UpdatedAt = s.UpdatedAt,
                    ImageSymbol = s.ImageSymbol,
                    ImageLogo = s.ImageLogo,
                    LegalityUnlimited = s.LegalityUnlimited,
                    LegalityStandard = s.LegalityStandard,
                    LegalityExpanded = s.LegalityExpanded,
                })
                .FirstOrDefaultAsync();
            if (setDto != null) return Ok(setDto);
            _logger.LogWarning("Sets not found");
            return NotFound("Pokemon sets not found"); // Status Not Found Code 404
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sets");
            // Internal Server Error Code 500
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
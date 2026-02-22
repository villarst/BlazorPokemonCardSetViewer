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

    [HttpGet("sets")] // Example: https://localhost:7240/api/PokemonSet/sets?pageNumber=1&pageSize=12&sortOrder=newest
    public async Task<ActionResult<PagedList<PokemonSetDataResponse>>> GetSets(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 12,
        [FromQuery] string? sortOrder = "newest")
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
                .ToListAsync();
            if (sortOrder != null && sortOrder.ToLower() == "newest")
            {
                setDto = setDto.OrderByDescending(set => set.ReleaseDate).ToList();
            }
            else // "oldest"
            {
                setDto = setDto.OrderBy(set => set.ReleaseDate).ToList();
            }
            return Ok(setDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sets");
            // Internal Server Error Code 500
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
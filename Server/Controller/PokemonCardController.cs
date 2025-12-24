using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Server.Data;
using Shared.Models;

namespace Server.Controller;

[ApiController]
[Route("api/[controller]")]
public class PokemonCardController : ControllerBase
{
    private readonly PokemonDbContext _context;
    private readonly ILogger<PokemonCardController> _logger;
    
    public PokemonCardController(PokemonDbContext context, ILogger<PokemonCardController> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    [HttpGet("{searchTerm}")]
    public async Task<ActionResult<PagedList<PokemonCard>>> GetCards(
        string searchTerm, 
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 12)
    {
        try
        {
            _logger.LogInformation("Getting cards: {SearchTerm}, Page: {PageNumber}, Size: {PageSize}", 
                searchTerm, pageNumber, pageSize);
            
            var query = _context.PokemonCards.AsQueryable();
            
            // Apply search filter - search by name (case-insensitive)
            if (!string.IsNullOrEmpty(searchTerm) && searchTerm != "*")
            {
                query = query.Where(c => EF.Functions.Like(c.Name.ToLower(), $"%{searchTerm.ToLower()}%"));
            }
            
            var totalCount = await query.CountAsync();
            
            _logger.LogInformation("Total matching cards: {TotalCount}", totalCount);
            
            if (totalCount == 0)
            {
                _logger.LogWarning("No card data found for search term: {SearchTerm}", searchTerm);
                return Ok(new PagedList<PokemonCard>
                {
                    Data = [],
                    TotalCount = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                });
            }
            
            // Get paginated results
            var cards = await query
                .OrderBy(c => c.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            var result = new PagedList<PokemonCard>
            {
                Data = cards,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            
            _logger.LogInformation("Returning {Count} cards out of {TotalCount} total", 
                cards.Count, totalCount);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cards {SearchTerm}", searchTerm);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    [HttpGet("card/{cardId}")]
    public async Task<ActionResult<PokemonCard>> GetCardById(string cardId)
    {
        try
        {
            _logger.LogInformation("Getting card by ID: {CardId}", cardId);
            
            var card = await _context.PokemonCards.FirstOrDefaultAsync(c => c.Id == cardId);

            if (card != null) return Ok(card);
            _logger.LogWarning("Card not found: {CardId}", cardId);
            return NotFound($"Card with ID {cardId} not found");

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting card {CardId}", cardId);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    [HttpGet("card/{cardId}/full")]
    public async Task<ActionResult<object>> GetFullCardData(string cardId)
    {
        try
        {
            var dbCard = await _context.PokemonCards.FirstOrDefaultAsync(c => c.Id == cardId);
            
            if (dbCard == null)
                return NotFound($"Card with ID {cardId} not found");
            
            if (string.IsNullOrEmpty(dbCard.RawJson))
                return NotFound("Full JSON data not available for this card");
            
            var fullData = JObject.Parse(dbCard.RawJson);
            return Ok(fullData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting full card data {CardId}", cardId);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    [HttpGet("sets")]
    public async Task<ActionResult<IEnumerable<object>>> GetSets()
    {
        try
        {
            var sets = await _context.PokemonCards
                .Where(c => !string.IsNullOrEmpty(c.SetId))
                .Select(c => new { c.SetId, c.SetName })
                .Distinct()
                .OrderBy(s => s.SetName)
                .ToListAsync();
            
            return Ok(sets);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sets");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
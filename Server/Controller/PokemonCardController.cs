using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Shared.Models;
using BlazorPokemonCardSetViewer.Features.PokemonCard;

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
    
    [HttpGet("{searchTerm}")] // Example: https://localhost:7240/api/PokemonCard/Pikachu?pageSize=12&pageNumber=1
    public async Task<ActionResult<PagedList<PokemonCardData>>> GetCards(
        string searchTerm, 
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 12)
    {
        // TODO: Need to eventually move to a PokemonBackEnd.cs file and have a PokemonFrontEnd.cs file. 
        try
        {
            _logger.LogInformation("Getting cards: {SearchTerm}, Page: {PageNumber}, Size: {PageSize}", 
                searchTerm, pageNumber, pageSize);
            
            var query = _context.PokemonCards.AsQueryable();
            
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query
                    .Where(c => c.Name.ToLower().StartsWith(searchTerm));
            }
            
            var totalCount = await query.CountAsync();
            
            _logger.LogInformation("Total matching cards: {TotalCount}", totalCount);
            
            if (totalCount == 0)
            {
                _logger.LogWarning("No card data found for search term: {SearchTerm}", searchTerm);
                return Ok(new PagedList<PokemonCardData>  // Changed to DTO
                {
                    Data = [],
                    TotalCount = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                });
            }
            
            // Get paginated results and map to the DTO in one query
            var cardDto = await query
                .OrderBy(c => c.Name)
                .Skip((pageNumber - 1) * pageSize) // Neat trick for pagination I did not know about until I saw at work.
                .Take(pageSize)
                .AsQueryable()
                .Select(c => new PokemonCardData  // Map to the DTO here
                {
                    Id = c.Id,
                    Name = c.Name,
                    Hp = c.Hp.ToString(),
                    CardNumber = c.SetNumber,
                    ImageSmall = c.ImageSmall,
                    ImageLarge = c.ImageLarge
                })
                .ToListAsync();
            
            var result = new PagedList<PokemonCardData>  // Changed to DTO
            {
                Data = cardDto,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            
            _logger.LogInformation("Returning {Count} cards out of {TotalCount} total", 
                cardDto.Count, totalCount);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cards {SearchTerm}", searchTerm);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    [HttpGet("id/{cardId}")] // Example: https://localhost:7240/api/PokemonCard/id/ex14-46/?pageSize=1&pageNumber=1
    public async Task<ActionResult<PagedList<PokemonCardData>>> GetCardById(  
        string cardId, 
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 12)
    {
        try
        {
            _logger.LogInformation("Getting card by ID: {CardId}", cardId);
            
            var cardDto = await _context.PokemonCards
                .Where(c => c.Id == cardId)
                .AsQueryable()
                .Select(c => new PokemonCardData  // Map to DTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Hp = c.Hp.ToString(),
                    CardNumber = c.SetNumber,
                    ImageSmall = c.ImageSmall,
                    ImageLarge = c.ImageLarge
                })
                .FirstOrDefaultAsync();
                
            if (cardDto != null) return Ok(cardDto);
            
            _logger.LogWarning("Card not found: {CardId}", cardId);
            return NotFound($"Card with ID {cardId} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting card {CardId}", cardId);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
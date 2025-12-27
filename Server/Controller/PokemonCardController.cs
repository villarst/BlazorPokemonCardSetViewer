using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Shared.Models;
using BlazorPokemonCardSetViewer.Features.PokemonCard; // Add this for your DTO

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
    public async Task<ActionResult<PagedList<PokemonCardData>>> GetCards(  // Changed return type
        string searchTerm, 
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 12)
    {
        try
        {
            _logger.LogInformation("Getting cards: {SearchTerm}, Page: {PageNumber}, Size: {PageSize}", 
                searchTerm, pageNumber, pageSize);
            
            var query = _context.PokemonCards.AsQueryable();
            
            if (!string.IsNullOrEmpty(searchTerm) && searchTerm != "*")
            {
                query = query.Where(c => EF.Functions.Like(c.Name.ToLower(), $"%{searchTerm.ToLower()}%"));
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
            
            // Get paginated results and map to DTO in one query
            var cardDtos = await query
                .OrderBy(c => c.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsQueryable()
                .Select(c => new PokemonCardData  // Map to DTO here
                {
                    Id = c.Id,
                    Name = c.Name,
                    Hp = c.Hp.ToString(),
                    CardNumber = c.SetNumber.ToString(),
                    ImageSmall = c.ImageSmall,   // Adjust these property names to match your PokemonCard model
                    ImageLarge = c.ImageLarge    // Adjust these property names to match your PokemonCard model
                })
                .ToListAsync();
            
            var result = new PagedList<PokemonCardData>  // Changed to DTO
            {
                Data = cardDtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            
            _logger.LogInformation("Returning {Count} cards out of {TotalCount} total", 
                cardDtos.Count, totalCount);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cards {SearchTerm}", searchTerm);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    [HttpGet("card/{cardId}")]
    public async Task<ActionResult<PokemonCardData>> GetCardById(string cardId)  // Changed return type
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
                    CardNumber = c.SetNumber.ToString(),
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
    
    // [HttpGet("card/{cardId}/full")]
    // public async Task<ActionResult<PokemonCardData>> GetFullCardData(string cardId)  // Changed return type
    // {
    //     try
    //     {
    //         var cardDto = await _context.PokemonCards
    //             .Where(c => c.Id == cardId)
    //             .Select(c => new PokemonCardData  // Map to DTO
    //             {
    //                 Id = c.Id,
    //                 Name = c.Name,
    //                 Hp = c.Hp,
    //                 ImageSmall = c.ImageSmall,
    //                 ImageLarge = c.ImageLarge
    //             })
    //             .FirstOrDefaultAsync();
    //         
    //         if (cardDto == null)
    //             return NotFound($"Card with ID {cardId} not found");
    //         
    //         return Ok(cardDto);
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error getting full card data {CardId}", cardId);
    //         return StatusCode(500, $"Internal server error: {ex.Message}");
    //     }
    // }
}
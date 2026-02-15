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
    
    [HttpPost ("search")] // Example: https://localhost:7240/api/PokemonCard/Pikachu?pageSize=12&pageNumber=1
    public async Task<ActionResult<PagedList<PokemonCardDataResponse>>> GetCardsTwo(
        [FromBody] PagedRequest request)
    {
        try
        {
            _logger.LogInformation($"Getting card with search term: {request}", request.SearchTerm) ;
            
            var query = _context.PokemonCards.AsQueryable();

            var searchTerm = request.SearchTerm;
            var pageNumber = request.PageNumber;
            var pageSize = request.PageSize;
            var rarities = request.Rarities;
            
            
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query
                    .Where(c => c.Name.ToLower().StartsWith(searchTerm));
            }
            
            // Need to filter here because we are doing filtering before actually creating the dto and applying the take.
            if (rarities is { Count: > 0 })
            {
                query = query.Where(c => rarities.Contains(c.Rarity!));
            }
            
            var totalCount = await query.CountAsync();
            
            _logger.LogInformation("Total matching cards: {TotalCount}", totalCount);
            
            if (totalCount == 0)
            {
                _logger.LogWarning("No card data found for search term: {SearchTerm}", request.SearchTerm );
                return Ok(new PagedList<PokemonCardDataResponse>  // Changed to DTO
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
                .Select(c => new PokemonCardDataResponse  // Map to the DTO here
                {
                    Id = c.Id,
                    Name = c.Name,
                    SuperType = c.SuperType,
                    Level = c.Level,
                    Hp = c.Hp,
                    EvolvesFrom = c.EvolvesFrom,
                    RetreatCost = c.RetreatCost,
                    SetNumber = c.SetNumber,
                    Artist = c.Artist,
                    FlavorText = c.FlavorText,
                    CardNumber = c.SetNumber,
                    ImageSmall = c.ImageSmall,
                    ImageLarge = c.ImageLarge,
                    Rarity = c.Rarity,
                    LegalityUnlimited = c.LegalityUnlimited,
                    LegalityStandard = c.LegalityStandard,
                    LegalityExpanded = c.LegalityExpanded
                })
                .ToListAsync();
            
            var result = new PagedList<PokemonCardDataResponse>  // Changed to DTO
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
            _logger.LogError(ex, "Error getting cards {SearchTerm}", request.SearchTerm);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("id/{cardId}")] // Example: https://localhost:7240/api/PokemonCard/id/ex14-46/?pageSize=1&pageNumber=1
    public async Task<ActionResult<PagedList<PokemonCardDataResponse>>> GetCardById(  
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
                .Select(c => new PokemonCardDataResponse  // Map to DTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Hp = c.Hp,
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

    [HttpGet("rarities")] // Example: https://localhost:7240/api/PokemonCard/rarities
    public async Task<ActionResult<PagedList<RarityResponse>>> GetCardRarities(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            _logger.LogInformation("Getting all card rarities.");
            var query = _context.PokemonCards.AsQueryable();

            var rarityDto = await query
                .OrderBy(c => c.Rarity)
                .Select(c => new RarityResponse
                {
                    Name = c.Rarity,
                })
                .Where(c => c.Name != null)
                .Distinct()
                .ToListAsync();
            var totalCount = rarityDto.Count();

            var result = new PagedList<RarityResponse>
            {
                Data = rarityDto,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            _logger.LogInformation("Returning {Count} results for card rarities.", totalCount);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting card rarities.");
            throw new HttpRequestException("Error getting card rarities. (Http Request Exception)");
        }
    }
}
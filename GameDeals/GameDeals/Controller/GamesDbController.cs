using GameDeals.Shared.Data;
using GameDeals.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameDeals.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesDbController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public GamesDbController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<List<IGDBGameEntity>> GetGames(int offset = 0, int limit = 12)
        {
            return await _dbContext.Games
                .Include(g => g.Cover)
                .Include(g => g.Prices)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }
    }

}

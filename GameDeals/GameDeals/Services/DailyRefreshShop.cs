using GameDeals.Shared.Data;
using GameDeals.Shared.Entities;
using GameDeals.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GameDeals.Services
{
    public class DailyRefreshService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DailyRefreshService> _logger;
        private TimeSpan _scheduledTime = new TimeSpan(1, 0, 0); // 1:00 Uhr

        public DailyRefreshService(IServiceProvider serviceProvider, ILogger<DailyRefreshService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task UpdateDatabaseAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Manuelle Datenbank-Aktualisierung gestartet.");

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var igdbService = scope.ServiceProvider.GetRequiredService<IGDBService>();
                    var itadService = scope.ServiceProvider.GetRequiredService<IsThereAnyDealService>();

                    // Alte Daten löschen
                    dbContext.Prices.RemoveRange(dbContext.Prices);
                    dbContext.Covers.RemoveRange(dbContext.Covers);
                    dbContext.Games.RemoveRange(dbContext.Games);
                    await dbContext.SaveChangesAsync(cancellationToken);

                    int offset = 0;
                    const int pageSize = 50;
                    bool moreData = true;

                    while (moreData && !cancellationToken.IsCancellationRequested)
                    {
                        var games = await igdbService.GetGamesPagedAsync(offset, pageSize);

                        if (games.Count == 0)
                        {
                            moreData = false;
                            break;
                        }

                        var entities = new List<IGDBGameEntity>();

                        foreach (var game in games)
                        {
                            var prices = await itadService.GetPricesByTitleAsync(game.Name);
                            if (!prices.Any(p => p.PriceNew > 0.5m))
                                continue;

                            var gameEntity = game.ToEntity();
                            gameEntity.Prices = prices.Select(p => new PriceEntryEntity
                            {
                                PriceNew = p.PriceNew,
                                Url = p.Url,
                                ShopName = p.Shop.Name
                            }).ToList();

                            entities.Add(gameEntity);
                        }

                        if (entities.Count > 0)
                        {
                            await dbContext.Games.AddRangeAsync(entities, cancellationToken);
                            await dbContext.SaveChangesAsync(cancellationToken);
                        }

                        offset += pageSize;
                    }

                    _logger.LogInformation("Manuelle Datenbank-Aktualisierung erfolgreich abgeschlossen.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler bei manueller Datenbank-Aktualisierung.");
                throw;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                var nextRunTime = DateTime.Today.Add(_scheduledTime);
                if (now > nextRunTime)
                    nextRunTime = nextRunTime.AddDays(1);

                var delay = nextRunTime - now;

                await Task.Delay(delay, stoppingToken);

                await UpdateDatabaseAsync(stoppingToken);
            }
        }
    }
}

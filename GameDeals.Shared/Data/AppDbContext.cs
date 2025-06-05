using GameDeals.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GameDeals.Shared.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<IGDBGameEntity> Games { get; set; }
        public DbSet<CoverEntity> Covers { get; set; }
        public DbSet<PriceEntryEntity> Prices { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // IGDBGameEntity → Preis (1:n)
            modelBuilder.Entity<IGDBGameEntity>()
                .HasMany(g => g.Prices)
                .WithOne(p => p.Game)
                .HasForeignKey(p => p.IGDBGameEntityId)
                .OnDelete(DeleteBehavior.Cascade);

            // IGDBGameEntity → Cover (1:1)
            modelBuilder.Entity<IGDBGameEntity>()
                .HasOne(g => g.Cover)
                .WithOne(c => c.Game)
                .HasForeignKey<CoverEntity>(c => c.IGDBGameEntityId)
                .OnDelete(DeleteBehavior.Cascade);

            // Tabellenbenennungen (optional)
            modelBuilder.Entity<IGDBGameEntity>().ToTable("Games");
            modelBuilder.Entity<CoverEntity>().ToTable("Covers");
            modelBuilder.Entity<PriceEntryEntity>().ToTable("Prices");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //var folderPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "GameDeals.Shared"));
            //Directory.CreateDirectory(folderPath);
            //var dbPath = Path.Combine(folderPath, "gamecache.db");

            optionsBuilder.UseSqlite($"Data Source=gamecache.db");
        }
    }

    // Designzeit-Fabrik für EF Core Migrationsunterstützung
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite("Data Source=gamecache.db");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}

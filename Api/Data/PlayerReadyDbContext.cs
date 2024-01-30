using Api.Divnolidi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Divnolidi.Api.Data
{
    public class PlayerReadyDbContext : DbContext
    {
        public PlayerReadyDbContext(DbContextOptions<PlayerReadyDbContext> options) : base(options)
        {

        }

        public DbSet<PlayerReady> player_readys { get; set; }
    }
}

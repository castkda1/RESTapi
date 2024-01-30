using Api.Divnolidi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Divnolidi.Api.Data
{
    public class KalendarJmenaDbContext : DbContext
    {
        public KalendarJmenaDbContext(DbContextOptions<KalendarJmenaDbContext> options) : base(options)
        {

        }

        public DbSet<KalendarJmena> kalendar_jmena { get; set; }
    }
}


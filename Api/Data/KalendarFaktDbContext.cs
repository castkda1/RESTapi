using Api.Divnolidi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Divnolidi.Api.Data
{
    public class KalendarFaktDbContext : DbContext
    {
        public KalendarFaktDbContext(DbContextOptions<KalendarFaktDbContext> options) : base(options)
        {

        }

        public DbSet<KalendarFakt> kalendar_fakt { get; set; }
    }
}


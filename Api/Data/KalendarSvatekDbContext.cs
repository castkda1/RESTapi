using Api.Divnolidi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Divnolidi.Api.Data
{
    public class KalendarSvatekDbContext : DbContext
    {
        public KalendarSvatekDbContext(DbContextOptions<KalendarSvatekDbContext> options) : base(options)
        {

        }

        public DbSet<KalendarSvatek> kalendar_svatek { get; set; }
    }
}


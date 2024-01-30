using Api.Divnolidi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Divnolidi.Api.Data
{
    public class TeamDbContext : DbContext
    {
        public TeamDbContext(DbContextOptions<TeamDbContext> options) : base(options)
        {

        }

        public DbSet<Team> teams { get; set; }
    }
}

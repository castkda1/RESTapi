using Api.Divnolidi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Divnolidi.Api.Data
{
    public class LolAccountDbContext : DbContext
    {
        public LolAccountDbContext(DbContextOptions<LolAccountDbContext> options) : base(options)
        {

        }

        public DbSet<LolAccount> lol_accounts { get; set; }
    }
}

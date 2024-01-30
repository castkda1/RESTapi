using Api.Token.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Token.Data
{
    public class TokenDbContext : DbContext
    {
        public TokenDbContext(DbContextOptions<TokenDbContext> options)
            : base(options) 
        { 

        }

        public DbSet<Models.Token> tokens { get; set; }
    }
}

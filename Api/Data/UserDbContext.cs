using Api.Divnolidi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Divnolidi.Api.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {

        }

        public DbSet<User> users { get; set; }
    }
}

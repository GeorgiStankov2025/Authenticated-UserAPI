using AuthUserAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthUserAPI.Data
{
    public class UserDbContext(DbContextOptions<UserDbContext>options): DbContext(options)
    {

        public DbSet<User> Users {  get; set; }



    }
}

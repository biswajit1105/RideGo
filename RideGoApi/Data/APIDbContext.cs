using Microsoft.EntityFrameworkCore;
using RideGoApi.Models;

namespace RideGoApi.Data
{
    /// <summary>
    /// Database Context Class
    /// </summary>
    public class APIDbContext : DbContext
    {
        /// <summary>
        /// Database context for web API
        /// </summary>
        /// <param name="options">Db Context Options Builder</param>
        public APIDbContext(DbContextOptions<APIDbContext> options) : base(options) { }

        /// <summary>
        /// Login Database Set
        /// </summary>
        public DbSet<LoginModel> Login { get; set; }

        public DbSet<NewUserModel> PersonalDetails { get; set; }
    }
}

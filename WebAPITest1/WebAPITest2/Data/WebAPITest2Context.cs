using Microsoft.EntityFrameworkCore;

namespace WebAPITest2.Data
{
    public class WebAPITest2Context : DbContext
    {
        public IConfiguration Configuration { get; }
        public WebAPITest2Context(DbContextOptions<WebAPITest2Context> options) : base(options)
        {
            Database.Migrate();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DBConnection"));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne<User>(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne<Role>(ur => ur.Role)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            modelBuilder.Entity<RefreshToken>()
                .HasOne<User>(r => r.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(r => r.UserId);
        }

        // entities
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}

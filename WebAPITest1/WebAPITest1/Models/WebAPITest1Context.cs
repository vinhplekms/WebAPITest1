using Microsoft.EntityFrameworkCore;

namespace WebAPITest1.Models
{
    public class WebAPITest1Context : DbContext
    {
        public IConfiguration Configuration { get;}
        public WebAPITest1Context(DbContextOptions<WebAPITest1Context> options) : base(options)
        {
                Database.Migrate();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder  optionsBuilder)
        {
            //optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DBConnection"));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>().HasKey(ur => new {ur.UserId, ur.RoleId});

            modelBuilder.Entity<UserRole>()
                .HasOne<User>(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne<Role>(ur => ur.Role)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.RoleId);
        }

        // entities
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
    }
}

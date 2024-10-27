using Microsoft.EntityFrameworkCore;

namespace CheezAPI
{
    public class CheezContext : DbContext
    {
        public CheezContext(DbContextOptions<CheezContext> options) : base(options)
        {
        }

        public DbSet<Models.User> Users { get; set; }
        public DbSet<Models.Topic> Topics { get; set; }
        public DbSet<Models.Fthread> Fthreads { get; set; }
        public DbSet<Models.Post> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.User>().ToTable("users");
            modelBuilder.Entity<Models.Topic>().ToTable("topics");
            modelBuilder.Entity<Models.Fthread>().ToTable("fthreads");
            modelBuilder.Entity<Models.Post>().ToTable("posts");
        }
    }
}

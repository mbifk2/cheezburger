using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;

namespace CheezAPI
{
    public class CheezContext : DbContext
    {
        public CheezContext(DbContextOptions<CheezContext> options) : base(options)
        {
        }

        public DbSet<Models.User> Users { get; set; }
        public DbSet<Models.Topic> Topics { get; set; }
        public DbSet<Models.Fthread> Threads { get; set; }
        public DbSet<Models.Post> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.User>().ToTable("users");
            modelBuilder.Entity<Models.Topic>().ToTable("topics");
            modelBuilder.Entity<Models.Fthread>().ToTable("threads");
            modelBuilder.Entity<Models.Post>().ToTable("posts");
        }
    }
}

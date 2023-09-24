using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TimelineSITE.Models;


namespace TimelineSITE.Data
{
    public class UsersDbContext : IdentityDbContext<ApplicationUser>
    {
        public UsersDbContext(DbContextOptions<UsersDbContext> options)
            : base(options)
        {
        }
        public DbSet<AlbumModel> Albums { get; set; }
        public DbSet<AlbumAccessModel> AlbumsAccesses { get; set; }
        public DbSet<ImageModel> Images { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the many-to-many relationship between AlbumModel and ApplicationUser
            modelBuilder.Entity<AlbumAccessModel>()
                .HasKey(sa => sa.Id);

            modelBuilder.Entity<AlbumAccessModel>()
                .HasIndex(aa => new { aa.AlbumId, aa.UserId })
                .IsUnique();

            modelBuilder.Entity<AlbumAccessModel>()
                .HasOne(sa => sa.Album)
                .WithMany(a => a.UsersAccess)
                .HasForeignKey(sa => sa.AlbumId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AlbumAccessModel>()
                .HasOne(sa => sa.User)
                .WithMany(u => u.AlbumsAccess)
                .HasForeignKey(sa => sa.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ImageModel>()
                .HasOne(sa => sa.Album)
                .WithMany(i => i.Images)
                .HasForeignKey(sa => sa.AlbumId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
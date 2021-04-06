using Microsoft.EntityFrameworkCore;
using leashApi.Models;

namespace leashApi.Models
{
    public class ParkContext : DbContext
    {
        public DbSet<ParkItem> ParkItems { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        public DbSet<PictureDogJoin> PictureDogJoins { get; set;}
        public DbSet<Dog> Dogs { get; set; }
        public DbSet<TokenSub> TokenSubs {get; set;}
        public DbSet<UserData> UserData { get; set; }
        public ParkContext(DbContextOptions<ParkContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PictureDogJoin>()
                .HasKey(pdj => new { pdj.PictureId, pdj.DogId});
            modelBuilder.Entity<PictureDogJoin>()
                .HasOne<Dog>(pdj => pdj.Dog)
                .WithMany(m => m.PictureDogJoins)
                .HasForeignKey( pdj => pdj.DogId);
            modelBuilder.Entity<PictureDogJoin>()
                .HasOne<Picture>(pdj => pdj.Picture)
                .WithMany(m => m.PictureDogJoins)
                .HasForeignKey( pdj => pdj.PictureId);
            modelBuilder.Entity<ParkItem>()
                .HasMany(u => u.ParkGoers)
                .WithOne(p => p.Park);
                
        }
       
    }
}
using FiveServices.Models;
using Microsoft.EntityFrameworkCore;

namespace FiveServices.Data
{
    public class MyGameDbContext : DbContext
    {
        public MyGameDbContext(DbContextOptions<MyGameDbContext> options)
            : base(options)
        {
        }

        public DbSet<Five> Fives { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Five>(entity =>
            {
                entity.ToTable("Five");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                      .ValueGeneratedOnAdd();
                entity.Property(e => e.Note)
                      .IsRequired()
                      .HasMaxLength(100)
                      .HasColumnType("varchar(100)");
            });
        }
    }
}

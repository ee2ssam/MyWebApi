using Microsoft.EntityFrameworkCore;
using MyGame01.Models;

namespace MyGame01.Data
{
    public class MyGameDbContext : DbContext
    {
        public MyGameDbContext(DbContextOptions<MyGameDbContext> options)
            : base(options)
        {
        }

        public DbSet<Five> Fives { get; set; } = null!;
        public DbSet<Game01User> Game01Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Five>(entity =>
            {
                entity.ToTable("Five");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Note).IsRequired().HasMaxLength(100).HasColumnType("varchar(100)");
            });

            modelBuilder.Entity<Game01User>(entity =>
            {
                entity.ToTable("game01_users");
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UserId).HasColumnName("userid").HasColumnType("char(8)").IsRequired();
                entity.Property(e => e.Passward).HasColumnName("passward").HasColumnType("char(20)").IsRequired();
                entity.Property(e => e.Name).HasColumnName("name").HasColumnType("varchar(20)").IsRequired();
                entity.Property(e => e.BirthYear).HasColumnName("birthyear").HasColumnType("smallint").IsRequired();
                entity.Property(e => e.Addr).HasColumnName("addr").HasColumnType("char(2)").IsRequired();
                entity.Property(e => e.Mobile).HasColumnName("mobile").HasColumnType("char(11)");
                entity.Property(e => e.Level).HasColumnName("level").HasColumnType("smallint").IsRequired();
                entity.Property(e => e.Health).HasColumnName("health").HasColumnType("integer").IsRequired();
                entity.Property(e => e.Gold).HasColumnName("gold").HasColumnType("integer").IsRequired();
                entity.Property(e => e.MDate).HasColumnName("mdate").HasColumnType("date");
            });
        }
    }
}

using BookShop.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }

        public DbSet<Pen> Pens { get; set; }

        public DbSet<Paper> Papers { get; set; }

        public DbSet<BookBuyer> BookBuyers { get; set; }

        public DbSet<PenBuyer> PenBuyers { get; set; }

        public DbSet<PaperBuyer> PaperBuyers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<BookBuyer>()
                .HasKey(b => new { b.BuyerId, b.BookId });

            builder.Entity<PenBuyer>()
                .HasKey(p => new { p.BuyerId, p.PenId });

            builder.Entity<PaperBuyer>()
                .HasKey(p => new { p.BuyerId, p.PaperId });

            builder.Entity<BookBuyer>()
                .HasOne(b => b.Buyer)
                .WithMany()
                .HasForeignKey(b => b.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PenBuyer>()
                .HasOne(b => b.Pen)
                .WithMany()
                .HasForeignKey(b => b.PenId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PaperBuyer>()
                .HasOne(b => b.Paper)
                .WithMany()
                .HasForeignKey(b => b.PaperId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(builder);
        }
    }
}

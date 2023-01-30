using AmigosAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

namespace AmigosAPI.Data
{
    public class BillManagerContext : DbContext
    {
        public BillManagerContext(DbContextOptions<BillManagerContext> options) : base(options)
        {
        }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<LedgerEntry> Ledger { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
            modelBuilder.Entity<Bill>().Property(b => b.Created).HasDefaultValueSql("getdate()");
            modelBuilder.Entity<Bill>().Property(b => b.Modified).HasDefaultValueSql("getdate()");
            modelBuilder.Entity<User>().Property(p => p.Created).HasDefaultValueSql("getdate()");
            modelBuilder.Entity<User>().Property(p => p.Modified).HasDefaultValueSql("getdate()");
        }
    }
}

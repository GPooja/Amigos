using AmigosAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;

namespace AmigosAPI.Data
{
    public class BillManagerContext : DbContext
    {
        public BillManagerContext(DbContextOptions<BillManagerContext> options) : base(options)
        {
        }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<PersonBill> PersonBills { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bill>().ToTable(nameof(Bill));
            modelBuilder.Entity<Person>().ToTable(nameof(Person));
            modelBuilder.Entity<PersonBill>().HasKey(pb => new { pb.PersonID, pb.BillID });
            modelBuilder.Entity<PersonBill>().ToTable(nameof(PersonBill));
        }
    }
}

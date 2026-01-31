using Microsoft.EntityFrameworkCore;
using ExpenseSplitter.Models;

namespace ExpenseSplitter.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ExpenseSplit> ExpenseSplits { get; set; }
        public DbSet<Settlement> Settlements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Group
            modelBuilder.Entity<Group>()
                .HasOne(g => g.Creator)
                .WithMany(u => u.CreatedGroups)
                .HasForeignKey(g => g.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // GroupMember
            modelBuilder.Entity<GroupMember>()
                .HasIndex(gm => new { gm.GroupId, gm.UserId })
                .IsUnique();

            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.Group)
                .WithMany(g => g.GroupMembers)
                .HasForeignKey(gm => gm.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.User)
                .WithMany(u => u.GroupMembers)
                .HasForeignKey(gm => gm.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Expense
            modelBuilder.Entity<Expense>()
                .HasOne(e => e.Group)
                .WithMany(g => g.Expenses)
                .HasForeignKey(e => e.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Expense>()
                .HasOne(e => e.PaidBy)
                .WithMany(u => u.PaidExpenses)
                .HasForeignKey(e => e.PaidByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ExpenseSplit
            modelBuilder.Entity<ExpenseSplit>()
                .HasOne(es => es.Expense)
                .WithMany(e => e.ExpenseSplits)
                .HasForeignKey(es => es.ExpenseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ExpenseSplit>()
                .HasOne(es => es.User)
                .WithMany(u => u.ExpenseSplits)
                .HasForeignKey(es => es.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Settlement
            modelBuilder.Entity<Settlement>()
                .HasOne(s => s.FromUser)
                .WithMany()
                .HasForeignKey(s => s.FromUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Settlement>()
                .HasOne(s => s.ToUser)
                .WithMany()
                .HasForeignKey(s => s.ToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Settlement>()
                .HasOne(s => s.Group)
                .WithMany(g => g.Settlements)
                .HasForeignKey(s => s.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

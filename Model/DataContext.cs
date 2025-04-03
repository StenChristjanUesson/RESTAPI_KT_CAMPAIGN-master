using Microsoft.EntityFrameworkCore;

namespace ITB2203Application.Model
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { }

        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Email> Emails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Email>()
                .HasOne<Campaign>()
                .WithMany(c => c.Emails)
                .HasForeignKey(e => e.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Email>()
                .HasOne<Company>()
                .WithMany(c => c.Emails)
                .HasForeignKey(e => e.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Email>()
                .HasIndex(e => new { e.CampaignId, e.CompanyId })
                .IsUnique();
            base.OnModelCreating(modelBuilder);
        }
    }
}

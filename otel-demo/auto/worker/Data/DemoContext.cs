using Microsoft.EntityFrameworkCore;

namespace OtelDemo.Worker.Data;

public partial class DemoContext : DbContext
{
    public DemoContext(DbContextOptions<DemoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<messages> messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<messages>(entity =>
        {
            entity.HasKey(e => e.id).HasName("messages_pkey");

            entity.ToTable("messages", "demo");

            entity.Property(e => e.id).ValueGeneratedNever();
            entity.Property(e => e.name).IsRequired();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

using System;
using System.Collections.Generic;
using Dual.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Dual.Api.Models;

public partial class DualContext : DbContext
{
    public DualContext()
    {
    }

    public DualContext(DbContextOptions<DualContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TCicle> TCicles { get; set; }

    public virtual DbSet<TTutor> TTutors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=tuteapps.ddns.net;Database=Dual;User Id=Dual;Password=Dual.psw;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TCicle>(entity =>
        {
            entity.ToTable("t_Cicles", "Escola");

            entity.HasIndex(e => e.Codi, "IX_t_Cicles_Codi").IsUnique();

            entity.HasIndex(e => e.Nom, "IX_t_Cicles_Nom").IsUnique();

            entity.Property(e => e.Codi).HasMaxLength(25);
            entity.Property(e => e.Nom).HasMaxLength(255);
        });

        modelBuilder.Entity<TTutor>(entity =>
        {
            entity.ToTable("t_Tutors", "Escola");

            entity.HasIndex(e => e.Correu, "IX_t_Tutors").IsUnique();

            entity.Property(e => e.Correu).HasMaxLength(255);
            entity.Property(e => e.Nom).HasMaxLength(255);

            entity.HasOne(d => d.IdCicleNavigation).WithMany(p => p.TTutors)
                .HasForeignKey(d => d.IdCicle)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_t_Tutors_t_Cicles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

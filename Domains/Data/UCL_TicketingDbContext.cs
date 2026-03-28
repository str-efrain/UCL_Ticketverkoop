using System;
using System.Collections.Generic;
using Domains.Entities;
using Microsoft.EntityFrameworkCore;

namespace Domains.Data;

public partial class UCL_TicketingDbContext : DbContext
{
    public UCL_TicketingDbContext()
    {
    }

    public UCL_TicketingDbContext(DbContextOptions<UCL_TicketingDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Abonnement> Abonnements { get; set; }

    public virtual DbSet<Club> Clubs { get; set; }

    public virtual DbSet<Match> Matches { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Stadium> Stadia { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<Vak> Vaks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\SQL22_VIVES; Database=UCL_TicketingDB; Trusted_Connection=True; TrustServerCertificate=True; MultipleActiveResultSets=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Abonnement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Abonneme__3214EC07122A5F15");

            entity.ToTable("Abonnement");

            entity.HasIndex(e => e.UserId, "IX_Abonnement_UserId");

            entity.Property(e => e.Seat)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Seizoen)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Club).WithMany(p => p.Abonnements)
                .HasForeignKey(d => d.ClubId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Abonnement_Club");

            entity.HasOne(d => d.Vak).WithMany(p => p.Abonnements)
                .HasForeignKey(d => d.VakId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Abonnement_Vak");
        });

        modelBuilder.Entity<Club>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Club__3214EC075E91C639");

            entity.ToTable("Club");

            entity.HasIndex(e => e.StadiumId, "IX_Club_StadiumId");

            entity.HasIndex(e => e.Name, "UQ__Club__737584F6D334C631").IsUnique();

            entity.Property(e => e.LogoUrl)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Stadium).WithMany(p => p.Clubs)
                .HasForeignKey(d => d.StadiumId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Club_Stadium");
        });

        modelBuilder.Entity<Match>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Match__3214EC07B87B1E28");

            entity.ToTable("Match");

            entity.HasIndex(e => e.AwayClubId, "IX_Match_AwayClubId");

            entity.HasIndex(e => e.HomeClubId, "IX_Match_HomeClubId");

            entity.HasOne(d => d.AwayClub).WithMany(p => p.MatchAwayClubs)
                .HasForeignKey(d => d.AwayClubId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Match_AwayClub");

            entity.HasOne(d => d.HomeClub).WithMany(p => p.MatchHomeClubs)
                .HasForeignKey(d => d.HomeClubId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Match_HomeClub");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Order__3214EC07F53CC625");

            entity.ToTable("Order");

            entity.Property(e => e.OrderDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UserId).HasMaxLength(450);
        });

        modelBuilder.Entity<Stadium>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Stadium__3214EC075B5F78D3");

            entity.ToTable("Stadium");

            entity.Property(e => e.City)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Ticket__3214EC07BE9CBA83");

            entity.ToTable("Ticket");

            entity.HasIndex(e => e.MatchId, "IX_Ticket_MatchId");

            entity.HasIndex(e => e.OrderId, "IX_Ticket_OrderId");

            entity.HasIndex(e => e.VakId, "IX_Ticket_VakId");

            entity.Property(e => e.Seat)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.VoucherId).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Match).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.MatchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_Match");

            entity.HasOne(d => d.Order).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_Ticket_Order");

            entity.HasOne(d => d.Vak).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.VakId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_Vak");
        });

        modelBuilder.Entity<Vak>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Vak__3214EC078AD48789");

            entity.ToTable("Vak");

            entity.HasIndex(e => e.StadiumId, "IX_Vak_StadiumId");

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Ring)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Stadium).WithMany(p => p.Vaks)
                .HasForeignKey(d => d.StadiumId)
                .HasConstraintName("FK_Vak_Stadium");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

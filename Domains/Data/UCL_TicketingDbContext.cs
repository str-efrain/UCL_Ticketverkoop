using Domains.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Domains.Data;

public partial class UCL_TicketingDbContext : IdentityDbContext
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

    public virtual DbSet<Seizoen> Seizoenen { get; set; }

    public virtual DbSet<Stadium> Stadia { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<Vak> Vaks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // install this packages: - Microsoft.Extensions.Configuration.Json
            IConfigurationRoot configuration = new ConfigurationBuilder()
                  .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                  .AddJsonFile("appsettings.json")
                  .Build();

            //Connectionstring uit appsettings.json halen
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Abonnement>(entity =>
        {
         
            entity.HasKey(e => e.Id).HasName("PK__Abonneme__3214EC07122A5F15");

            entity.ToTable("Abonnement");

            entity.HasIndex(e => e.OrderId, "IX_Abonnement_OrderId").IsUnique();
            entity.HasIndex(e => e.SeizoenId, "IX_Abonnement_SeizoenId");

            entity.Property(e => e.Seat)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Order).WithOne(p => p.Abonnement)
                .HasForeignKey<Abonnement>(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Abonnement_Order");

            entity.HasOne(d => d.Club).WithMany(p => p.Abonnements)
                .HasForeignKey(d => d.ClubId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Abonnement_Club");

            entity.HasOne(d => d.Vak).WithMany(p => p.Abonnements)
                .HasForeignKey(d => d.VakId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Abonnement_Vak");

            entity.HasOne(d => d.Seizoen).WithMany(p => p.Abonnements)
                .HasForeignKey(d => d.SeizoenId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Abonnement_Seizoen");
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
                .OnDelete(DeleteBehavior.Restrict)
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
            entity.Property(e => e.TotalPrice).HasColumnType("float");
            entity.Property(e => e.UserId);

            entity.HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Order_AspNetUsers");
        });

        modelBuilder.Entity<Seizoen>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Seizoen__3214EC07");

            entity.ToTable("Seizoen");

            entity.HasIndex(e => e.Name, "UQ__Seizoen__Name").IsUnique();

            entity.Property(e => e.StartDate).HasColumnType("date");
            entity.Property(e => e.EndDate).HasColumnType("date");
        });

        modelBuilder.Entity<Stadium>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Stadium__3214EC075B5F78D3");

            entity.ToTable("Stadium");

            entity.HasIndex(e => e.Name, "UQ__Stadium__Name").IsUnique();

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

            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .IsUnicode(false);

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
            entity.Property(e => e.Price).HasColumnType("float");
            entity.Property(e => e.Ring)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Stadium).WithMany(p => p.Vaks)
                .HasForeignKey(d => d.StadiumId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Vak_Stadium");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

using System;
using System.Collections.Generic;
using ClientManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ClientManagement.Database
{
    public partial class ClientManagementContext : IdentityDbContext<ApplicationUser>
    {
        public ClientManagementContext()
        {
        }

        public ClientManagementContext(DbContextOptions<ClientManagementContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Client> Clients { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Client>(entity =>
            {
                entity.Property(e => e.ClientName).HasMaxLength(50);

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.LicenceKey).HasDefaultValueSql("(newid())");
            });
            base.OnModelCreating(modelBuilder);

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

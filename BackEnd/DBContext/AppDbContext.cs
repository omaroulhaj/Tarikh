﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TarikhMaghribi.DBContext.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TarikhMaghribi.DBContext
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<JourFerie> JoursFeries { get; set; }
        public DbSet<Models.Task> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Assurez-vous que l'email est unique
            builder.Entity<AppUser>()
                .HasIndex(u => u.Email)
                .IsUnique();

            builder.Entity<IdentityRole>().HasData(
                    new IdentityRole
                    {
                        Name = "usernormal",
                        NormalizedName = "USERNORMAL"
                    },
                    new IdentityRole
                    {
                        Name = "superutilisateur",
                        NormalizedName = "SUPERUTILISATEUR"
                    }
            );
        }

    }
}
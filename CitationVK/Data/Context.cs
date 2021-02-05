using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;

namespace CitationVK.Models
{
    public class Context : DbContext
    {
        public Context (DbContextOptions<Context> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            byte[] adminSalt = new byte[128 / 8];
            byte[] userSalt = new byte[128 / 8];

            using (RandomNumberGenerator random = RandomNumberGenerator.Create())
            {
                random.GetBytes(adminSalt);
                random.GetBytes(userSalt);
            }

            byte[] adminPassword = KeyDerivation.Pbkdf2(
                password: "admin",
                salt: adminSalt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
            );

            byte[] userPassword = KeyDerivation.Pbkdf2(
                password: "user",
                salt: userSalt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
            );

            modelBuilder.Entity<Account>().HasData(
                new Account()
                {
                    Id = 1,
                    Email = "admin@email.com",
                    Password = Convert.ToBase64String(adminPassword),
                    Question = 1,
                    Answer = Convert.ToBase64String(adminPassword),
                    IsAdmin = true,
                    Salt = Convert.ToBase64String(adminSalt),
                    Date = DateTime.Now
                },
                new Account()
                {
                    Id = 2,
                    Email = "user@email.com",
                    Password = Convert.ToBase64String(userPassword),
                    Question = 1,
                    Answer = Convert.ToBase64String(userPassword),
                    IsAdmin = false,
                    Salt = Convert.ToBase64String(userSalt),
                    Date = DateTime.Now
                }
            );

            modelBuilder.Entity<AccountClassifier>()
                .HasKey(x => new { x.AccountId, x.ClassifierId });

            modelBuilder.Entity<AccountDataset>()
                .HasKey(x => new { x.AccountId, x.DatasetId });

            modelBuilder.Entity<Article>()
                .HasOne(x => x.Dataset)
                .WithMany(x => x.Articles)
                .HasForeignKey(x => x.DatasetId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Configuration>().HasData(new Configuration()
            {
                Id = 1,
                Description = "CitationVK is a web-based machine learning tool.",
                IsPublic = true
            });
        }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<AccountClassifier> AccountClassifiers { get; set; }

        public DbSet<AccountDataset> AccountDatasets { get; set; }

        public DbSet<Article> Articles { get; set; }

        public DbSet<Classifier> Classifiers { get; set; }

        public DbSet<Configuration> Configurations { get; set; }

        public DbSet<Dataset> Datasets { get; set; }
    }
}

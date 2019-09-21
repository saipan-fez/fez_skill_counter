using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using FEZSkillCounter.Model.Entity;
using System.IO;

namespace FEZSkillCounter.Model.Repository
{
    public class AppDbContext : DbContext
    {
        private const string DefaultDbFileName = "skillcount.db";

        public DbSet<SkillCountEntity> SkillCountDbSet { get; protected set; }
        public DbSet<SettingEntity> SettingDbSet { get; protected set; }
        public string DbFilePath { get; private set; }

        public AppDbContext() : this(null)
        {
        }

        public AppDbContext(string dbFilePath)
        {
            DbFilePath = dbFilePath;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var filePath = DbFilePath;
            if (string.IsNullOrEmpty(filePath) && !File.Exists(filePath))
            {
                filePath = GetDefaultDbFilePath();
            }

            var connectionString = new SqliteConnectionStringBuilder { DataSource = filePath }.ToString();
            optionsBuilder.UseSqlite(new SqliteConnection(connectionString));
        }

        private static string GetDefaultDbFilePath()
        {
            return Path.Combine(".\\", DefaultDbFileName);
        }
    }
}

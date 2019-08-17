using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RepositoryService.Entity;
using System.IO;

namespace RepositoryService
{
    public class SkillCountDbContext : DbContext
    {
        private const string DefaultDbFileName = "skillcount.db";

        public DbSet<SkillCountEntity> SkillCountDbSet { get; protected set; }
        public string DbFilePath { get; private set; }

        public SkillCountDbContext(string dbFilePath)
        {
            DbFilePath = dbFilePath;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var filePath = DbFilePath;
            if (!File.Exists(filePath))
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

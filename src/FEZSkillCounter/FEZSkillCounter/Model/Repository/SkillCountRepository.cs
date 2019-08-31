using Microsoft.EntityFrameworkCore;
using FEZSkillCounter.Model.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace FEZSkillCounter.Model.Repository
{
    public class SkillCountRepository : IDisposable
    {
        private SkillCountDbContext _skillCountDbContext;

        private SkillCountRepository(string dbFilePath)
        {
            _skillCountDbContext = new SkillCountDbContext(dbFilePath);
        }

        public static async Task<SkillCountRepository> CreateAsync(string dbFilePath)
        {
            var ret = new SkillCountRepository(dbFilePath);
            await ret._skillCountDbContext.Database.MigrateAsync();

            return ret;
        }

        public void Dispose()
        {
            if (_skillCountDbContext != null)
            {
                _skillCountDbContext.Dispose();
                _skillCountDbContext = null;
            }
        }

        public async Task AddAsync(SkillCountEntity skill)
        {
            await _skillCountDbContext.SkillCountDbSet.AddAsync(skill);
            await _skillCountDbContext.SaveChangesAsync();
        }

        public IEnumerable<SkillCountEntity> GetSkillCounts()
        {
            return _skillCountDbContext.SkillCountDbSet
                .Include(nameof(SkillCountEntity.Details))
                .OrderBy(x => x.RecordDate)
                .ToList();
        }
    }
}

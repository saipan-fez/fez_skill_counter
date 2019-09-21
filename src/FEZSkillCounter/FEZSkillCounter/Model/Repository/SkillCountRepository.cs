using Microsoft.EntityFrameworkCore;
using FEZSkillCounter.Model.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace FEZSkillCounter.Model.Repository
{
    public class SkillCountRepository
    {
        private AppDbContext _appDbContext;

        public SkillCountRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task SaveAsync(SkillCountEntity skill)
        {
            await _appDbContext.SkillCountDbSet.AddAsync(skill);
            await _appDbContext.SaveChangesAsync();
        }

        public IEnumerable<SkillCountEntity> GetSkillCounts()
        {
            return _appDbContext.SkillCountDbSet
                .Include(nameof(SkillCountEntity.Details))
                .OrderBy(x => x.RecordDate)
                .ToList();
        }
    }
}

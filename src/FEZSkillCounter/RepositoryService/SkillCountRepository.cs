using Microsoft.EntityFrameworkCore;
using RepositoryService.Entity;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RepositoryService
{
    public class SkillCountRepository
    {
        private SkillCountDbContext _skillCountDbContext;

        private SkillCountRepository(string dbFilePath)
        {
            AppDomain.CurrentDomain.FirstChanceException += (s, e) =>
            {
                Debug.WriteLine(e.Exception);
            };

            _skillCountDbContext = new SkillCountDbContext(dbFilePath);
            _skillCountDbContext.Database.Migrate();
        }

        public static SkillCountRepository Create(string dbFilePath)
        {
            return new SkillCountRepository(dbFilePath);
        }

        public void Add(SkillCountEntity skill)
        {
            _skillCountDbContext.SkillCountDbSet.Add(skill);
        }

        public IEnumerable<SkillCountEntity> Get()
        {
            return _skillCountDbContext.SkillCountDbSet;
        }
    }
}

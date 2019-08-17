using System;
using System.Collections.Generic;
using FEZSkillCounter;
using FEZSkillCounter.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FEZSkillCounterTest
{
    [TestClass]
    public class SkillCountRepositoryTest
    {
        [TestMethod]
        public void Test()
        {
            var entity        = new SkillCountEntity();
            entity.RecordDate = DateTime.Now;
            entity.MapName    = "test";
            entity.WorkName   = "worrier";
            entity.Details    = new List<SkillCountDetailEntity>()
            {
                new SkillCountDetailEntity()
                {
                    //SkillCountDetailId = 1,
                    SkillName = "SkillName1",
                    SkillShortName = "SkillShortName1",
                    Count = 10
                },
                new SkillCountDetailEntity()
                {
                    //SkillCountDetailId = 2,
                    SkillName = "SkillName2",
                    SkillShortName = "SkillShortName2",
                    Count = 20
                },
                new SkillCountDetailEntity()
                {
                    //SkillCountDetailId = 3,
                    SkillName = "SkillName3",
                    SkillShortName = "SkillShortName3",
                    Count = 30
                },
            };

            var repository = SkillCountRepository.Create(".\\skillcount.db");
            repository.Add(entity);


        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FEZSkillCounter.Model.Entity;
using FEZSkillCounter.Model.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FEZSkillCounterTest
{
    [TestClass]
    public class SkillCountRepositoryTest
    {
        [TestMethod]
        public async Task Test()
        {
            var entity1        = new SkillCountEntity();
            entity1.RecordDate = DateTime.Now;
            entity1.MapName    = "test1";
            entity1.WorkName   = "ウォーリアー";
            entity1.Details    = new List<SkillCountDetailEntity>()
            {
                new SkillCountDetailEntity()
                {
                    SkillName      = "SkillName1",
                    SkillShortName = "SkillShortName1",
                    Count          = 10,
                    WorkName       = "ウォーリアー",
                    Parent         = entity1
                },
                new SkillCountDetailEntity()
                {
                    SkillName      = "SkillName2",
                    SkillShortName = "SkillShortName2",
                    Count          = 20,
                    WorkName       = "ウォーリアー",
                    Parent         = entity1
                },
                new SkillCountDetailEntity()
                {
                    SkillName      = "SkillName3",
                    SkillShortName = "SkillShortName3",
                    Count          = 30,
                    WorkName       = "ウォーリアー",
                    Parent         = entity1
                },
            };
            var entity2        = new SkillCountEntity();
            entity2.RecordDate = DateTime.Now;
            entity2.MapName    = "test2";
            entity2.WorkName   = "ウォーリアー";
            entity2.Details    = new List<SkillCountDetailEntity>()
            {
                new SkillCountDetailEntity()
                {
                    SkillName      = "SkillName4",
                    SkillShortName = "SkillShortName4",
                    Count          = 40,
                    WorkName       = "ウォーリアー",
                    Parent         = entity2
                },
                new SkillCountDetailEntity()
                {
                    SkillName      = "SkillName5",
                    SkillShortName = "SkillShortName5",
                    Count          = 50,
                    WorkName       = "ウォーリアー",
                    Parent         = entity2
                },
                new SkillCountDetailEntity()
                {
                    SkillName      = "SkillName6",
                    SkillShortName = "SkillShortName6",
                    Count          = 60,
                    WorkName       = "ウォーリアー",
                    Parent         = entity2
                },
            };
            var entity3        = new SkillCountEntity();
            entity3.RecordDate = DateTime.Now;
            entity3.MapName    = "test3";
            entity3.WorkName   = "ソーサラー";
            entity3.Details    = new List<SkillCountDetailEntity>()
            {
                new SkillCountDetailEntity()
                {
                    SkillName      = "SkillName7",
                    SkillShortName = "SkillShortName7",
                    Count          = 70,
                    WorkName       = "ソーサラー",
                    Parent         = entity3
                },
                new SkillCountDetailEntity()
                {
                    SkillName      = "SkillName8",
                    SkillShortName = "SkillShortName8",
                    Count          = 80,
                    WorkName       = "ソーサラー",
                    Parent         = entity3
                },
                new SkillCountDetailEntity()
                {
                    SkillName      = "SkillName9",
                    SkillShortName = "SkillShortName9",
                    Count          = 90,
                    WorkName       = "ソーサラー",
                    Parent         = entity3
                },
            };

            var repository = await SkillCountRepository.CreateAsync(".\\skillcount.db");
            await repository.SaveAsync(entity1);
            await repository.SaveAsync(entity2);
            await repository.SaveAsync(entity3);
        }
    }
}

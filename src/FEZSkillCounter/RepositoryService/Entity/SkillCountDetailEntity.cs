using System;
using System.ComponentModel.DataAnnotations;

namespace RepositoryService.Entity
{
    public class SkillCountDetailEntity
    {
        [Key]
        public int SkillCountDetailId { get; set; }
        public string SkillName { get; set; }
        public string SkillShortName { get; set; }
        public int Count { get; set; }
    }
}

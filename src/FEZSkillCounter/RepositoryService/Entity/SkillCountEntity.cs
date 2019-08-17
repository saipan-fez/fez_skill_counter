using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RepositoryService.Entity
{
    public class SkillCountEntity
    {
        [Key]
        public int SkillCountId { get; set; }
        public DateTime RecordDate { get; set; }
        public string MapName { get; set; }
        public string WorkName { get; set; }
        public List<SkillCountDetailEntity> Details { get; set; }
    }
}

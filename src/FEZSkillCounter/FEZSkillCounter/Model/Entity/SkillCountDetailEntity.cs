using System.ComponentModel.DataAnnotations;

namespace FEZSkillCounter.Model.Entity
{
    public class SkillCountDetailEntity
    {
        [Key]
        public int SkillCountDetailId { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public string SkillShortName { get; set; } = string.Empty;
        public int Count { get; set; } = 0;
    }
}

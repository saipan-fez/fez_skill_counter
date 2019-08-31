using FEZSkillCounter.Model.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEZSkillCounter.Model.Repository
{
    public class SkillCountFileRepository
    {
        public string FilePath { get; }

        private SkillCountFileRepository(string filePath)
        {
            FilePath = filePath;
        }

        public static async Task<SkillCountFileRepository> CreateAsync(string filePath)
        {
            var ret = new SkillCountFileRepository(filePath);
            await ret.CreateAsync();

            return ret;
        }

        private async Task CreateAsync()
        {
            var fullPath  = Path.GetFullPath(FilePath);
            var directory = new DirectoryInfo(Path.GetDirectoryName(fullPath));
            var file      = new FileInfo(fullPath);

            if (!directory.Exists)
            {
                directory.Create();
            }

            if (!file.Exists)
            {
                using (file.Create()) { }
            }

            await Task.CompletedTask;
        }

        public void Save(IEnumerable<SkillCountDetailEntity> skills)
        {
            var text = string.Join(
                Environment.NewLine,
                skills.Select(x => x.SkillShortName + "：" + x.Count));

            using (var sw = new StreamWriter(FilePath, false, Encoding.UTF8))
            {
                sw.WriteLine(text);
                sw.Flush();
            }
        }
    }
}

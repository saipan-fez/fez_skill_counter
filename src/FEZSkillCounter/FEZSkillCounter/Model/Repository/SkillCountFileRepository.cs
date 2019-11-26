using FEZSkillCounter.Model.Entity;
using NeoSmart.AsyncLock;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEZSkillCounter.Model.Repository
{
    public class SkillCountFileRepository
    {
        private const int RetryCount = 5;

        public string TxtFilePath { get; }
        public string XmlFilePath { get; }

        private AsyncLock _lock = new AsyncLock();

        private SkillCountFileRepository(string txtFilePath, string xmlFilePath)
        {
            TxtFilePath = txtFilePath;
            XmlFilePath = xmlFilePath;
        }

        public static async Task<SkillCountFileRepository> CreateAsync(string txtFilePath, string xmlFilePath)
        {
            var ret = new SkillCountFileRepository(txtFilePath, xmlFilePath);
            await ret.CreateAsync();

            return ret;
        }

        private async Task CreateAsync()
        {
            var fullPath  = Path.GetFullPath(TxtFilePath);
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

        public async Task SaveAsync(IEnumerable<SkillCountDetailEntity> skills)
        {
            // ファイルに書き込む内容を生成
            var text      = CreateTxtContents(skills);
            var skillsDom = CreateXmlDom(skills);

            using (await _lock.LockAsync())
            {
                // txtファイルへの書き込み
                var txtWriteTask = Task.Run(() =>
                {
                    WriteWithRetry(TxtFilePath, text);
                });

                // xmlファイルへの書き込み
                var xmlWriteTask = Task.Run(() =>
                {
                    WriteWithRetry(XmlFilePath, skillsDom);
                });

                await Task.WhenAll(txtWriteTask, xmlWriteTask);
            }
        }

        private void WriteWithRetry(string path, string content)
        {
            for (int i = 0; i < RetryCount; i++)
            {
                try
                {
                    using (var sw = new StreamWriter(path, false, Encoding.UTF8))
                    {

                        sw.WriteLine(content);
                        sw.Flush();
                    }
                    break;
                }
                catch
                {
                    continue;
                }
            }
        }

        private string CreateTxtContents(IEnumerable<SkillCountDetailEntity> skills)
        {
            return string.Join("\n", skills.Select(x => x.SkillShortName + "：" + x.Count));
        }

        private string CreateXmlDom(IEnumerable<SkillCountDetailEntity> skills)
        {
            var skillDomCollection = skills.Select((x, i) =>
            {
                return
                        $"<skill id='{i}'>\n" +
                        $"    <name>{x.SkillName}</name>\n" +
                        $"    <shortname>{x.SkillShortName}</shortname>\n" +
                        $"    <count>{x.Count}</count>\n" +
                        $"</skill>";
            });

            var work = skills.FirstOrDefault()?.WorkName ?? "不明";

            var skillsDom =
                    $"<log>\n" +
                    $"<work>{work}</work>\n" +
                    $"<skills>\n" +
                    string.Join("\n", skillDomCollection) + "\n" +
                    $"</skills>\n" +
                    $"</log>\n";

            return skillsDom;
        }
    }
}

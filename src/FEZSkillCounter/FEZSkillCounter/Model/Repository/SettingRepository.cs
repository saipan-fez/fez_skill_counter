using FEZSkillCounter.Model.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace FEZSkillCounter.Model.Repository
{
    public class SettingRepository
    {
        private AppDbContext _appDbContext;

        public SettingRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task UpdateAsync()
        {
            await _appDbContext.SaveChangesAsync();
        }

        public SettingEntity GetSetting()
        {
            var setting = _appDbContext.SettingDbSet
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();

            if (setting == default(SettingEntity))
            {
                setting = new SettingEntity();
                _appDbContext.Add(setting);
            }

            return setting;
        }
    }
}

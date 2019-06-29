using FEZSkillUseCounter.Extension;
using System.Collections.Generic;

using R = FEZSkillUseCounter.Properties.Resources;

namespace FEZSkillUseCounter
{
    internal class PowStorage
    {
        public static Dictionary<string, int> Table;

        public static void Create()
        {
            Table = new Dictionary<string, int>()
            {
                { R.pow_0.FillPaddingToZero().SHA1Hash(), 0 },
                { R.pow_1.FillPaddingToZero().SHA1Hash(), 1 },
                { R.pow_2.FillPaddingToZero().SHA1Hash(), 2 },
                { R.pow_3.FillPaddingToZero().SHA1Hash(), 3 },
                { R.pow_4.FillPaddingToZero().SHA1Hash(), 4 },
                { R.pow_5.FillPaddingToZero().SHA1Hash(), 5 },
                { R.pow_6.FillPaddingToZero().SHA1Hash(), 6 },
                { R.pow_7.FillPaddingToZero().SHA1Hash(), 7 },
                { R.pow_8.FillPaddingToZero().SHA1Hash(), 8 },
                { R.pow_9.FillPaddingToZero().SHA1Hash(), 9 },
                { R.pow_none.FillPaddingToZero().SHA1Hash(), 0 },
            };
        }
    }
}

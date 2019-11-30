namespace SkillUseCounter.Entity
{
    /// <summary>
    /// キープ 残ポイント
    /// </summary>
    public class KeepDamage
    {
        public const double InvalidKeepDamage = double.NaN;

        /// <summary>
        /// 攻撃側 残ポイント
        /// </summary>
        public double AttackKeepDamage   { get; }

        /// <summary>
        /// 防衛側 残ポイント
        /// </summary>
        public double DefenceKeepDamage { get; }

        public static KeepDamage Empty
        {
            get
            {
                return new KeepDamage(InvalidKeepDamage, InvalidKeepDamage);
            }
        }

        public KeepDamage(double attack, double defence)
        {
            AttackKeepDamage  = attack;
            DefenceKeepDamage = defence;
        }

        public bool IsEmpty()
        {
            return double.IsNaN(AttackKeepDamage) || double.IsNaN(DefenceKeepDamage);
        }
    }
}

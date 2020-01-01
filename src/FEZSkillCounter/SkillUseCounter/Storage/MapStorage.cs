using SkillUseCounter.Entity;
using SkillUseCounter.Extension;
using System.Collections.Generic;

using R = SkillUseCounter.Properties.Resource;

namespace SkillUseCounter.Storage
{
    internal class MapStorage
    {
        public static IReadOnlyDictionary<string, Map> Table;

        public static void Create()
        {
            Table = new Dictionary<string, Map>()
            {
                { R.アシロマ山麓.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.アシロマ山麓)) },
                { R.アベル渓谷.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.アベル渓谷)) },
                { R.アンバーステップ平原.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.アンバーステップ平原)) },
                { R.アークトゥルス隕石跡.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.アークトゥルス隕石跡)) },
                { R.インベイ高地.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.インベイ高地)) },
                { R.ウィネッシュ渓谷.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.ウィネッシュ渓谷)) },
                { R.ウェンズデイ古戦場跡.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.ウェンズデイ古戦場跡)) },
                { R.ウォーロック古戦場跡.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.ウォーロック古戦場跡)) },
                { R.エルギル高原.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.エルギル高原)) },
                { R.オブシディアン荒地.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.オブシディアン荒地)) },
                { R.オリオン廃街.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.オリオン廃街)) },
                { R.カペラ隕石跡.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.カペラ隕石跡)) },
                { R.キンカッシュ古戦場跡.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.キンカッシュ古戦場跡)) },
                { R.クダン丘陵.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.クダン丘陵)) },
                { R.クノーラ雪原.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.クノーラ雪原)) },
                { R.クラウス山脈.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.クラウス山脈)) },
                { R.クローディア水源.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.クローディア水源)) },
                { R.グランフォーク河口.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.グランフォーク河口)) },
                { R.ゴブリンフォーク.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.ゴブリンフォーク)) },
                { R.ザーク古戦場跡.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.ザーク古戦場跡)) },
                { R.シディット水域.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.シディット水域)) },
                { R.シバーグ遺跡.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.シバーグ遺跡)) },
                { R.シュア島古戦場跡.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.シュア島古戦場跡)) },
                { R.ジャコル丘陵.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.ジャコル丘陵)) },
                { R.スピカ隕石跡.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.スピカ隕石跡)) },
                { R.セノビア荒地.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.セノビア荒地)) },
                { R.セルベーン高地.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.セルベーン高地)) },
                { R.セントウォーク高地.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.セントウォーク高地)) },
                { R.ソーン平原.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.ソーン平原)) },
                { R.タマライア水源.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.タマライア水源)) },
                { R.ダガー島.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.ダガー島)) },
                { R.デスパイア山麓.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.デスパイア山麓)) },
                { R.ドランゴラ荒地.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.ドランゴラ荒地)) },
                { R.ニコナ街道.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.ニコナ街道)) },
                { R.ネフタル雪原.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.ネフタル雪原)) },
                { R.ノイム草原.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.ノイム草原)) },
                { R.フェブェ雪原.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.フェブェ雪原)) },
                { R.ブリザール湿原.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.ブリザール湿原)) },
                { R.ブローデン古戦場跡.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.ブローデン古戦場跡)) },
                { R.ヘイムダル荒地.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.ヘイムダル荒地)) },
                { R.ベルタ平原.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.ベルタ平原)) },
                { R.ホークウィンド高地.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.ホークウィンド高地)) },
                { R.マスクス水源.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.マスクス水源)) },
                { R.ラインレイ渓谷.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.ラインレイ渓谷)) },
                { R.ラナス城跡.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.ラナス城跡)) },
                { R.ルダン雪原.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.ルダン雪原)) },
                { R.ルード雪原.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.ルード雪原)) },
                { R.レイクパス荒地.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.レイクパス荒地)) },
                { R.ログマール古戦場跡.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.ログマール古戦場跡)) },
                { R.ロザリオ高地.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.ロザリオ高地)) },
                { R.ロッシ雪原.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.ロッシ雪原)) },
                { R.ローグローブ台地.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.ローグローブ台地)) },
                { R.ワーグノスの地.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.ワーグノスの地)) },
                { R.ワードノール平原.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.ワードノール平原)) },
                { R.始まりの大地.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.始まりの大地)) },
                { R.闘技場.FillPaddingToZero().SHA1Hash(), new Map(nameof(R.闘技場)) },
            };
        }
    }
}

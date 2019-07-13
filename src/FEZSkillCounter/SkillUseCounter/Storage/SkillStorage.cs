﻿using SkillUseCounter.Entity;
using SkillUseCounter.Extension;
using System.Collections.Generic;

using R = SkillUseCounter.Properties.Resource;

namespace SkillUseCounter.Storage
{
    internal class SkillStorage
    {
        public static IReadOnlyDictionary<string, Skill> Table;

        public static void Create()
        {
            Table = new Dictionary<string, Skill>()
            {
                { R.Cestus_アースバインド.SHA1Hash(), Skill.Create(nameof(R.Cestus_アースバインド), "バインド", 42) },
                { R.Cestus_アースバインド_S.SHA1Hash(), Skill.Create(nameof(R.Cestus_アースバインド_S),"バインド", 42) },
                { R.Cestus_インテンスファイ.SHA1Hash(), Skill.Create(nameof(R.Cestus_インテンスファイ), "ファイ", 30) },
                { R.Cestus_インテンスファイ_S.SHA1Hash(), Skill.Create(nameof(R.Cestus_インテンスファイ_S), "ファイ", 30) },
                { R.Cestus_エナジースフィア.SHA1Hash(), Skill.Create(nameof(R.Cestus_エナジースフィア), "スフィア", 25) },
                { R.Cestus_エナジースフィア_S.SHA1Hash(), Skill.Create(nameof(R.Cestus_エナジースフィア_S), "スフィア", 25) },
                { R.Cestus_エンダーレイド.SHA1Hash(), Skill.Create(nameof(R.Cestus_エンダーレイド), "エンダー", 10) },
                { R.Cestus_エンダーレイド_S.SHA1Hash(), Skill.Create(nameof(R.Cestus_エンダーレイド_S), "エンダー", 10) },
                { R.Cestus_ゲイザースマッシュ.SHA1Hash(), Skill.Create(nameof(R.Cestus_ゲイザースマッシュ), "ゲイザー", 35) },
                { R.Cestus_ゲイザースマッシュ_S.SHA1Hash(), Skill.Create(nameof(R.Cestus_ゲイザースマッシュ_S), "ゲイザー", 35) },
                { R.Cestus_サイクロンディザスター.SHA1Hash(), Skill.Create(nameof(R.Cestus_サイクロンディザスター), "サイクロン", 46) },
                { R.Cestus_サイクロンディザスター_S.SHA1Hash(), Skill.Create(nameof(R.Cestus_サイクロンディザスター_S), "サイクロン", 46) },
                { R.Cestus_シャットアウト.SHA1Hash(), Skill.Create(nameof(R.Cestus_シャットアウト), "サイクロン", 60) },
                { R.Cestus_シャットアウト_S.SHA1Hash(), Skill.Create(nameof(R.Cestus_シャットアウト_S), "サイクロン", 60) },
                { R.Cestus_ショックウェイブ.SHA1Hash(), Skill.Create(nameof(R.Cestus_ショックウェイブ), "ショック", 35) },
                { R.Cestus_ショックウェイブ_S.SHA1Hash(), Skill.Create(nameof(R.Cestus_ショックウェイブ_S), "ショック", 35) },
                { R.Cestus_ソリッドストライク.SHA1Hash(), Skill.Create(nameof(R.Cestus_ソリッドストライク), "ソリッド", 36) },
                { R.Cestus_ソリッドストライク_S.SHA1Hash(), Skill.Create(nameof(R.Cestus_ソリッドストライク_S), "ソリッド", 36) },
                { R.Cestus_タワードミネーション.SHA1Hash(), Skill.Create(nameof(R.Cestus_タワードミネーション), "タワードミネーション", 25) },
                { R.Cestus_タワードミネーション_S.SHA1Hash(), Skill.Create(nameof(R.Cestus_タワードミネーション_S), "タワードミネーション", 25) },
                { R.Cestus_ドレインクロー.SHA1Hash(), Skill.Create(nameof(R.Cestus_ドレインクロー), "ドレイン", 24) },
                { R.Cestus_ドレインクロー_S.SHA1Hash(), Skill.Create(nameof(R.Cestus_ドレインクロー_S), "ドレイン", 24) },
                { R.Cestus_ノックインパクト.SHA1Hash(), Skill.Create(nameof(R.Cestus_ノックインパクト), "ノックインパクト", 34) },
                { R.Cestus_ノックインパクト_S.SHA1Hash(), Skill.Create(nameof(R.Cestus_ノックインパクト_S), "ノックインパクト", 34) },
                { R.Cestus_ハードレインフォース.SHA1Hash(), Skill.Create(nameof(R.Cestus_ハードレインフォース), "ハドレ", 40) },
                { R.Cestus_ハードレインフォース_S.SHA1Hash(), Skill.Create(nameof(R.Cestus_ハードレインフォース_S), "ハドレ", 40) },
                { R.Cestus_ハームアクティベイト.SHA1Hash(), Skill.Create(nameof(R.Cestus_ハームアクティベイト), "ハーム", 24) },
                { R.Cestus_ハームアクティベイト_S.SHA1Hash(), Skill.Create(nameof(R.Cestus_ハームアクティベイト_S), "ハーム", 24) },
                { R.Cestus_ホーネットスティング.SHA1Hash(), Skill.Create(nameof(R.Cestus_ホーネットスティング), "ホネ", 25) },
                { R.Cestus_ホーネットスティング_S.SHA1Hash(), Skill.Create(nameof(R.Cestus_ホーネットスティング_S), "ホネ", 25) },
                //{ R.Cestus_通常攻撃.SHA1Hash(), Skill.CreateFromResourceFileName(nameof(R.Cestus_通常攻撃), "通常", 0) },
                //{ R.Cestus_通常攻撃_S.SHA1Hash(), Skill.CreateFromResourceFileName(nameof(R.Cestus_通常攻撃_S), "通常", 0) },

                { R.Fencer_アクセラレーション.SHA1Hash(), Skill.Create(nameof(R.Fencer_アクセラレーション), "アクセラレーション", 25, 28) },
                { R.Fencer_アクセラレーション_S.SHA1Hash(), Skill.Create(nameof(R.Fencer_アクセラレーション_S), "アクセラレーション", 25, 28) },
                { R.Fencer_イレイスマジック.SHA1Hash(), Skill.Create(nameof(R.Fencer_イレイスマジック), "イレイス", 30, 33) },
                { R.Fencer_イレイスマジック_S.SHA1Hash(), Skill.Create(nameof(R.Fencer_イレイスマジック_S), "イレイス", 30, 33) },
                { R.Fencer_エリアルフォール.SHA1Hash(), Skill.Create(nameof(R.Fencer_エリアルフォール), "エリアル", 25) },
                { R.Fencer_エリアルフォール_S.SHA1Hash(), Skill.Create(nameof(R.Fencer_エリアルフォール_S), "エリアル", 25) },
                { R.Fencer_オブティンプロテクト.SHA1Hash(), Skill.Create(nameof(R.Fencer_オブティンプロテクト), "オブティンプロテクト", 25, 28) },
                { R.Fencer_オブティンプロテクト_S.SHA1Hash(), Skill.Create(nameof(R.Fencer_オブティンプロテクト_S), "オブティンプロテクト", 25, 28) },
                { R.Fencer_シャイニングクロス.SHA1Hash(), Skill.Create(nameof(R.Fencer_シャイニングクロス), "ワロス", 24, 26, 28) },
                { R.Fencer_シャイニングクロス_S.SHA1Hash(), Skill.Create(nameof(R.Fencer_シャイニングクロス_S), "ワロス", 24, 26, 28) },
                { R.Fencer_ストライクダウン.SHA1Hash(), Skill.Create(nameof(R.Fencer_ストライクダウン), "SD", 20, 24, 28) },
                { R.Fencer_ストライクダウン_S.SHA1Hash(), Skill.Create(nameof(R.Fencer_ストライクダウン_S), "SD", 20, 24, 28) },
                { R.Fencer_テンペストピアス.SHA1Hash(), Skill.Create(nameof(R.Fencer_テンペストピアス), "テンペスト", 28) },
                { R.Fencer_テンペストピアス_S.SHA1Hash(), Skill.Create(nameof(R.Fencer_テンペストピアス_S), "テンペスト", 28) },
                { R.Fencer_デュアルストライク.SHA1Hash(), Skill.Create(nameof(R.Fencer_デュアルストライク), "デュアル", 20) },
                { R.Fencer_デュアルストライク_S.SHA1Hash(), Skill.Create(nameof(R.Fencer_デュアルストライク_S), "デュアル", 20) },
                { R.Fencer_フィニッシュスラスト.SHA1Hash(), Skill.Create(nameof(R.Fencer_フィニッシュスラスト), "フィニ", 50) },
                { R.Fencer_フィニッシュスラスト_S.SHA1Hash(), Skill.Create(nameof(R.Fencer_フィニッシュスラスト_S), "フィニ", 50) },
                { R.Fencer_フラッシュスティンガー.SHA1Hash(), Skill.Create(nameof(R.Fencer_フラッシュスティンガー), "フラッシュ", 30) },
                { R.Fencer_フラッシュスティンガー_S.SHA1Hash(), Skill.Create(nameof(R.Fencer_フラッシュスティンガー_S), "フラッシュ", 30) },
                { R.Fencer_ペネトレイトスラスト.SHA1Hash(), Skill.Create(nameof(R.Fencer_ペネトレイトスラスト), "ペネ", 28) },
                { R.Fencer_ペネトレイトスラスト_S.SHA1Hash(), Skill.Create(nameof(R.Fencer_ペネトレイトスラスト_S), "ペネ", 28) },
                { R.Fencer_ラピッドファンデヴ.SHA1Hash(), Skill.Create(nameof(R.Fencer_ラピッドファンデヴ), "ラピッド", 10, 14) },
                { R.Fencer_ラピッドファンデヴ_S.SHA1Hash(), Skill.Create(nameof(R.Fencer_ラピッドファンデヴ_S), "ラピッド", 10, 14) },
                { R.Fencer_リバースキック.SHA1Hash(), Skill.Create(nameof(R.Fencer_リバースキック), "リバキ", 16) },
                { R.Fencer_リバースキック_S.SHA1Hash(), Skill.Create(nameof(R.Fencer_リバースキック_S), "リバキ", 16) },
                { R.Fencer_ヴィガーエイド.SHA1Hash(), Skill.Create(nameof(R.Fencer_ヴィガーエイド), "ヴィガー", 25, 28) },
                { R.Fencer_ヴィガーエイド_S.SHA1Hash(), Skill.Create(nameof(R.Fencer_ヴィガーエイド_S), "ヴィガー", 25, 28) },
                //{ R.Fencer_通常攻撃.SHA1Hash(), Skill.CreateFromResourceFileName(nameof(R.Fencer_通常攻撃), "通常", 0) },
                //{ R.Fencer_通常攻撃_S.SHA1Hash(), Skill.CreateFromResourceFileName(nameof(R.Fencer_通常攻撃_S), "通常", 0) },
                
                { R.Scout_アローレイン.SHA1Hash(), Skill.Create(nameof(R.Scout_アローレイン), "レイン", 36) },
                { R.Scout_アローレイン_S.SHA1Hash(), Skill.Create(nameof(R.Scout_アローレイン_S), "レイン", 36) },
                { R.Scout_アームブレイク.SHA1Hash(), Skill.Create(nameof(R.Scout_アームブレイク), "アムブレ", 16) },
                { R.Scout_アームブレイク_S.SHA1Hash(), Skill.Create(nameof(R.Scout_アームブレイク_S), "アムブレ", 16) },
                { R.Scout_イーグルショット.SHA1Hash(), Skill.Create(nameof(R.Scout_イーグルショット), "イーグル", 15) },
                { R.Scout_イーグルショット_S.SHA1Hash(), Skill.Create(nameof(R.Scout_イーグルショット_S), "イーグル", 15) },
                { R.Scout_エアレイド.SHA1Hash(), Skill.Create(nameof(R.Scout_エアレイド), "エアレイド", 10) },
                { R.Scout_エアレイド_S.SHA1Hash(), Skill.Create(nameof(R.Scout_エアレイド_S), "エアレイド", 10) },
                { R.Scout_ガードブレイク.SHA1Hash(), Skill.Create(nameof(R.Scout_ガードブレイク), "ガドブレ", 14) },
                { R.Scout_ガードブレイク_S.SHA1Hash(), Skill.Create(nameof(R.Scout_ガードブレイク_S), "ガドブレ", 14) },
                { R.Scout_スパイダーウェブ.SHA1Hash(), Skill.Create(nameof(R.Scout_スパイダーウェブ), "蜘蛛矢", 18) },
                { R.Scout_スパイダーウェブ_S.SHA1Hash(), Skill.Create(nameof(R.Scout_スパイダーウェブ_S), "蜘蛛矢", 18) },
                { R.Scout_ダガーストライク.SHA1Hash(), Skill.Create(nameof(R.Scout_ダガーストライク), "ダガスト", 22) },
                { R.Scout_ダガーストライク_S.SHA1Hash(), Skill.Create(nameof(R.Scout_ダガーストライク_S), "ダガスト", 22) },
                { R.Scout_トゥルーショット.SHA1Hash(), Skill.Create(nameof(R.Scout_トゥルーショット), "トゥルー", 18) },
                { R.Scout_トゥルーショット_S.SHA1Hash(), Skill.Create(nameof(R.Scout_トゥルーショット_S), "トゥルー", 18) },
                { R.Scout_ドッジシュート.SHA1Hash(), Skill.Create(nameof(R.Scout_ドッジシュート), "ドッジシュート", 28) },
                { R.Scout_ドッジシュート_S.SHA1Hash(), Skill.Create(nameof(R.Scout_ドッジシュート_S), "ドッジシュート", 28) },
                { R.Scout_ハイド.SHA1Hash(), Skill.Create(nameof(R.Scout_ハイド), "ハイド", 32) },
                { R.Scout_ハイド_S.SHA1Hash(), Skill.Create(nameof(R.Scout_ハイド_S), "ハイド", 32) },
                { R.Scout_パニッシングストライク.SHA1Hash(), Skill.Create(nameof(R.Scout_パニッシングストライク), "パニ", 84) },
                { R.Scout_パニッシングストライク_D.SHA1Hash(), Skill.Create(nameof(R.Scout_パニッシングストライク_D), "パニ", 84) },
                { R.Scout_パニッシングストライク_S.SHA1Hash(), Skill.Create(nameof(R.Scout_パニッシングストライク_S), "パニ", 84) },
                { R.Scout_パワーシュート.SHA1Hash(), Skill.Create(nameof(R.Scout_パワーシュート), "パワシュ", 32) },
                { R.Scout_パワーシュート_S.SHA1Hash(), Skill.Create(nameof(R.Scout_パワーシュート_S), "パワシュ", 32) },
                { R.Scout_パワーブレイク.SHA1Hash(), Skill.Create(nameof(R.Scout_パワーブレイク), "パワブレ", 12) },
                { R.Scout_パワーブレイク_S.SHA1Hash(), Skill.Create(nameof(R.Scout_パワーブレイク_S), "パワブレ", 12) },
                { R.Scout_ピアッシングシュート.SHA1Hash(), Skill.Create(nameof(R.Scout_ピアッシングシュート), "ピア", 76) },
                { R.Scout_ピアッシングシュート_S.SHA1Hash(), Skill.Create(nameof(R.Scout_ピアッシングシュート_S), "ピア", 76) },
                { R.Scout_ブレイズショット.SHA1Hash(), Skill.Create(nameof(R.Scout_ブレイズショット), "ブレイズ", 25) },
                { R.Scout_ブレイズショット_S.SHA1Hash(), Skill.Create(nameof(R.Scout_ブレイズショット_S), "ブレイズ", 25) },
                { R.Scout_ポイズンショット.SHA1Hash(), Skill.Create(nameof(R.Scout_ポイズンショット), "毒矢", 22) },
                { R.Scout_ポイズンショット_S.SHA1Hash(), Skill.Create(nameof(R.Scout_ポイズンショット_S), "毒矢", 22) },
                { R.Scout_ポイズンブロウ.SHA1Hash(), Skill.Create(nameof(R.Scout_ポイズンブロウ), "毒霧", 12) },
                { R.Scout_ポイズンブロウ_S.SHA1Hash(), Skill.Create(nameof(R.Scout_ポイズンブロウ_S), "毒霧", 12) },
                { R.Scout_レッグブレイク.SHA1Hash(), Skill.Create(nameof(R.Scout_レッグブレイク), "レグブレ", 15) },
                { R.Scout_レッグブレイク_S.SHA1Hash(), Skill.Create(nameof(R.Scout_レッグブレイク_S), "レグブレ", 15) },
                { R.Scout_ヴァイパーバイト.SHA1Hash(), Skill.Create(nameof(R.Scout_ヴァイパーバイト), "ヴァイパー", 18) },
                { R.Scout_ヴァイパーバイト_S.SHA1Hash(), Skill.Create(nameof(R.Scout_ヴァイパーバイト_S), "ヴァイパー", 18) },
                { R.Scout_ヴォイドダークネス.SHA1Hash(), Skill.Create(nameof(R.Scout_ヴォイドダークネス), "ヴォイド", 32) },
                { R.Scout_ヴォイドダークネス_S.SHA1Hash(), Skill.Create(nameof(R.Scout_ヴォイドダークネス_S), "ヴォイド", 32) },
                // 銃は非対応
                //{ R.Scout_クイックビート.SHA1Hash(), Skill.CreateFromResourceFileName(nameof(R.Scout_クイックビート),"クイックビート", 34) },
                //{ R.Scout_クイックビート_S.SHA1Hash(), Skill.CreateFromResourceFileName(nameof(R.Scout_クイックビート_S), "クイックビート", 34) },
                //{ R.Scout_クラッシュショット.SHA1Hash(), Skill.CreateFromResourceFileName(nameof(R.Scout_クラッシュショット), "クラッシュ", 50) },
                //{ R.Scout_クラッシュショット_S.SHA1Hash(), Skill.CreateFromResourceFileName(nameof(R.Scout_クラッシュショット_S), "クラッシュ", 50) },
                //{ R.Scout_コメットキャノン.SHA1Hash(), Skill.CreateFromResourceFileName(nameof(R.Scout_コメットキャノン), "コメット", 21) },
                //{ R.Scout_コメットキャノン_S.SHA1Hash(), Skill.CreateFromResourceFileName(nameof(R.Scout_コメットキャノン_S), "コメット", 21) },
                //{ R.Scout_スウィープキャノン.SHA1Hash(), Skill.CreateFromResourceFileName(nameof(R.Scout_スウィープキャノン), "スウィープ", 40) },
                //{ R.Scout_スウィープキャノン_S.SHA1Hash(), Skill.CreateFromResourceFileName(nameof(R.Scout_スウィープキャノン_S), "スウィープ", 40) },
                //{ R.Scout_バーストキャノン.SHA1Hash(), Skill.CreateFromResourceFileName(nameof(R.Scout_バーストキャノン), "バーストキャノン", 82) },
                //{ R.Scout_バーストキャノン_S.SHA1Hash(), Skill.CreateFromResourceFileName(nameof(R.Scout_バーストキャノン_S), "バーストキャノン", 82) },
                //{ R.Scout_ファストショット.SHA1Hash(), Skill.CreateFromResourceFileName(nameof(R.Scout_ファストショット), "ファストショット", 30) },
                //{ R.Scout_ファストショット_S.SHA1Hash(), Skill.CreateFromResourceFileName(nameof(R.Scout_ファストショット_S), "ファストショット", 30) },
                //{ R.Scout_フリックショット.SHA1Hash(), Skill.CreateFromResourceFileName(nameof(R.Scout_フリックショット), "フリックショット", 22) },
                //{ R.Scout_フリックショット_S.SHA1Hash(), Skill.CreateFromResourceFileName(nameof(R.Scout_フリックショット_S), "フリックショット", 22) },
                //{ R.Scout_ホワイトバレット.SHA1Hash(), Skill.CreateFromResourceFileName(nameof(R.Scout_ホワイトバレット), "ホワイトバレット", 30) },
                //{ R.Scout_ホワイトバレット_S.SHA1Hash(), Skill.CreateFromResourceFileName(nameof(R.Scout_ホワイトバレット_S), "ホワイトバレット", 30) },
                //{ R.Scout_ラッシュバレット.SHA1Hash(), Skill.CreateFromResourceFileName(nameof(R.Scout_ラッシュバレット), "ラッシュバレット", 32) },
                //{ R.Scout_ラッシュバレット_S.SHA1Hash(), Skill.CreateFromResourceFileName(nameof(R.Scout_ラッシュバレット_S), "ラッシュバレット", 32) },
                //{ R.Scout_通常攻撃.SHA1Hash(), Skill.CreateFromResourceFileName(nameof(R.Scout_通常攻撃), "通常", 0) },
                //{ R.Scout_通常攻撃_S.SHA1Hash(), Skill.CreateFromResourceFileName(nameof(R.Scout_通常攻撃_S), "通常", 0) },

                { R.Sorcerer_アイスジャベリン.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_アイスジャベリン), "ジャベ", 30) },
                { R.Sorcerer_アイスジャベリン_D.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_アイスジャベリン_D), "ジャベ", 30) },
                { R.Sorcerer_アイスジャベリン_S.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_アイスジャベリン_S), "ジャベ", 30) },
                { R.Sorcerer_アイスターゲット.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_アイスターゲット), "アイタゲ", 34) },
                { R.Sorcerer_アイスターゲット_D.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_アイスターゲット_D), "アイタゲ", 34) },
                { R.Sorcerer_アイスターゲット_S.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_アイスターゲット_S), "アイタゲ", 34) },
                { R.Sorcerer_アイスボルト.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_アイスボルト), "IB", 18) },
                { R.Sorcerer_アイスボルト_S.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_アイスボルト_S), "IB", 18) },
                { R.Sorcerer_エレキドライブ.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_エレキドライブ), "IB", 36) },
                { R.Sorcerer_エレキドライブ_D.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_エレキドライブ_D), "エレキ", 36) },
                { R.Sorcerer_エレキドライブ_S.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_エレキドライブ_S), "エレキ", 36) },
                { R.Sorcerer_グラビティキャプチャー.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_グラビティキャプチャー), "重力", 72) },
                { R.Sorcerer_グラビティキャプチャー_D.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_グラビティキャプチャー_D), "重力", 72) },
                { R.Sorcerer_グラビティキャプチャー_S.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_グラビティキャプチャー_S), "重力", 72) },
                { R.Sorcerer_サンダーボルト.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_サンダーボルト), "サンボル", 33) },
                { R.Sorcerer_サンダーボルト_D.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_サンダーボルト_D), "サンボル", 33) },
                { R.Sorcerer_サンダーボルト_S.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_サンダーボルト_S), "サンボル", 33) },
                { R.Sorcerer_ジャッジメントレイ.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_ジャッジメントレイ), "ジャッジ", 80) },
                { R.Sorcerer_ジャッジメントレイ_D.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_ジャッジメントレイ_D), "ジャッジ", 80) },
                { R.Sorcerer_ジャッジメントレイ_S.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_ジャッジメントレイ_S), "ジャッジ", 80) },
                { R.Sorcerer_スパークフレア.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_スパークフレア), "スパーク", 65) },
                { R.Sorcerer_スパークフレア_D.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_スパークフレア_D), "スパーク", 65) },
                { R.Sorcerer_スパークフレア_S.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_スパークフレア_S), "スパーク", 65) },
                { R.Sorcerer_ファイア.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_ファイア), "ファイア", 15) },
                { R.Sorcerer_ファイア_S.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_ファイア_S), "ファイア", 15) },
                { R.Sorcerer_ファイアランス.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_ファイアランス), "ランス", 32) },
                { R.Sorcerer_ファイアランス_D.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_ファイアランス_D), "ランス", 32) },
                { R.Sorcerer_ファイアランス_S.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_ファイアランス_S), "ランス", 32) },
                { R.Sorcerer_フリージングウェイブ.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_フリージングウェイブ), "ウェイブ", 44) },
                { R.Sorcerer_フリージングウェイブ_D.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_フリージングウェイブ_D), "ウェイブ", 44) },
                { R.Sorcerer_フリージングウェイブ_S.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_フリージングウェイブ_S), "ウェイブ", 44) },
                { R.Sorcerer_フレイムサークル.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_フレイムサークル), "フレイムサークル", 40) },
                { R.Sorcerer_フレイムサークル_D.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_フレイムサークル_D), "フレイムサークル", 40) },
                { R.Sorcerer_フレイムサークル_S.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_フレイムサークル_S), "フレイムサークル", 40) },
                { R.Sorcerer_ブリザードカレス.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_ブリザードカレス), "カレス", 68) },
                { R.Sorcerer_ブリザードカレス_D.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_ブリザードカレス_D), "カレス", 68) },
                { R.Sorcerer_ブリザードカレス_S.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_ブリザードカレス_S), "カレス", 68) },
                { R.Sorcerer_ヘルファイア.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_ヘルファイア), "ヘル", 72) },
                { R.Sorcerer_ヘルファイア_D.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_ヘルファイア_D), "ヘル", 72) },
                { R.Sorcerer_ヘルファイア_S.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_ヘルファイア_S), "ヘル", 72) },
                { R.Sorcerer_メテオインパクト.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_メテオインパクト), "メテオ", 68) },
                { R.Sorcerer_メテオインパクト_D.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_メテオインパクト_D), "メテオ", 68) },
                { R.Sorcerer_メテオインパクト_S.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_メテオインパクト_S), "メテオ", 68) },
                { R.Sorcerer_ライトニング.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_ライトニング), "ライト", 18) },
                { R.Sorcerer_ライトニング_S.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_ライトニング_S), "ライト", 18) },
                { R.Sorcerer_ライトニングスピア.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_ライトニングスピア), "スピア", 32) },
                { R.Sorcerer_ライトニングスピア_D.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_ライトニングスピア_D), "スピア", 32) },
                { R.Sorcerer_ライトニングスピア_S.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_ライトニングスピア_S), "スピア", 32) },
                { R.Sorcerer_レーザーブラスト.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_レーザーブラスト), "レーザー", 68) },
                { R.Sorcerer_レーザーブラスト_D.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_レーザーブラスト_D), "レーザー", 68) },
                { R.Sorcerer_レーザーブラスト_S.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_レーザーブラスト_S), "レーザー", 68) },
                { R.Sorcerer_詠唱.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_詠唱), "詠唱", 20) },
                { R.Sorcerer_詠唱_S.SHA1Hash(), Skill.Create(nameof(R.Sorcerer_詠唱_S), "詠唱", 20) },
                //{ R.Sorcerer_通常攻撃.SHA1Hash(), Skill.CreateFromResourceFileName(nameof(R.Sorcerer_通常攻撃), "通常", 0) },
                //{ R.Sorcerer_通常攻撃_S.SHA1Hash(), Skill.CreateFromResourceFileName(nameof(R.Sorcerer_通常攻撃_S), "通常", 0) },

                { R.Warrior_アサルトエッジ.SHA1Hash(), Skill.Create(nameof(R.Warrior_アサルトエッジ), "アサルト", 42) },
                { R.Warrior_アサルトエッジ_S.SHA1Hash(), Skill.Create(nameof(R.Warrior_アサルトエッジ_S), "アサルト", 42) },
                { R.Warrior_アタックレインフォース.SHA1Hash(), Skill.Create(nameof(R.Warrior_アタックレインフォース), "アタレ", 40) },
                { R.Warrior_アタックレインフォース_S.SHA1Hash(), Skill.Create(nameof(R.Warrior_アタックレインフォース_S), "アタレ", 40) },
                { R.Warrior_アーススタンプ.SHA1Hash(), Skill.Create(nameof(R.Warrior_アーススタンプ), "スタンプ", 36) },
                { R.Warrior_アーススタンプ_S.SHA1Hash(), Skill.Create(nameof(R.Warrior_アーススタンプ_S), "スタンプ", 36) },
                { R.Warrior_エクステンブレイド.SHA1Hash(), Skill.Create(nameof(R.Warrior_エクステンブレイド), "エクス", 35) },
                { R.Warrior_エクステンブレイド_S.SHA1Hash(), Skill.Create(nameof(R.Warrior_エクステンブレイド_S), "エクス", 35) },
                { R.Warrior_エンダーペイン.SHA1Hash(), Skill.Create(nameof(R.Warrior_エンダーペイン), "エンダー", 10) },
                { R.Warrior_エンダーペイン_S.SHA1Hash(), Skill.Create(nameof(R.Warrior_エンダーペイン_S), "エンダー", 10) },
                { R.Warrior_ガードレインフォース.SHA1Hash(), Skill.Create(nameof(R.Warrior_ガードレインフォース), "ガドレ", 40) },
                { R.Warrior_ガードレインフォース_S.SHA1Hash(), Skill.Create(nameof(R.Warrior_ガードレインフォース_S), "ガドレ", 40) },
                { R.Warrior_クラックバング.SHA1Hash(), Skill.Create(nameof(R.Warrior_クラックバング), "クラック", 76) },
                { R.Warrior_クラックバング_S.SHA1Hash(), Skill.Create(nameof(R.Warrior_クラックバング_S), "クラック", 76) },
                { R.Warrior_クランブルストーム.SHA1Hash(), Skill.Create(nameof(R.Warrior_クランブルストーム), "クラン", 60) },
                { R.Warrior_クランブルストーム_S.SHA1Hash(), Skill.Create(nameof(R.Warrior_クランブルストーム_S), "クラン", 60) },
                { R.Warrior_シールドバッシュ.SHA1Hash(), Skill.Create(nameof(R.Warrior_シールドバッシュ), "バッシュ", 40) },
                { R.Warrior_シールドバッシュ_S.SHA1Hash(), Skill.Create(nameof(R.Warrior_シールドバッシュ_S), "バッシュ", 40) },
                { R.Warrior_ストライクスマッシュ.SHA1Hash(), Skill.Create(nameof(R.Warrior_ストライクスマッシュ), "ストスマ", 28) },
                { R.Warrior_ストライクスマッシュ_S.SHA1Hash(), Skill.Create(nameof(R.Warrior_ストライクスマッシュ_S), "ストスマ", 28) },
                { R.Warrior_スマッシュ.SHA1Hash(), Skill.Create(nameof(R.Warrior_スマッシュ), "スマ", 12) },
                { R.Warrior_スマッシュ_S.SHA1Hash(), Skill.Create(nameof(R.Warrior_スマッシュ_S), "スマ", 12) },
                { R.Warrior_スラムアタック.SHA1Hash(), Skill.Create(nameof(R.Warrior_スラムアタック), "スラム", 18) },
                { R.Warrior_スラムアタック_S.SHA1Hash(), Skill.Create(nameof(R.Warrior_スラムアタック_S), "スラム", 18) },
                { R.Warrior_ソニックブーム.SHA1Hash(), Skill.Create(nameof(R.Warrior_ソニックブーム), "ソニック", 15) },
                { R.Warrior_ソニックブーム_S.SHA1Hash(), Skill.Create(nameof(R.Warrior_ソニックブーム_S), "ソニック", 15) },
                { R.Warrior_ソリッドウォール.SHA1Hash(), Skill.Create(nameof(R.Warrior_ソリッドウォール), "ソリッド", 30) },
                { R.Warrior_ソリッドウォール_S.SHA1Hash(), Skill.Create(nameof(R.Warrior_ソリッドウォール_S), "ソリッド", 30) },
                { R.Warrior_ソードランページ.SHA1Hash(), Skill.Create(nameof(R.Warrior_ソードランページ), "ランペ", 60) },
                { R.Warrior_ソードランページ_S.SHA1Hash(), Skill.Create(nameof(R.Warrior_ソードランページ_S), "ランペ", 60) },
                { R.Warrior_ドラゴンテイル.SHA1Hash(), Skill.Create(nameof(R.Warrior_ドラゴンテイル), "ドラテ", 60) },
                { R.Warrior_ドラゴンテイル_S.SHA1Hash(), Skill.Create(nameof(R.Warrior_ドラゴンテイル_S), "ドラテ", 60) },
                { R.Warrior_フォースインパクト.SHA1Hash(), Skill.Create(nameof(R.Warrior_フォースインパクト), "フォース", 26) },
                { R.Warrior_フォースインパクト_S.SHA1Hash(), Skill.Create(nameof(R.Warrior_フォースインパクト_S), "フォース", 26) },
                { R.Warrior_ブレイズスラッシュ.SHA1Hash(), Skill.Create(nameof(R.Warrior_ブレイズスラッシュ), "ブレイズ", 12) },
                { R.Warrior_ブレイズスラッシュ_S.SHA1Hash(), Skill.Create(nameof(R.Warrior_ブレイズスラッシュ_S), "ブレイズ", 12) },
                { R.Warrior_ヘビースマッシュ.SHA1Hash(), Skill.Create(nameof(R.Warrior_ヘビースマッシュ), "ヘビスマ", 34) },
                { R.Warrior_ヘビースマッシュ_S.SHA1Hash(), Skill.Create(nameof(R.Warrior_ヘビースマッシュ_S), "ヘビスマ", 34) },
                { R.Warrior_ベヒモステイル.SHA1Hash(), Skill.Create(nameof(R.Warrior_ベヒモステイル), "ベヒ", 32) },
                { R.Warrior_ベヒモステイル_S.SHA1Hash(), Skill.Create(nameof(R.Warrior_ベヒモステイル_S), "ベヒ", 32) },
                //{ R.Warrior_通常攻撃.SHA1Hash(), Skill.CreateFromResourceFileName(nameof(R.Warrior_通常攻撃), 0) },
                //{ R.Warrior_通常攻撃_S.SHA1Hash(), Skill.CreateFromResourceFileName(nameof(R.Warrior_通常攻撃_S), 0) },
            };
        }
    }
}

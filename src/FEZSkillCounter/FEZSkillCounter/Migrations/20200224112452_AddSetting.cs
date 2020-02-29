using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FEZSkillCounter.Migrations
{
    public partial class AddSetting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "EnchantSpellNotifyTimeSpan",
                table: "SettingDbSet",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAllwaysOnTop",
                table: "SettingDbSet",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnchantSpellNotifyTimeSpan",
                table: "SettingDbSet");

            migrationBuilder.DropColumn(
                name: "IsAllwaysOnTop",
                table: "SettingDbSet");
        }
    }
}

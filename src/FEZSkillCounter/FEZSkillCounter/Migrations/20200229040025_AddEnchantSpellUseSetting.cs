using Microsoft.EntityFrameworkCore.Migrations;

namespace FEZSkillCounter.Migrations
{
    public partial class AddEnchantSpellUseSetting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsNotifyEnchantUses",
                table: "SettingDbSet",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsNotifySpellUses",
                table: "SettingDbSet",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsNotifyEnchantUses",
                table: "SettingDbSet");

            migrationBuilder.DropColumn(
                name: "IsNotifySpellUses",
                table: "SettingDbSet");
        }
    }
}

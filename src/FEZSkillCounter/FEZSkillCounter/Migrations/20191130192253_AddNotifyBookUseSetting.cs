using Microsoft.EntityFrameworkCore.Migrations;

namespace FEZSkillCounter.Migrations
{
    public partial class AddNotifyBookUseSetting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsNotifyBookUses",
                table: "SettingDbSet",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsNotifyBookUses",
                table: "SettingDbSet");
        }
    }
}

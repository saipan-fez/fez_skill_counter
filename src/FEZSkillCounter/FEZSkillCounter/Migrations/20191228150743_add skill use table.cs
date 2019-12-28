using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FEZSkillCounter.Migrations
{
    public partial class addskillusetable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SkillUseEntity",
                columns: table => new
                {
                    SkillUseId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UseDate = table.Column<DateTime>(nullable: false),
                    SkillName = table.Column<string>(nullable: true),
                    SkillShortName = table.Column<string>(nullable: true),
                    ParentSkillCountId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillUseEntity", x => x.SkillUseId);
                    table.ForeignKey(
                        name: "FK_SkillUseEntity_SkillCountDbSet_ParentSkillCountId",
                        column: x => x.ParentSkillCountId,
                        principalTable: "SkillCountDbSet",
                        principalColumn: "SkillCountId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SkillUseEntity_ParentSkillCountId",
                table: "SkillUseEntity",
                column: "ParentSkillCountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SkillUseEntity");
        }
    }
}

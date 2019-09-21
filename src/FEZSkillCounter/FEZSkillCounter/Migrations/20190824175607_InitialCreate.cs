using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FEZSkillCounter.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SkillCountDbSet",
                columns: table => new
                {
                    SkillCountId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RecordDate = table.Column<DateTime>(nullable: false),
                    MapName = table.Column<string>(nullable: true),
                    WorkName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillCountDbSet", x => x.SkillCountId);
                });

            migrationBuilder.CreateTable(
                name: "SkillCountDetailEntity",
                columns: table => new
                {
                    SkillCountDetailId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SkillName = table.Column<string>(nullable: true),
                    SkillShortName = table.Column<string>(nullable: true),
                    WorkName = table.Column<string>(nullable: true),
                    Count = table.Column<int>(nullable: false),
                    ParentSkillCountId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillCountDetailEntity", x => x.SkillCountDetailId);
                    table.ForeignKey(
                        name: "FK_SkillCountDetailEntity_SkillCountDbSet_ParentSkillCountId",
                        column: x => x.ParentSkillCountId,
                        principalTable: "SkillCountDbSet",
                        principalColumn: "SkillCountId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SkillCountDetailEntity_ParentSkillCountId",
                table: "SkillCountDetailEntity",
                column: "ParentSkillCountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SkillCountDetailEntity");

            migrationBuilder.DropTable(
                name: "SkillCountDbSet");
        }
    }
}

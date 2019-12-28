﻿// <auto-generated />
using System;
using FEZSkillCounter.Model.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FEZSkillCounter.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20191228150743_add skill use table")]
    partial class addskillusetable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity("FEZSkillCounter.Model.Entity.SettingEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsDebugModeEnabled");

                    b.Property<bool>("IsNotifyBookUses");

                    b.Property<bool>("IsSkillCountFileSave");

                    b.HasKey("Id");

                    b.ToTable("SettingDbSet");
                });

            modelBuilder.Entity("FEZSkillCounter.Model.Entity.SkillCountDetailEntity", b =>
                {
                    b.Property<int>("SkillCountDetailId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Count");

                    b.Property<int?>("ParentSkillCountId");

                    b.Property<string>("SkillName");

                    b.Property<string>("SkillShortName");

                    b.Property<string>("WorkName");

                    b.HasKey("SkillCountDetailId");

                    b.HasIndex("ParentSkillCountId");

                    b.ToTable("SkillCountDetailEntity");
                });

            modelBuilder.Entity("FEZSkillCounter.Model.Entity.SkillCountEntity", b =>
                {
                    b.Property<int>("SkillCountId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("MapName");

                    b.Property<DateTime>("RecordDate");

                    b.Property<string>("WorkName");

                    b.HasKey("SkillCountId");

                    b.ToTable("SkillCountDbSet");
                });

            modelBuilder.Entity("FEZSkillCounter.Model.Entity.SkillUseEntity", b =>
                {
                    b.Property<int>("SkillUseId")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ParentSkillCountId");

                    b.Property<string>("SkillName");

                    b.Property<string>("SkillShortName");

                    b.Property<DateTime>("UseDate");

                    b.HasKey("SkillUseId");

                    b.HasIndex("ParentSkillCountId");

                    b.ToTable("SkillUseEntity");
                });

            modelBuilder.Entity("FEZSkillCounter.Model.Entity.SkillCountDetailEntity", b =>
                {
                    b.HasOne("FEZSkillCounter.Model.Entity.SkillCountEntity", "Parent")
                        .WithMany("Details")
                        .HasForeignKey("ParentSkillCountId");
                });

            modelBuilder.Entity("FEZSkillCounter.Model.Entity.SkillUseEntity", b =>
                {
                    b.HasOne("FEZSkillCounter.Model.Entity.SkillCountEntity", "Parent")
                        .WithMany("SkillUseHitories")
                        .HasForeignKey("ParentSkillCountId");
                });
#pragma warning restore 612, 618
        }
    }
}

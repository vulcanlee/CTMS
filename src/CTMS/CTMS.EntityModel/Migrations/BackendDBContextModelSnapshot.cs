﻿// <auto-generated />
using CTMS.EntityModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CTMS.EntityModel.Migrations
{
    [DbContext(typeof(BackendDBContext))]
    partial class BackendDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseCollation("Chinese_Taiwan_Stroke_CI_AS")
                .HasAnnotation("ProductVersion", "9.0.3");

            modelBuilder.Entity("CTMS.EntityModel.Models.Athlete", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ExamineTime")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ExcelData")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("FilesData")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("ProjectId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("Athlete");
                });

            modelBuilder.Entity("CTMS.EntityModel.Models.Examine", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AthleteId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ExamineTime")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ExcelData")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("FilesData")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AthleteId");

                    b.ToTable("Examine");
                });

            modelBuilder.Entity("CTMS.EntityModel.Models.MyUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Account")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("RoleJson")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("RoleViewId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Salt")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Status")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("RoleViewId");

                    b.ToTable("MyUser");
                });

            modelBuilder.Entity("CTMS.EntityModel.Models.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Project");
                });

            modelBuilder.Entity("CTMS.EntityModel.Models.RoleView", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("TabViewJson")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("RoleView");
                });

            modelBuilder.Entity("CTMS.EntityModel.Models.RoleViewProject", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ProjectId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RoleViewId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.HasIndex("RoleViewId");

                    b.ToTable("RoleViewProject");
                });

            modelBuilder.Entity("ProjectRoleView", b =>
                {
                    b.Property<int>("ProjectId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RoleViewId")
                        .HasColumnType("INTEGER");

                    b.HasKey("ProjectId", "RoleViewId");

                    b.HasIndex("RoleViewId");

                    b.ToTable("ProjectRoleView");
                });

            modelBuilder.Entity("CTMS.EntityModel.Models.Athlete", b =>
                {
                    b.HasOne("CTMS.EntityModel.Models.Project", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Project");
                });

            modelBuilder.Entity("CTMS.EntityModel.Models.Examine", b =>
                {
                    b.HasOne("CTMS.EntityModel.Models.Athlete", "Athlete")
                        .WithMany("Examine")
                        .HasForeignKey("AthleteId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Athlete");
                });

            modelBuilder.Entity("CTMS.EntityModel.Models.MyUser", b =>
                {
                    b.HasOne("CTMS.EntityModel.Models.RoleView", "RoleView")
                        .WithMany()
                        .HasForeignKey("RoleViewId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("RoleView");
                });

            modelBuilder.Entity("CTMS.EntityModel.Models.RoleViewProject", b =>
                {
                    b.HasOne("CTMS.EntityModel.Models.Project", "Project")
                        .WithMany("RoleViewProject")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("CTMS.EntityModel.Models.RoleView", "RoleView")
                        .WithMany("RoleViewProject")
                        .HasForeignKey("RoleViewId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Project");

                    b.Navigation("RoleView");
                });

            modelBuilder.Entity("ProjectRoleView", b =>
                {
                    b.HasOne("CTMS.EntityModel.Models.Project", null)
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("CTMS.EntityModel.Models.RoleView", null)
                        .WithMany()
                        .HasForeignKey("RoleViewId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("CTMS.EntityModel.Models.Athlete", b =>
                {
                    b.Navigation("Examine");
                });

            modelBuilder.Entity("CTMS.EntityModel.Models.Project", b =>
                {
                    b.Navigation("RoleViewProject");
                });

            modelBuilder.Entity("CTMS.EntityModel.Models.RoleView", b =>
                {
                    b.Navigation("RoleViewProject");
                });
#pragma warning restore 612, 618
        }
    }
}

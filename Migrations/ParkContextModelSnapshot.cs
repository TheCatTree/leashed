﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using leashApi.Models;

namespace leashApi.Migrations
{
    [DbContext(typeof(ParkContext))]
    partial class ParkContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("leashApi.Models.Dog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("character varying(255)")
                        .HasMaxLength(255);

                    b.Property<int>("UserDataId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserDataId");

                    b.ToTable("Dogs");
                });

            modelBuilder.Entity("leashApi.Models.ParkItem", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("character varying(255)")
                        .HasMaxLength(255);

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("character varying(255)")
                        .HasMaxLength(255);

                    b.Property<string>("IsLeashed")
                        .IsRequired()
                        .HasColumnType("character varying(255)")
                        .HasMaxLength(255);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("character varying(255)")
                        .HasMaxLength(255);

                    b.Property<bool>("RoadFront")
                        .HasColumnType("boolean");

                    b.Property<string>("Suburb")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("ParkItems");
                });

            modelBuilder.Entity("leashApi.Models.Picture", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("character varying(255)")
                        .HasMaxLength(255);

                    b.Property<bool>("IsPublic")
                        .HasColumnType("boolean");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("character varying(255)")
                        .HasMaxLength(255);

                    b.Property<int>("UserDataId")
                        .HasColumnType("integer");

                    b.Property<string[]>("canEdit")
                        .HasColumnType("text[]");

                    b.Property<string[]>("canRead")
                        .HasColumnType("text[]");

                    b.HasKey("Id");

                    b.HasIndex("UserDataId");

                    b.ToTable("Pictures");
                });

            modelBuilder.Entity("leashApi.Models.PictureDogJoin", b =>
                {
                    b.Property<int>("PictureId")
                        .HasColumnType("integer");

                    b.Property<int>("DogId")
                        .HasColumnType("integer");

                    b.HasKey("PictureId", "DogId");

                    b.HasIndex("DogId");

                    b.ToTable("PictureDogJoins");
                });

            modelBuilder.Entity("leashApi.Models.UserData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("character varying(255)")
                        .HasMaxLength(255);

                    b.Property<string>("TokenSub")
                        .IsRequired()
                        .HasColumnType("character varying(255)")
                        .HasMaxLength(255);

                    b.Property<int?>("UserDataId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserDataId");

                    b.ToTable("UserData");
                });

            modelBuilder.Entity("leashApi.Models.Dog", b =>
                {
                    b.HasOne("leashApi.Models.UserData", null)
                        .WithMany("Dogs")
                        .HasForeignKey("UserDataId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("leashApi.Models.Picture", b =>
                {
                    b.HasOne("leashApi.Models.UserData", null)
                        .WithMany("Pictures")
                        .HasForeignKey("UserDataId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("leashApi.Models.PictureDogJoin", b =>
                {
                    b.HasOne("leashApi.Models.Dog", "Dog")
                        .WithMany("PictureDogJoins")
                        .HasForeignKey("DogId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("leashApi.Models.Picture", "Picture")
                        .WithMany("PictureDogJoins")
                        .HasForeignKey("PictureId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("leashApi.Models.UserData", b =>
                {
                    b.HasOne("leashApi.Models.UserData", null)
                        .WithMany("friends")
                        .HasForeignKey("UserDataId");
                });
#pragma warning restore 612, 618
        }
    }
}

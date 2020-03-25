﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Storage.Laundry;

namespace Storage.Laundry.Migrations
{
    [DbContext(typeof(LaundryDbContext))]
    partial class LaundryDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Storage.Laundry.Models.AccountDetailsEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("AccountId")
                        .HasColumnName("accountId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedDateUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("ReaderId")
                        .HasColumnName("readerId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AccountId")
                        .IsUnique();

                    b.HasIndex("ReaderId");

                    b.ToTable("accountDetails");
                });

            modelBuilder.Entity("Storage.Laundry.Models.AccountEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("CreatedDateUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Email")
                        .HasColumnName("email")
                        .HasColumnType("text");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnName("login")
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnName("password")
                        .HasColumnType("text");

                    b.Property<string>("Roles")
                        .HasColumnType("text");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnName("name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("account");
                });

            modelBuilder.Entity("Storage.Laundry.Models.ClientEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<bool>("Active")
                        .HasColumnName("active")
                        .HasColumnType("boolean");

                    b.Property<string>("Address")
                        .HasColumnName("address")
                        .HasColumnType("text");

                    b.Property<int>("CityId")
                        .HasColumnName("cityId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedDateUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .HasColumnName("name")
                        .HasColumnType("text");

                    b.Property<int?>("ParentId")
                        .HasColumnName("parentId")
                        .HasColumnType("integer");

                    b.Property<string>("ShortName")
                        .HasColumnName("shortName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("client");
                });

            modelBuilder.Entity("Storage.Laundry.Models.ClientLinenEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("ClientId")
                        .HasColumnName("clientId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedDateUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("DepartmentId")
                        .HasColumnName("departmentId")
                        .HasColumnType("integer");

                    b.Property<int>("MasterLinenId")
                        .HasColumnName("masterLinenId")
                        .HasColumnType("integer");

                    b.Property<int>("PackingValue")
                        .HasColumnName("packingValue")
                        .HasColumnType("integer");

                    b.Property<string>("RfidTag")
                        .HasColumnName("rfidTag")
                        .HasColumnType("text");

                    b.Property<int?>("StaffId")
                        .HasColumnName("staffId")
                        .HasColumnType("integer");

                    b.Property<int>("StatusId")
                        .HasColumnName("statusId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("MasterLinenId");

                    b.HasIndex("StaffId");

                    b.ToTable("clientLinen");
                });

            modelBuilder.Entity("Storage.Laundry.Models.ClientStaffEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("CreatedDateUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("DepartmentId")
                        .HasColumnName("departmentId")
                        .HasColumnType("integer");

                    b.Property<string>("Email")
                        .HasColumnName("email")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnName("name")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnName("phoneNumber")
                        .HasColumnType("text");

                    b.Property<string>("StaffId")
                        .HasColumnName("staffId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("DepartmentId");

                    b.ToTable("clientStaff");
                });

            modelBuilder.Entity("Storage.Laundry.Models.ConveyorEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("BeltNumber")
                        .HasColumnName("beltNumber")
                        .HasColumnType("integer");

                    b.Property<int?>("ClientLinenId")
                        .HasColumnName("clientLinenId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedDateUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("SlotNumber")
                        .HasColumnName("slotNumber")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ClientLinenId");

                    b.ToTable("conveyor");
                });

            modelBuilder.Entity("Storage.Laundry.Models.DepartmentEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("ClientId")
                        .HasColumnName("clientId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedDateUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("DepartmentTypeId")
                        .HasColumnName("departmentTypeId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnName("name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("department");
                });

            modelBuilder.Entity("Storage.Laundry.Models.LaundryEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("CreatedDateUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasColumnName("description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnName("name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("laundry");
                });

            modelBuilder.Entity("Storage.Laundry.Models.MasterLinenEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("CreatedDateUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .HasColumnName("name")
                        .HasColumnType("text");

                    b.Property<int>("PackingValue")
                        .HasColumnName("packingValue")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("masterLinen");
                });

            modelBuilder.Entity("Storage.Laundry.Models.RfidAntennaEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("AntennaNumb")
                        .HasColumnName("antennaNumb")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedDateUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .HasColumnName("name")
                        .HasColumnType("text");

                    b.Property<int>("RfidReaderId")
                        .HasColumnName("rfidReaderId")
                        .HasColumnType("integer");

                    b.Property<double>("RxSensitivity")
                        .HasColumnName("rxSensitivity")
                        .HasColumnType("double precision");

                    b.Property<double>("TxPower")
                        .HasColumnName("txPower")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.HasIndex("RfidReaderId");

                    b.ToTable("rfidAntenna");
                });

            modelBuilder.Entity("Storage.Laundry.Models.RfidReaderEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("CreatedDateUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .HasColumnName("name")
                        .HasColumnType("text");

                    b.Property<string>("ReaderIp")
                        .HasColumnName("readerIp")
                        .HasColumnType("text");

                    b.Property<int>("ReaderPort")
                        .HasColumnName("readerPort")
                        .HasColumnType("integer");

                    b.Property<int>("TagPopulation")
                        .HasColumnName("tagPopulation")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("rfidReader");
                });

            modelBuilder.Entity("Storage.Laundry.Models.AccountDetailsEntity", b =>
                {
                    b.HasOne("Storage.Laundry.Models.AccountEntity", "AccountEntity")
                        .WithOne("AccountDetailsEntity")
                        .HasForeignKey("Storage.Laundry.Models.AccountDetailsEntity", "AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Storage.Laundry.Models.RfidReaderEntity", "RfidReaderEntity")
                        .WithMany("AccountDetailsEntities")
                        .HasForeignKey("ReaderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Storage.Laundry.Models.ClientEntity", b =>
                {
                    b.HasOne("Storage.Laundry.Models.ClientEntity", "Parent")
                        .WithMany("ChildEntities")
                        .HasForeignKey("ParentId");
                });

            modelBuilder.Entity("Storage.Laundry.Models.ClientLinenEntity", b =>
                {
                    b.HasOne("Storage.Laundry.Models.ClientEntity", "ClientEntity")
                        .WithMany("ClientLinenEntities")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Storage.Laundry.Models.DepartmentEntity", "DepartmentEntity")
                        .WithMany("ClientLinenEntities")
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Storage.Laundry.Models.MasterLinenEntity", "MasterLinenEntity")
                        .WithMany("ClientLinenEntities")
                        .HasForeignKey("MasterLinenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Storage.Laundry.Models.ClientStaffEntity", "ClientStaffEntity")
                        .WithMany("ClientLinenEntities")
                        .HasForeignKey("StaffId");
                });

            modelBuilder.Entity("Storage.Laundry.Models.ClientStaffEntity", b =>
                {
                    b.HasOne("Storage.Laundry.Models.DepartmentEntity", "DepartmentEntity")
                        .WithMany("ClientStaffEntities")
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Storage.Laundry.Models.ConveyorEntity", b =>
                {
                    b.HasOne("Storage.Laundry.Models.ClientLinenEntity", "ClientLinenEntity")
                        .WithMany("ConveyorEntities")
                        .HasForeignKey("ClientLinenId");
                });

            modelBuilder.Entity("Storage.Laundry.Models.DepartmentEntity", b =>
                {
                    b.HasOne("Storage.Laundry.Models.ClientEntity", "ClientEntity")
                        .WithMany("DepartmentEntities")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Storage.Laundry.Models.RfidAntennaEntity", b =>
                {
                    b.HasOne("Storage.Laundry.Models.RfidReaderEntity", "RfidReaderEntity")
                        .WithMany("RfidAntennaEntities")
                        .HasForeignKey("RfidReaderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}

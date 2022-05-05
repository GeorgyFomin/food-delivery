﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Persistence.MsSql;

#nullable disable

namespace Persistence.MsSql.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Entities.Domain.Delivery", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("ServiceName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<TimeSpan>("TimeSpan")
                        .HasColumnType("time");

                    b.HasKey("Id");

                    b.ToTable("Deliveries");
                });

            modelBuilder.Entity("Entities.Domain.Discount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<decimal>("Size")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Discounts");
                });

            modelBuilder.Entity("Entities.Domain.Employee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("Entities.Domain.Ingredient", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Ingredients");
                });

            modelBuilder.Entity("Entities.Domain.Menu", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.HasKey("Id");

                    b.ToTable("Menus");
                });

            modelBuilder.Entity("Entities.Domain.MenuItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int?>("MenuId")
                        .HasColumnType("int");

                    b.Property<int?>("ProductId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MenuId");

                    b.HasIndex("ProductId");

                    b.ToTable("MenuItems");
                });

            modelBuilder.Entity("Entities.Domain.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int?>("DeliveryId")
                        .HasColumnType("int");

                    b.Property<int?>("DiscountId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DeliveryId");

                    b.HasIndex("DiscountId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("Entities.Domain.OrderItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int?>("OrderId")
                        .HasColumnType("int");

                    b.Property<int?>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.HasIndex("ProductId");

                    b.ToTable("OrderItems");
                });

            modelBuilder.Entity("Entities.Domain.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<double>("Weight")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("Entities.Domain.ProductIngredient", b =>
                {
                    b.Property<int>("IngredientId")
                        .HasColumnType("int");

                    b.Property<int?>("ProductId")
                        .HasColumnType("int");

                    b.HasKey("IngredientId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductIngredient");
                });

            modelBuilder.Entity("Entities.Domain.MenuItem", b =>
                {
                    b.HasOne("Entities.Domain.Menu", null)
                        .WithMany("MenuItems")
                        .HasForeignKey("MenuId");

                    b.HasOne("Entities.Domain.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Entities.Domain.Order", b =>
                {
                    b.HasOne("Entities.Domain.Delivery", "Delivery")
                        .WithMany()
                        .HasForeignKey("DeliveryId");

                    b.HasOne("Entities.Domain.Discount", "Discount")
                        .WithMany()
                        .HasForeignKey("DiscountId");

                    b.OwnsOne("PhoneNumbers.PhoneNumber", "PhoneNumder", b1 =>
                        {
                            b1.Property<int>("OrderId")
                                .HasColumnType("int");

                            b1.Property<int>("CountryCode")
                                .HasColumnType("int");

                            b1.Property<int>("CountryCodeSource")
                                .HasColumnType("int");

                            b1.Property<string>("Extension")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<bool>("HasCountryCode")
                                .HasColumnType("bit");

                            b1.Property<bool>("HasCountryCodeSource")
                                .HasColumnType("bit");

                            b1.Property<bool>("HasExtension")
                                .HasColumnType("bit");

                            b1.Property<bool>("HasItalianLeadingZero")
                                .HasColumnType("bit");

                            b1.Property<bool>("HasNationalNumber")
                                .HasColumnType("bit");

                            b1.Property<bool>("HasNumberOfLeadingZeros")
                                .HasColumnType("bit");

                            b1.Property<bool>("HasPreferredDomesticCarrierCode")
                                .HasColumnType("bit");

                            b1.Property<bool>("HasRawInput")
                                .HasColumnType("bit");

                            b1.Property<bool>("ItalianLeadingZero")
                                .HasColumnType("bit");

                            b1.Property<decimal>("NationalNumber")
                                .HasColumnType("decimal(20,0)");

                            b1.Property<int>("NumberOfLeadingZeros")
                                .HasColumnType("int");

                            b1.Property<string>("PreferredDomesticCarrierCode")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("RawInput")
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("OrderId");

                            b1.ToTable("Orders");

                            b1.WithOwner()
                                .HasForeignKey("OrderId");
                        });

                    b.Navigation("Delivery");

                    b.Navigation("Discount");

                    b.Navigation("PhoneNumder");
                });

            modelBuilder.Entity("Entities.Domain.OrderItem", b =>
                {
                    b.HasOne("Entities.Domain.Order", null)
                        .WithMany("OrderElements")
                        .HasForeignKey("OrderId");

                    b.HasOne("Entities.Domain.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Entities.Domain.ProductIngredient", b =>
                {
                    b.HasOne("Entities.Domain.Ingredient", "Ingredient")
                        .WithMany("ProductsIngredients")
                        .HasForeignKey("IngredientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Entities.Domain.Product", "Product")
                        .WithMany("ProductsIngredients")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ingredient");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Entities.Domain.Ingredient", b =>
                {
                    b.Navigation("ProductsIngredients");
                });

            modelBuilder.Entity("Entities.Domain.Menu", b =>
                {
                    b.Navigation("MenuItems");
                });

            modelBuilder.Entity("Entities.Domain.Order", b =>
                {
                    b.Navigation("OrderElements");
                });

            modelBuilder.Entity("Entities.Domain.Product", b =>
                {
                    b.Navigation("ProductsIngredients");
                });
#pragma warning restore 612, 618
        }
    }
}

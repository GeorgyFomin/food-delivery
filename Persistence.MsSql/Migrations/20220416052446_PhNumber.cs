using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.MsSql.Migrations
{
    public partial class PhNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PhoneNumder_CountryCode",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PhoneNumder_CountryCodeSource",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumder_Extension",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumder_HasCountryCode",
                table: "Orders",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumder_HasCountryCodeSource",
                table: "Orders",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumder_HasExtension",
                table: "Orders",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumder_HasItalianLeadingZero",
                table: "Orders",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumder_HasNationalNumber",
                table: "Orders",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumder_HasNumberOfLeadingZeros",
                table: "Orders",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumder_HasPreferredDomesticCarrierCode",
                table: "Orders",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumder_HasRawInput",
                table: "Orders",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumder_ItalianLeadingZero",
                table: "Orders",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PhoneNumder_NationalNumber",
                table: "Orders",
                type: "decimal(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PhoneNumder_NumberOfLeadingZeros",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumder_PreferredDomesticCarrierCode",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumder_RawInput",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumder_CountryCode",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PhoneNumder_CountryCodeSource",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PhoneNumder_Extension",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PhoneNumder_HasCountryCode",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PhoneNumder_HasCountryCodeSource",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PhoneNumder_HasExtension",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PhoneNumder_HasItalianLeadingZero",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PhoneNumder_HasNationalNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PhoneNumder_HasNumberOfLeadingZeros",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PhoneNumder_HasPreferredDomesticCarrierCode",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PhoneNumder_HasRawInput",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PhoneNumder_ItalianLeadingZero",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PhoneNumder_NationalNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PhoneNumder_NumberOfLeadingZeros",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PhoneNumder_PreferredDomesticCarrierCode",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PhoneNumder_RawInput",
                table: "Orders");
        }
    }
}

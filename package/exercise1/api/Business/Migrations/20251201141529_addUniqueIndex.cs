using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StargateAPI.Migrations
{
    /// <inheritdoc />
    public partial class addUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "Person",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Person_NormalizedName",
                table: "Person",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.Sql(
                @"UPDATE Person
                  SET NormalizedName = UPPER(Name);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Person_NormalizedName",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                table: "Person");
        }
    }
}

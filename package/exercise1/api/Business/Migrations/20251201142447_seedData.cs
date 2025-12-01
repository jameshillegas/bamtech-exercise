using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StargateAPI.Migrations
{
    /// <inheritdoc />
    public partial class seedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Person",
                columns: new[] { "Id", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { 1, "John Doe", "JOHN DOE" },
                    { 2, "Jane Doe", "JANE DOE" }
                });

            migrationBuilder.InsertData(
                table: "AstronautDetail",
                columns: new[] { "Id", "PersonId", "CurrentRank", "CurrentDutyTitle", "CareerStartDate" },
                values: new object[]
                { 1, 1, "1LT", "Commander", DateTime.Now }
            );

            migrationBuilder.InsertData(
                table: "AstronautDuty",
                columns: new[] { "Id", "PersonId", "DutyStartDate", "DutyTitle", "Rank" },
                values: new object[]
                { 1, 1, DateTime.Now, "Commander", "1LT" }
            );

            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AstronautDetail",
                keyColumn: "Id",
                keyValue: 1,
                column: "CareerStartDate",
                value: new DateTime(2025, 12, 1, 9, 21, 59, 568, DateTimeKind.Local).AddTicks(4416));

            migrationBuilder.UpdateData(
                table: "AstronautDuty",
                keyColumn: "Id",
                keyValue: 1,
                column: "DutyStartDate",
                value: new DateTime(2025, 12, 1, 9, 21, 59, 568, DateTimeKind.Local).AddTicks(4459));
        }
    }
}

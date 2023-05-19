using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HajurkoCarRental.Migrations
{
    public partial class AddedCarreturnmodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarReturn",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RentalRequestId = table.Column<int>(type: "int", nullable: false),
                    FuelLevel = table.Column<float>(type: "real", nullable: false),
                    IsDamaged = table.Column<bool>(type: "bit", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarReturn", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarReturn_RentalRequest_RentalRequestId",
                        column: x => x.RentalRequestId,
                        principalSchema: "Identity",
                        principalTable: "RentalRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarReturn_RentalRequestId",
                schema: "Identity",
                table: "CarReturn",
                column: "RentalRequestId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarReturn",
                schema: "Identity");
        }
    }
}

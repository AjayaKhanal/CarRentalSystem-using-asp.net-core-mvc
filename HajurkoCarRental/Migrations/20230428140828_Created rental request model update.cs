using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HajurkoCarRental.Migrations
{
    public partial class Createdrentalrequestmodelupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CarId",
                schema: "Identity",
                table: "RentalRequest",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RentalRequest_CarId",
                schema: "Identity",
                table: "RentalRequest",
                column: "CarId");

            migrationBuilder.AddForeignKey(
                name: "FK_RentalRequest_Car_CarId",
                schema: "Identity",
                table: "RentalRequest",
                column: "CarId",
                principalSchema: "Identity",
                principalTable: "Car",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RentalRequest_Car_CarId",
                schema: "Identity",
                table: "RentalRequest");

            migrationBuilder.DropIndex(
                name: "IX_RentalRequest_CarId",
                schema: "Identity",
                table: "RentalRequest");

            migrationBuilder.DropColumn(
                name: "CarId",
                schema: "Identity",
                table: "RentalRequest");
        }
    }
}

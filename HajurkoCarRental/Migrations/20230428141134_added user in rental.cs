using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HajurkoCarRental.Migrations
{
    public partial class addeduserinrental : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                schema: "Identity",
                table: "RentalRequest",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_RentalRequest_UserId",
                schema: "Identity",
                table: "RentalRequest",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RentalRequest_User_UserId",
                schema: "Identity",
                table: "RentalRequest",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RentalRequest_User_UserId",
                schema: "Identity",
                table: "RentalRequest");

            migrationBuilder.DropIndex(
                name: "IX_RentalRequest_UserId",
                schema: "Identity",
                table: "RentalRequest");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "Identity",
                table: "RentalRequest");
        }
    }
}

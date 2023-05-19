using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HajurkoCarRental.Migrations
{
    public partial class updatedrequestmodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RentalRequest_User_UserId",
                schema: "Identity",
                table: "RentalRequest");

            migrationBuilder.DropIndex(
                name: "IX_RentalRequest_UserId",
                schema: "Identity",
                table: "RentalRequest");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                schema: "Identity",
                table: "RentalRequest",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                schema: "Identity",
                table: "RentalRequest",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RentalRequest_UserId1",
                schema: "Identity",
                table: "RentalRequest",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_RentalRequest_User_UserId1",
                schema: "Identity",
                table: "RentalRequest",
                column: "UserId1",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RentalRequest_User_UserId1",
                schema: "Identity",
                table: "RentalRequest");

            migrationBuilder.DropIndex(
                name: "IX_RentalRequest_UserId1",
                schema: "Identity",
                table: "RentalRequest");

            migrationBuilder.DropColumn(
                name: "UserId1",
                schema: "Identity",
                table: "RentalRequest");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "Identity",
                table: "RentalRequest",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

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
    }
}

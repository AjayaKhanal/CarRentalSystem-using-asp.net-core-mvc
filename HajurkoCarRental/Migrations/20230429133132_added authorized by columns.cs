using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HajurkoCarRental.Migrations
{
    public partial class addedauthorizedbycolumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RentalRequest_User_AuthorizedById",
                schema: "Identity",
                table: "RentalRequest");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorizedById",
                schema: "Identity",
                table: "RentalRequest",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_RentalRequest_User_AuthorizedById",
                schema: "Identity",
                table: "RentalRequest",
                column: "AuthorizedById",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RentalRequest_User_AuthorizedById",
                schema: "Identity",
                table: "RentalRequest");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorizedById",
                schema: "Identity",
                table: "RentalRequest",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RentalRequest_User_AuthorizedById",
                schema: "Identity",
                table: "RentalRequest",
                column: "AuthorizedById",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}

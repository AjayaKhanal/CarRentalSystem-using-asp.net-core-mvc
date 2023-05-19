using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HajurkoCarRental.Migrations
{
    public partial class addedauthorizedbycolumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorizedById",
                schema: "Identity",
                table: "RentalRequest",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_RentalRequest_AuthorizedById",
                schema: "Identity",
                table: "RentalRequest",
                column: "AuthorizedById");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RentalRequest_User_AuthorizedById",
                schema: "Identity",
                table: "RentalRequest");

            migrationBuilder.DropIndex(
                name: "IX_RentalRequest_AuthorizedById",
                schema: "Identity",
                table: "RentalRequest");

            migrationBuilder.DropColumn(
                name: "AuthorizedById",
                schema: "Identity",
                table: "RentalRequest");
        }
    }
}

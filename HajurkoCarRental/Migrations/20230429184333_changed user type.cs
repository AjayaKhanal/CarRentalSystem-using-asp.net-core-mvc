using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HajurkoCarRental.Migrations
{
    public partial class changedusertype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Damage_User_UserId1",
                schema: "Identity",
                table: "Damage");

            migrationBuilder.DropIndex(
                name: "IX_Damage_UserId1",
                schema: "Identity",
                table: "Damage");

            migrationBuilder.DropColumn(
                name: "UserId1",
                schema: "Identity",
                table: "Damage");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "Identity",
                table: "Damage",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Damage_UserId",
                schema: "Identity",
                table: "Damage",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Damage_User_UserId",
                schema: "Identity",
                table: "Damage",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Damage_User_UserId",
                schema: "Identity",
                table: "Damage");

            migrationBuilder.DropIndex(
                name: "IX_Damage_UserId",
                schema: "Identity",
                table: "Damage");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                schema: "Identity",
                table: "Damage",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                schema: "Identity",
                table: "Damage",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Damage_UserId1",
                schema: "Identity",
                table: "Damage",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Damage_User_UserId1",
                schema: "Identity",
                table: "Damage",
                column: "UserId1",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}

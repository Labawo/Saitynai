using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestLS.Migrations
{
    /// <inheritdoc />
    public partial class _4thmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SessionReceits_Patient_PatId",
                table: "SessionReceits");

            migrationBuilder.DropIndex(
                name: "IX_SessionReceits_PatId",
                table: "SessionReceits");

            migrationBuilder.DropColumn(
                name: "PatId",
                table: "SessionReceits");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PatId",
                table: "SessionReceits",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SessionReceits_PatId",
                table: "SessionReceits",
                column: "PatId");

            migrationBuilder.AddForeignKey(
                name: "FK_SessionReceits_Patient_PatId",
                table: "SessionReceits",
                column: "PatId",
                principalTable: "Patient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

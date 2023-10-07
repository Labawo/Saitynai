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
                name: "FK_Appointments_Patient_PatId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_PatId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "PatId",
                table: "Appointments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PatId",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PatId",
                table: "Appointments",
                column: "PatId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Patient_PatId",
                table: "Appointments",
                column: "PatId",
                principalTable: "Patient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cogtive.DevAssignment.Api.Migrations
{
    /// <inheritdoc />
    public partial class AdjustFieldName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Model",
                table: "Machines",
                newName: "SerialNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SerialNumber",
                table: "Machines",
                newName: "Model");
        }
    }
}

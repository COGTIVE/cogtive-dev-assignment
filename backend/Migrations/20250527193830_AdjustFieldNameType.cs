using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cogtive.DevAssignment.Api.Migrations
{
    /// <inheritdoc />
    public partial class AdjustFieldNameType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Machines",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Machines");
        }
    }
}

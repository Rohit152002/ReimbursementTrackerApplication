using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReimbursementTrackingApplication.Migrations
{
    public partial class stageaddcolumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Stage",
                table: "ReimbursementRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stage",
                table: "ReimbursementRequests");
        }
    }
}

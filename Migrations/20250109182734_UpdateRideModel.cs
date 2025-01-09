using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RideSharing.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRideModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TotalDistance",
                table: "Rides",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TotalDuration",
                table: "Rides",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalDistance",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "TotalDuration",
                table: "Rides");
        }
    }
}

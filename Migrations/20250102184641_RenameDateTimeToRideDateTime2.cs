using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RideSharing.Migrations
{
    /// <inheritdoc />
    public partial class RenameDateTimeToRideDateTime2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RideRequests_Rides_RideId",
                table: "RideRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_RideRequests_Vehicles_VehicleId",
                table: "RideRequests");

            migrationBuilder.RenameColumn(
                name: "DateTime",
                table: "Rides",
                newName: "RideDateTime");

            migrationBuilder.AlterColumn<int>(
                name: "RideId",
                table: "RideRequests",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RideRequests_Rides_RideId",
                table: "RideRequests",
                column: "RideId",
                principalTable: "Rides",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RideRequests_Vehicles_VehicleId",
                table: "RideRequests",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RideRequests_Rides_RideId",
                table: "RideRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_RideRequests_Vehicles_VehicleId",
                table: "RideRequests");

            migrationBuilder.RenameColumn(
                name: "RideDateTime",
                table: "Rides",
                newName: "DateTime");

            migrationBuilder.AlterColumn<int>(
                name: "RideId",
                table: "RideRequests",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_RideRequests_Rides_RideId",
                table: "RideRequests",
                column: "RideId",
                principalTable: "Rides",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RideRequests_Vehicles_VehicleId",
                table: "RideRequests",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RideSharing.Migrations
{
    /// <inheritdoc />
    public partial class AddRidesToVehicle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RideRequests_AspNetUsers_PassengerId",
                table: "RideRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_RideRequests_Rides_RideId",
                table: "RideRequests");

            migrationBuilder.AlterColumn<int>(
                name: "RideId",
                table: "RideRequests",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Destination",
                table: "RideRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Origin",
                table: "RideRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "VehicleId",
                table: "RideRequests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RideRequests_VehicleId",
                table: "RideRequests",
                column: "VehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_RideRequests_AspNetUsers_PassengerId",
                table: "RideRequests",
                column: "PassengerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RideRequests_AspNetUsers_PassengerId",
                table: "RideRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_RideRequests_Rides_RideId",
                table: "RideRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_RideRequests_Vehicles_VehicleId",
                table: "RideRequests");

            migrationBuilder.DropIndex(
                name: "IX_RideRequests_VehicleId",
                table: "RideRequests");

            migrationBuilder.DropColumn(
                name: "Destination",
                table: "RideRequests");

            migrationBuilder.DropColumn(
                name: "Origin",
                table: "RideRequests");

            migrationBuilder.DropColumn(
                name: "VehicleId",
                table: "RideRequests");

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
                name: "FK_RideRequests_AspNetUsers_PassengerId",
                table: "RideRequests",
                column: "PassengerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RideRequests_Rides_RideId",
                table: "RideRequests",
                column: "RideId",
                principalTable: "Rides",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

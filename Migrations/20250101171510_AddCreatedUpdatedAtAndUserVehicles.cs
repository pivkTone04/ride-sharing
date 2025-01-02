using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RideSharing.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedUpdatedAtAndUserVehicles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RideRequests_AspNetUsers_PassengerId",
                table: "RideRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Rides_AspNetUsers_DriverId",
                table: "Rides");

            migrationBuilder.DropForeignKey(
                name: "FK_Rides_Vehicles_VehicleId",
                table: "Rides");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_AspNetUsers_DriverId",
                table: "Vehicles");

            migrationBuilder.AlterColumn<string>(
                name: "DriverId",
                table: "Vehicles",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "DriverId",
                table: "Rides",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Rides",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Rides",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_RideRequests_AspNetUsers_PassengerId",
                table: "RideRequests",
                column: "PassengerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rides_AspNetUsers_DriverId",
                table: "Rides",
                column: "DriverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rides_Vehicles_VehicleId",
                table: "Rides",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_AspNetUsers_DriverId",
                table: "Vehicles",
                column: "DriverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RideRequests_AspNetUsers_PassengerId",
                table: "RideRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Rides_AspNetUsers_DriverId",
                table: "Rides");

            migrationBuilder.DropForeignKey(
                name: "FK_Rides_Vehicles_VehicleId",
                table: "Rides");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_AspNetUsers_DriverId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Rides");

            migrationBuilder.AlterColumn<string>(
                name: "DriverId",
                table: "Vehicles",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DriverId",
                table: "Rides",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RideRequests_AspNetUsers_PassengerId",
                table: "RideRequests",
                column: "PassengerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rides_AspNetUsers_DriverId",
                table: "Rides",
                column: "DriverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rides_Vehicles_VehicleId",
                table: "Rides",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_AspNetUsers_DriverId",
                table: "Vehicles",
                column: "DriverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

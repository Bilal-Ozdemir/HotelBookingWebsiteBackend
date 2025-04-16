using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedDecimalPrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Users_UserId",
                table: "Payments");

            migrationBuilder.UpdateData(
                table: "HotelRooms",
                keyColumn: "Id",
                keyValue: 1,
                column: "Price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "HotelRooms",
                keyColumn: "Id",
                keyValue: 2,
                column: "Price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "HotelRooms",
                keyColumn: "Id",
                keyValue: 3,
                column: "Price",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "RoomTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: "One person room");

            migrationBuilder.UpdateData(
                table: "RoomTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "Description",
                value: "Two person room");

            migrationBuilder.UpdateData(
                table: "RoomTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "Description",
                value: "Premium suite");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Users_UserId",
                table: "Payments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Users_UserId",
                table: "Payments");

            migrationBuilder.UpdateData(
                table: "HotelRooms",
                keyColumn: "Id",
                keyValue: 1,
                column: "Price",
                value: 100m);

            migrationBuilder.UpdateData(
                table: "HotelRooms",
                keyColumn: "Id",
                keyValue: 2,
                column: "Price",
                value: 150m);

            migrationBuilder.UpdateData(
                table: "HotelRooms",
                keyColumn: "Id",
                keyValue: 3,
                column: "Price",
                value: 250m);

            migrationBuilder.UpdateData(
                table: "RoomTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: "A room for one person.");

            migrationBuilder.UpdateData(
                table: "RoomTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "Description",
                value: "A room for two people.");

            migrationBuilder.UpdateData(
                table: "RoomTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "Description",
                value: "A spacious room with a separate living area.");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Users_UserId",
                table: "Payments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

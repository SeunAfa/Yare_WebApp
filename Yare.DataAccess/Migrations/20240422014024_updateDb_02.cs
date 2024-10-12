using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yare.DataAccess.Migrations
{
    public partial class updateDb_02 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_Collections_Products_AccessoryId",
                table: "Product_Collections");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_Collections_Products_JewelleryId",
                table: "Product_Collections");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_Collections_Products_WatchId",
                table: "Product_Collections");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Product_Collections_Accessory_Product_CollectionId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Product_Collections_Watch_Product_CollectionId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_Accessory_Product_CollectionId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_Watch_Product_CollectionId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Product_Collections_AccessoryId",
                table: "Product_Collections");

            migrationBuilder.DropIndex(
                name: "IX_Product_Collections_JewelleryId",
                table: "Product_Collections");

            migrationBuilder.DropIndex(
                name: "IX_Product_Collections_WatchId",
                table: "Product_Collections");

            migrationBuilder.DropColumn(
                name: "Accessory_Product_CollectionId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Watch_Product_CollectionId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AccessoryId",
                table: "Product_Collections");

            migrationBuilder.DropColumn(
                name: "JewelleryId",
                table: "Product_Collections");

            migrationBuilder.DropColumn(
                name: "WatchId",
                table: "Product_Collections");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Accessory_Product_CollectionId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Watch_Product_CollectionId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccessoryId",
                table: "Product_Collections",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JewelleryId",
                table: "Product_Collections",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WatchId",
                table: "Product_Collections",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_Accessory_Product_CollectionId",
                table: "Products",
                column: "Accessory_Product_CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Watch_Product_CollectionId",
                table: "Products",
                column: "Watch_Product_CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_Collections_AccessoryId",
                table: "Product_Collections",
                column: "AccessoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_Collections_JewelleryId",
                table: "Product_Collections",
                column: "JewelleryId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_Collections_WatchId",
                table: "Product_Collections",
                column: "WatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Collections_Products_AccessoryId",
                table: "Product_Collections",
                column: "AccessoryId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Collections_Products_JewelleryId",
                table: "Product_Collections",
                column: "JewelleryId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Collections_Products_WatchId",
                table: "Product_Collections",
                column: "WatchId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Product_Collections_Accessory_Product_CollectionId",
                table: "Products",
                column: "Accessory_Product_CollectionId",
                principalTable: "Product_Collections",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Product_Collections_Watch_Product_CollectionId",
                table: "Products",
                column: "Watch_Product_CollectionId",
                principalTable: "Product_Collections",
                principalColumn: "Id");
        }
    }
}

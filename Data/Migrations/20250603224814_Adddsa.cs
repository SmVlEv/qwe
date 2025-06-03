using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnityAssetStore.Migrations
{
    /// <inheritdoc />
    public partial class Adddsa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssetId1",
                table: "OrderItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_AssetId1",
                table: "OrderItems",
                column: "AssetId1");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Assets_AssetId1",
                table: "OrderItems",
                column: "AssetId1",
                principalTable: "Assets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Assets_AssetId1",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_AssetId1",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "AssetId1",
                table: "OrderItems");
        }
    }
}

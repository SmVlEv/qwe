using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UnityAssetStore.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnityPackageFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Assets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Assets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "3D Models" },
                    { 2, "Scripts" },
                    { 3, "Textures" }
                });

            migrationBuilder.InsertData(
                table: "Assets",
                columns: new[] { "Id", "CategoryId", "Description", "FilePath", "Name", "PreviewImageUrl", "Price" },
                values: new object[,]
                {
                    { 1, 1, "Набор низкополигональных деревьев и кустов для Unity.", "/uploads/assets/low_poly_forest.unitypackage", "Low Poly Forest Pack", "/images/forest_pack.jpg", 29.99m },
                    { 2, 2, "Плагин Cinemachine для создания динамичных камер в Unity.", "/uploads/assets/cinemachine_utilities.unitypackage", "Cinemachine Utilities", "/images/cinemachine.jpg", 19.99m },
                    { 3, 3, "Высококачественные текстуры пустыни для Unity Terrains.", "/uploads/assets/desert_textures.unitypackage", "Desert Terrain Textures", "/images/desert_textures.jpg", 14.99m }
                });
        }
    }
}

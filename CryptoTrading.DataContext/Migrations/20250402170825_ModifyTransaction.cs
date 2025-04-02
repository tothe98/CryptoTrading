using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoTrading.DataContext.Migrations
{
    /// <inheritdoc />
    public partial class ModifyTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CryptoId",
                table: "TransactionLogs",
                newName: "UserId");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "TransactionLogs",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CryptoCurrencyId",
                table: "TransactionLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_CryptoCurrencyId",
                table: "TransactionLogs",
                column: "CryptoCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_UserId",
                table: "TransactionLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CryptoPriceFluctuation_CryptoCurrencyId",
                table: "CryptoPriceFluctuation",
                column: "CryptoCurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_CryptoPriceFluctuation_CryptoCurrencies_CryptoCurrencyId",
                table: "CryptoPriceFluctuation",
                column: "CryptoCurrencyId",
                principalTable: "CryptoCurrencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionLogs_CryptoCurrencies_CryptoCurrencyId",
                table: "TransactionLogs",
                column: "CryptoCurrencyId",
                principalTable: "CryptoCurrencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionLogs_Users_UserId",
                table: "TransactionLogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CryptoPriceFluctuation_CryptoCurrencies_CryptoCurrencyId",
                table: "CryptoPriceFluctuation");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionLogs_CryptoCurrencies_CryptoCurrencyId",
                table: "TransactionLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionLogs_Users_UserId",
                table: "TransactionLogs");

            migrationBuilder.DropIndex(
                name: "IX_TransactionLogs_CryptoCurrencyId",
                table: "TransactionLogs");

            migrationBuilder.DropIndex(
                name: "IX_TransactionLogs_UserId",
                table: "TransactionLogs");

            migrationBuilder.DropIndex(
                name: "IX_CryptoPriceFluctuation_CryptoCurrencyId",
                table: "CryptoPriceFluctuation");

            migrationBuilder.DropColumn(
                name: "CryptoCurrencyId",
                table: "TransactionLogs");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "TransactionLogs",
                newName: "CryptoId");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "TransactionLogs",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}

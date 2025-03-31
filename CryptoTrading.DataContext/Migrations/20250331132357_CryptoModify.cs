using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoTrading.DataContext.Migrations
{
    /// <inheritdoc />
    public partial class CryptoModify : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "OldPrice",
                table: "CryptoPriceFluctuation",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CurrentPrice",
                table: "CryptoCurrencies",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_WalletHoldings_CryptoCurrencyId",
                table: "WalletHoldings",
                column: "CryptoCurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletHoldings_CryptoCurrencies_CryptoCurrencyId",
                table: "WalletHoldings",
                column: "CryptoCurrencyId",
                principalTable: "CryptoCurrencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletHoldings_CryptoCurrencies_CryptoCurrencyId",
                table: "WalletHoldings");

            migrationBuilder.DropIndex(
                name: "IX_WalletHoldings_CryptoCurrencyId",
                table: "WalletHoldings");

            migrationBuilder.DropColumn(
                name: "OldPrice",
                table: "CryptoPriceFluctuation");

            migrationBuilder.DropColumn(
                name: "CurrentPrice",
                table: "CryptoCurrencies");
        }
    }
}

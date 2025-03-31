using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoTrading.DataContext.Migrations
{
    /// <inheritdoc />
    public partial class WalletModify : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WalletHoldings",
                table: "WalletHoldings");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "WalletHoldings");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WalletHoldings",
                table: "WalletHoldings",
                columns: new[] { "WalletId", "CryptoCurrencyId" });

            migrationBuilder.AddForeignKey(
                name: "FK_WalletHoldings_Wallets_WalletId",
                table: "WalletHoldings",
                column: "WalletId",
                principalTable: "Wallets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletHoldings_Wallets_WalletId",
                table: "WalletHoldings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WalletHoldings",
                table: "WalletHoldings");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "WalletHoldings",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WalletHoldings",
                table: "WalletHoldings",
                column: "Id");
        }
    }
}

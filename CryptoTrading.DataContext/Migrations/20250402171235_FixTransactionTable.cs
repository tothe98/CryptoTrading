using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoTrading.DataContext.Migrations
{
    /// <inheritdoc />
    public partial class FixTransactionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsertId",
                table: "TransactionLogs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsertId",
                table: "TransactionLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderTotalsAndLifecycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CanceledAt",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Orders",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAt",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "Orders",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TotalItems",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(
                """
                UPDATE o
                SET o.Code = UPPER(REPLACE(CONVERT(varchar(36), o.Id), '-', '')),
                    o.TotalAmount = totals.TotalAmount,
                    o.TotalItems = totals.TotalItems,
                    o.PaidAt = CASE WHEN o.Status IN (2, 3, 4) THEN o.UpdatedAt ELSE NULL END,
                    o.CanceledAt = CASE WHEN o.Status = 4 THEN o.UpdatedAt ELSE NULL END
                FROM Orders o
                CROSS APPLY (
                    SELECT
                        COALESCE(SUM(i.Quantity * i.UnitPrice), 0) AS TotalAmount,
                        COALESCE(SUM(i.Quantity), 0) AS TotalItems
                    FROM OrderItems i
                    WHERE i.OrderId = o.Id AND i.DeletedAt IS NULL
                ) totals;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Code",
                table: "Orders",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_Code",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CanceledAt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaidAt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TotalItems",
                table: "Orders");
        }
    }
}

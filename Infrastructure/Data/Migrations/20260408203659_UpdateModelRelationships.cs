using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Products_ProductId1",
                schema: "public",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Coupons_CouponId1",
                schema: "public",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Products_ProductId1",
                schema: "public",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_ProductId1",
                schema: "public",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_Carts_CouponId1",
                schema: "public",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_ProductId1",
                schema: "public",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "ProductId1",
                schema: "public",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "CouponId1",
                schema: "public",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "ProductId1",
                schema: "public",
                table: "CartItems");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                schema: "public",
                table: "Users",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                schema: "public",
                table: "Users",
                type: "varchar(20)",
                nullable: false,
                defaultValue: "Customer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                schema: "public",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Role",
                schema: "public",
                table: "Users");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId1",
                schema: "public",
                table: "OrderItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CouponId1",
                schema: "public",
                table: "Carts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId1",
                schema: "public",
                table: "CartItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId1",
                schema: "public",
                table: "OrderItems",
                column: "ProductId1");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_CouponId1",
                schema: "public",
                table: "Carts",
                column: "CouponId1");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ProductId1",
                schema: "public",
                table: "CartItems",
                column: "ProductId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Products_ProductId1",
                schema: "public",
                table: "CartItems",
                column: "ProductId1",
                principalSchema: "public",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Coupons_CouponId1",
                schema: "public",
                table: "Carts",
                column: "CouponId1",
                principalSchema: "public",
                principalTable: "Coupons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Products_ProductId1",
                schema: "public",
                table: "OrderItems",
                column: "ProductId1",
                principalSchema: "public",
                principalTable: "Products",
                principalColumn: "Id");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PFC.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddedRecurrenceGenerateTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RecurrenceId",
                table: "Transactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "GeneratesTransaction",
                table: "recurrences",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_RecurrenceId",
                table: "Transactions",
                column: "RecurrenceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_recurrences_RecurrenceId",
                table: "Transactions",
                column: "RecurrenceId",
                principalTable: "recurrences",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_recurrences_RecurrenceId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_RecurrenceId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "RecurrenceId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "GeneratesTransaction",
                table: "recurrences");
        }
    }
}

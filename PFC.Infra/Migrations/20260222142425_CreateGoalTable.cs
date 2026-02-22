using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PFC.Infra.Migrations
{
    /// <inheritdoc />
    public partial class CreateGoalTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GoalId",
                table: "Transactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AccountId1",
                table: "recurrences",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId1",
                table: "recurrences",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "goals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    TargetAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrentAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    Deadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_goals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_goals_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_GoalId",
                table: "Transactions",
                column: "GoalId");

            migrationBuilder.CreateIndex(
                name: "IX_recurrences_AccountId1",
                table: "recurrences",
                column: "AccountId1");

            migrationBuilder.CreateIndex(
                name: "IX_recurrences_CategoryId1",
                table: "recurrences",
                column: "CategoryId1");

            migrationBuilder.CreateIndex(
                name: "IX_goals_IsActive",
                table: "goals",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_goals_UserId",
                table: "goals",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_recurrences_Accounts_AccountId1",
                table: "recurrences",
                column: "AccountId1",
                principalTable: "Accounts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_recurrences_Categories_CategoryId1",
                table: "recurrences",
                column: "CategoryId1",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_goals_GoalId",
                table: "Transactions",
                column: "GoalId",
                principalTable: "goals",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_recurrences_Accounts_AccountId1",
                table: "recurrences");

            migrationBuilder.DropForeignKey(
                name: "FK_recurrences_Categories_CategoryId1",
                table: "recurrences");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_goals_GoalId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "goals");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_GoalId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_recurrences_AccountId1",
                table: "recurrences");

            migrationBuilder.DropIndex(
                name: "IX_recurrences_CategoryId1",
                table: "recurrences");

            migrationBuilder.DropColumn(
                name: "GoalId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "AccountId1",
                table: "recurrences");

            migrationBuilder.DropColumn(
                name: "CategoryId1",
                table: "recurrences");
        }
    }
}

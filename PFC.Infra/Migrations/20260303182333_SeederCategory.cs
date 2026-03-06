using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PFC.Infra.Migrations
{
    /// <inheritdoc />
    public partial class SeederCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 4. Popula as categorias padrão
            migrationBuilder.Sql(@"
            INSERT INTO ""Categories"" (""Id"", ""UserId"", ""Name"", ""Type"", ""Color"", ""Icon"", ""IsActive"", ""IsDefault"", ""CreatedAt"")
            VALUES
                -- Despesas
                ('11111111-1111-1111-1111-111111111101', NULL, 'Alimentação',  'Expense', '#FF6B6B', '🍽️',  true, true, '2026-03-03T12:00:00Z'),
                ('11111111-1111-1111-1111-111111111102', NULL, 'Farmácia',     'Expense', '#E74C3C', '💊',   true, true, '2026-03-03T12:00:00Z'),
                ('11111111-1111-1111-1111-111111111103', NULL, 'Mercado',      'Expense', '#27AE60', '🛒',   true, true, '2026-03-03T12:00:00Z'),
                ('11111111-1111-1111-1111-111111111104', NULL, 'Transporte',   'Expense', '#3498DB', '🚗',   true, true, '2026-03-03T12:00:00Z'),
                ('11111111-1111-1111-1111-111111111105', NULL, 'Assinaturas',  'Expense', '#9B59B6', '📱',   true, true, '2026-03-03T12:00:00Z'),
                ('11111111-1111-1111-1111-111111111106', NULL, 'Moradia',      'Expense', '#E67E22', '🏠',   true, true, '2026-03-03T12:00:00Z'),
                ('11111111-1111-1111-1111-111111111107', NULL, 'Saúde',        'Expense', '#E91E63', '❤️',   true, true, '2026-03-03T12:00:00Z'),
                ('11111111-1111-1111-1111-111111111108', NULL, 'Educação',     'Expense', '#2980B9', '📚',   true, true, '2026-03-03T12:00:00Z'),
                ('11111111-1111-1111-1111-111111111109', NULL, 'Lazer',        'Expense', '#F39C12', '🎮',   true, true, '2026-03-03T12:00:00Z'),
                ('11111111-1111-1111-1111-111111111110', NULL, 'Vestuário',    'Expense', '#1ABC9C', '👔',   true, true, '2026-03-03T12:00:00Z'),
                ('11111111-1111-1111-1111-111111111111', NULL, 'Restaurante',  'Expense', '#FF5722', '🍴',   true, true, '2026-03-03T12:00:00Z'),
                ('11111111-1111-1111-1111-111111111112', NULL, 'Combustível',  'Expense', '#795548', '⛽',   true, true, '2026-03-03T12:00:00Z'),
                ('11111111-1111-1111-1111-111111111113', NULL, 'Beleza',       'Expense', '#EC407A', '💅',   true, true, '2026-03-03T12:00:00Z'),
                ('11111111-1111-1111-1111-111111111114', NULL, 'Pets',         'Expense', '#8BC34A', '🐾',   true, true, '2026-03-03T12:00:00Z'),
                -- Receitas
                ('11111111-1111-1111-1111-111111111115', NULL, 'Salário',      'Income',  '#2ECC71', '💰',   true, true, '2026-03-03T12:00:00Z'),
                ('11111111-1111-1111-1111-111111111116', NULL, 'Freelance',    'Income',  '#1ABC9C', '💻',   true, true, '2026-03-03T12:00:00Z'),
                ('11111111-1111-1111-1111-111111111117', NULL, 'Investimentos','Income',  '#16A085', '📈',   true, true, '2026-03-03T12:00:00Z'),
                ('11111111-1111-1111-1111-111111111118', NULL, 'Rendimentos',  'Income',  '#27AE60', '💵',   true, true, '2026-03-03T12:00:00Z'),
                ('11111111-1111-1111-1111-111111111119', NULL, 'Outros',       'Income',  '#95A5A6', '➕',   true, true, '2026-03-03T12:00:00Z'),
                ('11111111-1111-1111-1111-111111111120', NULL, 'Outros',       'Expense', '#2E2E2E', NULL,   true, true, '2026-03-03T12:00:00Z'),
                ('11111111-1111-1111-1111-111111111121', NULL, 'Reembolso',    'Income',  '#46295A', NULL,   true, true, '2026-03-03T12:00:00Z');
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove as categorias padrão inseridas
            migrationBuilder.Sql(@"
            DELETE FROM ""Categories""
            WHERE ""Id"" IN (
                '11111111-1111-1111-1111-111111111101',
                '11111111-1111-1111-1111-111111111102',
                '11111111-1111-1111-1111-111111111103',
                '11111111-1111-1111-1111-111111111104',
                '11111111-1111-1111-1111-111111111105',
                '11111111-1111-1111-1111-111111111106',
                '11111111-1111-1111-1111-111111111107',
                '11111111-1111-1111-1111-111111111108',
                '11111111-1111-1111-1111-111111111109',
                '11111111-1111-1111-1111-111111111110',
                '11111111-1111-1111-1111-111111111111',
                '11111111-1111-1111-1111-111111111112',
                '11111111-1111-1111-1111-111111111113',
                '11111111-1111-1111-1111-111111111114',
                '11111111-1111-1111-1111-111111111115',
                '11111111-1111-1111-1111-111111111116',
                '11111111-1111-1111-1111-111111111117',
                '11111111-1111-1111-1111-111111111118',
                '11111111-1111-1111-1111-111111111119'
            );");

            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_Categories_Name_Default""");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "Categories");

            // Remove categorias com UserId nulo antes de tornar a coluna obrigatória
            migrationBuilder.Sql(@"DELETE FROM ""Categories"" WHERE ""UserId"" IS NULL");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Categories",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldNullable: true,
                oldType: "uuid");
        }
    }
}
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevPulse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeUserIdTypeToGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"ALTER TABLE ""ErrorLogs"" ALTER COLUMN ""UserId"" TYPE uuid USING ""UserId""::uuid;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ErrorLogs",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldMaxLength: 450,
                oldNullable: true);
        }
    }
}

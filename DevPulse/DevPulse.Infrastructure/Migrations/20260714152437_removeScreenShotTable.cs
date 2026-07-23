using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevPulse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removeScreenShotTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ErrorScreenshots");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ErrorScreenshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ErrorLogId = table.Column<Guid>(type: "uuid", nullable: false),
                    BucketName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Checksum = table.Column<string>(type: "text", nullable: true),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ObjectKey = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Region = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SizeInBytes = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErrorScreenshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ErrorScreenshots_ErrorLogs_ErrorLogId",
                        column: x => x.ErrorLogId,
                        principalTable: "ErrorLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ErrorScreenshots_ErrorLogId",
                table: "ErrorScreenshots",
                column: "ErrorLogId",
                unique: true);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevPulse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removeErrorGroupTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ErrorScreenshots_ErrorEvents_ErrorEventId",
                table: "ErrorScreenshots");

            migrationBuilder.DropTable(
                name: "ErrorEvents");

            migrationBuilder.DropTable(
                name: "ErrorGroups");

            migrationBuilder.RenameColumn(
                name: "ErrorEventId",
                table: "ErrorScreenshots",
                newName: "ErrorLogId");

            migrationBuilder.RenameIndex(
                name: "IX_ErrorScreenshots_ErrorEventId",
                table: "ErrorScreenshots",
                newName: "IX_ErrorScreenshots_ErrorLogId");

            migrationBuilder.CreateTable(
                name: "ErrorLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    StackTrace = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ExceptionType = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Method = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RequestBody = table.Column<string>(type: "text", nullable: false),
                    QueryString = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    UserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    Browser = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IpAddress = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErrorLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ErrorLogs_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ErrorLogs_CreatedAt",
                table: "ErrorLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ErrorLogs_ExceptionType",
                table: "ErrorLogs",
                column: "ExceptionType");

            migrationBuilder.CreateIndex(
                name: "IX_ErrorLogs_ProjectId",
                table: "ErrorLogs",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ErrorLogs_ProjectId_CreatedAt",
                table: "ErrorLogs",
                columns: new[] { "ProjectId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ErrorLogs_Url",
                table: "ErrorLogs",
                column: "Url");

            migrationBuilder.AddForeignKey(
                name: "FK_ErrorScreenshots_ErrorLogs_ErrorLogId",
                table: "ErrorScreenshots",
                column: "ErrorLogId",
                principalTable: "ErrorLogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ErrorScreenshots_ErrorLogs_ErrorLogId",
                table: "ErrorScreenshots");

            migrationBuilder.DropTable(
                name: "ErrorLogs");

            migrationBuilder.RenameColumn(
                name: "ErrorLogId",
                table: "ErrorScreenshots",
                newName: "ErrorEventId");

            migrationBuilder.RenameIndex(
                name: "IX_ErrorScreenshots_ErrorLogId",
                table: "ErrorScreenshots",
                newName: "IX_ErrorScreenshots_ErrorEventId");

            migrationBuilder.CreateTable(
                name: "ErrorGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExceptionType = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Fingerprint = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    FirstSeen = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsResolved = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    LastSeen = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastStackTrace = table.Column<string>(type: "text", nullable: true),
                    Message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Occurrences = table.Column<int>(type: "integer", nullable: false),
                    Url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErrorGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ErrorGroups_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ErrorEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ErrorGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Browser = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IpAddress = table.Column<string>(type: "text", nullable: true),
                    Method = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    QueryString = table.Column<string>(type: "text", nullable: true),
                    RequestBody = table.Column<string>(type: "text", nullable: false),
                    StackTrace = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    UserId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErrorEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ErrorEvents_ErrorGroups_ErrorGroupId",
                        column: x => x.ErrorGroupId,
                        principalTable: "ErrorGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ErrorEvents_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ErrorEvents_ErrorGroupId",
                table: "ErrorEvents",
                column: "ErrorGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ErrorEvents_ErrorGroupId_CreatedAt",
                table: "ErrorEvents",
                columns: new[] { "ErrorGroupId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ErrorEvents_ProjectId",
                table: "ErrorEvents",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ErrorEvents_ProjectId_CreatedAt",
                table: "ErrorEvents",
                columns: new[] { "ProjectId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ErrorGroups_ProjectId",
                table: "ErrorGroups",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ErrorGroups_ProjectId_Fingerprint",
                table: "ErrorGroups",
                columns: new[] { "ProjectId", "Fingerprint" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ErrorGroups_ProjectId_IsResolved",
                table: "ErrorGroups",
                columns: new[] { "ProjectId", "IsResolved" });

            migrationBuilder.CreateIndex(
                name: "IX_ErrorGroups_ProjectId_LastSeen",
                table: "ErrorGroups",
                columns: new[] { "ProjectId", "LastSeen" });

            migrationBuilder.AddForeignKey(
                name: "FK_ErrorScreenshots_ErrorEvents_ErrorEventId",
                table: "ErrorScreenshots",
                column: "ErrorEventId",
                principalTable: "ErrorEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

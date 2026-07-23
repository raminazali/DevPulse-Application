using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevPulse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ApiKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ErrorGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Fingerprint = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    LastStackTrace = table.Column<string>(type: "text", nullable: true),
                    ExceptionType = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Occurrences = table.Column<int>(type: "integer", nullable: false),
                    FirstSeen = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastSeen = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsResolved = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
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
                    Url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Method = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    RequestBody = table.Column<string>(type: "text", nullable: false),
                    QueryString = table.Column<string>(type: "text", nullable: true),
                    StackTrace = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Browser = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IpAddress = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "ErrorScreenshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ErrorEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    BucketName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ObjectKey = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Region = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SizeInBytes = table.Column<long>(type: "bigint", nullable: false),
                    Checksum = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErrorScreenshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ErrorScreenshots_ErrorEvents_ErrorEventId",
                        column: x => x.ErrorEventId,
                        principalTable: "ErrorEvents",
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

            migrationBuilder.CreateIndex(
                name: "IX_ErrorScreenshots_ErrorEventId",
                table: "ErrorScreenshots",
                column: "ErrorEventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ApiKey",
                table: "Projects",
                column: "ApiKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_IsActive",
                table: "Projects",
                column: "IsActive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ErrorScreenshots");

            migrationBuilder.DropTable(
                name: "ErrorEvents");

            migrationBuilder.DropTable(
                name: "ErrorGroups");

            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}

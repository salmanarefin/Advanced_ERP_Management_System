using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP_version1.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditLogModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    AuditLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserEmail = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ModuleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TableName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RecordId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.AuditLogId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");
        }
    }
}

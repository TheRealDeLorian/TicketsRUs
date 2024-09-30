using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebApiTRU.Migrations
{
    /// <inheritdoc />
    public partial class DorianTRU : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "DorianTRU");

            migrationBuilder.CreateTable(
                name: "concert",
                schema: "DorianTRU",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    start_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    end_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    event_name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("concert_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ticket",
                schema: "DorianTRU",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    qrhash = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    email = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    concert_id = table.Column<int>(type: "integer", nullable: true),
                    timescanned = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ticket_pkey", x => x.id);
                    table.ForeignKey(
                        name: "ticket_concert_id_fkey",
                        column: x => x.concert_id,
                        principalSchema: "DorianTRU",
                        principalTable: "concert",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ticket_concert_id",
                schema: "DorianTRU",
                table: "ticket",
                column: "concert_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ticket",
                schema: "DorianTRU");

            migrationBuilder.DropTable(
                name: "concert",
                schema: "DorianTRU");
        }
    }
}

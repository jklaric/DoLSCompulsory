using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Indexer.Migrations
{
    /// <inheritdoc />
    public partial class CreateSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "emails",
                columns: table => new
                {
                    emailid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    emailname = table.Column<string>(type: "text", nullable: false),
                    emailcontent = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_emails", x => x.emailid);
                });

            migrationBuilder.CreateTable(
                name: "words",
                columns: table => new
                {
                    wordid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    wordvalue = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_words", x => x.wordid);
                });

            migrationBuilder.CreateTable(
                name: "occurrences",
                columns: table => new
                {
                    occurrenceid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    wordid = table.Column<int>(type: "integer", nullable: false),
                    emailid = table.Column<int>(type: "integer", nullable: false),
                    count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_occurrences", x => x.occurrenceid);
                    table.ForeignKey(
                        name: "FK_occurrences_emails_emailid",
                        column: x => x.emailid,
                        principalTable: "emails",
                        principalColumn: "emailid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_occurrences_words_wordid",
                        column: x => x.wordid,
                        principalTable: "words",
                        principalColumn: "wordid",
                        onDelete: ReferentialAction.Cascade);
                });

            // ✅ Add Unique Constraints
            migrationBuilder.CreateIndex(
                name: "IX_emails_emailname",
                table: "emails",
                column: "emailname",
                unique: true); // Ensure uniqueness of EmailName

            migrationBuilder.CreateIndex(
                name: "IX_words_wordvalue",
                table: "words",
                column: "wordvalue",
                unique: true); // Ensure uniqueness of WordValue

            migrationBuilder.CreateIndex(
                name: "IX_occurrences_emailid",
                table: "occurrences",
                column: "emailid");

            migrationBuilder.CreateIndex(
                name: "IX_occurrences_wordid",
                table: "occurrences",
                column: "wordid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "occurrences");

            migrationBuilder.DropTable(
                name: "emails");

            migrationBuilder.DropTable(
                name: "words");
        }
    }
}

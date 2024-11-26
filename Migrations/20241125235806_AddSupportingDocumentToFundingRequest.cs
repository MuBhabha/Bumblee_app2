using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BumbleBee.Migrations
{
    /// <inheritdoc />
    public partial class AddSupportingDocumentToFundingRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FundingRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyBackground = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProjectDetails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FundingAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IntendedImpact = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SupportingDocument = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundingRequests", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FundingRequests");
        }
    }
}

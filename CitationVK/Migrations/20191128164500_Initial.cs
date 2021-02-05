using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CitationVK.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    Question = table.Column<int>(nullable: false),
                    Answer = table.Column<string>(nullable: false),
                    Salt = table.Column<string>(nullable: true),
                    IsAdmin = table.Column<bool>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Classifiers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Model = table.Column<string>(nullable: true),
                    Accuracy = table.Column<double>(nullable: false),
                    Precision = table.Column<double>(nullable: false),
                    Recall = table.Column<double>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classifiers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Configurations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: false),
                    IsPublic = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Datasets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Datasets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccountClassifiers",
                columns: table => new
                {
                    AccountId = table.Column<int>(nullable: false),
                    ClassifierId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountClassifiers", x => new { x.AccountId, x.ClassifierId });
                    table.ForeignKey(
                        name: "FK_AccountClassifiers_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountClassifiers_Classifiers_ClassifierId",
                        column: x => x.ClassifierId,
                        principalTable: "Classifiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountDatasets",
                columns: table => new
                {
                    AccountId = table.Column<int>(nullable: false),
                    DatasetId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountDatasets", x => new { x.AccountId, x.DatasetId });
                    table.ForeignKey(
                        name: "FK_AccountDatasets_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountDatasets_Datasets_DatasetId",
                        column: x => x.DatasetId,
                        principalTable: "Datasets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Pmid = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Abstract = table.Column<string>(nullable: true),
                    Classification = table.Column<bool>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    DatasetId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Articles_Datasets_DatasetId",
                        column: x => x.DatasetId,
                        principalTable: "Datasets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Answer", "Date", "Email", "IsAdmin", "Password", "Question", "Salt" },
                values: new object[] { 1, "ZT6lnJoLZ7Y+49+22sCLJcLJetDMlxXAJtej1hKSuZY=", new DateTime(2019, 11, 28, 11, 44, 59, 934, DateTimeKind.Local).AddTicks(9355), "admin@email.com", true, "ZT6lnJoLZ7Y+49+22sCLJcLJetDMlxXAJtej1hKSuZY=", 1, "zihAoF7T1ueScNEFeHRhzw==" });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Answer", "Date", "Email", "IsAdmin", "Password", "Question", "Salt" },
                values: new object[] { 2, "TXO/6bqbTq270XBfK9sgayiF3bcHjf1k6YrgdC5TdqY=", new DateTime(2019, 11, 28, 11, 44, 59, 936, DateTimeKind.Local).AddTicks(6885), "user@email.com", false, "TXO/6bqbTq270XBfK9sgayiF3bcHjf1k6YrgdC5TdqY=", 1, "hfUOBYEYQMlpjyMVp4rscw==" });

            migrationBuilder.InsertData(
                table: "Configurations",
                columns: new[] { "Id", "Description", "IsPublic" },
                values: new object[] { 1, "CitationVK is a web-based machine learning tool.", true });

            migrationBuilder.CreateIndex(
                name: "IX_AccountClassifiers_ClassifierId",
                table: "AccountClassifiers",
                column: "ClassifierId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountDatasets_DatasetId",
                table: "AccountDatasets",
                column: "DatasetId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_DatasetId",
                table: "Articles",
                column: "DatasetId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountClassifiers");

            migrationBuilder.DropTable(
                name: "AccountDatasets");

            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "Configurations");

            migrationBuilder.DropTable(
                name: "Classifiers");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Datasets");
        }
    }
}

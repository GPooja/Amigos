using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmigosAPI.Migrations
{
    /// <inheritdoc />
    public partial class BaseMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AmigoUser",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AmigoUser", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AmigosLedger",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    EntryType = table.Column<int>(type: "int", nullable: false),
                    GivenByUserID = table.Column<int>(type: "int", nullable: false),
                    GivenToUserID = table.Column<int>(type: "int", nullable: false),
                    BillID = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AmigosLedger", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AmigosLedger_AmigoUser_GivenByUserID",
                        column: x => x.GivenByUserID,
                        principalTable: "AmigoUser",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_AmigosLedger_AmigoUser_GivenToUserID",
                        column: x => x.GivenToUserID,
                        principalTable: "AmigoUser",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Bill",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BillDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    AmountCAD = table.Column<double>(type: "float", nullable: false),
                    ConversionRate = table.Column<double>(type: "float", nullable: false),
                    PaidByUserID = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserID = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    ModifiedByUserID = table.Column<int>(type: "int", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bill", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Bill_AmigoUser_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "AmigoUser",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bill_AmigoUser_ModifiedByUserID",
                        column: x => x.ModifiedByUserID,
                        principalTable: "AmigoUser",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bill_AmigoUser_PaidByUserID",
                        column: x => x.PaidByUserID,
                        principalTable: "AmigoUser",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AmigosLedger_GivenByUserID",
                table: "AmigosLedger",
                column: "GivenByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_AmigosLedger_GivenToUserID",
                table: "AmigosLedger",
                column: "GivenToUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Bill_CreatedByUserID",
                table: "Bill",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Bill_ModifiedByUserID",
                table: "Bill",
                column: "ModifiedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Bill_PaidByUserID",
                table: "Bill",
                column: "PaidByUserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AmigosLedger");

            migrationBuilder.DropTable(
                name: "Bill");

            migrationBuilder.DropTable(
                name: "AmigoUser");
        }
    }
}

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Directory.Data.Migrations {
    [ExcludeFromCodeCoverage]
    public partial class Initial : Migration {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.CreateTable(
                name: "brother",
                columns: table => new {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    firstName = table.Column<string>(type: "varchar(40)", nullable: false),
                    lastName = table.Column<string>(type: "varchar(40)", nullable: false),
                    picture = table.Column<byte[]>(type: "mediumblob", nullable: true),
                    dateJoined = table.Column<DateTime>(type: "date", nullable: true),
                    dateInitiated = table.Column<DateTime>(type: "date", nullable: true),
                    chapterDesignation = table.Column<string>(type: "varchar(16)", nullable: true),
                    zetaNumber = table.Column<int>(type: "int(11)", nullable: true),
                    expectedGraduation = table.Column<DateTime>(type: "date", nullable: true),
                    bigBrotherID = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_brother", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "extracurricular",
                columns: table => new {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    name = table.Column<string>(type: "varchar(200)", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_extracurricular", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "major",
                columns: table => new {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    name = table.Column<string>(type: "varchar(75)", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_major", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "minor",
                columns: table => new {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    name = table.Column<string>(type: "varchar(75)", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_minor", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "position",
                columns: table => new {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    name = table.Column<string>(type: "varchar(75)", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_position", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "question",
                columns: table => new {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    questionText = table.Column<string>(type: "varchar(512)", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_question", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "inactive_brother",
                columns: table => new {
                    id = table.Column<int>(type: "int(11)", nullable: false),
                    reason = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_inactive_brother", x => x.id);
                    table.ForeignKey(
                        name: "inactive_brother_ibfk_1",
                        column: x => x.id,
                        principalTable: "brother",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "brother_extracurricular",
                columns: table => new {
                    brotherID = table.Column<int>(type: "int(11)", nullable: false),
                    extracurricularID = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PRIMARY", x => new { x.brotherID, x.extracurricularID });
                    table.ForeignKey(
                        name: "brother_extracurricular_ibfk_1",
                        column: x => x.brotherID,
                        principalTable: "brother",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "brother_extracurricular_ibfk_2",
                        column: x => x.extracurricularID,
                        principalTable: "extracurricular",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "brother_major",
                columns: table => new {
                    brotherID = table.Column<int>(type: "int(11)", nullable: false),
                    majorID = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PRIMARY", x => new { x.brotherID, x.majorID });
                    table.ForeignKey(
                        name: "brother_major_ibfk_1",
                        column: x => x.brotherID,
                        principalTable: "brother",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "brother_major_ibfk_2",
                        column: x => x.majorID,
                        principalTable: "major",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "brother_minor",
                columns: table => new {
                    brotherID = table.Column<int>(type: "int(11)", nullable: false),
                    minorID = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PRIMARY", x => new { x.brotherID, x.minorID });
                    table.ForeignKey(
                        name: "brother_minor_ibfk_1",
                        column: x => x.brotherID,
                        principalTable: "brother",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "brother_minor_ibfk_2",
                        column: x => x.minorID,
                        principalTable: "minor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "brother_position",
                columns: table => new {
                    brotherID = table.Column<int>(type: "int(11)", nullable: false),
                    positionID = table.Column<int>(type: "int(11)", nullable: false),
                    start = table.Column<DateTime>(type: "date", nullable: false),
                    end = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PRIMARY", x => new { x.brotherID, x.positionID, x.start, x.end });
                    table.ForeignKey(
                        name: "brother_position_ibfk_1",
                        column: x => x.brotherID,
                        principalTable: "brother",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "brother_position_ibfk_2",
                        column: x => x.positionID,
                        principalTable: "position",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "answer",
                columns: table => new {
                    questionID = table.Column<int>(type: "int(11)", nullable: false),
                    brotherID = table.Column<int>(type: "int(11)", nullable: false),
                    answerText = table.Column<string>(type: "varchar(2048)", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PRIMARY", x => new { x.questionID, x.brotherID });
                    table.ForeignKey(
                        name: "answer_ibfk_1",
                        column: x => x.brotherID,
                        principalTable: "brother",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "answer_ibfk_2",
                        column: x => x.questionID,
                        principalTable: "question",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "brotherID",
                table: "answer",
                column: "brotherID");

            migrationBuilder.CreateIndex(
                name: "extracurricularID",
                table: "brother_extracurricular",
                column: "extracurricularID");

            migrationBuilder.CreateIndex(
                name: "majorID",
                table: "brother_major",
                column: "majorID");

            migrationBuilder.CreateIndex(
                name: "minorID",
                table: "brother_minor",
                column: "minorID");

            migrationBuilder.CreateIndex(
                name: "positionID",
                table: "brother_position",
                column: "positionID");
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "answer");

            migrationBuilder.DropTable(
                name: "brother_extracurricular");

            migrationBuilder.DropTable(
                name: "brother_major");

            migrationBuilder.DropTable(
                name: "brother_minor");

            migrationBuilder.DropTable(
                name: "brother_position");

            migrationBuilder.DropTable(
                name: "inactive_brother");

            migrationBuilder.DropTable(
                name: "question");

            migrationBuilder.DropTable(
                name: "extracurricular");

            migrationBuilder.DropTable(
                name: "major");

            migrationBuilder.DropTable(
                name: "minor");

            migrationBuilder.DropTable(
                name: "position");

            migrationBuilder.DropTable(
                name: "brother");
        }
    }
}

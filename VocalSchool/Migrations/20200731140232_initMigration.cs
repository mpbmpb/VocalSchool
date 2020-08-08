using Microsoft.EntityFrameworkCore.Migrations;

namespace VocalSchool.Migrations
{
    public partial class initMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Days",
                columns: table => new
                {
                    DayId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Days", x => x.DayId);
                });

            migrationBuilder.CreateTable(
                name: "Seminars",
                columns: table => new
                {
                    SeminarId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seminars", x => x.SeminarId);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    SubjectId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    RequiredReading = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.SubjectId);
                });

            migrationBuilder.CreateTable(
                name: "CourseDesigns",
                columns: table => new
                {
                    CourseDesignId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    SeminarId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseDesigns", x => x.CourseDesignId);
                    table.ForeignKey(
                        name: "FK_CourseDesigns_Seminars_SeminarId",
                        column: x => x.SeminarId,
                        principalTable: "Seminars",
                        principalColumn: "SeminarId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SeminarDays",
                columns: table => new
                {
                    SeminarId = table.Column<int>(nullable: false),
                    DayId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeminarDays", x => new { x.SeminarId, x.DayId });
                    table.ForeignKey(
                        name: "FK_SeminarDays_Days_DayId",
                        column: x => x.DayId,
                        principalTable: "Days",
                        principalColumn: "DayId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SeminarDays_Seminars_SeminarId",
                        column: x => x.SeminarId,
                        principalTable: "Seminars",
                        principalColumn: "SeminarId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DaySubjects",
                columns: table => new
                {
                    DayId = table.Column<int>(nullable: false),
                    SubjectId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DaySubjects", x => new { x.DayId, x.SubjectId });
                    table.ForeignKey(
                        name: "FK_DaySubjects_Days_DayId",
                        column: x => x.DayId,
                        principalTable: "Days",
                        principalColumn: "DayId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DaySubjects_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseSeminars",
                columns: table => new
                {
                    CourseDesignId = table.Column<int>(nullable: false),
                    SeminarId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseSeminars", x => new { x.CourseDesignId, x.SeminarId });
                    table.ForeignKey(
                        name: "FK_CourseSeminars_CourseDesigns_CourseDesignId",
                        column: x => x.CourseDesignId,
                        principalTable: "CourseDesigns",
                        principalColumn: "CourseDesignId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseSeminars_Seminars_SeminarId",
                        column: x => x.SeminarId,
                        principalTable: "Seminars",
                        principalColumn: "SeminarId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseDesigns_SeminarId",
                table: "CourseDesigns",
                column: "SeminarId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseSeminars_SeminarId",
                table: "CourseSeminars",
                column: "SeminarId");

            migrationBuilder.CreateIndex(
                name: "IX_DaySubjects_SubjectId",
                table: "DaySubjects",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SeminarDays_DayId",
                table: "SeminarDays",
                column: "DayId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseSeminars");

            migrationBuilder.DropTable(
                name: "DaySubjects");

            migrationBuilder.DropTable(
                name: "SeminarDays");

            migrationBuilder.DropTable(
                name: "CourseDesigns");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "Days");

            migrationBuilder.DropTable(
                name: "Seminars");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace VocalSchool.Migrations
{
    public partial class addChangeProxy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_CourseDesigns_CourseDesignId",
                table: "Courses");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Venues",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Courses",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CourseDesignId",
                table: "Courses",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxStudents",
                table: "Courses",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EndTime",
                table: "CourseDates",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Contacts",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Changes",
                columns: table => new
                {
                    ChangeId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EntityType = table.Column<string>(nullable: true),
                    EntityId = table.Column<int>(nullable: false),
                    PropertyName = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    ForeignKey = table.Column<int>(nullable: true),
                    CourseId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Changes", x => x.ChangeId);
                    table.ForeignKey(
                        name: "FK_Changes_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Changes_CourseId",
                table: "Changes",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_CourseDesigns_CourseDesignId",
                table: "Courses",
                column: "CourseDesignId",
                principalTable: "CourseDesigns",
                principalColumn: "CourseDesignId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_CourseDesigns_CourseDesignId",
                table: "Courses");

            migrationBuilder.DropTable(
                name: "Changes");

            migrationBuilder.DropColumn(
                name: "MaxStudents",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "CourseDates");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Venues",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Courses",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<int>(
                name: "CourseDesignId",
                table: "Courses",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Contacts",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_CourseDesigns_CourseDesignId",
                table: "Courses",
                column: "CourseDesignId",
                principalTable: "CourseDesigns",
                principalColumn: "CourseDesignId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

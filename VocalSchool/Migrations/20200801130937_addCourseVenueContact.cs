using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VocalSchool.Migrations
{
    public partial class addCourseVenueContact : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    ContactId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Adress = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.ContactId);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    CourseId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    CourseDesignId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.CourseId);
                    table.ForeignKey(
                        name: "FK_Courses_CourseDesigns_CourseDesignId",
                        column: x => x.CourseDesignId,
                        principalTable: "CourseDesigns",
                        principalColumn: "CourseDesignId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Venues",
                columns: table => new
                {
                    VenueId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Info = table.Column<string>(nullable: true),
                    Email1 = table.Column<string>(nullable: true),
                    Email2 = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Adress = table.Column<string>(nullable: true),
                    MapsUrl = table.Column<string>(nullable: true),
                    Contact1ContactId = table.Column<int>(nullable: true),
                    Contact2ContactId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Venues", x => x.VenueId);
                    table.ForeignKey(
                        name: "FK_Venues_Contacts_Contact1ContactId",
                        column: x => x.Contact1ContactId,
                        principalTable: "Contacts",
                        principalColumn: "ContactId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Venues_Contacts_Contact2ContactId",
                        column: x => x.Contact2ContactId,
                        principalTable: "Contacts",
                        principalColumn: "ContactId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CourseDates",
                columns: table => new
                {
                    CourseDateId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(nullable: false),
                    VenueId = table.Column<int>(nullable: false),
                    ReservationInfo = table.Column<string>(nullable: true),
                    Rider = table.Column<string>(nullable: true),
                    CourseId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseDates", x => x.CourseDateId);
                    table.ForeignKey(
                        name: "FK_CourseDates_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseDates_Venues_VenueId",
                        column: x => x.VenueId,
                        principalTable: "Venues",
                        principalColumn: "VenueId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseDates_CourseId",
                table: "CourseDates",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseDates_VenueId",
                table: "CourseDates",
                column: "VenueId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CourseDesignId",
                table: "Courses",
                column: "CourseDesignId");

            migrationBuilder.CreateIndex(
                name: "IX_Venues_Contact1ContactId",
                table: "Venues",
                column: "Contact1ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Venues_Contact2ContactId",
                table: "Venues",
                column: "Contact2ContactId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseDates");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Venues");

            migrationBuilder.DropTable(
                name: "Contacts");
        }
    }
}

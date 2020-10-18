﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VocalSchool.Data;

namespace VocalSchool.Migrations
{
    [DbContext(typeof(SchoolContext))]
    partial class SchoolContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.7");

            modelBuilder.Entity("VocalSchool.Models.Change", b =>
                {
                    b.Property<int>("ChangeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CourseId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("EntityId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("EntityType")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ForeignKey")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PropertyName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("ChangeId");

                    b.HasIndex("CourseId");

                    b.ToTable("Changes");
                });

            modelBuilder.Entity("VocalSchool.Models.Contact", b =>
                {
                    b.Property<int>("ContactId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Adress")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Phone")
                        .HasColumnType("TEXT");

                    b.HasKey("ContactId");

                    b.ToTable("Contacts");
                });

            modelBuilder.Entity("VocalSchool.Models.Course", b =>
                {
                    b.Property<int>("CourseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CourseDesignId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<int>("MaxStudents")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("CourseId");

                    b.HasIndex("CourseDesignId");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("VocalSchool.Models.CourseDate", b =>
                {
                    b.Property<int>("CourseDateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CourseId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<string>("EndTime")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ReservationInfo")
                        .HasColumnType("TEXT");

                    b.Property<string>("Rider")
                        .HasColumnType("TEXT");

                    b.Property<int>("VenueId")
                        .HasColumnType("INTEGER");

                    b.HasKey("CourseDateId");

                    b.HasIndex("CourseId");

                    b.HasIndex("VenueId");

                    b.ToTable("CourseDates");
                });

            modelBuilder.Entity("VocalSchool.Models.CourseDesign", b =>
                {
                    b.Property<int>("CourseDesignId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("SeminarId")
                        .HasColumnType("INTEGER");

                    b.HasKey("CourseDesignId");

                    b.HasIndex("SeminarId");

                    b.ToTable("CourseDesigns");
                });

            modelBuilder.Entity("VocalSchool.Models.CourseSeminar", b =>
                {
                    b.Property<int>("CourseDesignId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SeminarId")
                        .HasColumnType("INTEGER");

                    b.HasKey("CourseDesignId", "SeminarId");

                    b.HasIndex("SeminarId");

                    b.ToTable("CourseSeminars");
                });

            modelBuilder.Entity("VocalSchool.Models.Day", b =>
                {
                    b.Property<int>("DayId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("DayId");

                    b.ToTable("Days");
                });

            modelBuilder.Entity("VocalSchool.Models.DaySubject", b =>
                {
                    b.Property<int>("DayId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SubjectId")
                        .HasColumnType("INTEGER");

                    b.HasKey("DayId", "SubjectId");

                    b.HasIndex("SubjectId");

                    b.ToTable("DaySubjects");
                });

            modelBuilder.Entity("VocalSchool.Models.Seminar", b =>
                {
                    b.Property<int>("SeminarId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("SeminarId");

                    b.ToTable("Seminars");
                });

            modelBuilder.Entity("VocalSchool.Models.SeminarDay", b =>
                {
                    b.Property<int>("SeminarId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("DayId")
                        .HasColumnType("INTEGER");

                    b.HasKey("SeminarId", "DayId");

                    b.HasIndex("DayId");

                    b.ToTable("SeminarDays");
                });

            modelBuilder.Entity("VocalSchool.Models.Subject", b =>
                {
                    b.Property<int>("SubjectId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("RequiredReading")
                        .HasColumnType("TEXT");

                    b.HasKey("SubjectId");

                    b.ToTable("Subjects");
                });

            modelBuilder.Entity("VocalSchool.Models.Venue", b =>
                {
                    b.Property<int>("VenueId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Adress")
                        .HasColumnType("TEXT");

                    b.Property<int?>("Contact1ContactId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Contact2ContactId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email1")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email2")
                        .HasColumnType("TEXT");

                    b.Property<string>("Info")
                        .HasColumnType("TEXT");

                    b.Property<string>("MapsUrl")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Phone")
                        .HasColumnType("TEXT");

                    b.HasKey("VenueId");

                    b.HasIndex("Contact1ContactId");

                    b.HasIndex("Contact2ContactId");

                    b.ToTable("Venues");
                });

            modelBuilder.Entity("VocalSchool.Models.Change", b =>
                {
                    b.HasOne("VocalSchool.Models.Course", "Course")
                        .WithMany("Changes")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VocalSchool.Models.Course", b =>
                {
                    b.HasOne("VocalSchool.Models.CourseDesign", "CourseDesign")
                        .WithMany()
                        .HasForeignKey("CourseDesignId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VocalSchool.Models.CourseDate", b =>
                {
                    b.HasOne("VocalSchool.Models.Course", "Course")
                        .WithMany("CourseDates")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VocalSchool.Models.Venue", "Venue")
                        .WithMany()
                        .HasForeignKey("VenueId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VocalSchool.Models.CourseDesign", b =>
                {
                    b.HasOne("VocalSchool.Models.Seminar", null)
                        .WithMany("CourseDesigns")
                        .HasForeignKey("SeminarId");
                });

            modelBuilder.Entity("VocalSchool.Models.CourseSeminar", b =>
                {
                    b.HasOne("VocalSchool.Models.CourseDesign", "CourseDesign")
                        .WithMany("CourseSeminars")
                        .HasForeignKey("CourseDesignId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VocalSchool.Models.Seminar", "Seminar")
                        .WithMany()
                        .HasForeignKey("SeminarId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VocalSchool.Models.DaySubject", b =>
                {
                    b.HasOne("VocalSchool.Models.Day", "Day")
                        .WithMany("DaySubjects")
                        .HasForeignKey("DayId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VocalSchool.Models.Subject", "Subject")
                        .WithMany("DaySubjects")
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VocalSchool.Models.SeminarDay", b =>
                {
                    b.HasOne("VocalSchool.Models.Day", "Day")
                        .WithMany("SeminarDays")
                        .HasForeignKey("DayId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VocalSchool.Models.Seminar", "Seminar")
                        .WithMany("SeminarDays")
                        .HasForeignKey("SeminarId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VocalSchool.Models.Venue", b =>
                {
                    b.HasOne("VocalSchool.Models.Contact", "Contact1")
                        .WithMany()
                        .HasForeignKey("Contact1ContactId");

                    b.HasOne("VocalSchool.Models.Contact", "Contact2")
                        .WithMany()
                        .HasForeignKey("Contact2ContactId");
                });
#pragma warning restore 612, 618
        }
    }
}

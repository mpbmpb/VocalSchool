using System;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using VocalSchool.Models;
using VocalSchool.Controllers;
using VocalSchool.Test.Infrastructure;
using VocalSchool.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using VocalSchool.Data;

namespace VocalSchool.Test.Controllers
{
    public class CourseControllerTests : UnitTestBase

    {
        public CourseControllerTests(ConfigFixture fixture) : base(fixture)
        {
        }

        private CourseController Controller => new CourseController(Context);

        private static Course Course7 => new Course
        {
            Name = "Course7",
            Description = "Learn about effects",
            CourseDates = new List<CourseDate>()
        };
        
        [Fact]
        public async Task Index_returns_ViewResult()
        {
            var result = await Controller.Index();
            
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Index_returns_Courses()
        {
            var result = await Controller.Index();

            result.As<ViewResult>().Model.Should().BeOfType<List<Course>>();
        }

        [Fact]
        public async Task Index_returns_All_Courses_without_uid()
        {
            var result = await Controller.Index();

            result.As<ViewResult>().Model.As<List<Course>>().Should().HaveCount(3);
        }

        [Fact]
        public async Task Details_returns_Course()
        {
            var result = await Controller.Details(1);

            result.As<ViewResult>().Model.Should().BeAssignableTo<Course>();
        }

        [Fact]
        public async Task Details_returns_Correct_Course()
        {
            var result = await Controller.Details(1);

            result.As<ViewResult>().Model.As<Course>().Name.Should().Be("Course1");
        }

        [Fact]
        public async Task Details_returns_Course_with_Correct_CourseDesign()
        {
            var result = await Controller.Details(2);

            result.As<ViewResult>().Model.As<Course>().CourseDesign.Name.Should()
                .Match("CourseDesign2");
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(2, 0)]
        [InlineData(3, 0)]
        public async Task Details_returns_Course_with_assigned_CourseDates(int id, int expected)
        {
            var result = await Controller.Details(id);

            result.As<ViewResult>().Model.As<Course>().CourseDates
                .Should().HaveCount(expected, because: "we seeded the Db with 2 CourseDates for Course1");
        }

        [Fact]
        public async Task Details_returns_Notfound_if_given_unknown_id()
        {
            var result = await Controller.Details(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_returns_view_when_not_passed_Id()
        {
            var result = await Controller.Create();

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Create_shows_only_CourseDesigns_without_uid()
        {
            var cd = new CourseDesign {Name = "[uid] test", CourseDesignId = 7};
            Context.Add(cd);
            await Context.SaveChangesAsync();
            var result = await Controller.Create();

            result.As<ViewResult>().Model.As<CourseViewModel>().CourseDesigns
                .Count(x => x.Name.Substring(0, 1) == "[")
                .Should().Be(0);
        }


        [Fact]
        public async Task Create_stores_new_Course()
        {
            var courseView = new CourseViewModel(new List<CourseDesign>()) {Course = Course7};
            courseView.Course.CourseDesign = Context.CourseDesigns.FirstOrDefault(x => x.CourseDesignId == 1);
            
            await Controller.Create(courseView);

            Resultcontext.Courses.Should().HaveCount(4);
        }

        [Fact]
        public async Task Create_stores_Course_with_correct_properties()
        {
            var courseView = new CourseViewModel(new List<CourseDesign>()) {Course = Course7};
            courseView.Course.CourseDesign = Context.CourseDesigns.FirstOrDefault(x => x.CourseDesignId == 1);
            
            await Controller.Create(courseView);

            var result = Resultcontext.Courses.FirstOrDefault(x => x.CourseId == 4); 
            
            result?.Name.Should().Match(Course7.Name);
            result?.Description.Should().Match(Course7.Description);
            result?.CourseDesign.CourseDesignId.Should().Be(4, because:"we seeded the db with 3 CourseDesigns");
        }
        
        [Fact]
        public async Task CopyAndPrepend_does_just_that()
        {
            var s = await Context.Subjects.FirstOrDefaultAsync(x => x.SubjectId == 1);
            var result = await s.CopyAndPrependNameWithAsync("[test]", new DbHandler(Context));

            result.Name.Should().Match("[test] Introduction");
            result.Description.Should().Match(s.Description);
            result.RequiredReading.Should().Match(s.RequiredReading);
            result.SubjectId.Should().Be(7);
        }


        [Fact]
        public async Task Create_stores_Course_with_correct_CourseDesign()
        {
            var courseView = new CourseViewModel(new List<CourseDesign>()) {Course = Course7};
            courseView.Course.CourseDesign = new CourseDesign
            {
                CourseDesignId = 3
            };

            await Controller.Create(courseView);
            var result = Resultcontext.Courses.FirstOrDefault(x => x.CourseId == 4);

            result?.CourseDesign.Name.Should().Match("[Course7_4] CourseDesign3");
        }

        [Theory]
        [InlineData(1, 11)]
        [InlineData(2, 15)]
        [InlineData(3, 22)]
        public async Task Create_copies_all_subjects_in_CourseDesign(int id, int expected)
        {
            var courseView = new CourseViewModel(new List<CourseDesign>()) {Course = Course7};
            courseView.Course.CourseDesign = new CourseDesign
            {
                CourseDesignId = id
            };

            await Controller.Create(courseView);
            Resultcontext.Subjects.Should().HaveCount(expected);
        }


        [Fact]
        public async Task Edit_returns_Notfound_if_given_unknown_id()
        {
            var result = await Controller.Edit(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Edit_returns_CourseViewModel()
        {
            var result = await Controller.Edit(1);

            result.As<ViewResult>().Model.Should().BeOfType<CourseViewModel>();
        }

        [Fact]
        public async Task Edit_returns_CourseViewModel_with_DesignList()
        {
            var result = await Controller.Edit(1);

            result.As<ViewResult>().Model.As<CourseViewModel>().DesignList
                .Should().NotBeNullOrEmpty(because: "we seeded the Db with CourseDesigns");
        }

        [Fact]
        public async Task Edit_returns_CourseViewModel_with_correct_Course()
        {
            var result = await Controller.Edit(1);

            result.As<ViewResult>().Model.As<CourseViewModel>().Course.Name.Should().Be("Course1");
        }

        [Fact]
        public async Task Edit_updates_Course_with_correct_properties()
        {            
            var c = Context.Courses.FirstOrDefault(x => x.CourseId == 1);
            c.Name = "Effects";
            c.Description = "Learn about effects";

            var courseView = new CourseViewModel {Course = c, DesignList = new List<SelectListItem>()};

            await Controller.Edit(1, courseView);

            var result = Resultcontext.Courses.FirstOrDefault(x => x.CourseId == 1);
                
            result?.Name.Should().Be(c.Name);
            result?.Description.Should().Be(c.Description);
        }

        [Fact]
        public async Task Edit_only_loads_current_CourseDesign_in_designList()
        {
            var result = await Controller.Edit(1);

            result.As<ViewResult>().Model.As<CourseViewModel>().DesignList.Count().Should().Be(2, because:"1st item is --select-- 2nd item is CourseDesign");
        }

        [Fact]
        public async Task Edit_makes_new_copies_if_name_differs_from_design_name_uid()
        {
            var actionResult = await Controller.Edit(1);
            var model = actionResult.As<ViewResult>().Model.As<CourseViewModel>();
            model.Course.CourseDesign.Name = "[test_0] name";
            await Controller.Edit(1, model);

            Resultcontext.Subjects.Count(x => x.Name.Substring(0, 1) == "[").Should().Be(5,
                because: "courseDesign 1 has 5 subjects now copied and prepended with uid");
        }

        [Fact]
        public async Task Edit_makes_new_copies_deletes_old_elements_if_name_changes_and_design_name_has_uid()
        {
            var courseView = new CourseViewModel(new List<CourseDesign>()) {Course = Course7};
            courseView.Course.CourseDesign = new CourseDesign
            {
                CourseDesignId = 1
            };
            await Controller.Create(courseView);
            
            var actionResult = await Controller.Edit(4);
            var model = actionResult.As<ViewResult>().Model.As<CourseViewModel>();
            
            model.Course.Name = "test";
            await Controller.Edit(4, model);

            Resultcontext.Subjects.Count().Should().Be(11,
                because: "courseDesign 1 has 5 subjects now copied twice and removed once and added to the 6 seeded subjects");
            Resultcontext.Subjects.Count(x => x.Name.Length >= 6 
                                              && x.Name.Substring(0, 6) == "[test_")
                .Should().Be(5, "5 subjects are now copied and prepended with uid");
            Resultcontext.Courses.Count().Should().Be(4);
        }

        [Fact]
        public async Task Edit_returns_NotFound_if_Id_changes()
        {            
            var c = Context.Courses.FirstOrDefault(x => x.CourseId == 1);
            var courseView = new CourseViewModel {Course = c, DesignList = new List<SelectListItem>()};

            var result = await Controller.Edit(8, courseView);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Edit_returns_View_if_model_state_not_valid()
        {     
            var controller = Controller;
            controller.ViewData.ModelState.AddModelError("key", "some error");
            var courseView = new CourseViewModel 
                {Course = new Course() {CourseId = 1}, DesignList = new List<SelectListItem>()};

            var result = await controller.Edit(1, courseView);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Edit_returns_NotFound_if_concurrencyException_occurs()
        {            
            var c = await Context.Courses.FirstOrDefaultAsync(x => x.CourseId == 1);
            var courseDesigns = await Context.CourseDesigns.ToListAsync();
            var courseView = new CourseViewModel(c, courseDesigns);

            Context.Remove(c);
            await Context.SaveChangesAsync();

            var result = await Controller.Edit(1, courseView);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_returns_Correct_Course()
        {
            var result = await Controller.Delete(1);

            result.As<ViewResult>().Model.As<Course>().Name.Should().Be("Course1");
        }

        [Fact]
        public async Task Delete_returns_Notfound_if_given_unknown_id()
        {
            var result = await Controller.Delete(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_removes_Course_from_Db()
        {
            await Controller.DeleteConfirmed(1);

            Resultcontext.Courses.FirstOrDefault(x => x.CourseId == 1).Should().BeNull();
        }

        [Fact]
        public async Task Delete_removes_all_related_course_elements_if_it_has_uid()
        {
            var courseView = new CourseViewModel(new List<CourseDesign>()) {Course = Course7};
            courseView.Course.CourseDesign = new CourseDesign
            {
                CourseDesignId = 2
            };
            courseView.Course.CourseId = 7;
            await Controller.Create(courseView);
            
            await Controller.DeleteConfirmed(7);
            
            Resultcontext.CourseDesigns.Should().HaveCount(3, because:"We seeded the db with 3 CourseDesigns");
            Resultcontext.Seminars.Should().HaveCount(3, because:"We seeded the db with 3 seminars");
            Resultcontext.Days.Should().HaveCount(6, because:"We seeded the db with 6 days");
            Resultcontext.Subjects.Should().HaveCount(6, because:"We seeded the db with 6 subjects");
        }


        [Fact]
        public async Task AddCourseDates_returns_Notfound_if_given_unknown_id()
        {
            var result = await Controller.AddCourseDates(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task AddCourseDates_returns_ViewResult()
        {
            var result = await Controller.AddCourseDates(1);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task AddCourseDates_returns_CourseViewModel()
        {
            var result = await Controller.AddCourseDates(1);

            result.As<ViewResult>().Model.Should().BeOfType<CourseViewModel>();
        }

        [Fact]
        public async Task AddCourseDates_returns_CourseViewModel_with_CourseDates()
        {
            var result = await Controller.AddCourseDates(1);

            result.As<ViewResult>().Model.As<CourseViewModel>()
                .CourseDates.Should().NotBeNullOrEmpty(because:
                "we seeded the Db with 2 CourseDates for course 1");
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(2, 0)]
        public async Task AddCourseDates_returns_CourseViewModel_with_All_assigned_CourseDates(int id, int expected)
        {
            var result = await Controller.AddCourseDates(id);

            result.As<ViewResult>().Model.As<CourseViewModel>()
                .CourseDates.Should().HaveCount(expected, because:
                "we seeded the Db with 2 CourseDates for course 1");
        }

        [Theory]
        [InlineData(1,2)]
        [InlineData(2,4)]
        [InlineData(3,6)]
        public async Task AddCourseDates_returns_CourseViewModel_with__correct_DayCount(int id, int expected)
        {
            var result = await Controller.AddCourseDates(id);
            
            result.As<ViewResult>().Model.As<CourseViewModel>().DayCount.Should().Be(expected);
        }

        [Fact]
        public async Task AddCourseDates_adds_CourseDate()
        {            
            var course = await Context.Courses.FindAsync(2);
            var courseDates = await Context.CourseDates.ToListAsync();
            var model = new CourseViewModel(course, courseDates);
            var date = new CourseDate
            {
                Date = new DateTime(2080, 01, 03),
                Venue = await Context.Venues.FindAsync(2),
                Course = await Context.Courses.FindAsync(2),
                ReservationInfo = "Reserveringsnr: 1335"
            };
            model.CourseDates.Add(date);

            await Controller.AddCourseDates(2, model);

            Resultcontext.CourseDates.Should().HaveCount(3, because:
                "we seeded the Db with 2 CourseDates");
        }

        [Theory]
        [InlineData(1, 3)]
        [InlineData(2, 1)]
        [InlineData(3, 1)]
        public async Task AddCourseDates_adds_CourseDate_to_correct_course(int id, int expected)
        {            
            var course = await Context.Courses.FindAsync(id);
            var courseDates = await Context.CourseDates.ToListAsync();
            var model = new CourseViewModel(course, courseDates);
            var date = new CourseDate
            {
                Date = new DateTime(2080, 01, 03),
                Venue = await Context.Venues.FindAsync(2),
                Course = await Context.Courses.FindAsync(id),
                ReservationInfo = "Reserveringsnr: 1335"
            };
            model.CourseDates.Add(date);

            await Controller.AddCourseDates(id, model);

            var result = await Controller.Details(id);

            result.As<ViewResult>().Model.As<Course>().CourseDates
                .Should().HaveCount(expected);
        }

        [Fact]
        public async Task AddCourseDates_adds_CourseDate_with_correct_properties()
        {            
            var course = await Context.Courses.FindAsync(2);
            var courseDates = await Context.CourseDates.ToListAsync();
            var model = new CourseViewModel(course, courseDates);
            var date = new CourseDate
            {
                Date = new DateTime(2080, 01, 03),
                Venue = await Context.Venues.FindAsync(2),
                Course = await Context.Courses.FindAsync(2),
                ReservationInfo = "Reserveringsnr: 1335"
            };
            model.CourseDates.Add(date);
            await Controller.AddCourseDates(2, model);

            var result = await Controller.Details(2);

            result.As<ViewResult>().Model.As<Course>().CourseDates
                .FirstOrDefault().Should().BeEquivalentTo(date);
        }
        
        [Fact]
        public async Task AddCourseDates_updates_CourseDate_with_correct_properties()
        {            
            var course = await Context.Courses.FindAsync(1);
            var courseDates = await Context.CourseDates.ToListAsync();
            var model = new CourseViewModel(course, courseDates);
            var date = new CourseDate
            {
                CourseDateId = 1,
                Date = new DateTime(2080, 01, 03),
                VenueId = 2,
                Venue = await Context.Venues.FindAsync(2),
                CourseId = 1,
                Course = course,
                ReservationInfo = "Reserveringsnr: 1335"
            };
            model.CourseDates[1] = date;
            await Controller.AddCourseDates(1, model);

            var result = await Resultcontext.CourseDates
                .FirstOrDefaultAsync(cd => cd.CourseDateId ==1);

            result.As<CourseDate>().Should().BeEquivalentTo(date,
                options => options.Excluding(cd => cd.Course));
        }

        [Fact]
        public async Task AddCourseDates_returns_NotFound_if_Id_changes()
        {            
            var c = Context.Courses.FirstOrDefault(x => x.CourseId == 1);
            var courseView = new CourseViewModel {Course = c, DesignList = new List<SelectListItem>()};

            var result = await Controller.AddCourseDates(8, courseView);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task AddCourseDates_returns_View_if_modelstate_not_valid()
        {
            var controller = Controller;
            controller.ViewData.ModelState.AddModelError("key", "some error");
            var courseView = new CourseViewModel 
                {Course = new Course() {CourseId = 1}, DesignList = new List<SelectListItem>()};

            var result = await controller.AddCourseDates(1, courseView);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task AddCourseDates_returns_NotFound_if_concurrencyException_occurs()
        {            
            var c = await Context.Courses.FirstOrDefaultAsync(x => x.CourseId == 1);
            var courseDesigns = await Context.CourseDesigns.ToListAsync();
            var courseView = new CourseViewModel(c, courseDesigns);

            Context.Remove(c);
            await Context.SaveChangesAsync();

            var result = await Controller.AddCourseDates(1, courseView);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void Validation_Leaving_Name_Null_or_short_causes_modelstate_not_valid()
        {
            var c = new Course();
            var c2 = new Course();
            c.CourseId = 1;
            c2.CourseId = 1;
            c2.Name = "123";

            var result = Validator.TryValidateObject(c, new ValidationContext(c), null, true);
            var result2 = Validator.TryValidateObject(c2, new ValidationContext(c2), null, true);

            result.Should().BeFalse();
            result2.Should().BeFalse();
        }


    }
}

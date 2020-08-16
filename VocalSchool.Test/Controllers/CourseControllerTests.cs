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

namespace VocalSchool.Test.Controllers
{
    public class CourseControllerTests : VocalSchoolTestBase

    {
        [Fact]
        public async Task Index_returns_ViewResult()
        {
            //Arrange
            var controller = new CourseController(_context);

            //Act
            IActionResult result = await controller.Index();

            //Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Index_returns_Courses()
        {
            var controller = new CourseController(_context);

            IActionResult result = await controller.Index();

            result.As<ViewResult>().Model.Should().BeOfType<List<Course>>();
        }

        [Fact]
        public async Task Index_returns_All_Courses()
        {
            var controller = new CourseController(_context);

            IActionResult result = await controller.Index();

            result.As<ViewResult>().Model.As<List<Course>>().Should().HaveCount(3);
        }

        [Fact]
        public async Task Details_returns_Course()
        {
            var controller = new CourseController(_context);

            IActionResult result = await controller.Details(1);

            result.As<ViewResult>().Model.Should().BeAssignableTo<Course>();
        }

        [Fact]
        public async Task Details_returns_Correct_Course()
        {
            var controller = new CourseController(_context);

            IActionResult result = await controller.Details(1);

            result.As<ViewResult>().Model.As<Course>().Name.Should().Be("Course1");
        }

        [Fact]
        public async Task Details_returns_Course_with_Correct_CourseDesign()
        {
            var controller = new CourseController(_context);

            IActionResult result = await controller.Details(2);

            result.As<ViewResult>().Model.As<Course>().CourseDesign.Name.Should()
                .Match("CourseDesign2");
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(2, 0)]
        [InlineData(3, 0)]
        public async Task Details_returns_Course_with_assigned_CourseDates(int id, int expected)
        {
            var controller = new CourseController(_context);

            IActionResult result = await controller.Details(id);

            result.As<ViewResult>().Model.As<Course>().CourseDates
                .Should().HaveCount(expected, because: "we seeded the Db with 2 CourseDates for Course1");
        }

        [Fact]
        public async Task Details_returns_Notfound_if_given_unknown_id()
        {
            var controller = new CourseController(_context);

            IActionResult result = await controller.Details(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_returns_view_when_not_passed_Id()
        {
            var controller = new CourseController(_context);

            IActionResult result = await controller.Create();

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Create_stores_new_Course()
        {
            var controller = new CourseController(_context);
            Course c = new Course
            {
                CourseId = 7,
                Name = "Course7",
                Description = "Learn about effects",
            };

            CourseViewModel CourseView = new CourseViewModel();
            CourseView.Course = c;
            CourseView.DesignList = new List<SelectListItem>();

            await controller.Create(CourseView);

            _context.Courses.Should().HaveCount(4);
        }

        [Fact]
        public async Task Create_stores_Course_with_correct_properties()
        {
            var controller = new CourseController(_context);
            Course c = new Course
            {
                CourseId = 7,
                Name = "Course7",
                Description = "Learn about effects",
            };

            CourseViewModel CourseView = new CourseViewModel();
            CourseView.Course = c;
            CourseView.DesignList = new List<SelectListItem>();

            await controller.Create(CourseView);

            _context.Courses.FirstOrDefault(x => x.CourseId == 7).Should().BeEquivalentTo(c);
        }

        [Fact]
        public async Task Create_stores_Course_with_correct_CourseDesign()
        {
            var controller = new CourseController(_context);
            Course c = new Course
            {
                CourseId = 7,
                Name = "Course7",
                Description = "Learn about effects",
            };
            var CourseDesigns = await _context.CourseDesigns.ToListAsync();
            CourseViewModel CourseView = new CourseViewModel(CourseDesigns);
            CourseView.Course = c;
            CourseView.Course.CourseDesign = new CourseDesign();
            CourseView.Course.CourseDesign.CourseDesignId = Int32.Parse(CourseView.DesignList[1].Value);

            await controller.Create(CourseView);

            var result = _context.Courses.FirstOrDefault(x => x.CourseId == 7);

            result.CourseDesign.Name.Should().Match("CourseDesign3");
        }

        [Fact]
        public async Task Edit_returns_Notfound_if_given_unknown_id()
        {
            var controller = new CourseController(_context);

            IActionResult result = await controller.Edit(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Edit_returns_CourseViewModel()
        {
            var controller = new CourseController(_context);

            IActionResult result = await controller.Edit(1);

            result.As<ViewResult>().Model.Should().BeOfType<CourseViewModel>();
        }

        [Fact]
        public async Task Edit_returns_CourseViewModel_with_DesignList()
        {
            var controller = new CourseController(_context);

            IActionResult result = await controller.Edit(1);

            result.As<ViewResult>().Model.As<CourseViewModel>().DesignList
                .Should().NotBeNullOrEmpty(because: "we seeded the Db with CourseDesigns");
        }

        [Fact]
        public async Task Edit_returns_CourseViewModel_with_correct_Course()
        {
            var controller = new CourseController(_context);

            IActionResult result = await controller.Edit(1);

            result.As<ViewResult>().Model.As<CourseViewModel>().Course.Name.Should().Be("Course1");
        }

        [Fact]
        public async Task Edit_returns_CourseViewModel_with_ALL_CourseDesigns_injected_into_DesignList()
        {
            var controller = new CourseController(_context);

            IActionResult result = await controller.Edit(1);

            result.As<ViewResult>().Model.As<CourseViewModel>().DesignList
                .Should().HaveCount(4).And.Contain(x => x.Text == "CourseDesign1")
                .And.Contain(x => x.Text == "CourseDesign2").And.Contain(x => x.Text == "CourseDesign3");
        }

        [Fact]
        public async Task Edit_updates_Course_with_correct_properties()
        {
            var controller = new CourseController(_context);
            Course c = _context.Courses.FirstOrDefault(x => x.CourseId == 1);
            c.Name = "Effects";
            c.Description = "Learn about effects";

            CourseViewModel CourseView = new CourseViewModel();
            CourseView.Course = c;
            CourseView.DesignList = new List<SelectListItem>();

            await controller.Edit(1, CourseView);

            _context.Courses.FirstOrDefault(x => x.CourseId == 1).Should().BeEquivalentTo<Course>(c);
        }

        [Fact]
        public async Task Edit_updates_Course_with_correct_CourseDesign()
        {
            var controller = new CourseController(_context);
            Course c = _context.Courses.FirstOrDefault(x => x.CourseId == 1);
            var CourseDesigns = await _context.CourseDesigns.ToListAsync();

            CourseViewModel CourseView = new CourseViewModel(c, CourseDesigns);
            CourseView.Course.CourseDesign = new CourseDesign();
            CourseView.Course.CourseDesign.CourseDesignId = Int32.Parse(CourseView.DesignList[1].Value);

            await controller.Edit(1, CourseView);

            _context.Courses.FirstOrDefault(x => x.CourseId == 1).CourseDesign
                .Name.Should().Match("CourseDesign3");
        }

        [Fact]
        public async Task Edit_returns_NotFound_if_Id_changes()
        {
            var controller = new CourseController(_context);
            Course c = _context.Courses.FirstOrDefault(x => x.CourseId == 1);

            CourseViewModel CourseView = new CourseViewModel();
            CourseView.Course = c;
            CourseView.DesignList = new List<SelectListItem>();

            IActionResult result = await controller.Edit(8, CourseView);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Edit_returns_View_if_modelstate_not_valid()
        {
            var controller = new CourseController(_context);
            controller.ViewData.ModelState.AddModelError("key", "some error");
            CourseViewModel CourseView = new CourseViewModel();
            CourseView.Course = new Course() { CourseId = 1 };
            CourseView.DesignList = new List<SelectListItem>();

            IActionResult result = await controller.Edit(1, CourseView);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Edit_returns_NotFound_if_concurrencyException_occurs()
        {
            var controller = new CourseController(_context);
            Course c = await _context.Courses.FirstOrDefaultAsync(x => x.CourseId == 1);
            var CourseDesigns = await _context.CourseDesigns.ToListAsync();
            CourseViewModel CourseView = new CourseViewModel(c, CourseDesigns);

            _context.Remove(c);
            await _context.SaveChangesAsync();

            IActionResult result = await controller.Edit(1, CourseView);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_returns_Correct_Course()
        {
            var controller = new CourseController(_context);

            IActionResult result = await controller.Delete(1);

            result.As<ViewResult>().Model.As<Course>().Name.Should().Be("Course1");
        }

        [Fact]
        public async Task Delete_returns_Notfound_if_given_unknown_id()
        {
            var controller = new CourseController(_context);

            IActionResult result = await controller.Delete(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_removes_Course_from_Db()
        {
            var controller = new CourseController(_context);

            var Course = _context.Courses.FirstOrDefault(x => x.CourseId == 1);

            await controller.DeleteConfirmed(1);

            var result = _context.Courses.FirstOrDefault(x => x.CourseId == 1);

            result.Should().BeNull();
        }

        [Fact]
        public async Task AddCourseDates_returns_Notfound_if_given_unknown_id()
        {
            var controller = new CourseController(_context);

            IActionResult result = await controller.AddCourseDates(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task AddCourseDates_returns_ViewResult()
        {
            var controller = new CourseController(_context);

            IActionResult result = await controller.AddCourseDates(1);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task AddCourseDates_returns_CourseViewModel()
        {
            var controller = new CourseController(_context);

            IActionResult result = await controller.AddCourseDates(1);

            result.As<ViewResult>().Model.Should().BeOfType<CourseViewModel>();
        }

        [Fact]
        public async Task AddCourseDates_returns_CourseViewModel_with_CourseDates()
        {
            var controller = new CourseController(_context);

            IActionResult result = await controller.AddCourseDates(1);

            result.As<ViewResult>().Model.As<CourseViewModel>()
                .CourseDates.Should().NotBeNullOrEmpty(because:
                "we seeded the Db with 2 CourseDates for course 1");
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(2, 0)]
        public async Task AddCourseDates_returns_CourseViewModel_with_All_assigned_CourseDates(int id, int expected)
        {
            var controller = new CourseController(_context);

            IActionResult result = await controller.AddCourseDates(id);

            result.As<ViewResult>().Model.As<CourseViewModel>()
                .CourseDates.Should().HaveCount(expected, because:
                "we seeded the Db with 2 CourseDates for course 1");

        }

        [Theory]
        [InlineData(1,2)]
        [InlineData(2,4)]
        [InlineData(3,6)]
        public async Task AddCourseDates_returns_CourseViewModel_with_DayCount_equal_to_Days_in_Course(int id, int expected)
        {
            var controller = new CourseController(_context);

            IActionResult result = await controller.AddCourseDates(id);

            if (ViewContainsDay("Course","Index"))
            {
                result.As<ViewResult>().Model.As<CourseViewModel>()
                .DayCount.Should().Be(expected);
            }
            else
            {
                result.As<ViewResult>().Model.As<CourseViewModel>()
                .DayCount.Should().Be(1);
            }
        }

        [Fact]
        public async Task AddCourseDates_adds_CourseDate()
        {
            var controller = new CourseController(_context);
            var course = await _context.Courses.FindAsync(2);
            var courseDates = await _context.CourseDates.ToListAsync();
            var model = new CourseViewModel(course, courseDates);
            var date = new CourseDate
            {
                Date = new DateTime(2080, 01, 03),
                Venue = _context.Venues.Find(2),
                Course = _context.Courses.Find(2),
                ReservationInfo = "Reserveringsnr: 1335"
            };
            model.CourseDates.Add(date);

            IActionResult result = await controller.AddCourseDates(2, model);

            _context.CourseDates.Should().HaveCount(3, because:
                "we seeded the Db with 2 CourseDates");
        }

        [Theory]
        [InlineData(1, 3)]
        [InlineData(2, 1)]
        [InlineData(3, 1)]
        public async Task AddCourseDates_adds_CourseDate_to_correct_course(int id, int expected)
        {
            var controller = new CourseController(_context);
            var course = await _context.Courses.FindAsync(id);
            var courseDates = await _context.CourseDates.ToListAsync();
            var model = new CourseViewModel(course, courseDates);
            var date = new CourseDate
            {
                Date = new DateTime(2080, 01, 03),
                Venue = _context.Venues.Find(2),
                Course = _context.Courses.Find(id),
                ReservationInfo = "Reserveringsnr: 1335"
            };
            model.CourseDates.Add(date);

            await controller.AddCourseDates(id, model);

            IActionResult result = await controller.Details(id);

            result.As<ViewResult>().Model.As<Course>().CourseDates
                .Should().HaveCount(expected);
        }

        [Fact]
        public async Task AddCourseDates_adds_CourseDate_with_correct_properties()
        {
            var controller = new CourseController(_context);
            var course = await _context.Courses.FindAsync(2);
            var courseDates = await _context.CourseDates.ToListAsync();
            var model = new CourseViewModel(course, courseDates);
            var date = new CourseDate
            {
                Date = new DateTime(2080, 01, 03),
                Venue = _context.Venues.Find(2),
                Course = _context.Courses.Find(2),
                ReservationInfo = "Reserveringsnr: 1335"
            };
            model.CourseDates.Add(date);
            await controller.AddCourseDates(2, model);

            IActionResult result = await controller.Details(2);

            result.As<ViewResult>().Model.As<Course>().CourseDates
                .FirstOrDefault().Should().BeEquivalentTo(date);
        }
        
        [Fact]
        public async Task AddCourseDates_updates_CourseDate_with_correct_properties()
        {
            var controller = new CourseController(_context);
            var course = await _context.Courses.FindAsync(1);
            var courseDates = await _context.CourseDates.ToListAsync();
            var model = new CourseViewModel(course, courseDates);
            var date = new CourseDate
            {
                CourseDateId = 1,
                Date = new DateTime(2080, 01, 03),
                VenueId = 1,
                Venue = _context.Venues.Find(2),
                CourseId = 1,
                Course = _context.Courses.Find(1),
                ReservationInfo = "Reserveringsnr: 1335"
            };
            model.CourseDates[1] = date;
            await controller.AddCourseDates(1, model);

            var result = await _resultcontext.CourseDates
                .FirstOrDefaultAsync(e => e.CourseDateId ==1);

            result.As<CourseDate>().Should().BeEquivalentTo(date,
                options => options.Excluding(e => e.Course)
                .Excluding(e => e.Venue));
        }

        [Fact]
        public async Task AddCourseDates_returns_NotFound_if_Id_changes()
        {
            var controller = new CourseController(_context);
            Course c = _context.Courses.FirstOrDefault(x => x.CourseId == 1);

            CourseViewModel CourseView = new CourseViewModel();
            CourseView.Course = c;
            CourseView.DesignList = new List<SelectListItem>();

            IActionResult result = await controller.AddCourseDates(8, CourseView);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task AddCourseDates_returns_View_if_modelstate_not_valid()
        {
            var controller = new CourseController(_context);
            controller.ViewData.ModelState.AddModelError("key", "some error");
            CourseViewModel CourseView = new CourseViewModel();
            CourseView.Course = new Course() { CourseId = 1 };
            CourseView.DesignList = new List<SelectListItem>();

            IActionResult result = await controller.AddCourseDates(1, CourseView);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task AddCourseDates_returns_NotFound_if_concurrencyException_occurs()
        {
            var controller = new CourseController(_context);
            Course c = await _context.Courses.FirstOrDefaultAsync(x => x.CourseId == 1);
            var CourseDesigns = await _context.CourseDesigns.ToListAsync();
            CourseViewModel CourseView = new CourseViewModel(c, CourseDesigns);

            _context.Remove(c);
            await _context.SaveChangesAsync();

            IActionResult result = await controller.AddCourseDates(1, CourseView);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void Validation_Leaving_Name_Null_or_short_causes_modelstate_not_valid()
        {
            Course c = new Course();
            Course c2 = new Course();
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

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

namespace VocalSchool.Test.Controllers
{
    public class SeminarControllerTests : VocalSchoolTestBase

    {
        [Fact]
        public async Task Index_returns_ViewResult()
        {
            //Arrange
            var controller = new SeminarController(_context);

            //Act
            IActionResult result = await controller.Index();

            //Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Index_returns_Seminars()
        {
            var controller = new SeminarController(_context);

            IActionResult result = await controller.Index();

            result.As<ViewResult>().Model.Should().BeOfType<List<Seminar>>();
        }

        [Fact]
        public async Task Index_returns_All_Seminars()
        {
            var controller = new SeminarController(_context);

            IActionResult result = await controller.Index();

            result.As<ViewResult>().Model.As<List<Seminar>>().Should().HaveCount(3);
        }

        [Fact]
        public async Task Details_returns_Seminar()
        {
            var controller = new SeminarController(_context);

            IActionResult result = await controller.Details(1);

            result.As<ViewResult>().Model.Should().BeOfType<Seminar>();
        }

        [Fact]
        public async Task Details_returns_Correct_Seminar()
        {
            var controller = new SeminarController(_context);

            IActionResult result = await controller.Details(1);

            result.As<ViewResult>().Model.As<Seminar>().Name.Should().Be("Seminar1");
        }

        [Fact]
        public async Task Details_returns_Correct_Seminar_with_All_SeminarDays()
        {
            var controller = new SeminarController(_context);

            IActionResult result = await controller.Details(1);

            result.As<ViewResult>().Model.As<Seminar>().SeminarDays.Should()
                .HaveCount(2).And.Contain(x => x.Day.Name == "Day1")
                .And.Contain(x => x.Day.Name == "Day2");
        }

        [Fact]
        public async Task Details_returns_Notfound_if_given_unknown_id()
        {
            var controller = new SeminarController(_context);

            IActionResult result = await controller.Details(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_returns_view_when_not_passed_Id()
        {
            var controller = new SeminarController(_context);

            IActionResult result = await controller.Create();

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Create_stores_new_Seminar()
        {
            var controller = new SeminarController(_context);
            Seminar s = new Seminar
            {
                SeminarId = 7,
                Name = "Seminar7",
                Description = "Learn about effects",
            };

            SeminarViewModel SeminarView = new SeminarViewModel();
            SeminarView.Seminar = s;
            SeminarView.CheckList = new List<CheckedId>();

            await controller.Create(SeminarView);

            _context.Seminars.Should().HaveCount(4);
        }

        [Fact]
        public async Task Create_stores_Seminar_with_correct_properties()
        {
            var controller = new SeminarController(_context);
            Seminar s = new Seminar
            {
                SeminarId = 7,
                Name = "Seminar7",
                Description = "Learn about effects",
            };

            SeminarViewModel SeminarView = new SeminarViewModel();
            SeminarView.Seminar = s;
            SeminarView.CheckList = new List<CheckedId>();

            await controller.Create(SeminarView);

            _context.Seminars.FirstOrDefault(x => x.SeminarId == 7).Should().BeEquivalentTo<Seminar>(s);
        }

        [Fact]
        public async Task Create_stores_Seminar_with_correct_SeminarDays()
        {
            var controller = new SeminarController(_context);
            Seminar s = new Seminar
            {
                SeminarId = 7,
                Name = "Seminar7",
                Description = "Learn about effects",
            };
            var days = await _context.Days.ToListAsync();
            SeminarViewModel SeminarView = new SeminarViewModel(days);
            SeminarView.Seminar = s;
            SeminarView.CheckList[0].IsSelected = true;
            SeminarView.CheckList[4].IsSelected = true;

            await controller.Create(SeminarView);

            var result = _context.Seminars.FirstOrDefault(x => x.SeminarId == 7);

            result.SeminarDays.Should().HaveCount(2).And.Contain(x => x.Day.Name == "Day2")
                .And.Contain(x => x.Day.Name == "Day6");
        }

        [Fact]
        public async Task Edit_returns_Notfound_if_given_unknown_id()
        {
            var controller = new SeminarController(_context);

            IActionResult result = await controller.Edit(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Edit_returns_SeminarViewModel()
        {
            var controller = new SeminarController(_context);

            IActionResult result = await controller.Edit(1);

            result.As<ViewResult>().Model.Should().BeOfType<SeminarViewModel>();
        }

        [Fact]
        public async Task Edit_returns_SeminarViewModel_with_checklist()
        {
            var controller = new SeminarController(_context);

            IActionResult result = await controller.Edit(1);

            result.As<ViewResult>().Model.As<SeminarViewModel>().CheckList
                .Should().NotBeNullOrEmpty(because: "we seeded the Db with subjects");
        }

        [Fact]
        public async Task Edit_returns_SeminarViewModel_with_correct_Seminar()
        {
            var controller = new SeminarController(_context);

            IActionResult result = await controller.Edit(1);

            result.As<ViewResult>().Model.As<SeminarViewModel>().Seminar.Name.Should().Be("Seminar1");
        }


        [Fact]
        public async Task Edit_returns_SeminarViewModel_with_ALL_subjects_injected_into_checklist()
        {
            var controller = new SeminarController(_context);
            var subjects = await _context.Subjects.ToListAsync();

            IActionResult result = await controller.Edit(1);

            result.As<ViewResult>().Model.As<SeminarViewModel>().CheckList
                .Should().HaveCount(6).And.Contain(x => x.Name == "Introduction")
                .And.Contain(x => x.Name == "Overview").And.Contain(x => x.Name == "Support")
                .And.Contain(x => x.Name == "Neutral").And.Contain(x => x.Name == "Overdrive")
                .And.Contain(x => x.Name == "Edge");
        }

        [Fact]
        public async Task Edit_updates_Seminar_with_correct_properties()
        {
            var controller = new SeminarController(_context);
            Seminar s = _context.Seminars.FirstOrDefault(x => x.SeminarId == 1);
            s.Name = "Effects";
            s.Description = "Learn about effects";

            SeminarViewModel SeminarView = new SeminarViewModel();
            SeminarView.Seminar = s;
            SeminarView.CheckList = new List<CheckedId>();

            await controller.Edit(1, SeminarView);

            _context.Seminars.FirstOrDefault(x => x.SeminarId == 1).Should().BeEquivalentTo<Seminar>(s);
        }

        [Fact]
        public async Task Edit_updates_Seminar_with_correct_SeminarDays()
        {
            var controller = new SeminarController(_context);
            Seminar s = _context.Seminars.FirstOrDefault(x => x.SeminarId == 1);
            var days = await _context.Days.ToListAsync();

            SeminarViewModel SeminarView = new SeminarViewModel(s, days);
            SeminarView.CheckList[0].IsSelected = false;
            SeminarView.CheckList[1].IsSelected = false;
            SeminarView.CheckList[2].IsSelected = false;
            SeminarView.CheckList[3].IsSelected = true;

            await controller.Edit(1, SeminarView);

            _context.Seminars.FirstOrDefault(x => x.SeminarId == 1).SeminarDays.Should()
                .HaveCount(1).And.Contain(x => x.Day.Name == "Neutral");
        }

        [Fact]
        public async Task Edit_returns_NotFound_if_Id_changes()
        {
            var controller = new SeminarController(_context);
            Seminar s = _context.Seminars.FirstOrDefault(x => x.SeminarId == 1);

            SeminarViewModel SeminarView = new SeminarViewModel();
            SeminarView.Seminar = s;
            SeminarView.CheckList = new List<CheckedId>();

            IActionResult result = await controller.Edit(8, SeminarView);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Edit_returns_Redirect_if_modelstate_not_valid()
        {
            var controller = new SeminarController(_context);
            Seminar s = _context.Seminars.FirstOrDefault(x => x.SeminarId == 1);
            s.Name = "";

            SeminarViewModel SeminarView = new SeminarViewModel();
            SeminarView.Seminar = s;
            SeminarView.CheckList = new List<CheckedId>();

            IActionResult result = await controller.Edit(1, SeminarView);

            result.Should().BeOfType<RedirectToActionResult>();
        }

        [Fact]
        public async Task Edit_returns_Redirect_to_Index_if_concurrencyException_occurs()
        {
            var controller = new SeminarController(_context);
            Seminar s = _context.Seminars.FirstOrDefault(x => x.SeminarId == 1);
            var days = _context.Days.ToList();
            SeminarViewModel seminarView = new SeminarViewModel(s, days);

            _context.Remove(s);
            await _context.SaveChangesAsync();

            
            IActionResult result = await controller.Edit(1, seminarView);

            result.As<RedirectToActionResult>().ActionName.Should().Match("Index");
        }

        [Fact]
        public async Task Delete_returns_Correct_Seminar()
        {
            var controller = new SeminarController(_context);

            IActionResult result = await controller.Delete(1);

            result.As<ViewResult>().Model.As<Seminar>().Name.Should().Be("Seminar1");
        }

        [Fact]
        public async Task Delete_returns_Notfound_if_given_unknown_id()
        {
            var controller = new SeminarController(_context);

            IActionResult result = await controller.Delete(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_removes_Seminar_from_Db()
        {
            var controller = new SeminarController(_context);

            var Seminar = _context.Seminars.FirstOrDefault(x => x.SeminarId == 1);

            await controller.DeleteConfirmed(1);

            var result = _context.Seminars.FirstOrDefault(x => x.SeminarId == 1);

            result.Should().BeNull();
        }

    }
}

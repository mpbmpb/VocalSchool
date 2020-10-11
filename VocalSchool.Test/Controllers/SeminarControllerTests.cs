﻿using System;
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

namespace VocalSchool.Test.Controllers
{
    public class SeminarControllerTests : VocalSchoolTestBase

    {
        [Fact]
        public async Task Index_returns_ViewResult()
        {
            //Arrange
            var controller = new SeminarController(Context);

            //Act
            IActionResult result = await controller.Index();

            //Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Index_returns_Seminars()
        {
            var controller = new SeminarController(Context);

            IActionResult result = await controller.Index();

            result.As<ViewResult>().Model.Should().BeOfType<List<Seminar>>();
        }

        [Fact]
        public async Task Index_returns_All_Seminars()
        {
            var controller = new SeminarController(Context);

            IActionResult result = await controller.Index();

            result.As<ViewResult>().Model.As<List<Seminar>>().Should().HaveCount(3);
        }

        [Fact]
        public async Task Index_returns_Seminars_with_Days()
        {
            var controller = new SeminarController(Context);

            IActionResult result = await controller.Index();

            result.As<ViewResult>().Model.As<IEnumerable<Seminar>>().FirstOrDefault()
                .SeminarDays.FirstOrDefault().Day.DaySubjects.Should().NotBeNull();

        }

        [Fact]
        public async Task Details_returns_Seminar()
        {
            var controller = new SeminarController(Context);

            IActionResult result = await controller.Details(1);

            result.As<ViewResult>().Model.Should().BeAssignableTo<Seminar>();
        }

        [Fact]
        public async Task Details_returns_Correct_Seminar()
        {
            var controller = new SeminarController(Context);

            IActionResult result = await controller.Details(1);

            result.As<ViewResult>().Model.As<Seminar>().Name.Should().Be("Seminar1");
        }

        [Fact]
        public async Task Details_returns_Correct_Seminar_with_All_SeminarDays()
        {
            var controller = new SeminarController(Context);

            IActionResult result = await controller.Details(1);

            result.As<ViewResult>().Model.As<Seminar>().SeminarDays.Should()
                .HaveCount(2).And.Contain(x => x.Day.Name == "Day1")
                .And.Contain(x => x.Day.Name == "Day2");
        }

        [Fact]
        public async Task Details_returns_Notfound_if_given_unknown_id()
        {
            var controller = new SeminarController(Context);

            IActionResult result = await controller.Details(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_returns_view_when_not_passed_Id()
        {
            var controller = new SeminarController(Context);

            IActionResult result = await controller.Create();

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Create_stores_new_Seminar()
        {
            var controller = new SeminarController(Context);
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

            Context.Seminars.Should().HaveCount(4);
        }

        [Fact]
        public async Task Create_stores_Seminar_with_correct_properties()
        {
            var controller = new SeminarController(Context);
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

            Context.Seminars.FirstOrDefault(x => x.SeminarId == 7).Should().BeEquivalentTo<Seminar>(s);
        }

        [Fact]
        public async Task Create_stores_Seminar_with_correct_SeminarDays()
        {
            var controller = new SeminarController(Context);
            Seminar s = new Seminar
            {
                SeminarId = 7,
                Name = "Seminar7",
                Description = "Learn about effects",
            };
            var days = await Context.Days.ToListAsync();
            SeminarViewModel SeminarView = new SeminarViewModel(days);
            SeminarView.Seminar = s;
            SeminarView.CheckList[0].IsSelected = true;
            SeminarView.CheckList[4].IsSelected = true;

            await controller.Create(SeminarView);

            var result = Context.Seminars.FirstOrDefault(x => x.SeminarId == 7);

            result.SeminarDays.Should().HaveCount(2).And.Contain(x => x.Day.Name == "Day2")
                .And.Contain(x => x.Day.Name == "Day6");
        }

        [Fact]
        public async Task Edit_returns_Notfound_if_given_unknown_id()
        {
            var controller = new SeminarController(Context);

            IActionResult result = await controller.Edit(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Edit_returns_SeminarViewModel()
        {
            var controller = new SeminarController(Context);

            IActionResult result = await controller.Edit(1);

            result.As<ViewResult>().Model.Should().BeOfType<SeminarViewModel>();
        }

        [Fact]
        public async Task Edit_returns_SeminarViewModel_with_checklist()
        {
            var controller = new SeminarController(Context);

            IActionResult result = await controller.Edit(1);

            result.As<ViewResult>().Model.As<SeminarViewModel>().CheckList
                .Should().NotBeNullOrEmpty(because: "we seeded the Db with days");
        }

        [Fact]
        public async Task Edit_returns_SeminarViewModel_with_correct_Seminar()
        {
            var controller = new SeminarController(Context);

            IActionResult result = await controller.Edit(1);

            result.As<ViewResult>().Model.As<SeminarViewModel>().Seminar.Name.Should().Be("Seminar1");
        }


        [Fact]
        public async Task Edit_returns_SeminarViewModel_with_ALL_Days_injected_into_checklist()
        {
            var controller = new SeminarController(Context);

            IActionResult result = await controller.Edit(1);

            result.As<ViewResult>().Model.As<SeminarViewModel>().CheckList
                .Should().HaveCount(6).And.Contain(x => x.Name == "Day1")
                .And.Contain(x => x.Name == "Day2").And.Contain(x => x.Name == "Day3")
                .And.Contain(x => x.Name == "Day4").And.Contain(x => x.Name == "Day5")
                .And.Contain(x => x.Name == "Day6");
        }

        [Fact]
        public async Task Edit_updates_Seminar_with_correct_properties()
        {
            var controller = new SeminarController(Context);
            Seminar s = Context.Seminars.FirstOrDefault(x => x.SeminarId == 1);
            s.Name = "Effects";
            s.Description = "Learn about effects";

            SeminarViewModel SeminarView = new SeminarViewModel();
            SeminarView.Seminar = s;
            SeminarView.CheckList = new List<CheckedId>();

            await controller.Edit(1, SeminarView);

            Context.Seminars.FirstOrDefault(x => x.SeminarId == 1).Should().BeEquivalentTo<Seminar>(s);
        }

        [Fact]
        public async Task Edit_updates_Seminar_with_correct_SeminarDays()
        {
            var controller = new SeminarController(Context);
            Seminar s = Context.Seminars.FirstOrDefault(x => x.SeminarId == 1);
            var days = await Context.Days.ToListAsync();

            SeminarViewModel SeminarView = new SeminarViewModel(s, days);
            SeminarView.CheckList[5].IsSelected = false;
            SeminarView.CheckList[4].IsSelected = false;
            SeminarView.CheckList[3].IsSelected = true;

            await controller.Edit(1, SeminarView);

            Context.Seminars.FirstOrDefault(x => x.SeminarId == 1).SeminarDays.Should()
                .HaveCount(1).And.Contain(x => x.Day.Name == "Day3");
        }

        [Fact]
        public async Task Edit_returns_NotFound_if_Id_changes()
        {
            var controller = new SeminarController(Context);
            Seminar s = Context.Seminars.FirstOrDefault(x => x.SeminarId == 1);

            SeminarViewModel SeminarView = new SeminarViewModel();
            SeminarView.Seminar = s;
            SeminarView.CheckList = new List<CheckedId>();

            IActionResult result = await controller.Edit(8, SeminarView);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Edit_returns_View_if_modelstate_not_valid()
        {
            var controller = new SeminarController(Context);
            controller.ViewData.ModelState.AddModelError("key", "some error");
            SeminarViewModel seminarView = new SeminarViewModel();
            seminarView.Seminar = new Seminar() { SeminarId = 1 };
            seminarView.CheckList = new List<CheckedId>();

            IActionResult result = await controller.Edit(1, seminarView);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Edit_returns_Redirect_to_Index_if_concurrencyException_occurs()
        {
            var controller = new SeminarController(Context);
            Seminar s = await Context.Seminars.FirstOrDefaultAsync(x => x.SeminarId == 1);
            var days = await Context.Days.ToListAsync();
            SeminarViewModel seminarView = new SeminarViewModel(s, days);

            Context.Remove(s);
            await Context.SaveChangesAsync();

            
            IActionResult result = await controller.Edit(1, seminarView);

            result.As<RedirectToActionResult>().ActionName.Should().Match("Index");
        }

        [Fact]
        public async Task Delete_returns_Correct_Seminar()
        {
            var controller = new SeminarController(Context);

            IActionResult result = await controller.Delete(1);

            result.As<ViewResult>().Model.As<Seminar>().Name.Should().Be("Seminar1");
        }

        [Fact]
        public async Task Delete_returns_Notfound_if_given_unknown_id()
        {
            var controller = new SeminarController(Context);

            IActionResult result = await controller.Delete(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_removes_Seminar_from_Db()
        {
            var controller = new SeminarController(Context);

            var Seminar = Context.Seminars.FirstOrDefault(x => x.SeminarId == 1);

            await controller.DeleteConfirmed(1);

            var result = Context.Seminars.FirstOrDefault(x => x.SeminarId == 1);

            result.Should().BeNull();
        }

        [Fact]
        public void Validation_Leaving_Name_Null_or_short_causes_modelstate_not_valid()
        {
            var controller = new SubjectController(Context);
            Seminar s = new Seminar();
            Seminar s2 = new Seminar();
            s.SeminarId = 1;
            s2.SeminarId = 1;
            s2.Name = "123";

            var result = Validator.TryValidateObject(s, new ValidationContext(s), null, true);
            var result2 = Validator.TryValidateObject(s2, new ValidationContext(s2), null, true);

            result.Should().BeFalse();
            result2.Should().BeFalse();
        }

    }
}

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
    public class CourseDesignControllerTests : VocalSchoolTestBase

    {
        [Fact]
        public async Task Index_returns_ViewResult()
        {
            //Arrange
            var controller = new CourseDesignController(Context);

            //Act
            IActionResult result = await controller.Index();

            //Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Index_returns_CourseDesigns()
        {
            var controller = new CourseDesignController(Context);

            IActionResult result = await controller.Index();

            result.As<ViewResult>().Model.Should().BeOfType<List<CourseDesign>>();
        }

        [Fact]
        public async Task Index_returns_All_CourseDesigns()
        {
            var controller = new CourseDesignController(Context);

            IActionResult result = await controller.Index();

            result.As<ViewResult>().Model.As<List<CourseDesign>>().Should().HaveCount(3);
        }

        [Fact]
        public async Task Details_returns_CourseDesign()
        {
            var controller = new CourseDesignController(Context);

            IActionResult result = await controller.Details(1);

            result.As<ViewResult>().Model.Should().BeAssignableTo<CourseDesign>();
        }

        [Fact]
        public async Task Details_returns_Correct_CourseDesign()
        {
            var controller = new CourseDesignController(Context);

            IActionResult result = await controller.Details(1);

            result.As<ViewResult>().Model.As<CourseDesign>().Name.Should().Be("CourseDesign1");
        }

        [Fact]
        public async Task Details_returns_Correct_CourseDesign_with_All_CourseSeminars()
        {
            var controller = new CourseDesignController(Context);

            IActionResult result = await controller.Details(2);

            result.As<ViewResult>().Model.As<CourseDesign>().CourseSeminars.Should()
                .HaveCount(2).And.Contain(x => x.Seminar.Name == "Seminar1")
                .And.Contain(x => x.Seminar.Name == "Seminar2");
        }

        [Fact]
        public async Task Details_returns_Notfound_if_given_unknown_id()
        {
            var controller = new CourseDesignController(Context);

            IActionResult result = await controller.Details(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_returns_view_when_not_passed_Id()
        {
            var controller = new CourseDesignController(Context);

            IActionResult result = await controller.Create();

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Create_stores_new_CourseDesign()
        {
            var controller = new CourseDesignController(Context);
            CourseDesign cd = new CourseDesign
            {
                CourseDesignId = 7,
                Name = "CourseDesign7",
                Description = "Learn about effects",
            };

            CourseDesignViewModel CourseDesignView = new CourseDesignViewModel();
            CourseDesignView.CourseDesign = cd;
            CourseDesignView.CheckList = new List<CheckedId>();

            await controller.Create(CourseDesignView);

            Context.CourseDesigns.Should().HaveCount(4);
        }

        [Fact]
        public async Task Create_stores_CourseDesign_with_correct_properties()
        {
            var controller = new CourseDesignController(Context);
            CourseDesign cd = new CourseDesign
            {
                CourseDesignId = 7,
                Name = "CourseDesign7",
                Description = "Learn about effects",
            };

            CourseDesignViewModel CourseDesignView = new CourseDesignViewModel();
            CourseDesignView.CourseDesign = cd;
            CourseDesignView.CheckList = new List<CheckedId>();

            await controller.Create(CourseDesignView);

            Context.CourseDesigns.FirstOrDefault(x => x.CourseDesignId == 7).Should().BeEquivalentTo<CourseDesign>(cd);
        }

        [Fact]
        public async Task Create_stores_CourseDesign_with_correct_CourseSeminars()
        {
            var controller = new CourseDesignController(Context);
            CourseDesign cd = new CourseDesign
            {
                CourseDesignId = 7,
                Name = "CourseDesign7",
                Description = "Learn about effects",
            };
            var Seminars = await Context.Seminars.ToListAsync();
            CourseDesignViewModel CourseDesignView = new CourseDesignViewModel(Seminars);
            CourseDesignView.CourseDesign = cd;
            CourseDesignView.CheckList[0].IsSelected = true;
            CourseDesignView.CheckList[2].IsSelected = true;

            await controller.Create(CourseDesignView);

            var result = Context.CourseDesigns.FirstOrDefault(x => x.CourseDesignId == 7);

            result.CourseSeminars.Should().HaveCount(2).And.Contain(x => x.Seminar.Name == "Seminar1")
                .And.Contain(x => x.Seminar.Name == "Seminar3");
        }

        [Fact]
        public async Task Edit_returns_Notfound_if_given_unknown_id()
        {
            var controller = new CourseDesignController(Context);

            IActionResult result = await controller.Edit(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Edit_returns_CourseDesignViewModel()
        {
            var controller = new CourseDesignController(Context);

            IActionResult result = await controller.Edit(1);

            result.As<ViewResult>().Model.Should().BeOfType<CourseDesignViewModel>();
        }

        [Fact]
        public async Task Edit_returns_CourseDesignViewModel_with_checklist()
        {
            var controller = new CourseDesignController(Context);

            IActionResult result = await controller.Edit(1);

            result.As<ViewResult>().Model.As<CourseDesignViewModel>().CheckList
                .Should().NotBeNullOrEmpty(because: "we seeded the Db with seminars");
        }

        [Fact]
        public async Task Edit_returns_CourseDesignViewModel_with_correct_CourseDesign()
        {
            var controller = new CourseDesignController(Context);

            IActionResult result = await controller.Edit(1);

            result.As<ViewResult>().Model.As<CourseDesignViewModel>().CourseDesign.Name.Should().Be("CourseDesign1");
        }

        [Fact]
        public async Task Edit_returns_CourseDesignViewModel_with_ALL_Seminars_injected_into_checklist()
        {
            var controller = new CourseDesignController(Context);

            IActionResult result = await controller.Edit(1);

            result.As<ViewResult>().Model.As<CourseDesignViewModel>().CheckList
                .Should().HaveCount(3).And.Contain(x => x.Name == "Seminar1")
                .And.Contain(x => x.Name == "Seminar2").And.Contain(x => x.Name == "Seminar3");
        }

        [Fact]
        public async Task Edit_updates_CourseDesign_with_correct_properties()
        {
            var controller = new CourseDesignController(Context);
            CourseDesign cd = Context.CourseDesigns.FirstOrDefault(x => x.CourseDesignId == 1);
            cd.Name = "Effects";
            cd.Description = "Learn about effects";

            CourseDesignViewModel CourseDesignView = new CourseDesignViewModel();
            CourseDesignView.CourseDesign = cd;
            CourseDesignView.CheckList = new List<CheckedId>();

            await controller.Edit(1, CourseDesignView);

            Context.CourseDesigns.FirstOrDefault(x => x.CourseDesignId == 1).Should().BeEquivalentTo<CourseDesign>(cd);
        }

        [Fact]
        public async Task Edit_updates_CourseDesign_with_correct_CourseSeminars()
        {
            var controller = new CourseDesignController(Context);
            CourseDesign cd = Context.CourseDesigns.FirstOrDefault(x => x.CourseDesignId == 1);
            var Seminars = await Context.Seminars.ToListAsync();

            CourseDesignViewModel CourseDesignView = new CourseDesignViewModel(cd, Seminars);
            CourseDesignView.CheckList[2].IsSelected = false;
            CourseDesignView.CheckList[1].IsSelected = false;
            CourseDesignView.CheckList[0].IsSelected = true;

            await controller.Edit(1, CourseDesignView);

            Context.CourseDesigns.FirstOrDefault(x => x.CourseDesignId == 1).CourseSeminars.Should()
                .HaveCount(1).And.Contain(x => x.Seminar.Name == "Seminar3");
        }

        [Fact]
        public async Task Edit_returns_NotFound_if_Id_changes()
        {
            var controller = new CourseDesignController(Context);
            CourseDesign cd = Context.CourseDesigns.FirstOrDefault(x => x.CourseDesignId == 1);

            CourseDesignViewModel CourseDesignView = new CourseDesignViewModel();
            CourseDesignView.CourseDesign = cd;
            CourseDesignView.CheckList = new List<CheckedId>();

            IActionResult result = await controller.Edit(8, CourseDesignView);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Edit_returns_View_if_modelstate_not_valid()
        {
            var controller = new CourseDesignController(Context);
            controller.ViewData.ModelState.AddModelError("key", "some error");
            CourseDesignViewModel CourseDesignView = new CourseDesignViewModel();
            CourseDesignView.CourseDesign = new CourseDesign() { CourseDesignId = 1 };
            CourseDesignView.CheckList = new List<CheckedId>();

            IActionResult result = await controller.Edit(1, CourseDesignView);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Edit_returns_NotFound_if_concurrencyException_occurs()
        {
            var controller = new CourseDesignController(Context);
            CourseDesign cd = await Context.CourseDesigns.FirstOrDefaultAsync(x => x.CourseDesignId == 1);
            var Seminars = await Context.Seminars.ToListAsync();
            CourseDesignViewModel CourseDesignView = new CourseDesignViewModel(cd, Seminars);

            Context.Remove(cd);
            await Context.SaveChangesAsync();

            IActionResult result = await controller.Edit(1, CourseDesignView);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_returns_Correct_CourseDesign()
        {
            var controller = new CourseDesignController(Context);

            IActionResult result = await controller.Delete(1);

            result.As<ViewResult>().Model.As<CourseDesign>().Name.Should().Be("CourseDesign1");
        }

        [Fact]
        public async Task Delete_returns_Notfound_if_given_unknown_id()
        {
            var controller = new CourseDesignController(Context);

            IActionResult result = await controller.Delete(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_removes_CourseDesign_from_Db()
        {
            var controller = new CourseDesignController(Context);

            var CourseDesign = Context.CourseDesigns.FirstOrDefault(x => x.CourseDesignId == 1);

            await controller.DeleteConfirmed(1);

            var result = Context.CourseDesigns.FirstOrDefault(x => x.CourseDesignId == 1);

            result.Should().BeNull();
        }

        [Fact]
        public void Validation_Leaving_Name_Null_or_short_causes_modelstate_not_valid()
        {
            var controller = new SubjectController(Context);
            CourseDesign cd = new CourseDesign();
            CourseDesign cd2 = new CourseDesign();
            cd.CourseDesignId = 1;
            cd2.CourseDesignId = 1;
            cd2.Name = "123";

            var result = Validator.TryValidateObject(cd, new ValidationContext(cd), null, true);
            var result2 = Validator.TryValidateObject(cd2, new ValidationContext(cd2), null, true);

            result.Should().BeFalse();
            result2.Should().BeFalse();
        }

    }
}

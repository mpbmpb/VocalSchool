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
    public class DayControllerTests : VocalSchoolTestBase

    {
        [Fact]
        public async Task Index_returns_ViewResult()
        {
            var controller = new DayController(Context);
            
            IActionResult result = await controller.Index();
            
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Index_returns_Days()
        {
            var controller = new DayController(Context);

            IActionResult result = await controller.Index();

            result.As<ViewResult>().Model.Should().BeOfType<List<Day>>();
        }

        [Fact]
        public async Task Index_returns_All_Days()
        {
            var controller = new DayController(Context);

            IActionResult result = await controller.Index();

            result.As<ViewResult>().Model.As<List<Day>>().Should().HaveCount(6);
        }
        
        [Fact]
        public async Task Details_returns_Day()
        {
            var controller = new DayController(Context);

            IActionResult result = await controller.Details(1);

            result.As<ViewResult>().Model.Should().BeAssignableTo<Day>();
        }

        [Fact]
        public async Task Details_returns_Correct_Day()
        {
            var controller = new DayController(Context);

            IActionResult result = await controller.Details(1);

            result.As<ViewResult>().Model.As<Day>().Name.Should().Be("Day1");
        }
        
        [Fact]
        public async Task Details_returns_Correct_Day_with_All_DaySubjects()
        {
            var controller = new DayController(Context);

            IActionResult result = await controller.Details(1);

            result.As<ViewResult>().Model.As<Day>().DaySubjects.Should()
                .HaveCount(3).And.Contain(x => x.Subject.Name == "Introduction")
                .And.Contain(x => x.Subject.Name == "Overview")
                .And.Contain(x => x.Subject.Name == "Support");
        }

        [Fact]
        public async Task Details_returns_Notfound_if_given_unknown_id()
        {
            var controller = new DayController(Context);

            IActionResult result = await controller.Details(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_returns_view_when_not_passed_Id()
        {
            var controller = new DayController(Context);

            IActionResult result = await controller.Create();

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Create_stores_new_Day()
        {
            var controller = new DayController(Context);

            Day d = new Day
            {
                DayId = 7,
                Name = "Day7",
                Description = "Learn about effects",
            };

            DayViewModel dayView = new DayViewModel { Day = d, CheckList = new List<CheckedId>()};

            await controller.Create(dayView);

            Context.Days.Should().HaveCount(7);
        }

        [Fact]
        public async Task Create_stores_Day_with_correct_properties()
        { 
            var controller = new DayController(Context);

            Day d = new Day
            {
                DayId = 7,
                Name = "Day7",
                Description = "Learn about effects",
            };

            DayViewModel dayView = new DayViewModel { Day = d, CheckList = new List<CheckedId>()};

            await controller.Create(dayView);

            Context.Days.FirstOrDefault(x => x.DayId == 7).Should().BeEquivalentTo(d);
        }
        
        [Fact]
        public async Task Create_stores_Day_with_correct_daySubjects()
        {
            var controller = new DayController(Context);

            Day d = new Day { DayId = 7, Name = "Day7", Description = "Learn about effects",};
            var subjects = await Context.Subjects.ToListAsync();
            DayViewModel dayView = new DayViewModel(subjects) {Day = d};
            dayView.CheckList[0].IsSelected = true;
            dayView.CheckList[4].IsSelected = true;

            await controller.Create(dayView);

            Context.Days.FirstOrDefault(x => x.DayId == 7)?.DaySubjects.Should()
                .HaveCount(2).And.Contain(x => x.Subject.Name == "Introduction")
                .And.Contain(x => x.Subject.Name == "Overdrive");
        }

        [Fact]
        public async Task Edit_returns_Notfound_if_given_unknown_id()
        {
            var controller = new DayController(Context);

            IActionResult result = await controller.Edit(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Edit_returns_DayViewModel()
        {
            var controller = new DayController(Context);

            IActionResult result = await controller.Edit(1);

            result.As<ViewResult>().Model.Should().BeOfType<DayViewModel>();
        }
        
        [Fact]
        public async Task Edit_returns_DayViewModel_with_checklist()
        {
            var controller = new DayController(Context);

            IActionResult result = await controller.Edit(1);

            result.As<ViewResult>().Model.As<DayViewModel>().CheckList
                .Should().NotBeNullOrEmpty(because: "we seeded the Db with subjects");
        }

        [Fact]
        public async Task Edit_returns_DayViewModel_with_correct_Day()
        {
            var controller = new DayController(Context);

            IActionResult result = await controller.Edit(1);

            result.As<ViewResult>().Model.As<DayViewModel>().Day.Name.Should().Be("Day1");
        }


        [Fact]
        public async Task Edit_returns_DayViewModel_with_ALL_subjects_injected_into_checklist()
        {
            var controller = new DayController(Context);

            IActionResult result = await controller.Edit(1);

            result.As<ViewResult>().Model.As<DayViewModel>().CheckList
                .Should().HaveCount(6).And.Contain(x => x.Name == "Introduction")
                .And.Contain(x => x.Name == "Overview").And.Contain(x => x.Name == "Support")
                .And.Contain(x => x.Name == "Neutral").And.Contain(x => x.Name == "Overdrive")
                .And.Contain(x => x.Name == "Edge");
        }

        [Fact]
        public async Task Edit_updates_Day_with_correct_properties()
        {
            var controller = new DayController(Context);

            Day d = Context.Days.FirstOrDefault(x => x.DayId == 1);
            d.Name = "Effects";
            d.Description = "Learn about effects";

            DayViewModel dayView = new DayViewModel { Day = d, CheckList = new List<CheckedId>()};

            await controller.Edit(1, dayView);

            Resultcontext.Days.FirstOrDefault(x => x.DayId == 1)?.Name.Should().Be("Effects");
            Resultcontext.Days.FirstOrDefault(x => x.DayId == 1)?.Description.Should().Be("Learn about effects");
        }
        
        [Fact]
        public async Task Edit_updates_Day_with_correct_DaySubjects()
        {
            var controller = new DayController(Context);

            Day d = Context.Days.FirstOrDefault(x => x.DayId == 1);
            var subjects = await Context.Subjects.ToListAsync();
            
            DayViewModel dayView = new DayViewModel(d, subjects);
            dayView.CheckList[0].IsSelected = false;
            dayView.CheckList[1].IsSelected = false;
            dayView.CheckList[2].IsSelected = false;
            dayView.CheckList[3].IsSelected = true;

            await controller.Edit(1, dayView);

            Resultcontext.Days.FirstOrDefault(x => x.DayId == 1)?.DaySubjects.Should()
                .HaveCount(1).And.Contain(x => x.Subject.Name == "Neutral");
        }

        [Fact]
        public async Task Edit_returns_NotFound_if_Id_changes()
        {
            var controller = new DayController(Context);

            Day d = Context.Days.FirstOrDefault(x => x.DayId == 1);

            DayViewModel dayView = new DayViewModel { Day = d, CheckList = new List<CheckedId>()};

            IActionResult result = await controller.Edit(8, dayView);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Edit_returns_View_if_Model_state_not_valid()
        {
            var controller = new DayController(Context);
            
            controller.ViewData.ModelState.AddModelError("key", "Some Exception");
            DayViewModel dayView = new DayViewModel { Day = new Day {DayId = 1}, CheckList = new List<CheckedId>()};

            IActionResult result = await controller.Edit(1, dayView);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Edit_returns_Redirect_to_Index_if_concurrencyException_occurs()
        {
            var controller = new DayController(Context);

            Day d = Context.Days.FirstOrDefault(x => x.DayId == 1);
            Context.Remove(d);
            await Context.SaveChangesAsync();

            DayViewModel dayView = new DayViewModel();
            dayView.Day = d;
            dayView.CheckList = new List<CheckedId>();

            IActionResult result = await controller.Edit(1, dayView);

            result.As<RedirectToActionResult>().ActionName.Should().Match("Index");
        }

        [Fact]
        public async Task Delete_returns_Correct_Day()
        {
            var controller = new DayController(Context);

            IActionResult result = await controller.Delete(1);

            result.As<ViewResult>().Model.As<Day>().Name.Should().Be("Day1");
        }

        [Fact]
        public async Task Delete_returns_Notfound_if_given_unknown_id()
        {
            var controller = new DayController(Context);

            IActionResult result = await controller.Delete(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_removes_Day_from_Db()
        {
            var controller = new DayController(Context);
            
            await controller.DeleteConfirmed(1);

            var result = Context.Days.FirstOrDefault(x => x.DayId == 1);

            result.Should().BeNull();
        }

        [Fact]
        public void Validation_Leaving_Name_Null_or_short_causes_modelstate_not_valid()
        {
            Day d = new Day();
            Day d2 = new Day();
            d.DayId = 1;
            d2.DayId = 1;
            d2.Name = "123";

            var result = Validator.TryValidateObject(d, new ValidationContext(d), null, true);
            var result2 = Validator.TryValidateObject(d2, new ValidationContext(d2), null, true);

            result.Should().BeFalse();
            result2.Should().BeFalse();
        }
        
    }
}

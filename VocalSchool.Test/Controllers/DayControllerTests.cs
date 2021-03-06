﻿using Xunit;
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
    public class DayControllerTests : UnitTestBase

    {
        public DayControllerTests(ConfigFixture fixture) : base(fixture)
        {
        }
        
        private DayController Controller => new DayController(Context);
        
        private static Day Day7 => new Day { DayId = 7, Name = "Day7",
            Description = "Learn about effects"};

        [Fact]
        public async Task Index_returns_ViewResult()
        {
            var result = await Controller.Index();
            
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Index_returns_Days()
        {
            var result = await Controller.Index();

            result.As<ViewResult>().Model.Should().BeOfType<List<Day>>();
        }

        [Fact]
        public async Task Index_returns_All_Days_without_uid()
        {
            var result = await Controller.Index();

            result.As<ViewResult>().Model.As<List<Day>>().Should().HaveCount(6);
        }
        
        [Fact]
        public async Task Index_only_returns_days_without_uid()
        {
            Context.Add(new Day{Name = "[prepend test] day7"});
            await Context.SaveChangesAsync();
            var result = await Controller.Index();

            result.As<ViewResult>().Model.As<List<Day>>().Should().HaveCount(6);
        }
        
        [Fact]
        public async Task Details_returns_Day()
        {
            var result = await Controller.Details(1);

            result.As<ViewResult>().Model.Should().BeAssignableTo<Day>();
        }

        [Fact]
        public async Task Details_returns_Correct_Day()
        {
            var result = await Controller.Details(1);

            result.As<ViewResult>().Model.As<Day>().Name.Should().Be("Day1");
        }
        
        [Fact]
        public async Task Details_returns_Correct_Day_with_All_DaySubjects()
        {
            var result = await Controller.Details(1);

            result.As<ViewResult>().Model.As<Day>().DaySubjects.Should()
                .HaveCount(3).And.Contain(x => x.Subject.Name == "Introduction")
                .And.Contain(x => x.Subject.Name == "Overview")
                .And.Contain(x => x.Subject.Name == "Support");
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
        public async Task Create_shows_only_Subjects_without_uid()
        {
            var s = new Subject {Name = "[uid] test", SubjectId = 7};
            Context.Add(s);
            await Context.SaveChangesAsync();
            var result = await Controller.Create();

            result.As<ViewResult>().Model.As<DayViewModel>().CheckList
                .Count(x => x.Name.Substring(0, 1) == "[")
                .Should().Be(0);
        }


        [Fact]
        public async Task Create_stores_new_Day()
        {
            var model = new DayViewModel(Day7, "http://completevocaltraining.nl") { CheckList = new List<CheckedId>()};

            await Controller.Create(model);

            Context.Days.Should().HaveCount(7);
        }

        [Fact]
        public async Task Create_stores_Day_with_correct_properties()
        {
            var model = new DayViewModel(Day7, "http://completevocaltraining.nl") { CheckList = new List<CheckedId>()};

            await Controller.Create(model);

            Context.Days.FirstOrDefault(x => x.DayId == 7).Should().BeEquivalentTo(Day7);
        }
        
        [Fact]
        public async Task Create_stores_Day_with_correct_daySubjects()
        {
            var subjects = await Context.Subjects.ToListAsync();
            var model = new DayViewModel(subjects, "http://www.completevocaltraining.nl") {Day = Day7};
            model.CheckList[0].IsSelected = true;
            model.CheckList[4].IsSelected = true;

            await Controller.Create(model);

            Context.Days.FirstOrDefault(x => x.DayId == 7)?.DaySubjects.Should()
                .HaveCount(2).And.Contain(x => x.Subject.Name == "Introduction")
                .And.Contain(x => x.Subject.Name == "Overdrive");
        }

        [Fact]
        public async Task Edit_returns_Notfound_if_given_unknown_id()
        {
            var result = await Controller.Edit(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Edit_returns_DayViewModel()
        {
            var result = await Controller.Edit(1);

            result.As<ViewResult>().Model.Should().BeOfType<DayViewModel>();
        }
        
        [Fact]
        public async Task Edit_returns_DayViewModel_with_checklist()
        {
            var result = await Controller.Edit(1);

            result.As<ViewResult>().Model.As<DayViewModel>().CheckList
                .Should().NotBeNullOrEmpty(because: "we seeded the Db with subjects");
        }

        [Fact]
        public async Task Edit_returns_DayViewModel_with_correct_Day()
        {
            var result = await Controller.Edit(1);

            result.As<ViewResult>().Model.As<DayViewModel>().Day.Name.Should().Be("Day1");
        }

        [Fact]
        public async Task Edit_removes_uid_from_name_puts_it_in_Uid_prop()
        {
            var d = new Day(){ Name = "[test] name", DayId = 7};
            Context.Add(d);
            await Context.SaveChangesAsync();

            var result = await Controller.Edit(7);

            result.As<ViewResult>().Model.As<DayViewModel>().Uid.Should().Match("[test]");
            result.As<ViewResult>().Model.As<DayViewModel>().Day.Name.Should().Match("name");
        }

        [Fact]
        public async Task Edit_puts_uid_and_name_back_together_when_saving_to_context()
        {
            const string name = "[test] name";
            var d = new Day{ Name = name, DayId = 7};
            Context.Add(d);
            await Context.SaveChangesAsync();
            var actionResult = await Controller.Edit(7);
            var model = actionResult.As<ViewResult>().Model.As<DayViewModel>();

            await Controller.Edit(7, model);
            var result = Context.Days.FirstOrDefault(x => x.DayId == 7);

            result?.Name.Should().Be(name);
        }

        [Fact]
        public async Task Edit_returns_DayViewModel_with_ALL_subjects_injected_into_checklist()
        {
            var result = await Controller.Edit(1);

            result.As<ViewResult>().Model.As<DayViewModel>().CheckList
                .Should().HaveCount(6).And.Contain(x => x.Name == "Introduction")
                .And.Contain(x => x.Name == "Overview")
                .And.Contain(x => x.Name == "Support")
                .And.Contain(x => x.Name == "Neutral")
                .And.Contain(x => x.Name == "Overdrive")
                .And.Contain(x => x.Name == "Edge");
        }
        
        [Fact]
        public async Task Edit_shows_only_Subjects_without_uid_if_Day_has_no_uid()
        {
            var s = new Subject {Name = "[uid] test", SubjectId = 7};
            Context.Add(s);
            await Context.SaveChangesAsync();
            
            var result = await Controller.Edit(1);

            result.As<ViewResult>().Model.As<DayViewModel>().CheckList
                .Count(x => x.Name.Substring(0, 1) == "[")
                .Should().Be(0);
        }
        
        [Fact]
        public async Task Edit_shows_only_Subjects_with_matching_uid()
        {
            await MakeNewCourse(1);
            
            var result = await Controller.Edit(7);

            result.As<ViewResult>().Model.As<DayViewModel>().CheckList.Count
                .Should().Be(5, because:"CourseDesign 1 contains 5 Subjects now copied and prepended with [test]");
            result.As<ViewResult>().Model.As<DayViewModel>().CheckList
                .Count(x => x.Name.Substring(0, 1) == "[")
                .Should().Be(5, because:"CourseDesign 1 contains 5 Subjects now copied and prepended with [test]");
        }


        [Fact]
        public async Task Edit_updates_Day_with_correct_properties()
        {
            var model = await GetModel<DayViewModel>(Controller.Edit, 1);
            model.Day.Name = "Effects";
            model.Day.Description = "Learn about effects";
            model.LastPage = "http://www.completevocaltraining.nl";
            await Controller.Edit(1, model);

            Resultcontext.Days.FirstOrDefault(x => x.DayId == 1)?.Name.Should().Be("Effects");
            Resultcontext.Days.FirstOrDefault(x => x.DayId == 1)?.Description.Should().Be("Learn about effects");
        }

        [Fact]
        public async Task Edit_updates_Day_with_correct_DaySubjects()
        {
            var d = Context.Days.FirstOrDefault(x => x.DayId == 1);
            var subjects = await Context.Subjects.ToListAsync();
            
            var model = new DayViewModel(d, subjects);
            model.CheckList[0].IsSelected = false;
            model.CheckList[1].IsSelected = false;
            model.CheckList[2].IsSelected = false;
            model.CheckList[3].IsSelected = true;
            model.LastPage = "http://www.completevocaltraining.nl";

            await Controller.Edit(1, model);

            Resultcontext.Days.FirstOrDefault(x => x.DayId == 1)?.DaySubjects.Should()
                .HaveCount(1).And.Contain(x => x.Subject.Name == "Neutral");
        }

        [Fact]
        public async Task Edit_returns_NotFound_if_Id_changes()
        {
            var d = Context.Days.FirstOrDefault(x => x.DayId == 1);
            var model = new DayViewModel { Day = d, CheckList = new List<CheckedId>()};

            var result = await Controller.Edit(8, model);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Edit_returns_View_if_Model_state_not_valid()
        {
            var controller = Controller;
            controller.ViewData.ModelState.AddModelError("key", "Some Exception");
            var model = new DayViewModel { Day = new Day {DayId = 1}, CheckList = new List<CheckedId>()};

            var result = await controller.Edit(1, model);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Edit_returns_Redirect_to_Index_if_concurrencyException_occurs()
        {
            var d = Context.Days.FirstOrDefault(x => x.DayId == 1);
            Context.Remove(d);
            await Context.SaveChangesAsync();

            var model = new DayViewModel();
            model.Day = d;
            model.CheckList = new List<CheckedId>();
            model.LastPage = "http://www.completevocaltraining.nl";
            var result = await Controller.Edit(1, model);

            result.As<RedirectToActionResult>().ActionName.Should().Match("Index");
        }

        [Fact]
        public async Task Delete_returns_Correct_Day()
        {
            var result = await Controller.Delete(1);

            result.As<ViewResult>().Model.As<DayViewModel>().Day.Name.Should().Be("Day1");
        }

        [Fact]
        public async Task Delete_returns_Notfound_if_given_unknown_id()
        {
            var result = await Controller.Delete(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_removes_Day_from_Db()
        {
            await Controller.DeleteConfirmed(new DayViewModel(new Day(){DayId = 1}, "http://completevocaltraining.nl" ));

            var result = Context.Days.FirstOrDefault(x => x.DayId == 1);

            result.Should().BeNull();
        }

        [Fact]
        public void Validation_Leaving_Name_Null_or_short_causes_modelstate_not_valid()
        {
            var d = new Day();
            var d2 = new Day();
            d.DayId = 1;
            d2.DayId = 1;
            d2.Name = "123";

            var result = Validator.TryValidateObject(d, new ValidationContext(d), null, true);
            var result2 = Validator.TryValidateObject(d2, new ValidationContext(d2), null, true);

            result.Should().BeFalse();
            result2.Should().BeFalse();
        }
        
        [Fact]
        public void Validation_entering_disallowed_character_in_Name_field_causes_modelstate_not_valid()
        {            
            var x = new Day();
            var x2 = new Day();
            var x3 = new Day();
            x.Name = "[123";
            x2.Name = "123]";
            x3.Name = "12_3";

            var result = Validator.TryValidateObject(x, new ValidationContext(x), null, true);
            var result2 = Validator.TryValidateObject(x2, new ValidationContext(x2), null, true);
            var result3 = Validator.TryValidateObject(x3, new ValidationContext(x3), null, true);

            result.Should().BeFalse();
            result2.Should().BeFalse();
            result3.Should().BeFalse();
        }
    }
}

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
    public class SeminarControllerTests : UnitTestBase

    {
        public SeminarControllerTests(ConfigFixture fixture) : base(fixture)
        {
        }

        private SeminarController Controller => new SeminarController(Context);

        private static Seminar Seminar7 => new Seminar
        {
            SeminarId = 7,
            Name = "Seminar7",
            Description = "Learn about effects",
        };
        
        [Fact]
        public async Task Index_returns_ViewResult()
        {
            var result = await Controller.Index();

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Index_returns_Seminars()
        {
            var result = await Controller.Index();

            result.As<ViewResult>().Model.Should().BeOfType<List<Seminar>>();
        }

        [Fact]
        public async Task Index_returns_All_Seminars_without_uid()
        {
            var result = await Controller.Index();

            result.As<ViewResult>().Model.As<List<Seminar>>().Should().HaveCount(3);
        }
        
        [Fact]
        public async Task Index_only_returns_Seminars_without_uid()
        {
            Context.Add(new Seminar{Name = "[prepend test] seminar7"});
            await Context.SaveChangesAsync();
            var result = await Controller.Index();

            result.As<ViewResult>().Model.As<List<Seminar>>().Should().HaveCount(3);
        }

        [Fact]
        public async Task Index_returns_Seminars_with_Days()
        {
            var result = await Controller.Index();

            result?.As<ViewResult>().Model.As<IEnumerable<Seminar>>().FirstOrDefault()?
                .SeminarDays.FirstOrDefault()?.Day.DaySubjects.Should().NotBeNull();

        }

        [Fact]
        public async Task Details_returns_Seminar()
        {
            var result = await Controller.Details(1);

            result.As<ViewResult>().Model.Should().BeAssignableTo<Seminar>();
        }

        [Fact]
        public async Task Details_returns_Correct_Seminar()
        {
            var result = await Controller.Details(1);

            result.As<ViewResult>().Model.As<Seminar>().Name.Should().Be("Seminar1");
        }

        [Fact]
        public async Task Details_returns_Correct_Seminar_with_All_SeminarDays()
        {
            var result = await Controller.Details(1);

            result.As<ViewResult>().Model.As<Seminar>().SeminarDays.Should()
                .HaveCount(2).And.Contain(x => x.Day.Name == "Day1")
                .And.Contain(x => x.Day.Name == "Day2");
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
        public async Task Create_shows_only_Days_without_uid()
        {
            var d = new Day {Name = "[uid] test", DayId = 7};
            Context.Add(d);
            await Context.SaveChangesAsync();
            var result = await Controller.Create();

            result.As<ViewResult>().Model.As<SeminarViewModel>().Days
                .Count(x => x.Name.Substring(0, 1) == "[")
                .Should().Be(0);
        }

        [Fact]
        public async Task Create_stores_new_Seminar()
        {
            var seminarView = new SeminarViewModel(Seminar7,  new List<Day>(), "", "http://completevocaltraining.nl");

            await Controller.Create(seminarView);

            Context.Seminars.Should().HaveCount(4);
        }

        [Fact]
        public async Task Create_stores_Seminar_with_correct_properties()
        {
            var seminarView = new SeminarViewModel(Seminar7,  new List<Day>(), "", "http://completevocaltraining.nl");

            await Controller.Create(seminarView);

            Context.Seminars.FirstOrDefault(x => x.SeminarId == 7).Should().BeEquivalentTo(Seminar7);
        }

        [Fact]
        public async Task Create_stores_Seminar_with_correct_SeminarDays()
        {
            var days = await Context.Days.ToListAsync();
            var seminarView = new SeminarViewModel(days, "http://completevocaltraining.nl") {Seminar = Seminar7};
            seminarView.CheckList[0].IsSelected = true;
            seminarView.CheckList[4].IsSelected = true;

            await Controller.Create(seminarView);

            var result = Context.Seminars.FirstOrDefault(x => x.SeminarId == 7);

            result?.SeminarDays.Should().HaveCount(2)
                .And.Contain(x => x.Day.Name == "Day2")
                .And.Contain(x => x.Day.Name == "Day6");
        }

        [Fact]
        public async Task Edit_returns_Notfound_if_given_unknown_id()
        {
            var result = await Controller.Edit(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Edit_returns_SeminarViewModel()
        {
            var result = await Controller.Edit(1);

            result.As<ViewResult>().Model.Should().BeOfType<SeminarViewModel>();
        }

        [Fact]
        public async Task Edit_returns_SeminarViewModel_with_checklist()
        {
            var result = await Controller.Edit(1);

            result.As<ViewResult>().Model.As<SeminarViewModel>().CheckList
                .Should().NotBeNullOrEmpty(because: "we seeded the Db with days");
        }

        [Fact]
        public async Task Edit_returns_SeminarViewModel_with_correct_Seminar()
        {
            var result = await Controller.Edit(1);

            result.As<ViewResult>().Model.As<SeminarViewModel>().Seminar.Name.Should().Be("Seminar1");
        }
        
        [Fact]
        public async Task Edit_removes_uid_from_name_puts_it_in_Uid_prop()
        {
            var s = new Seminar(){ Name = "[test] name", SeminarId = 7};
            Context.Add(s);
            await Context.SaveChangesAsync();

            var result = await Controller.Edit(7);

            result.As<ViewResult>().Model.As<SeminarViewModel>().Uid.Should().Match("[test]");
            result.As<ViewResult>().Model.As<SeminarViewModel>().Seminar.Name.Should().Match("name");
        }
        
        [Fact]
        public async Task Edit_puts_uid_and_name_back_together_when_saving_to_context()
        {
            const string name = "[test] name";
            var s = new Seminar{ Name = name, SeminarId = 7};
            Context.Add(s);
            await Context.SaveChangesAsync();
            var actionResult = await Controller.Edit(7);
            var model = actionResult.As<ViewResult>().Model.As<SeminarViewModel>();

            await Controller.Edit(7, model);
            var result = Context.Seminars.FirstOrDefault(x => x.SeminarId == 7);

            result?.Name.Should().Be(name);
        }


        [Fact]
        public async Task Edit_returns_SeminarViewModel_with_ALL_Days_injected_into_checklist()
        {
            var result = await Controller.Edit(1);

            result.As<ViewResult>().Model.As<SeminarViewModel>().CheckList
                .Should().HaveCount(6).And.Contain(x => x.Name == "Day1")
                .And.Contain(x => x.Name == "Day2")
                .And.Contain(x => x.Name == "Day3")
                .And.Contain(x => x.Name == "Day4")
                .And.Contain(x => x.Name == "Day5")
                .And.Contain(x => x.Name == "Day6");
        }
        
        [Fact]
        public async Task Edit_shows_only_Days_without_uid_if_Seminar_has_no_uid()
        {
            var d = new Day {Name = "[uid] test", DayId = 7};
            Context.Add(d);
            await Context.SaveChangesAsync();
            
            var result = await Controller.Edit(1);

            result.As<ViewResult>().Model.As<SeminarViewModel>().CheckList
                .Count(x => x.Name.Substring(0, 1) == "[")
                .Should().Be(0);
        }
        
        [Fact]
        public async Task Edit_shows_only_Days_with_matching_uid()
        {
            await MakeNewCourse(1);
            
            var result = await Controller.Edit(4);
            
            result.As<ViewResult>().Model.As<SeminarViewModel>().CheckList.Count
                .Should().Be(2, because:"CourseDesign 1 contains 2 Days now copied and prepended with [test]");
            result.As<ViewResult>().Model.As<SeminarViewModel>().CheckList
                .Count(x => x.Name.Substring(0, 1) == "[")
                .Should().Be(2, because:"CourseDesign 1 contains 2 Days now copied and prepended with [test]");
        }



        [Fact]
        public async Task Edit_updates_Seminar_with_correct_properties()
        {            
            var s = Context.Seminars.FirstOrDefault(x => x.SeminarId == 1);
            s.Name = "Effects";
            s.Description = "Learn about effects";

            var seminarView = new SeminarViewModel (s, "http://completevocaltraining.nl"){ CheckList = new List<CheckedId>()};

            await Controller.Edit(1, seminarView);

            Context.Seminars.FirstOrDefault(x => x.SeminarId == 1).Should().BeEquivalentTo(s);
        }

        [Fact]
        public async Task Edit_updates_Seminar_with_correct_SeminarDays()
        {            
            var s = Context.Seminars.FirstOrDefault(x => x.SeminarId == 1);
            var days = await Context.Days.ToListAsync();

            var seminarView = new SeminarViewModel(s, days, "", "http://completevocaltraining.nl");
            seminarView.CheckList[5].IsSelected = false;
            seminarView.CheckList[4].IsSelected = false;
            seminarView.CheckList[3].IsSelected = true;

            await Controller.Edit(1, seminarView);

            Context.Seminars.FirstOrDefault(x => x.SeminarId == 1)?.SeminarDays.Should()
                .HaveCount(1).And.Contain(x => x.Day.Name == "Day3");
        }

        [Fact]
        public async Task Edit_returns_NotFound_if_Id_changes()
        {            var s = Context.Seminars.FirstOrDefault(x => x.SeminarId == 1);

            var seminarView = new SeminarViewModel {Seminar = s, CheckList = new List<CheckedId>()};

            var result = await Controller.Edit(8, seminarView);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Edit_returns_View_if_modelstate_not_valid()
        {
            var controller = Controller;
            controller.ViewData.ModelState.AddModelError("key", "some error");
            var seminarView = new SeminarViewModel
            {
                Seminar = new Seminar() {SeminarId = 1}, CheckList = new List<CheckedId>()
            };

            var result = await controller.Edit(1, seminarView);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Edit_returns_Redirect_to_Index_if_concurrencyException_occurs()
        {            
            var s = await Context.Seminars.FirstOrDefaultAsync(x => x.SeminarId == 1);
            var days = await Context.Days.ToListAsync();
            var seminarView = new SeminarViewModel(s, days);

            Context.Remove(s);
            await Context.SaveChangesAsync();

            
            var result = await Controller.Edit(1, seminarView);

            result.As<RedirectToActionResult>().ActionName.Should().Match("Index");
        }

        [Fact]
        public async Task Delete_returns_Correct_Seminar()
        {
            var result = await Controller.Delete(1);

            result.As<ViewResult>().Model.As<SeminarViewModel>().Seminar.Name.Should().Be("Seminar1");
        }

        [Fact]
        public async Task Delete_returns_Notfound_if_given_unknown_id()
        {
            var result = await Controller.Delete(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_removes_Seminar_from_Db()
        {
            await Controller.DeleteConfirmed(new SeminarViewModel(new Seminar(){SeminarId = 1}, "http://completevocaltraining.nl"));

            var result = Context.Seminars.FirstOrDefault(x => x.SeminarId == 1);

            result.Should().BeNull();
        }

        [Fact]
        public void Validation_Leaving_Name_Null_or_short_causes_modelstate_not_valid()
        {            
            var s = new Seminar();
            var s2 = new Seminar();
            s.SeminarId = 1;
            s2.SeminarId = 1;
            s2.Name = "123";

            var result = Validator.TryValidateObject(s, new ValidationContext(s), null, true);
            var result2 = Validator.TryValidateObject(s2, new ValidationContext(s2), null, true);

            result.Should().BeFalse();
            result2.Should().BeFalse();
        }

        [Fact]
        public void Validation_entering_disallowed_character_in_Name_field_causes_modelstate_not_valid()
        {            
            var x = new Seminar();
            var x2 = new Seminar();
            var x3 = new Seminar();
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

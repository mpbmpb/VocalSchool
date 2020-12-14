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
    public class CourseDesignControllerTests : UnitTestBase

    {
        public CourseDesignControllerTests(ConfigFixture fixture) : base(fixture)
        {
        }

        private CourseDesignController Controller => new CourseDesignController(Context);

        private CourseDesign CourseDesign7 => new CourseDesign
        {
            CourseDesignId = 7,
            Name = "CourseDesign7",
            Description = "Learn about effects",
            CourseSeminars = new List<CourseSeminar>(),
        };
        
        [Fact]
        public async Task Index_returns_ViewResult()
        {
            var result = await Controller.Index();
            
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Index_returns_CourseDesigns()
        {
            var result = await Controller.Index();

            result.As<ViewResult>().Model.Should().BeOfType<List<CourseDesign>>();
        }

        [Fact]
        public async Task Index_returns_All_CourseDesigns_without_uid()
        {
            var result = await Controller.Index();

            result.As<ViewResult>().Model.As<List<CourseDesign>>().Should().HaveCount(3);
        }
        
        [Fact]
        public async Task Index_only_returns_CourseDesigns_without_uid()
        {
            Context.Add(new CourseDesign{Name = "[prepend test] CourseDesign7"});
            await Context.SaveChangesAsync();
            var result = await Controller.Index();

            result.As<ViewResult>().Model.As<List<CourseDesign>>().Should().HaveCount(3);
        }

        [Fact]
        public async Task Details_returns_CourseDesign()
        {
            var result = await Controller.Details(1);

            result.As<ViewResult>().Model.Should().BeAssignableTo<CourseDesign>();
        }

        [Fact]
        public async Task Details_returns_Correct_CourseDesign()
        {
            var result = await Controller.Details(1);

            result.As<ViewResult>().Model.As<CourseDesign>().Name.Should().Be("CourseDesign1");
        }

        [Fact]
        public async Task Details_returns_Correct_CourseDesign_with_All_CourseSeminars()
        {
            var result = await Controller.Details(2);

            result.As<ViewResult>().Model.As<CourseDesign>().CourseSeminars.Should()
                .HaveCount(2).And.Contain(x => x.Seminar.Name == "Seminar1")
                .And.Contain(x => x.Seminar.Name == "Seminar2");
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
        public async Task Create_shows_only_Seminars_without_uid()
        {
            var s = new Seminar {Name = "[uid] test", SeminarId = 7};
            Context.Add(s);
            await Context.SaveChangesAsync();
            var result = await Controller.Create();

            result.As<ViewResult>().Model.As<CourseDesignViewModel>().Seminars
                .Count(x => x.Name.Substring(0, 1) == "[")
                .Should().Be(0);
        }


        [Fact]
        public async Task Create_stores_new_CourseDesign()
        {            
            var courseDesignView = new CourseDesignViewModel(CourseDesign7, "http://www.completevocaltraining.nl") 
                {CheckList = new List<CheckedId>()};

            await Controller.Create(courseDesignView);

            Resultcontext.CourseDesigns.Should().HaveCount(4);
        }

        [Fact]
        public async Task Create_stores_CourseDesign_with_correct_properties()
        {            
            var courseDesignView = new CourseDesignViewModel(CourseDesign7, "http://www.completevocaltraining.nl")
                {CheckList = new List<CheckedId>()};

            await Controller.Create(courseDesignView);

            Resultcontext.CourseDesigns.FirstOrDefault(x => x.CourseDesignId == 7)
                .Should().BeEquivalentTo(CourseDesign7);
        }

        [Fact]
        public async Task Create_stores_CourseDesign_with_correct_CourseSeminars()
        {            
            var seminars = await Context.Seminars.ToListAsync();
            var courseDesignView = new CourseDesignViewModel(seminars, "http://www.completevocaltraining.nl") 
                {CourseDesign = CourseDesign7};
            courseDesignView.CheckList[0].IsSelected = true;
            courseDesignView.CheckList[2].IsSelected = true;

            await Controller.Create(courseDesignView);

            var result = Resultcontext.CourseDesigns.FirstOrDefault(x => x.CourseDesignId == 7);

            result?.CourseSeminars.Should().HaveCount(2)
                .And.Contain(x => x.Seminar.Name == "Seminar1")
                .And.Contain(x => x.Seminar.Name == "Seminar3");
        }

        [Fact]
        public async Task Edit_returns_Notfound_if_given_unknown_id()
        {
            var result = await Controller.Edit(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Edit_returns_CourseDesignViewModel()
        {
            var result = await Controller.Edit(1);

            result.As<ViewResult>().Model.Should().BeOfType<CourseDesignViewModel>();
        }

        [Fact]
        public async Task Edit_returns_CourseDesignViewModel_with_checklist()
        {
            var result = await Controller.Edit(1);

            result.As<ViewResult>().Model.As<CourseDesignViewModel>().CheckList
                .Should().HaveCount(3, because: "we seeded the Db with 3 seminars");
        }

        [Fact]
        public async Task Edit_shows_only_Seminars_without_uid_if_CourseDesign_has_no_uid()
        {
            var s = new Seminar {Name = "[uid] test", SeminarId = 7};
            Context.Add(s);
            await Context.SaveChangesAsync();
            
            var result = await Controller.Edit(1);

            result.As<ViewResult>().Model.As<CourseDesignViewModel>().CheckList
                .Count(x => x.Name.Substring(0, 1) == "[")
                .Should().Be(0);
        }

        [Fact]
        public async Task Edit_shows_only_Seminars_with_matching_uid()
        {
            await MakeNewCourse(1);
            
            var result = await Controller.Edit(4);

            result.As<ViewResult>().Model.As<CourseDesignViewModel>().CheckList.Count
                .Should().Be(1, because:"CourseDesign 1 only contains 1 Seminar now copied and prepended with [test]");
            result.As<ViewResult>().Model.As<CourseDesignViewModel>().CheckList
                .Count(x => x.Name.Substring(0, 1) == "[")
                .Should().Be(1, because:"CourseDesign 1 only contains 1 Seminar now copied and prepended with [test]");
        }


        [Fact]
        public async Task Edit_returns_CourseDesignViewModel_with_correct_CourseDesign()
        {
            var result = await Controller.Edit(1);

            result.As<ViewResult>().Model.As<CourseDesignViewModel>().CourseDesign
                .Name.Should().Be("CourseDesign1");
        }
        
        [Fact]
        public async Task Edit_removes_uid_from_name_puts_it_in_Uid_prop()
        {
            var cd = new CourseDesign(){ Name = "[test] name", CourseDesignId = 7};
            Context.Add(cd);
            await Context.SaveChangesAsync();

            var result = await Controller.Edit(7);

            result.As<ViewResult>().Model.As<CourseDesignViewModel>().Uid.Should().Match("[test]");
            result.As<ViewResult>().Model.As<CourseDesignViewModel>().CourseDesign.Name.Should().Match("name");
        }
        
        [Fact]
        public async Task Edit_puts_uid_and_name_back_together_when_saving_to_context()
        {
            const string name = "[test] name";
            var cd = new CourseDesign{ Name = name, CourseDesignId = 7};
            Context.Add(cd);
            await Context.SaveChangesAsync();
            var actionResult = await Controller.Edit(7);
            var model = actionResult.As<ViewResult>().Model.As<CourseDesignViewModel>();

            await Controller.Edit(7, model);
            var result = Context.CourseDesigns.FirstOrDefault(x => x.CourseDesignId == 7);

            result?.Name.Should().Be(name);
        }

        [Fact]
        public async Task Edit_returns_CourseDesignViewModel_with_ALL_Seminars_injected_into_checklist()
        {
            var result = await Controller.Edit(1);

            result.As<ViewResult>().Model.As<CourseDesignViewModel>().CheckList
                .Should().HaveCount(3).And.Contain(x => x.Name == "Seminar1")
                .And.Contain(x => x.Name == "Seminar2")
                .And.Contain(x => x.Name == "Seminar3");
        }

        [Fact]
        public async Task Edit_updates_CourseDesign_with_correct_properties()
        {            
            var courseDesign = Context.CourseDesigns.FirstOrDefault(x => x.CourseDesignId == 1);
            courseDesign.Name = "Effects";
            courseDesign.Description = "Learn about effects";

            var courseDesignView = new CourseDesignViewModel(courseDesign, "http://www.completevocaltraining.nl") {CheckList = new List<CheckedId>()};

            await Controller.Edit(1, courseDesignView);

            Resultcontext.CourseDesigns.FirstOrDefault(x => x.CourseDesignId == 1)
                .Should().BeEquivalentTo(courseDesign, options => options
                    .Excluding(cd => cd.CourseSeminars));
        }

        [Fact]
        public async Task Edit_updates_CourseDesign_with_correct_CourseSeminars()
        {            
            var cd = Context.CourseDesigns.FirstOrDefault(x => x.CourseDesignId == 1);
            var seminars = await Context.Seminars.ToListAsync();

            var courseDesignView = new CourseDesignViewModel(cd, seminars, "", "http://www.completevocaltraining.nl") ;
            courseDesignView.CheckList[2].IsSelected = false;
            courseDesignView.CheckList[1].IsSelected = false;
            courseDesignView.CheckList[0].IsSelected = true;

            await Controller.Edit(1, courseDesignView);

            Resultcontext.CourseDesigns.FirstOrDefault(x => x.CourseDesignId == 1)?.CourseSeminars.Should()
                .HaveCount(1).And.Contain(x => x.Seminar.Name == "Seminar3");
        }

        [Fact]
        public async Task Edit_returns_NotFound_if_Id_changes()
        {            
            var cd = Context.CourseDesigns.FirstOrDefault(x => x.CourseDesignId == 1);

            var courseDesignView = new CourseDesignViewModel {CourseDesign = cd, CheckList = new List<CheckedId>()};

            var result = await Controller.Edit(8, courseDesignView);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Edit_returns_View_if_modelstate_not_valid()
        {      
            var controller = Controller;
            controller.ViewData.ModelState.AddModelError("key", "some error");
            var courseDesignView = new CourseDesignViewModel
            {
                CourseDesign = new CourseDesign() {CourseDesignId = 1}, CheckList = new List<CheckedId>()
            };

            var result = await controller.Edit(1, courseDesignView);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Edit_returns_NotFound_if_concurrencyException_occurs()
        {            
            var cd = await Context.CourseDesigns.FirstOrDefaultAsync(x => x.CourseDesignId == 1);
            var seminars = await Context.Seminars.ToListAsync();
            var courseDesignView = new CourseDesignViewModel(cd, seminars);

            Context.Remove(cd);
            await Context.SaveChangesAsync();

            var result = await Controller.Edit(1, courseDesignView);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_returns_Correct_CourseDesign()
        {
            var result = await Controller.Delete(1);

            result.As<ViewResult>().Model.As<CourseDesignViewModel>().CourseDesign.Name.Should().Be("CourseDesign1");
        }

        [Fact]
        public async Task Delete_returns_Notfound_if_given_unknown_id()
        {
            var result = await Controller.Delete(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_removes_CourseDesign_from_Db()
        {
            await Controller.DeleteConfirmed(new CourseDesignViewModel(
                new CourseDesign(){CourseDesignId = 1}, "http://www.completevocaltraining.nl"));

            var result = Resultcontext.CourseDesigns.FirstOrDefault(x => x.CourseDesignId == 1);

            result.Should().BeNull();
        }

        [Fact]
        public async Task Delete_removes_all_related_course_elements_if_it_has_uid()
        {
            var courseView = new CourseViewModel(new List<CourseDesign>()) {Course = new Course(){Name = "test"}};
            courseView.Course.CourseDesign = new CourseDesign
            {
                CourseDesignId = 2
            };
            var controller = new CourseController(Context);
            await controller.Create(courseView);
            
            await Controller.DeleteConfirmed(new CourseDesignViewModel(
                new CourseDesign(){CourseDesignId = 4}, "http://www.completevocaltraining.nl"));
            
            Resultcontext.CourseDesigns.Should().HaveCount(3, because:"We seeded the db with 3 CourseDesigns");
            Resultcontext.Seminars.Should().HaveCount(3, because:"We seeded the db with 3 seminars");
            Resultcontext.Days.Should().HaveCount(6, because:"We seeded the db with 6 days");
            Resultcontext.Subjects.Should().HaveCount(6, because:"We seeded the db with 6 subjects");
        }
        
        [Fact]
        public void Validation_Leaving_Name_Null_or_short_causes_modelstate_not_valid()
        {            
            var cd = new CourseDesign();
            var cd2 = new CourseDesign();
            cd.CourseDesignId = 1;
            cd2.CourseDesignId = 1;
            cd2.Name = "123";

            var result = Validator.TryValidateObject(cd, new ValidationContext(cd), null, true);
            var result2 = Validator.TryValidateObject(cd2, new ValidationContext(cd2), null, true);

            result.Should().BeFalse();
            result2.Should().BeFalse();
        }

        [Fact]
        public void Validation_entering_disallowed_character_in_Name_field_causes_modelstate_not_valid()
        {            
            var x = new CourseDesign();
            var x2 = new CourseDesign();
            var x3 = new CourseDesign();
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

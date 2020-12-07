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
        public async Task Index_returns_All_CourseDesigns()
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
        public async Task Create_stores_new_CourseDesign()
        {            
            var courseDesignView = new CourseDesignViewModel {CourseDesign = CourseDesign7, CheckList = new List<CheckedId>()};

            await Controller.Create(courseDesignView);

            Resultcontext.CourseDesigns.Should().HaveCount(4);
        }

        [Fact]
        public async Task Create_stores_CourseDesign_with_correct_properties()
        {            
            var courseDesignView = new CourseDesignViewModel {CourseDesign = CourseDesign7, CheckList = new List<CheckedId>()};

            await Controller.Create(courseDesignView);

            Resultcontext.CourseDesigns.FirstOrDefault(x => x.CourseDesignId == 7)
                .Should().BeEquivalentTo(CourseDesign7);
        }

        [Fact]
        public async Task Create_stores_CourseDesign_with_correct_CourseSeminars()
        {            
            var seminars = await Context.Seminars.ToListAsync();
            var courseDesignView = new CourseDesignViewModel(seminars) {CourseDesign = CourseDesign7};
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
                .Should().NotBeNullOrEmpty(because: "we seeded the Db with seminars");
        }

        [Fact]
        public async Task Edit_returns_CourseDesignViewModel_with_correct_CourseDesign()
        {
            var result = await Controller.Edit(1);

            result.As<ViewResult>().Model.As<CourseDesignViewModel>().CourseDesign
                .Name.Should().Be("CourseDesign1");
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
            var cd = Context.CourseDesigns.FirstOrDefault(x => x.CourseDesignId == 1);
            cd.Name = "Effects";
            cd.Description = "Learn about effects";

            var courseDesignView = new CourseDesignViewModel {CourseDesign = cd, CheckList = new List<CheckedId>()};

            await Controller.Edit(1, courseDesignView);

            Resultcontext.CourseDesigns.FirstOrDefault(x => x.CourseDesignId == 1)
                .Should().BeEquivalentTo(cd, options => options
                    .Excluding(cd => cd.CourseSeminars));
        }

        [Fact]
        public async Task Edit_updates_CourseDesign_with_correct_CourseSeminars()
        {            
            var cd = Context.CourseDesigns.FirstOrDefault(x => x.CourseDesignId == 1);
            var seminars = await Context.Seminars.ToListAsync();

            var courseDesignView = new CourseDesignViewModel(cd, seminars);
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

            result.As<ViewResult>().Model.As<CourseDesign>().Name.Should().Be("CourseDesign1");
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
            await Controller.DeleteConfirmed(1);

            var result = Resultcontext.CourseDesigns.FirstOrDefault(x => x.CourseDesignId == 1);

            result.Should().BeNull();
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

    }
}

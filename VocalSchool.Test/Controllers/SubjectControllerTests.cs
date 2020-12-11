using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using VocalSchool.Models;
using VocalSchool.Controllers;
using VocalSchool.Test.Infrastructure;
using System.ComponentModel.DataAnnotations;
using VocalSchool.ViewModels;

namespace VocalSchool.Test.Controllers
{
    public class SubjectControllerTests : UnitTestBase

    {
        public SubjectControllerTests(ConfigFixture fixture) : base(fixture)
        {
        }
        
        private static Subject Subject7 => new Subject { SubjectId = 7, Name = "Effects",
                                    Description = "Learn about effects", RequiredReading = "CVT App" };

        private SubjectController Controller => new SubjectController(Context);
        
        [Fact]
        public async Task Index_returns_ViewResult()
        {
            var result = await Controller.Index();

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Index_returns_Subjects()
        {
            var result = await Controller.Index();

            result.As<ViewResult>().Model.Should().BeOfType<List<Subject>>();
        }

        [Fact]
        public async Task Index_returns_All_Subjects()
        {
            var result = await Controller.Index();

            result.As<ViewResult>().Model.As<List<Subject>>().Should().HaveCount(6);
        }

        [Fact]
        public async Task Index_only_returns_subjects_without_uid()
        {
            var s = new Subject() {Name = "[prepend test] Support", SubjectId = 7};
            await Controller.Create(s);

            var result = await Controller.Index();

            result.As<ViewResult>().Model.As<List<Subject>>().Should().HaveCount(6);
        }


        [Fact]
        public async Task Details_returns_Subject()
        {
            var result = await Controller.Details(1);

            result.As<ViewResult>().Model.Should().BeAssignableTo<Subject>();
        }

        [Fact]
        public async Task Details_returns_Correct_Subject()
        {
            var result = await Controller.Details(1);

            result.As<ViewResult>().Model.As<Subject>().Name.Should().Be("Introduction");
        }
        
        [Fact]
        public async Task Details_returns_Notfound_if_given_unknown_id()
        {
            var result = await Controller.Details(8);

            result.Should().BeOfType<NotFoundResult>();
        }
        
        [Fact]
        public void Create_returns_view_when_not_passed_Id()
        {
            var result = Controller.Create();

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Create_stores_new_Subject()
        {
            await Controller.Create(Subject7);

            Context.Subjects.Should().HaveCount(7);
        }
        
        [Fact]
        public async Task Create_stores_Subject_with_correct_properties()
        {
            await Controller.Create(Subject7);

            Context.Subjects.FirstOrDefault(x => x.SubjectId == 7).Should().BeEquivalentTo(Subject7);
        }

        [Fact]
        public async Task Edit_returns_Notfound_if_given_unknown_id()
        {
            var result = await Controller.Edit(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Edit_returns_SubjectViewModel()
        {
            var result = await Controller.Edit(1);

            result.As<ViewResult>().Model.Should().BeAssignableTo<SubjectViewModel>();
        }

        [Fact]
        public async Task Edit_returns_Correct_Subject()
        {
            var result = await Controller.Edit(1);

            result.As<ViewResult>().Model.As<SubjectViewModel>().Subject.Name.Should().Be("Introduction");
        }

        [Fact]
        public async Task Edit_removes_uid_from_name_puts_it_in_Uid_prop()
        {
            var s = new Subject{ Name = "[test] name", SubjectId = 7};
            Context.Add(s);
            await Context.SaveChangesAsync();

            var result = await Controller.Edit(7);

            result.As<ViewResult>().Model.As<SubjectViewModel>().Uid.Should().Match("[test]");
            result.As<ViewResult>().Model.As<SubjectViewModel>().Subject.Name.Should().Match("name");
        }

        [Fact]
        public async Task Edit_puts_uid_and_name_back_together_when_saving_to_context()
        {
            var s = new Subject{ Name = "[test] name", SubjectId = 7};
            Context.Add(s);
            await Context.SaveChangesAsync();
            var actionResult = await Controller.Edit(7);
            var model = actionResult.As<ViewResult>().Model.As<SubjectViewModel>();

            await Controller.Edit(model);
            var result = Context.Subjects.FirstOrDefault(x => x.SubjectId == 7);

            result?.Name.Should().Be(s.Name);
        }
        
        [Fact]
        public async Task Edit_updates_Subject_with_correct_properties()
        {
            var s = Subject7;
            s.SubjectId = 1;
            var vm = new SubjectViewModel(s, "");

            await Controller.Edit(vm);

            Context.Subjects.FirstOrDefault(x => x.SubjectId == 1).Should().BeEquivalentTo(s);
        }
       
        [Fact]
        public async Task Edit_returns_Redirect_if_modelstate_not_valid()
        {
            var controller = Controller;
            var s = new Subject  {SubjectId = 1};
            var vm = new SubjectViewModel(s, "");
            controller.ViewData.ModelState.AddModelError("key", "Some Exception");

            var result = await controller.Edit(vm);

            result.Should().BeOfType<ViewResult>();
        }
       
        [Fact]
        public async Task Edit_returns_Redirect_to_Index_if_concurrencyException_occurs()
        {            
            var s = Context.Subjects.FirstOrDefault(x => x.SubjectId == 1);
            var vm = new SubjectViewModel(s, "");
            Context.Remove(s);
            await Context.SaveChangesAsync();

            var result = await Controller.Edit(vm);

            result.As<RedirectToActionResult>().ActionName.Should().Match("Index");
        }

        [Fact]
        public async Task Delete_returns_Correct_Subject()
        {
            var result = await Controller.Delete(1);

            result.As<ViewResult>().Model.As<Subject>().Name.Should().Be("Introduction");
        }

        [Fact]
        public async Task Delete_returns_Notfound_if_given_unknown_id()
        {
            var result = await Controller.Delete(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_removes_subject_from_Db()
        {
            await Controller.DeleteConfirmed(1);
            var result = Context.Subjects.FirstOrDefault(x => x.SubjectId == 1);

            result.Should().BeNull();
        }

        [Fact]
        public void Validation_Leaving_Name_Null_or_short_causes_modelstate_not_valid()
        {            
            var s = new Subject();
            var s2 = new Subject();
            s.SubjectId = 1;
            s2.SubjectId = 1;
            s2.Name = "123";

            var result = Validator.TryValidateObject(s, new ValidationContext(s), null, true);
            var result2 = Validator.TryValidateObject(s2, new ValidationContext(s2), null, true);

            result.Should().BeFalse();
            result2.Should().BeFalse();
        }

    }
}

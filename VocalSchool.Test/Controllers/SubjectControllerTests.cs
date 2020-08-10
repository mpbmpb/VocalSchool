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

namespace VocalSchool.Test.Controllers
{
    public class SubjectControllerTests : VocalSchoolTestBase

    {
        [Fact]
        public async Task Index_returns_ViewResult()
        {
            //Arrange
            var controller = new SubjectController(_context);

            //Act
            IActionResult result = await controller.Index();

            //Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Index_returns_Subjects()
        {
            var controller = new SubjectController(_context);

            IActionResult result = await controller.Index();

            result.As<ViewResult>().Model.Should().BeOfType<List<Subject>>();
        }

        [Fact]
        public async Task Index_returns_All_Subjects()
        {
            var controller = new SubjectController(_context);

            IActionResult result = await controller.Index();

            result.As<ViewResult>().Model.As<List<Subject>>().Should().HaveCount(6);
        }

        [Fact]
        public async Task Details_returns_Subject()
        {
            var controller = new SubjectController(_context);

            IActionResult result = await controller.Details(1);

            result.As<ViewResult>().Model.Should().BeOfType<Subject>();
        }

        [Fact]
        public async Task Details_returns_Correct_Subject()
        {
            var controller = new SubjectController(_context);

            IActionResult result = await controller.Details(1);

            result.As<ViewResult>().Model.As<Subject>().Name.Should().Be("Introduction");
        }
        
        [Fact]
        public async Task Details_returns_Notfound_if_given_unknown_id()
        {
            var controller = new SubjectController(_context);

            IActionResult result = await controller.Details(8);

            result.Should().BeOfType<NotFoundResult>();
        }
        
        [Fact]
        public void Create_returns_view_when_not_passed_Id()
        {
            var controller = new SubjectController(_context);

            IActionResult result = controller.Create();

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Create_stores_new_Subject()
        {
            var controller = new SubjectController(_context);
            Subject s = new Subject { SubjectId = 7, Name = "Effects",
                Description = "Learn about effects", RequiredReading = "CVT App" };

            await controller.Create(s);

            _context.Subjects.Should().HaveCount(7);
        }
        
        [Fact]
        public async Task Create_stores_Subject_with_correct_properties()
        {
            var controller = new SubjectController(_context);
            Subject s = new Subject { SubjectId = 7, Name = "Effects",
                Description = "Learn about effects", RequiredReading = "CVT App" };

            await controller.Create(s);

            _context.Subjects.FirstOrDefault(x => x.SubjectId == 7).Should().BeEquivalentTo<Subject>(s);
        }

        [Fact]
        public async Task Edit_returns_Notfound_if_given_unknown_id()
        {
            var controller = new SubjectController(_context);

            IActionResult result = await controller.Edit(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Edit_returns_Subject()
        {
            var controller = new SubjectController(_context);

            IActionResult result = await controller.Edit(1);

            result.As<ViewResult>().Model.Should().BeOfType<Subject>();
        }

        [Fact]
        public async Task Edit_returns_Correct_Subject()
        {
            var controller = new SubjectController(_context);

            IActionResult result = await controller.Edit(1);

            result.As<ViewResult>().Model.As<Subject>().Name.Should().Be("Introduction");
        }

        [Fact]
        public async Task Edit_updates_Subject_with_correct_properties()
        {
            var controller = new SubjectController(_context);
            Subject s = _context.Subjects.FirstOrDefault(x => x.SubjectId == 1);
            s.Name = "Effects";
            s.Description = "Learn about effects";
            s.RequiredReading = "CVT App";

            await controller.Edit(1,s);

            _context.Subjects.FirstOrDefault(x => x.SubjectId == 1).Should().BeEquivalentTo<Subject>(s);
        }
        
        [Fact]
        public async Task Edit_returns_NotFound_if_Id_changes()
        {
            var controller = new SubjectController(_context);
            Subject s = _context.Subjects.FirstOrDefault(x => x.SubjectId == 1);
            s.Name = "Effects";
            s.Description = "Learn about effects";
            s.RequiredReading = "CVT App";

            IActionResult result = await controller.Edit(8,s);

            result.Should().BeOfType<NotFoundResult>();
        }
        
        [Fact]
        public async Task Edit_returns_Redirect_if_modelstate_not_valid()
        {
            var controller = new SubjectController(_context);
            Subject s = _context.Subjects.FirstOrDefault(x => x.SubjectId == 1);
            s.Name = null;
            s.Description = "Learn about effects";
            s.RequiredReading = "CVT App";

            IActionResult result = await controller.Edit(1,s);

            result.Should().BeOfType<RedirectToActionResult>();
        }
       
        [Fact]
        public async Task Edit_returns_Redirect_to_Index_if_concurrencyException_occurs()
        {
            var controller = new SubjectController(_context);
            Subject s = _context.Subjects.FirstOrDefault(x => x.SubjectId == 1);
            _context.Remove(s);
            await _context.SaveChangesAsync();

            IActionResult result = await controller.Edit(1,s);

            result.As<RedirectToActionResult>().ActionName.Should().Match("Index");
        }

        [Fact]
        public async Task Delete_returns_Correct_Subject()
        {
            var controller = new SubjectController(_context);

            IActionResult result = await controller.Delete(1);

            result.As<ViewResult>().Model.As<Subject>().Name.Should().Be("Introduction");
        }

        [Fact]
        public async Task Delete_returns_Notfound_if_given_unknown_id()
        {
            var controller = new SubjectController(_context);

            IActionResult result = await controller.Delete(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_removes_subject_from_Db()
        {
            var controller = new SubjectController(_context);

            var subject = _context.Subjects.FirstOrDefault(x => x.SubjectId == 1);

            await controller.DeleteConfirmed(1);

            var result = _context.Subjects.FirstOrDefault(x => x.SubjectId == 1);

            result.Should().BeNull();
        }

    }
}

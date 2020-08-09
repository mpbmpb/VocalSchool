using System;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

    }
}

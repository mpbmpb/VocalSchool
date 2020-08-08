using System;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Index_returns_Subjects()
        {
            //Arrange
            var controller = new SubjectController(_context);

            //Act
            IActionResult result = await controller.Index();
            ViewResult view = (ViewResult)result;
            var model = view.Model;

            //Assert
            Assert.IsType<List<Subject>>(model);
        }
        
        [Fact]
        public async Task Index_returns_All_Subjects()
        {
            //Arrange
            var controller = new SubjectController(_context);

            //Act
            IActionResult result = await controller.Index();
            ViewResult view = (ViewResult)result;
            List<Subject> model = (List<Subject>)view.Model;

            //Assert
            Assert.Equal(6, model.Count);
        }
        
        [Fact]
        public async Task Details_returns_Subject()
        {
            //Arrange
            var controller = new SubjectController(_context);

            //Act
            IActionResult result = await controller.Details(1);
            ViewResult view = (ViewResult)result;
            var model = view.Model;

            //Assert
            Assert.IsType<Subject>(model);
        }



    }
}

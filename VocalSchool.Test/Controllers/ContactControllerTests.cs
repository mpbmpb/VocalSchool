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
using System.ComponentModel.DataAnnotations;

namespace VocalSchool.Test.Controllers
{
    public class ContactControllerTests : VocalSchoolTestBase

    {
        [Fact]
        public async Task Index_returns_ViewResult()
        {
            //Arrange
            var controller = new ContactController(_context);

            //Act
            IActionResult result = await controller.Index();

            //Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Index_returns_Contacts()
        {
            var controller = new ContactController(_context);

            IActionResult result = await controller.Index();

            result.As<ViewResult>().Model.Should().BeOfType<List<Contact>>();
        }

        [Fact]
        public async Task Index_returns_All_Contacts()
        {
            var controller = new ContactController(_context);

            IActionResult result = await controller.Index();

            result.As<ViewResult>().Model.As<List<Contact>>().Should().HaveCount(3);
        }

        [Fact]
        public async Task Details_returns_Contact()
        {
            var controller = new ContactController(_context);

            IActionResult result = await controller.Details(1);

            result.As<ViewResult>().Model.Should().BeOfType<Contact>();
        }

        [Fact]
        public async Task Details_returns_Correct_Contact()
        {
            var controller = new ContactController(_context);

            IActionResult result = await controller.Details(1);

            result.As<ViewResult>().Model.As<Contact>().Name.Should().Be("Contact1");
        }

        [Fact]
        public async Task Details_returns_Notfound_if_given_unknown_id()
        {
            var controller = new ContactController(_context);

            IActionResult result = await controller.Details(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void Create_returns_view_when_not_passed_Id()
        {
            var controller = new ContactController(_context);

            IActionResult result = controller.Create();

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Create_stores_new_Contact()
        {
            var controller = new ContactController(_context);
            Contact c = new Contact
            {
                ContactId = 7,
                Name = "Effects",
                Email = "info@acme.com",
                Adress = "straat 1 Amsterdam"
            };

            await controller.Create(c);

            _context.Contacts.Should().HaveCount(4);
        }

        [Fact]
        public async Task Create_stores_Contact_with_correct_properties()
        {
            var controller = new ContactController(_context);
            Contact c = new Contact
            {
                ContactId = 7,
                Name = "Effects",
                Email = "info@acme.com",
                Adress = "straat 1 Amsterdam"
            };

            await controller.Create(c);

            _context.Contacts.FirstOrDefault(x => x.ContactId == 7).Should().BeEquivalentTo(c);
        }

        [Fact]
        public async Task Edit_returns_Notfound_if_given_unknown_id()
        {
            var controller = new ContactController(_context);

            IActionResult result = await controller.Edit(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Edit_returns_Contact()
        {
            var controller = new ContactController(_context);

            IActionResult result = await controller.Edit(1);

            result.As<ViewResult>().Model.Should().BeOfType<Contact>();
        }

        [Fact]
        public async Task Edit_returns_Correct_Contact()
        {
            var controller = new ContactController(_context);

            IActionResult result = await controller.Edit(1);

            result.As<ViewResult>().Model.As<Contact>().Name.Should().Be("Contact1");
        }

        [Fact]
        public async Task Edit_updates_Contact_with_correct_properties()
        {
            var controller = new ContactController(_context);
            Contact c = _context.Contacts.FirstOrDefault(x => x.ContactId == 1);
            c.Name = "Effects";
            c.Email = "info@acme.com";
            c.Adress = "straat 1 Amsterdam";

            await controller.Edit(1, c);

            _context.Contacts.FirstOrDefault(x => x.ContactId == 1).Should().BeEquivalentTo(c);
        }

        [Fact]
        public async Task Edit_returns_NotFound_if_Id_changes()
        {
            var controller = new ContactController(_context);
            Contact c = _context.Contacts.FirstOrDefault(x => x.ContactId == 1);
            c.Name = "Effects";
            c.Email = "info@acme.com";
            c.Adress = "straat 1 Amsterdam";

            IActionResult result = await controller.Edit(8, c);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Edit_returns_Redirect_if_modelstate_not_valid()
        {
            var controller = new ContactController(_context);
            controller.ViewData.ModelState.AddModelError("key", "Some Exception");
            Contact c = new Contact();
            c.ContactId = 1;

            IActionResult result = await controller.Edit(1, c);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Edit_returns_NotFound_if_concurrencyException_occurs()
        {
            var controller = new ContactController(_context);
            Contact c = _context.Contacts.FirstOrDefault(x => x.ContactId == 1);
            _context.Remove(c);
            await _context.SaveChangesAsync();

            IActionResult result = await controller.Edit(1, c);

            result.As<ActionResult>().Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_returns_Correct_Contact()
        {
            var controller = new ContactController(_context);

            IActionResult result = await controller.Delete(1);

            result.As<ViewResult>().Model.As<Contact>().Name.Should().Be("Contact1");
        }

        [Fact]
        public async Task Delete_returns_Notfound_if_given_unknown_id()
        {
            var controller = new ContactController(_context);

            IActionResult result = await controller.Delete(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_removes_Contact_from_Db()
        {
            var controller = new ContactController(_context);

            await controller.DeleteConfirmed(1);
            var result = _context.Contacts.FirstOrDefault(x => x.ContactId == 1);

            result.Should().BeNull();
        }

        [Fact]
        public void Validation_Leaving_Name_short_causes_modelstate_not_valid()
        {
            var controller = new ContactController(_context);
            Contact c = new Contact();
            c.ContactId = 1;
            c.Name = "123";

            var result = Validator.TryValidateObject(c, new ValidationContext(c), null, true);

            result.Should().BeFalse();
        }

    }
}

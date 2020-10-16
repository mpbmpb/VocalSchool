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
    public class ContactControllerTests : UnitTestBase
    {
        public ContactControllerTests(ConfigFixture fixture) : base(fixture)
        {
        }

        private ContactController Controller => new ContactController(Context);

        private static Contact Contact7 => new Contact
        {
            ContactId = 7,
            Name = "Effects",
            Email = "info@acme.com",
            Adress = "straat 1 Amsterdam"
        };    

        [Fact]
        public async Task Index_returns_ViewResult()
        {
            var result = await Controller.Index();

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Index_returns_Contacts()
        {
            var result = await Controller.Index();

            result.As<ViewResult>().Model.Should().BeOfType<List<Contact>>();
        }

        [Fact]
        public async Task Index_returns_All_Contacts()
        {
            var result = await Controller.Index();

            result.As<ViewResult>().Model.As<List<Contact>>().Should().HaveCount(3);
        }

        [Fact]
        public async Task Details_returns_Contact()
        {
            var result = await Controller.Details(1);

            result.As<ViewResult>().Model.Should().BeAssignableTo<Contact>();
        }

        [Fact]
        public async Task Details_returns_Correct_Contact()
        {
            var result = await Controller.Details(1);

            result.As<ViewResult>().Model.As<Contact>().Name.Should().Be("Contact1");
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
        public async Task Create_stores_new_Contact()
        {
            await Controller.Create(Contact7);

            Resultcontext.Contacts.Should().HaveCount(4);
        }

        [Fact]
        public async Task Create_stores_Contact_with_correct_properties()
        {
            await Controller.Create(Contact7);

            Resultcontext.Contacts.FirstOrDefault(x => x.ContactId == 7).Should().BeEquivalentTo(Contact7);
        }

        [Fact]
        public async Task Edit_returns_Notfound_if_given_unknown_id()
        {
            var result = await Controller.Edit(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Edit_returns_Contact()
        {
            var result = await Controller.Edit(1);

            result.As<ViewResult>().Model.Should().BeAssignableTo<Contact>();
        }

        [Fact]
        public async Task Edit_returns_Correct_Contact()
        {
            var result = await Controller.Edit(1);

            result.As<ViewResult>().Model.As<Contact>().Name.Should().Be("Contact1");
        }

        [Fact]
        public async Task Edit_updates_Contact_with_correct_properties()
        {
            var c = Contact7;
            c.ContactId = 1;

            await Controller.Edit(1, c);

            Resultcontext.Contacts.FirstOrDefault(x => x.ContactId == 1).Should().BeEquivalentTo(c);
        }

        [Fact]
        public async Task Edit_returns_NotFound_if_Id_changes()
        {                
            var c = Contact7;
            c.ContactId = 1;

            var result = await Controller.Edit(8, c);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Edit_returns_Redirect_if_modelstate_not_valid()
        {
            var controller = Controller;
            controller.ViewData.ModelState.AddModelError("key", "Some Exception");
            var c = new Contact {ContactId = 1};

            var result = await controller.Edit(1, c);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Edit_returns_NotFound_if_concurrencyException_occurs()
        {            
            var c = Context.Contacts.FirstOrDefault(x => x.ContactId == 1);
            Context.Remove(c);
            await Context.SaveChangesAsync();

            var result = await Controller.Edit(1, c);

            result.As<ActionResult>().Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_returns_Correct_Contact()
        {
            var result = await Controller.Delete(1);

            result.As<ViewResult>().Model.As<Contact>().Name.Should().Be("Contact1");
        }

        [Fact]
        public async Task Delete_returns_Notfound_if_given_unknown_id()
        {
            var result = await Controller.Delete(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_removes_Contact_from_Db()
        {
            await Controller.DeleteConfirmed(1);
            var result = Resultcontext.Contacts.FirstOrDefault(x => x.ContactId == 1);

            result.Should().BeNull();
        }

        [Fact]
        public void Validation_Leaving_Name_Null_Or_short_causes_modelstate_not_valid()
        {            
            var c = new Contact();
            var c2 = new Contact();
            c.ContactId = 1;
            c2.ContactId = 1;
            c2.Name = "123";

            var result = Validator.TryValidateObject(c, new ValidationContext(c), null, true);
            var result2 = Validator.TryValidateObject(c2, new ValidationContext(c2), null, true);

            result.Should().BeFalse();
            result2.Should().BeFalse();
        }

    }
}

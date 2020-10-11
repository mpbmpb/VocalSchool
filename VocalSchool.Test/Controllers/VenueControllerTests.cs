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
using VocalSchool.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace VocalSchool.Test.Controllers
{
    public class VenueControllerTests : VocalSchoolTestBase

    {
        [Fact]
        public async Task Index_returns_ViewResult()
        {
            //Arrange
            var controller = new VenueController(Context);

            //Act
            IActionResult result = await controller.Index();

            //Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Index_returns_Venues()
        {
            var controller = new VenueController(Context);

            IActionResult result = await controller.Index();

            result.As<ViewResult>().Model.Should().BeOfType<List<Venue>>();
        }

        [Fact]
        public async Task Index_returns_All_Venues()
        {
            var controller = new VenueController(Context);

            IActionResult result = await controller.Index();

            result.As<ViewResult>().Model.As<List<Venue>>().Should().HaveCount(2);
        }

        [Fact]
        public async Task Details_returns_Venue()
        {
            var controller = new VenueController(Context);

            IActionResult result = await controller.Details(1);

            result.As<ViewResult>().Model.Should().BeAssignableTo<Venue>();
        }

        [Fact]
        public async Task Details_returns_Correct_Venue()
        {
            var controller = new VenueController(Context);

            IActionResult result = await controller.Details(1);

            result.As<ViewResult>().Model.As<Venue>().Name.Should().Be("Venue1");
        }

        [Fact]
        public async Task Details_returns_Correct_Venue_with_All_Contacts()
        {
            var controller = new VenueController(Context);

            IActionResult result = await controller.Details(1);

            result.As<ViewResult>().Model.As<Venue>().Contact1.Name.Should().Match("Contact1");
            result.As<ViewResult>().Model.As<Venue>().Contact2.Name.Should().Match("Contact2");
        }

        [Fact]
        public async Task Details_returns_Notfound_if_given_unknown_id()
        {
            var controller = new VenueController(Context);

            IActionResult result = await controller.Details(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_returns_view_when_not_passed_Id()
        {
            var controller = new VenueController(Context);

            IActionResult result = await controller.Create();

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Create_stores_new_Venue()
        {
            var controller = new VenueController(Context);
            Venue v = new Venue
            {
                VenueId = 7,
                Name = "Venue7",
                Info = "TBD",
            };
            var contacts = await Context.Contacts.ToListAsync();
            VenueViewModel VenueView = new VenueViewModel(contacts);
            VenueView.Venue = v;

            await controller.Create(VenueView);

            Context.Venues.Should().HaveCount(3);
        }

        [Fact]
        public async Task Create_stores_Venue_with_correct_properties()
        {
            var controller = new VenueController(Context);
            Venue v = new Venue
            {
                VenueId = 7,
                Name = "Venue7",
                Info = "TBD",
            };
            VenueViewModel VenueView = new VenueViewModel();
            VenueView.Venue = v;
            VenueView.ContactList = new List<SelectListItem>();

            await controller.Create(VenueView);

            Context.Venues.FirstOrDefault(x => x.VenueId == 7).Should().BeEquivalentTo<Venue>(v);
        }

        [Fact]
        public async Task Create_stores_Venue_with_correct_Contacts()
        {
            var controller = new VenueController(Context);
            Venue v = new Venue
            {
                VenueId = 7,
                Name = "Venue7",
                Info = "TBD",
            };
            var Contacts = await Context.Contacts.ToListAsync();
            VenueViewModel VenueView = new VenueViewModel(Contacts);
            VenueView.Venue = v;
            VenueView.Venue.Contact1 = new Contact();
            VenueView.Venue.Contact2 = new Contact();
            VenueView.Venue.Contact1.ContactId = Int32.Parse(VenueView.ContactList[1].Value);
            VenueView.Venue.Contact2.ContactId = Int32.Parse(VenueView.ContactList[3].Value);

            await controller.Create(VenueView);

            var result = Context.Venues.FirstOrDefault(x => x.VenueId == 7);

            result.Contact1.Name.Should().Match("Contact3");
            result.Contact2.Name.Should().Match("Contact1");
        }

        [Fact]
        public async Task Edit_returns_Notfound_if_given_unknown_id()
        {
            var controller = new VenueController(Context);

            IActionResult result = await controller.Edit(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Edit_returns_VenueViewModel()
        {
            var controller = new VenueController(Context);

            IActionResult result = await controller.Edit(1);

            result.As<ViewResult>().Model.Should().BeOfType<VenueViewModel>();
        }

        [Fact]
        public async Task Edit_returns_VenueViewModel_with_ContactList()
        {
            var controller = new VenueController(Context);

            IActionResult result = await controller.Edit(1);

            result.As<ViewResult>().Model.As<VenueViewModel>().ContactList
                .Should().NotBeNullOrEmpty(because: "we seeded the Db with Contacts");
        }

        [Fact]
        public async Task Edit_returns_VenueViewModel_with_correct_Venue()
        {
            var controller = new VenueController(Context);

            IActionResult result = await controller.Edit(1);

            result.As<ViewResult>().Model.As<VenueViewModel>().Venue.Name.Should().Be("Venue1");
        }


        [Fact]
        public async Task Edit_returns_VenueViewModel_with_ALL_Contacts_injected_into_ContactList()
        {
            var controller = new VenueController(Context);

            IActionResult result = await controller.Edit(1);

            result.As<ViewResult>().Model.As<VenueViewModel>().ContactList
                .Should().HaveCount(4).And.Contain(x => x.Text == "Contact1")
                .And.Contain(x => x.Text == "Contact2").And.Contain(x => x.Text == "Contact3");
        }

        [Fact]
        public async Task Edit_updates_Venue_with_correct_properties()
        {
            var controller = new VenueController(Context);
            Venue v = Context.Venues.FirstOrDefault(x => x.VenueId == 1);
            v.Name = "Effects";
            v.Info = "TBD";

            VenueViewModel VenueView = new VenueViewModel();
            VenueView.Venue = v;
            VenueView.ContactList = new List<SelectListItem>();

            await controller.Edit(1, VenueView);

            Context.Venues.FirstOrDefault(x => x.VenueId == 1).Should().BeEquivalentTo<Venue>(v);
        }

        [Fact]
        public async Task Edit_updates_Venue_with_correct_Contacts()
        {
            var controller = new VenueController(Context);
            Venue v = Context.Venues.FirstOrDefault(x => x.VenueId == 1);
            var Contacts = await Context.Contacts.ToListAsync();

            VenueViewModel VenueView = new VenueViewModel(v, Contacts);
            VenueView.Venue.Contact1.ContactId = Int32.Parse(VenueView.ContactList[1].Value);

            await controller.Edit(1, VenueView);

            Context.Venues.FirstOrDefault(x => x.VenueId == 1).Contact1
                .Name.Should().Match("Contact3");
        }

        [Fact]
        public async Task Edit_returns_NotFound_if_Id_changes()
        {
            var controller = new VenueController(Context);
            Venue v = Context.Venues.FirstOrDefault(x => x.VenueId == 1);

            VenueViewModel VenueView = new VenueViewModel();
            VenueView.Venue = v;
            VenueView.ContactList = new List<SelectListItem>();

            IActionResult result = await controller.Edit(8, VenueView);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Edit_returns_View_if_modelstate_not_valid()
        {
            var controller = new VenueController(Context);
            controller.ViewData.ModelState.AddModelError("key", "some error");
            VenueViewModel VenueView = new VenueViewModel();
            VenueView.Venue = new Venue() { VenueId = 1 };
            VenueView.ContactList = new List<SelectListItem>();

            IActionResult result = await controller.Edit(1, VenueView);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Edit_returns_Redirect_to_Index_if_concurrencyException_occurs()
        {
            var controller = new VenueController(Context);
            Venue v = await Context.Venues.FirstOrDefaultAsync(x => x.VenueId == 1);
            var Contacts = await Context.Contacts.ToListAsync();
            VenueViewModel VenueView = new VenueViewModel(v, Contacts);

            Context.Remove(v);
            await Context.SaveChangesAsync();
            IActionResult result = await controller.Edit(1, VenueView);

            result.As<RedirectToActionResult>().ActionName.Should().Match("Index");
        }

        [Fact]
        public async Task Delete_returns_Correct_Venue()
        {
            var controller = new VenueController(Context);

            IActionResult result = await controller.Delete(1);

            result.As<ViewResult>().Model.As<Venue>().Name.Should().Be("Venue1");
        }

        [Fact]
        public async Task Delete_returns_Notfound_if_given_unknown_id()
        {
            var controller = new VenueController(Context);

            IActionResult result = await controller.Delete(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_removes_Venue_from_Db()
        {
            var controller = new VenueController(Context);

            var Venue = Context.Venues.FirstOrDefault(x => x.VenueId == 1);

            await controller.DeleteConfirmed(1);

            var result = Context.Venues.FirstOrDefault(x => x.VenueId == 1);

            result.Should().BeNull();
        }

        [Fact]
        public void Validation_Leaving_Name_Null_or_short_causes_modelstate_not_valid()
        {
            var controller = new SubjectController(Context);
            Venue v = new Venue();
            Venue v2 = new Venue();
            v.VenueId = 1;
            v2.VenueId = 1;
            v2.Name = "123";

            var result = Validator.TryValidateObject(v, new ValidationContext(v), null, true);
            var result2 = Validator.TryValidateObject(v2, new ValidationContext(v2), null, true);

            result.Should().BeFalse();
            result2.Should().BeFalse();
        }

    }
}

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
    public class VenueControllerTests : UnitTestBase

    {
        public VenueControllerTests(ConfigFixture fixture) : base(fixture)
        {
        }

        private VenueController Controller => new VenueController(Context);
        
        [Fact]
        public async Task Index_returns_ViewResult()
        {
            var result = await Controller.Index();

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Index_returns_Venues()
        {
            var result = await Controller.Index();

            result.As<ViewResult>().Model.Should().BeOfType<List<Venue>>();
        }

        [Fact]
        public async Task Index_returns_All_Venues_without_uid()
        {
            var result = await Controller.Index();

            result.As<ViewResult>().Model.As<List<Venue>>().Should().HaveCount(2);
        }

        [Fact]
        public async Task Details_returns_Venue()
        {
            var result = await Controller.Details(1);

            result.As<ViewResult>().Model.Should().BeAssignableTo<Venue>();
        }

        [Fact]
        public async Task Details_returns_Correct_Venue()
        {
            var result = await Controller.Details(1);

            result.As<ViewResult>().Model.As<Venue>().Name.Should().Be("Venue1");
        }

        [Fact]
        public async Task Details_returns_Correct_Venue_with_All_Contacts()
        {
            var result = await Controller.Details(1);

            result.As<ViewResult>().Model.As<Venue>().Contact1.Name.Should().Match("Contact1");
            result.As<ViewResult>().Model.As<Venue>().Contact2.Name.Should().Match("Contact2");
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
        public async Task Create_stores_new_Venue()
        {            
            var v = new Venue
            {
                VenueId = 7,
                Name = "Venue7",
                Info = "TBD",
            };
            var contacts = await Context.Contacts.ToListAsync();
            var venueView = new VenueViewModel(contacts) {Venue = v};

            await Controller.Create(venueView);

            Context.Venues.Should().HaveCount(3);
        }

        [Fact]
        public async Task Create_stores_Venue_with_correct_properties()
        {            
            var v = new Venue
            {
                VenueId = 7,
                Name = "Venue7",
                Info = "TBD",
            };
            var venueView = new VenueViewModel {Venue = v, ContactList = new List<SelectListItem>()};

            await Controller.Create(venueView);

            Context.Venues.FirstOrDefault(x => x.VenueId == 7).Should().BeEquivalentTo(v);
        }

        [Fact]
        public async Task Create_stores_Venue_with_correct_Contacts()
        {            
            var v = new Venue
            {
                VenueId = 7,
                Name = "Venue7",
                Info = "TBD",
            };
            var contacts = await Context.Contacts.ToListAsync();
            var venueView = new VenueViewModel(contacts) {Venue = v};
            venueView.Venue.Contact1 = new Contact();
            venueView.Venue.Contact2 = new Contact();
            venueView.Venue.Contact1.ContactId = Int32.Parse(venueView.ContactList[1].Value);
            venueView.Venue.Contact2.ContactId = Int32.Parse(venueView.ContactList[3].Value);

            await Controller.Create(venueView);

            var result = Context.Venues.FirstOrDefault(x => x.VenueId == 7);

            result?.Contact1.Name.Should().Match("Contact3");
            result?.Contact2.Name.Should().Match("Contact1");
        }

        [Fact]
        public async Task Edit_returns_Notfound_if_given_unknown_id()
        {
            var result = await Controller.Edit(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Edit_returns_VenueViewModel()
        {
            var result = await Controller.Edit(1);

            result.As<ViewResult>().Model.Should().BeOfType<VenueViewModel>();
        }

        [Fact]
        public async Task Edit_returns_VenueViewModel_with_ContactList()
        {
            var result = await Controller.Edit(1);

            result.As<ViewResult>().Model.As<VenueViewModel>().ContactList
                .Should().NotBeNullOrEmpty(because: "we seeded the Db with Contacts");
        }

        [Fact]
        public async Task Edit_returns_VenueViewModel_with_correct_Venue()
        {
            var result = await Controller.Edit(1);

            result.As<ViewResult>().Model.As<VenueViewModel>().Venue.Name.Should().Be("Venue1");
        }


        [Fact]
        public async Task Edit_returns_VenueViewModel_with_ALL_Contacts_injected_into_ContactList()
        {
            var result = await Controller.Edit(1);

            result.As<ViewResult>().Model.As<VenueViewModel>().ContactList
                .Should().HaveCount(4).And.Contain(x => x.Text == "Contact1")
                .And.Contain(x => x.Text == "Contact2")
                .And.Contain(x => x.Text == "Contact3");
        }

        [Fact]
        public async Task Edit_updates_Venue_with_correct_properties()
        {            
            var v = Context.Venues.FirstOrDefault(x => x.VenueId == 1);
            v.Name = "Effects";
            v.Info = "TBD";

            var venueView = new VenueViewModel {Venue = v, ContactList = new List<SelectListItem>()};

            await Controller.Edit(1, venueView);

            Context.Venues.FirstOrDefault(x => x.VenueId == 1).Should().BeEquivalentTo(v);
        }

        [Fact]
        public async Task Edit_updates_Venue_with_correct_Contacts()
        {            var v = Context.Venues.FirstOrDefault(x => x.VenueId == 1);
            var contacts = await Context.Contacts.ToListAsync();

            var venueView = new VenueViewModel(v, contacts);
            venueView.Venue.Contact1.ContactId = Int32.Parse(venueView.ContactList[1].Value);

            await Controller.Edit(1, venueView);

            Context.Venues.FirstOrDefault(x => x.VenueId == 1)?.Contact1
                .Name.Should().Match("Contact3");
        }

        [Fact]
        public async Task Edit_returns_NotFound_if_Id_changes()
        {            
            var v = Context.Venues.FirstOrDefault(x => x.VenueId == 1);

            var venueView = new VenueViewModel {Venue = v, ContactList = new List<SelectListItem>()};

            var result = await Controller.Edit(8, venueView);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Edit_returns_View_if_modelstate_not_valid()
        {
            var controller = Controller;
            controller.ViewData.ModelState.AddModelError("key", "some error");
            var venueView = new VenueViewModel
            {
                Venue = new Venue() {VenueId = 1}, ContactList = new List<SelectListItem>()
            };

            var result = await controller.Edit(1, venueView);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Edit_returns_Redirect_to_Index_if_concurrencyException_occurs()
        {            
            var v = await Context.Venues.FirstOrDefaultAsync(x => x.VenueId == 1);
            var contacts = await Context.Contacts.ToListAsync();
            var venueView = new VenueViewModel(v, contacts);

            Context.Remove(v);
            await Context.SaveChangesAsync();
            var result = await Controller.Edit(1, venueView);

            result.As<RedirectToActionResult>().ActionName.Should().Match("Index");
        }

        [Fact]
        public async Task Delete_returns_Correct_Venue()
        {
            var result = await Controller.Delete(1);

            result.As<ViewResult>().Model.As<Venue>().Name.Should().Be("Venue1");
        }

        [Fact]
        public async Task Delete_returns_Notfound_if_given_unknown_id()
        {
            var result = await Controller.Delete(8);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_removes_Venue_from_Db()
        {
            await Controller.DeleteConfirmed(1);

            var result = Context.Venues.FirstOrDefault(x => x.VenueId == 1);

            result.Should().BeNull();
        }

        [Fact]
        public void Validation_Leaving_Name_Null_or_short_causes_modelstate_not_valid()
        {            
            var v = new Venue();
            var v2 = new Venue();
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using VacationRental.Api.Models;
using Xunit;

namespace VacationRental.Api.Tests
{
    [Collection("Integration")]
    public class PostBookingTests
    {
        private readonly HttpClient _client;

        public PostBookingTests(IntegrationFixture fixture)
        {
            _client = fixture.Client;
        }


        [Theory]
        [InlineData(0, 1, 3, 4, 1)]
        [InlineData(2, 1, 3, 6, 1)]
        public async Task GivenCompleteRequest_WhenPostBooking_ThenAGetReturnsTheCreatedBooking(int prepDays, int book1day, int nights1, int book2day, int nights2)
        {
            var postRentalRequest = new RentalBindingModel
            {
                Units = 4,
                PreparationTimeInDays = prepDays
            };

            ResourceIdViewModel postRentalResult;
            using (var postRentalResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", postRentalRequest))
            {
                Assert.True(postRentalResponse.IsSuccessStatusCode);
                postRentalResult = await postRentalResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            var bookings = new List<BookingBindingModel>()
            {
                new BookingBindingModel
                {
                     RentalId = postRentalResult.Id,
                     Nights = nights1,
                     Start = new DateTime(2001, 01, book1day)
                },
                new BookingBindingModel
                {
                    RentalId = postRentalResult.Id,
                    Nights = nights2,
                    Start = new DateTime(2001, 01, book2day)
                }
            };
            foreach(var booking in bookings)
            {
                ResourceIdViewModel postBookingResult;
                using (var postBookingResponse = await _client.PostAsJsonAsync($"/api/v1/bookings", booking))
                {
                    Assert.True(postBookingResponse.IsSuccessStatusCode);
                    postBookingResult = await postBookingResponse.Content.ReadAsAsync<ResourceIdViewModel>();
                }
                using (var getBookingResponse = await _client.GetAsync($"/api/v1/bookings/{postBookingResult.Id}"))
                {
                    Assert.True(getBookingResponse.IsSuccessStatusCode);

                    var getBookingResult = await getBookingResponse.Content.ReadAsAsync<BookingViewModel>();
                    Assert.Equal(booking.RentalId, getBookingResult.RentalId);
                    Assert.Equal(booking.Nights, getBookingResult.Nights);
                    Assert.Equal(booking.Start, getBookingResult.Start);
                }
            }
        }

        [Theory]
        [InlineData(0, 1, 3, 2, 1)]
        [InlineData(2, 1, 3, 5, 1)]
        public async Task GivenCompleteRequest_WhenPostBooking_ThenAPostReturnsErrorWhenThereIsOverbooking(int prepDays, int book1day, int nights1, int book2day, int nights2)
        {
            var postRentalRequest = new RentalBindingModel
            {
                Units = 1,
                PreparationTimeInDays = prepDays
            };

            ResourceIdViewModel postRentalResult;
            using (var postRentalResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", postRentalRequest))
            {
                Assert.True(postRentalResponse.IsSuccessStatusCode);
                postRentalResult = await postRentalResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            var postBooking1Request = new BookingBindingModel
            {
                RentalId = postRentalResult.Id,
                Nights = nights1,
                Start = new DateTime(2002, 01, book1day)
            };

            using (var postBooking1Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking1Request))
            {
                Assert.True(postBooking1Response.IsSuccessStatusCode);
            }

            var postBooking2Request = new BookingBindingModel
            {
                RentalId = postRentalResult.Id,
                Nights = nights2,
                Start = new DateTime(2002, 01, book2day)
            };

            await Assert.ThrowsAsync<ApplicationException>(async () =>
            {
                using (var postBooking2Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking2Request))
                {
                }
            });
        }
    }
}

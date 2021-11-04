using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VacationRental.Api.Models;
using Xunit;

namespace VacationRental.Api.Tests
{
    [Collection("Integration")]
    public class PutRentalTests
    {
        private readonly HttpClient _client;

        public PutRentalTests(IntegrationFixture fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPutRental_ThenAGetReturnsTheUpdatedRental()
        {
            var request = new RentalBindingModel
            {
                Units = 1,
                PreparationTimeInDays = 0
            };
            ResourceIdViewModel postResult;
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", request))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
                postResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            using (var getResponse = await _client.GetAsync($"/api/v1/rentals/{postResult.Id}"))
            {
                Assert.True(getResponse.IsSuccessStatusCode);

                var getResult = await getResponse.Content.ReadAsAsync<RentalViewModel>();
                Assert.Equal(request.Units, getResult.Units);
                Assert.Equal(request.PreparationTimeInDays, getResult.PreparationTimeInDays);
            }

            var bookings = new List<BookingBindingModel>()
            {
                new BookingBindingModel
                {
                     RentalId = postResult.Id,
                     Nights = 3,
                     Start = new DateTime(2001, 01, 01)
                },
                new BookingBindingModel
                {
                    RentalId = postResult.Id,
                    Nights = 2,
                    Start = new DateTime(2001, 01, 06)
                }
            };
            foreach (var booking in bookings)
            {
                ResourceIdViewModel postBookingResult;
                using (var postBookingResponse = await _client.PostAsJsonAsync($"/api/v1/bookings", booking))
                {
                    Assert.True(postBookingResponse.IsSuccessStatusCode);
                    postBookingResult = await postBookingResponse.Content.ReadAsAsync<ResourceIdViewModel>();
                }
            }

            request = new RentalBindingModel
            {
                Units = 3,
                PreparationTimeInDays = 1
            };
            using (var postResponse = await _client.PutAsJsonAsync($"/api/v1/rentals/{postResult.Id}", request))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
                postResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            using (var getResponse = await _client.GetAsync($"/api/v1/rentals/{postResult.Id}"))
            {
                Assert.True(getResponse.IsSuccessStatusCode);

                var getResult = await getResponse.Content.ReadAsAsync<RentalViewModel>();
                Assert.Equal(request.Units, getResult.Units);
                Assert.Equal(request.PreparationTimeInDays, getResult.PreparationTimeInDays);
            }
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPutRental_ThenAGetReturnsExceptionIfOverbooking()
        {
            var request = new RentalBindingModel
            {
                Units = 1,
                PreparationTimeInDays = 0
            };
            ResourceIdViewModel postResult;
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", request))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
                postResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            using (var getResponse = await _client.GetAsync($"/api/v1/rentals/{postResult.Id}"))
            {
                Assert.True(getResponse.IsSuccessStatusCode);

                var getResult = await getResponse.Content.ReadAsAsync<RentalViewModel>();
                Assert.Equal(request.Units, getResult.Units);
                Assert.Equal(request.PreparationTimeInDays, getResult.PreparationTimeInDays);
            }

            var bookings = new List<BookingBindingModel>()
            {
                new BookingBindingModel
                {
                     RentalId = postResult.Id,
                     Nights = 3,
                     Start = new DateTime(2001, 01, 01)
                },
                new BookingBindingModel
                {
                    RentalId = postResult.Id,
                    Nights = 2,
                    Start = new DateTime(2001, 01, 06)
                }
            };
            foreach (var booking in bookings)
            {
                ResourceIdViewModel postBookingResult;
                using (var postBookingResponse = await _client.PostAsJsonAsync($"/api/v1/bookings", booking))
                {
                    Assert.True(postBookingResponse.IsSuccessStatusCode);
                    postBookingResult = await postBookingResponse.Content.ReadAsAsync<ResourceIdViewModel>();
                }
            }

            request = new RentalBindingModel
            {
                Units = 1,
                PreparationTimeInDays = 4
            };

            await Assert.ThrowsAsync<ApplicationException>(async () =>
            {
                using (var postBooking2Response = await _client.PutAsJsonAsync($"/api/v1/rentals/{postResult.Id}", request))
                {
                }
            });
        }
    }
}

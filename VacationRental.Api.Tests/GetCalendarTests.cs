using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using VacationRental.Api.Models;
using Xunit;

namespace VacationRental.Api.Tests
{
    [Collection("Integration")]
    public class GetCalendarTests
    {
        private readonly HttpClient _client;

        public GetCalendarTests(IntegrationFixture fixture)
        {
            _client = fixture.Client;
        }

        [Theory]
        [ClassData(typeof(TestData))]
        public async Task GivenCompleteRequest_WhenGetCalendar_ThenAGetReturnsTheCalculatedCalendar(int preparationDays, int[][] bookingsByDate, int[][] preparationsByDate)
        {
            var postRentalRequest = new RentalBindingModel
            {
                Units = 2,
                PreparationTimeInDays = preparationDays
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
                 Nights = 2,
                 Start = new DateTime(2000, 01, 02)
            };

            ResourceIdViewModel postBooking1Result;
            using (var postBooking1Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking1Request))
            {
                Assert.True(postBooking1Response.IsSuccessStatusCode);
                postBooking1Result = await postBooking1Response.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            var postBooking2Request = new BookingBindingModel
            {
                RentalId = postRentalResult.Id,
                Nights = 2,
                Start = new DateTime(2000, 01, 03)
            };

            ResourceIdViewModel postBooking2Result;
            using (var postBooking2Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking2Request))
            {
                Assert.True(postBooking2Response.IsSuccessStatusCode);
                postBooking2Result = await postBooking2Response.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            using (var getCalendarResponse = await _client.GetAsync($"/api/v1/calendar?rentalId={postRentalResult.Id}&start=2000-01-01&nights=5"))
            {
                Assert.True(getCalendarResponse.IsSuccessStatusCode);

                var getCalendarResult = await getCalendarResponse.Content.ReadAsAsync<CalendarViewModel>();
                
                Assert.Equal(postRentalResult.Id, getCalendarResult.RentalId);
                Assert.Equal(5, getCalendarResult.Dates.Count);

                for (int i = 0; i < 5; i++)
                {
                    Assert.Equal(new DateTime(2000, 01, i+1), getCalendarResult.Dates[i].Date);

                    Assert.Equal(bookingsByDate[i].Length, getCalendarResult.Dates[i].Bookings.Count);
                    var bookedUnits = getCalendarResult.Dates[i].Bookings.Select(b => b.Unit);
                    bookingsByDate[i].ToList().ForEach(unit => Assert.Contains(bookedUnits, bu => bu == unit));

                    Assert.Equal(preparationsByDate[i].Length, getCalendarResult.Dates[i].PreparationTimes.Count);
                    var prepUnits = getCalendarResult.Dates[i].PreparationTimes.Select(p => p.Unit);
                    preparationsByDate[i].ToList().ForEach(unit => Assert.Contains(prepUnits, pu => pu == unit));


                }
            }
        }
    }

    public class TestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { 0,
                new int[][] { new int[0], new int[] { 1 }, new int[] { 1, 2 }, new int[] { 2 }, new int[0] },
                new int[][] { new int[0], new int[0], new int[0], new int[0], new int[0] } };
            yield return new object[] { 2,
                new int[][] { new int[0], new int[] { 1 }, new int[] { 1, 2 }, new int[] { 2 }, new int[0] },
                new int[][] { new int[0], new int[0], new int[0], new int[] { 1 }, new int[] { 1, 2 } } };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }
}

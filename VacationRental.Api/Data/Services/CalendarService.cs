using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationRental.Api.Data.Contracts;
using VacationRental.Api.Models;

namespace VacationRental.Api.Data.Services
{
    public class CalendarService : ICalendarService
    {
        private readonly IDataRepository<RentalViewModel> _rentalRepository;
        private readonly IDataRepository<BookingViewModel> _bookingRepository;

        public CalendarService(
            IDataRepository<RentalViewModel> rentalRepository,
            IDataRepository<BookingViewModel> bookingRepository)
        {
            _rentalRepository = rentalRepository ?? throw new ArgumentNullException(nameof(rentalRepository));
            _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
        }
        public CalendarViewModel Get(int rentalId, DateTime start, int nights)
        {
            if (nights < 0)
                throw new ApplicationException("Nights must be positive");

            var rental = _rentalRepository.Get(rentalId);
            if (rental == null)
                throw new ApplicationException("Rental not found");

            var result = new CalendarViewModel
            {
                RentalId = rentalId,
                Dates = new List<CalendarDateViewModel>()
            };

            var bookings = _bookingRepository.Get();

            for (var i = 0; i < nights; i++)
            {
                var date = new CalendarDateViewModel
                {
                    Date = start.Date.AddDays(i),
                    Bookings = new List<CalendarBookingViewModel>(),
                    PreparationTimes = new List<CalendarPreparationViewModel>()
                };

                foreach (var booking in bookings)
                {
                    if (booking.RentalId == rentalId)
                    {
                        var bookingEnd = booking.Start.AddDays(booking.Nights);
                        var preparationEnd = booking.Start.AddDays(booking.Nights + rental.PreparationTimeInDays);

                        if (date.Date >= booking.Start && date.Date < bookingEnd )
                            date.Bookings.Add(new CalendarBookingViewModel { Id = booking.Id, Unit = booking.Unit });
                        else
                            if (date.Date >= bookingEnd  && date.Date < preparationEnd)
                            date.PreparationTimes.Add(new CalendarPreparationViewModel { Unit = booking.Unit });
                    }
                }
                result.Dates.Add(date);
            }

            return result;
        }
    }
}

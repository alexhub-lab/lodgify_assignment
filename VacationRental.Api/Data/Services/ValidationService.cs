using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationRental.Api.Data.Contracts;
using VacationRental.Api.Models;

namespace VacationRental.Api.Data.Services
{
    public class ValidationService : IValidationService
    {
        private readonly IDataRepository<RentalViewModel> _rentalsRepository;
        private readonly IDataRepository<BookingViewModel> _bookingRepository;

        public ValidationService(
            IDataRepository<RentalViewModel> rentalsRepository,
            IDataRepository<BookingViewModel> bookingRepository)
        {
            _rentalsRepository = rentalsRepository ?? throw new ArgumentNullException(nameof(rentalsRepository));
            _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
        }

        public BookingViewModel BuildModel(BookingBindingModel bindingModel)
        {
            if (bindingModel.Nights <= 0)
                throw new ApplicationException("Nights must be positive");

            var rental = _rentalsRepository.Get(bindingModel.RentalId);
            if (rental == null)
                throw new ApplicationException("Rental not found");

            var count = 0;
            foreach (var book in _bookingRepository.Get())
            {
                var availableOn = book.Start.AddDays(book.Nights + rental.PreparationTimeInDays);
                var requiredUntil = bindingModel.Start.AddDays(bindingModel.Nights);

                if (book.RentalId == bindingModel.RentalId
                    && bindingModel.Start.Date < availableOn
                    && requiredUntil >= book.Start)
                {
                    count++;
                }
            }
            if (count >= rental.Units)
                throw new ApplicationException("Not available");

            var booking = new BookingViewModel()
            {
                Id = _bookingRepository.GenerateId(),
                Nights = bindingModel.Nights,
                RentalId = bindingModel.RentalId,
                Unit = count + 1,
                Start = bindingModel.Start.Date
            };

            return booking;
        }

        public RentalViewModel BuildModel(int id, RentalBindingModel bindingModel)
        {
            var oldRental = _rentalsRepository.Get(id);
            if (oldRental == null)
                throw new ApplicationException("Rental not found");

            var rental = new RentalViewModel()
            {
                Id = id,
                Units = bindingModel.Units,
                PreparationTimeInDays = bindingModel.PreparationTimeInDays
            };

            if (rental.Units >= oldRental.Units && rental.PreparationTimeInDays <= oldRental.PreparationTimeInDays)
                return rental;

            var bookings = _bookingRepository.Get()
                                    .Where(b=>b.RentalId == id)
                                    .OrderBy(b=>b.Start)
                                    .ToList();
            var bookingsCount = bookings.Count();
            for (int i = 0; i < bookingsCount; i++)
            {
                int unitsOverlap = 1;
                for (int j = i + 1; j < bookingsCount; j++)
                    if (bookings[i].Start.AddDays(bookings[i].Nights + bindingModel.PreparationTimeInDays) > bookings[j].Start)
                        unitsOverlap++;

                if(unitsOverlap > bindingModel.Units)
                    throw new ApplicationException("Cannot proceed with Rental update");
            }

            return rental;
        }
        public RentalViewModel BuildModel(RentalBindingModel bindingModel)
        {
            var rental = new RentalViewModel()
            {
                Id = _rentalsRepository.GenerateId(),
                Units = bindingModel.Units,
                PreparationTimeInDays = bindingModel.PreparationTimeInDays
            };

            return rental;
        }
    }
}

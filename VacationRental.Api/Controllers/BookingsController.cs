using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Data.Contracts;
using VacationRental.Api.Models;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/bookings")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IDataRepository<BookingViewModel> _bookingRepository;
        private readonly IValidationService _validationService;

        public BookingsController(IDataRepository<BookingViewModel> bookingRepository, IValidationService validationService)
        {
            _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
        }

        [HttpGet]
        [Route("{bookingId:int}")]
        public BookingViewModel Get(int bookingId)
        {
            var booking = _bookingRepository.Get(bookingId);
            if( booking == null)
                throw new ApplicationException("Booking not found");

            return booking;
        }

        [HttpPost]
        public ResourceIdViewModel Post(BookingBindingModel model)
        {
            var booking = _validationService.BuildModel(model);
            _bookingRepository.Save(booking);
            return new ResourceIdViewModel { Id = booking.Id }; ;
        }
    }
}

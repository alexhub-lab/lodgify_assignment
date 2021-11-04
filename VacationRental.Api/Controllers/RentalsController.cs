using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Data.Contracts;
using VacationRental.Api.Models;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/rentals")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly IDataRepository<RentalViewModel> _rentalRepository;
        private readonly IValidationService _validationService;

        public RentalsController(IDataRepository<RentalViewModel> rentalRepository, IValidationService validationService)
        {
            _rentalRepository = rentalRepository ?? throw new ArgumentNullException(nameof(rentalRepository));
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
        }

        [HttpGet]
        [Route("{rentalId:int}")]
        public RentalViewModel Get(int rentalId)
        {
            var rental = _rentalRepository.Get(rentalId);
            if (rental == null)
                throw new ApplicationException("Rental not found");

            return rental;
        }

        [HttpPost]
        public ResourceIdViewModel Post(RentalBindingModel model)
        {
            var rental = _validationService.BuildModel(model);
            _rentalRepository.Save(rental);
            return new ResourceIdViewModel { Id = rental.Id }; ;
        }

        [HttpPut]
        [Route("{rentalId:int}")]
        public ResourceIdViewModel Put(int rentalId, RentalBindingModel model)
        {
            var rental = _validationService.BuildModel(rentalId, model);
            _rentalRepository.Save(rental);
            return new ResourceIdViewModel { Id = rental.Id }; ;
        }
    }
}

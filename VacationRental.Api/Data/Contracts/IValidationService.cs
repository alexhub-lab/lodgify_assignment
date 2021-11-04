using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationRental.Api.Models;

namespace VacationRental.Api.Data.Contracts
{
    public interface IValidationService
    {
        BookingViewModel BuildModel(BookingBindingModel bindingModel);
        RentalViewModel BuildModel(RentalBindingModel bindingModel);
        RentalViewModel BuildModel(int id, RentalBindingModel bindingModel);
    }
}

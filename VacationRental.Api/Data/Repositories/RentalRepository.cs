using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationRental.Api.Data.Contracts;
using VacationRental.Api.Models;

namespace VacationRental.Api.Data.Repositories
{
    public class RentalRepository :  RepositoryBase<RentalViewModel>
    {
        public RentalRepository(IDictionary<int, RentalViewModel> rentals)
            :base(rentals)
        {
        }
    }
}

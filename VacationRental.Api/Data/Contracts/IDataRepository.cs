using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationRental.Api.Models;

namespace VacationRental.Api.Data.Contracts
{

    public interface  IDataRepository<IViewModel>
    {
        IEnumerable<IViewModel> Get();
        IViewModel Get(int id);
        void Save(IViewModel model);
        int GenerateId();
    }


}

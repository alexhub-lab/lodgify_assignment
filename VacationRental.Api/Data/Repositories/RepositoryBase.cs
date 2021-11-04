using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationRental.Api.Data.Contracts;
using VacationRental.Api.Models;

namespace VacationRental.Api.Data.Repositories
{
    public abstract class RepositoryBase<IViewModel>: IDataRepository<IViewModel>
                where IViewModel : class, IIdentityViewModel
    {
        private readonly IDictionary<int, IViewModel> _models;

        public RepositoryBase(IDictionary<int, IViewModel> models)
        {
            _models = models ?? throw new ArgumentNullException(nameof(models));
        }

        public IEnumerable<IViewModel> Get()
        {
            return _models.Values;
        }
        public IViewModel Get(int id)
        {
            if (!_models.ContainsKey(id))
                return null;
            return _models[id];
        }
        public void Save(IViewModel model)
        {
            if (_models.ContainsKey(model.Id))
                _models[model.Id] = model;
            else
                _models.Add(model.Id, model);
        }
        public int GenerateId()
        {
            return _models.Keys.Count + 1;
        }
    }
}

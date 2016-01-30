using System;
using System.Collections.Generic;

namespace Dominio.Core
{
    public interface IRepository<TEntity> : IDisposable
        where TEntity : Entity
    {
        IUnitOfWork UnitOfWork { get; }

        IEnumerable<TEntity> GetAll();
    }
}

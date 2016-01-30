using Dominio.Core;
using System.Data.Entity;

namespace Infraestructura.Core
{
    public interface IQueryableUnitOfWork : IUnitOfWork, ISql
    {
        DbSet<TEntity> CreateSet<TEntity>() where TEntity : class;
    }
}

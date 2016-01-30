using Dominio.Core;

namespace Dominio.BoundedContext.Genericos
{
    public interface IIngenieriaRepository<TEntity> : IRepository<TEntity>
        where TEntity : Entity
    {
    }
}

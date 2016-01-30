using Dominio.BoundedContext.Genericos;
using Dominio.Core;
using infraestructura.BoundedContext.Ingenieria.UnitOfWork;
using Infraestructura.Core;

namespace infraestructura.BoundedContext.Genericos
{
    public class IngenieriaRepository<TEntity>:Repository<TEntity>, IIngenieriaRepository<TEntity>
        where TEntity : Entity
    {
        public IngenieriaRepository(IngenieriaBCUnitOfWork unitOfWork)
            :base(unitOfWork)
        {

        }
    }
}

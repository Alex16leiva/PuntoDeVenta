using Dominio.Core;
using System.Data.Entity;
using System.Collections.Generic;
using System;

namespace Infraestructura.Core
{
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : Entity
    {
        
        private readonly IQueryableUnitOfWork _unitOfWork;

        public Repository(IQueryableUnitOfWork unitOfWork)
        {
            if (unitOfWork == null) throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
        }

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _unitOfWork;
            }
        }
        #region Constructor

        #endregion
        public void Dispose()
        {
                
        }
        #region IRepository
        public IEnumerable<TEntity> GetAll()
        {
            return GetSet();
        }
        #endregion

        #region Metodos Privados
        IDbSet<TEntity> GetSet()
        {
            return _unitOfWork.CreateSet<TEntity>();
        }
        #endregion
    }
}

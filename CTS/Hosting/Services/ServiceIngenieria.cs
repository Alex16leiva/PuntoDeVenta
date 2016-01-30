using Aplicacion.BoundedContext.Ingenieria.PlantaServices;
using ServiceStack.ServiceHost;
using System;

namespace Hosting.Services
{
    public class ServiceIngenieria : IService
    {
        #region MiembrosPrivados
        private readonly IPlantaAppService _plantaAppService;
        #endregion

        #region Contructor
        public ServiceIngenieria(IPlantaAppService plantaAppService)
        {
            if (plantaAppService == null) throw new ArgumentNullException("plantaAppService");
            _plantaAppService = plantaAppService;
        }
        #endregion

        #region IPlantaAppService
        
        #endregion
    }
}

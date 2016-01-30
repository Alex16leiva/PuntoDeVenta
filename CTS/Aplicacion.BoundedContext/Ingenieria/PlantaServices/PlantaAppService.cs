using Aplicacion.Core;
using AplicacionDTO.DTOs.Ingenieria.PlantaDtos;
using Dominio.BoundedContext.Genericos;
using Dominio.BoundedContext.Ingenieria.Aggregates.PlantaAgg;
using Framework.Core;
using System;
using System.Collections.Generic;

namespace Aplicacion.BoundedContext.Ingenieria.PlantaServices
{
    public class PlantaAppService: BaseDisposable, IPlantaAppService
    {
        #region Miembros Privados
        private readonly IIngenieriaRepository<Planta> _plantaRepository;
        #endregion
         
        #region Constructor
        public PlantaAppService(IIngenieriaRepository<Planta> plantaAppService)
        {
            if (plantaAppService == null) throw new ArgumentNullException("plantaAppService");
            _plantaRepository = plantaAppService;
        }
        #endregion

        #region IPlantaAppService

        public List<PlantaDTO> ObtenerPlantas()
        {
            var listaPlantas = _plantaRepository.GetAll();
            return listaPlantas.ProjectedAsCollection<PlantaDTO>();
        }
                

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (_plantaRepository != null) _plantaRepository.Dispose();
            base.Dispose(disposing);
        }
    }
}

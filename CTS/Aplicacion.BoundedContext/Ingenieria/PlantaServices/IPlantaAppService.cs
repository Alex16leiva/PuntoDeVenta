using AplicacionDTO.DTOs.Ingenieria.PlantaDtos;
using System;
using System.Collections.Generic;

namespace Aplicacion.BoundedContext.Ingenieria.PlantaServices
{
    public interface IPlantaAppService : IDisposable
    {
        List<PlantaDTO> ObtenerPlantas();
    }
}

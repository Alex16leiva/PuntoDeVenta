using AplicacionDTO.DTOs.Ingenieria.PlantaDtos;
using AutoMapper;
using Dominio.BoundedContext.Ingenieria.Aggregates.PlantaAgg;

namespace AplicacionDTO.Profiles
{
    public class IngenieriaProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<Planta, PlantaDTO>();
         
            base.Configure();
        }
    }
}

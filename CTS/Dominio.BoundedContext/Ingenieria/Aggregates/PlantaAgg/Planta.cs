using Dominio.Core;

namespace Dominio.BoundedContext.Ingenieria.Aggregates.PlantaAgg
{
    public class Planta : Entity
    {
        public string PlantaId { get; set; }
        public string NombrePlanta { get; set; }
        public string Descripcion { get; set; }
    }
}

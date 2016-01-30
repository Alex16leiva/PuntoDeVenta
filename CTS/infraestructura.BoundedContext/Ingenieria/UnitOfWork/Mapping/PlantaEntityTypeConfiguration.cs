using Dominio.BoundedContext.Ingenieria.Aggregates.PlantaAgg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infraestructura.BoundedContext.Ingenieria.UnitOfWork.Mapping
{
    internal class PlantaEntityTypeConfiguration : EntityConfiguration<Planta>
    {
        public PlantaEntityTypeConfiguration()
        {
            HasKey(t => t.PlantaId);
            ToTable("Planta", "Comunes");
            Property(t => t.PlantaId).HasColumnName("PlantaId").IsRequired().IsUnicode(false).HasMaxLength(10);
            Property(t => t.NombrePlanta).HasColumnName("NombrePlanta").IsRequired().IsUnicode(false).HasMaxLength(50);
            Property(t => t.Descripcion).HasColumnName("Descripcion").IsRequired().IsUnicode(false).HasMaxLength(50);
        }
    }
}

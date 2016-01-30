using Dominio.Core;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace infraestructura.BoundedContext
{
    public class EntityConfiguration<TEntity> : EntityTypeConfiguration<TEntity>
        where TEntity : Entity
    {
        public EntityConfiguration()
        {
            Property(t => t.FechaTransaccion).HasColumnName("FechaTransaccion");
            Property(t => t.DescripcionTransaccion).HasColumnName("DescripcionTransaccion").IsRequired().
                IsUnicode(false).HasMaxLength(50);
            Property(t => t.ModificadoPor).HasColumnName("ModificadoPor").IsRequired().IsUnicode(false).
                HasMaxLength(20);
            Property(t => t.RowVersion).HasColumnName("RowVersion").
                HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired().
                IsConcurrencyToken().IsFixedLength().HasMaxLength(8);
            Property(t => t.TransaccionUId).HasColumnName("TransaccionUId");
            Property(t => t.TipoTransaccion).HasColumnName("TipoTransaccion").IsUnicode(false).
                HasMaxLength(50);
            Property(t => t.FechaTransaccionUtc).HasColumnName("FechaTransaccionUtc");
        }
    }
}

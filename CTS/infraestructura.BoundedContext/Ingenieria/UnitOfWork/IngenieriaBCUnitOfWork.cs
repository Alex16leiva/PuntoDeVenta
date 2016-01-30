using Dominio.BoundedContext.Ingenieria.Aggregates.PlantaAgg;
using infraestructura.BoundedContext.Ingenieria.UnitOfWork.Mapping;
using Infraestructura.Core;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace infraestructura.BoundedContext.Ingenieria.UnitOfWork
{
    public class IngenieriaBCUnitOfWork:BCUnitOfWork,IQueryableUnitOfWork
    {
        public IngenieriaBCUnitOfWork()
            :base("connectionString")
        {
            Database.SetInitializer<IngenieriaBCUnitOfWork>(null);
        }

        public IDbSet<Planta> Planta{ get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Remove unused conventions
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Configurations.Add(new PlantaEntityTypeConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}

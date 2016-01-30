using Microsoft.Practices.Unity;
using Dominio.BoundedContext.Genericos;
using infraestructura.BoundedContext.Genericos;
using infraestructura.BoundedContext.Ingenieria.UnitOfWork;
using Aplicacion.BoundedContext.Ingenieria.PlantaServices;
using System;

namespace ServerIntance.ContainerRegistrations
{
    public class IngenieriaContainer
    {
        internal static void Configure(IUnityContainer currentContainer)
        {
            RegisterUnitOfWork(currentContainer);

            RegisterRepositories(currentContainer);

            RegisterAppService(currentContainer);

            RegisterDominioService(currentContainer);
        }

        private static void RegisterUnitOfWork(IUnityContainer currentContainer)
        {
            currentContainer.RegisterType(typeof(IngenieriaBCUnitOfWork), new PerResolveLifetimeManager());
        }

        private static void RegisterRepositories(IUnityContainer currentContainer)
        {
            currentContainer.RegisterType(typeof(IIngenieriaRepository<>), typeof(IngenieriaRepository<>));
        }

        private static void RegisterAppService(IUnityContainer currentContainer)
        {
            currentContainer.RegisterType<IPlantaAppService, PlantaAppService>();
        }
        
        private static void RegisterDominioService(IUnityContainer currentContainer)
        {
         
        }
    }
}

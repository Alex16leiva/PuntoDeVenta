using CrossCutting.Adapters;
using CrossCutting.Identity;
using CrossCutting.Logging;
using CrossCutting.Network.Adapter;
using CrossCutting.Network.Identity;
using CrossCutting.Network.Logging;
using CrossCutting.Network.Validator;
using CrossCutting.Validator;
using Microsoft.Practices.Unity;
using ServerIntance.ContainerRegistrations;

namespace ServerIntance
{
    public static class Container
    {
        static IUnityContainer _currentContainer;
        public static IUnityContainer Current
        {
            get
            {
                return _currentContainer;
            }
        }

        static Container()
        {
            ConfigureContainerModulos();
            ConfigureFactories();
        }

        private static void ConfigureContainerModulos()
        {
            _currentContainer = new UnityContainer();
            ConfigureContainer();
            RegisterDependency();
        }

        private static void ConfigureContainer()
        {
            //-> Adapters
            _currentContainer.RegisterType<ITypeAdapterFactory, AutomapperTypeAdapterFactory>
                (new ContainerControlledLifetimeManager()); 

            //->Identity Generation
            _currentContainer.RegisterType<IIdentityFactory, ADOIdentityGeneratorFactory>
                (new ContainerControlledLifetimeManager());
        }

        private static void RegisterDependency()
        {
            IngenieriaContainer.Configure(_currentContainer);
        }

        private static void ConfigureFactories()
        {
            LoggerFactory.SetCurrent(new TraceSourceLogFactory());
            EntityValidatorFactory.SetCurrent(new DataAnnotationsEntityValidatorFactory());

            var typeAdapterFactory = _currentContainer.Resolve<ITypeAdapterFactory>();
            TypeAdapterFactory.SetCurrent(typeAdapterFactory);

            var identityGeneratorFactory = _currentContainer.Resolve<IIdentityFactory>();
            IdentityFactory.SetCurrent(identityGeneratorFactory);
        }
    }
}

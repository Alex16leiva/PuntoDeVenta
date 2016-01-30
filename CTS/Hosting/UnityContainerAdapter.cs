using Microsoft.Practices.Unity;
using ServiceStack.Configuration;

namespace Hosting
{
    public class UnityContainerAdapter : IContainerAdapter
    {
        private readonly IUnityContainer _unityContainer;

        public UnityContainerAdapter(IUnityContainer container)
        {
            _unityContainer = container;
        }

        public T Resolve<T>()
        {
            return _unityContainer.Resolve<T>();
        }

        public T TryResolve<T>()
        {
            if (_unityContainer.IsRegistered(typeof(T)))
            {
                return (T)_unityContainer.Resolve(typeof(T));
            }

            return default(T);
        }
    }
}

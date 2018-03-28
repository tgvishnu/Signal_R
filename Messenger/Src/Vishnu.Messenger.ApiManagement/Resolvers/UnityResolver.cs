using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Dependencies;
using Unity;
using Unity.Exceptions;

namespace Vishnu.Messenger.ApiManagement.Resolvers
{
    public class UnityResolver : IDependencyResolver
    {
        protected IUnityContainer container;

        public UnityResolver(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            this.container = container;
        }

        public IDependencyScope BeginScope()
        {
            // for every call, a new instace of controller is initialized.
            // WEB api will call BeginScope for ever call then call Dispose method for clean up.
            var child = container.CreateChildContainer();
            return new UnityResolver(child);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public object GetService(Type serviceType)
        {
            try
            {
                return container.Resolve(serviceType);
            }
            catch(ResolutionFailedException)
            {
                // should not return exception.  
                // WEB API will create instance, if Unity cannot resolve the serviceType.
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return container.ResolveAll(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return new List<object>(); 
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            container.Dispose();
        }
    }
}
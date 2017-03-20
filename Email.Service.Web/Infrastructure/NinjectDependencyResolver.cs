using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using Ninject;

namespace Email.Service.Web.Infrastructure
{
    public class NinjectDependencyResolver : IDependencyResolver, System.Web.Mvc.IDependencyResolver
    {
        private IKernel _kernel;

        public NinjectDependencyResolver():this (new StandardKernel()) {}

        public NinjectDependencyResolver(IKernel kernel)
        {
            _kernel = kernel;
            AddBindings();
        }

        public IDependencyScope BeginScope()
        {
            return this;
        }

        public void Dispose()
        {
        }

        public object GetService(Type serviceType)
        {
            return _kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _kernel.GetAll(serviceType);
        }
        /// <summary>
        /// Сдесь размещаем привязки интерфейсов к реализации.
        /// </summary>
        private void AddBindings()
        {

        }
    }
}
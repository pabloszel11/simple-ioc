using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoCEngine
{
    public delegate SimpleContainer ContainerProviderDelegate();

    public interface IService { };


    public class ServiceLocator : IService
    {
        private static ServiceLocator instance;
        private static SimpleContainer container;
        public static void SetContainerProvider(ContainerProviderDelegate ContainerProvider)
        {
            container = ContainerProvider.Invoke();
        }

        public static ServiceLocator Current
        {
            get
            {
                if (instance == null)
                    instance = new ServiceLocator();
                return instance;
            }
        }

        public T GetInstance<T>() where T: class
        {
            return container as T;
        }
    }
}

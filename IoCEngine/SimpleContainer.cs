using IoCEngine.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IoCEngine
{
    public class SimpleContainer
    {
        Dictionary<Type, IObjectType> RegisteredTypes;

        public SimpleContainer()
        {
            RegisteredTypes = new Dictionary<Type, IObjectType>();
        }
        public void RegisterType<T>(bool Singleton) where T : class
        {
            var type = typeof(T);

            if (Singleton)
                this.RegisteredTypes.Add(type, new Singleton(type.GetConstructor(new Type[] { })));
            else
                this.RegisteredTypes.Add(type, new MyObject(type.GetConstructor(new Type[] { })));

        }
        public void RegisterType<From, To>(bool Singleton) where To : From
        {
            var typeFrom = typeof(From);
            var typeTo = typeof(To);

            if (Singleton)
                this.RegisteredTypes.Add(typeFrom, new Singleton(typeTo.GetConstructor(new Type[] { })));
            else
                this.RegisteredTypes.Add(typeFrom, new MyObject(typeTo.GetConstructor(new Type[] { })));
        }

        public void BuildUp<T>(T t)
        {
            MyObject.SetDependency(t);
        }

        public T Resolve<T>()
        {
            return (T)ResolveObject(typeof(T));
        }

        public object ResolveObject(Type type)
        {
            IObjectType value;
            if (RegisteredTypes.TryGetValue(type, out value))
            {
                if (value.GetType() == typeof(Singleton))
                    return value.GetInstance();
                else
                    return GetInstanceInv(type);
            }
            else
            {
                value = new MyObject(type.GetConstructor(new Type[] { }));
                this.RegisteredTypes.Add(type, value);
                return GetInstanceInv(type);
            }
        }




        public object GetInstanceInv(Type type)
        {
            int len = 0;
            ConstructorInfo con = null;            

            foreach (var constructor in type.GetConstructors())
            {
                try
                {
                    if (constructor.GetCustomAttribute<DependencyConstructor>() != null)
                    {
                        con = constructor;
                        break;
                    }
                    
                    if (len < constructor.GetParameters().Length)
                    {
                        con = constructor;
                        len = con.GetParameters().Length;
                    }

                }
                catch(Exception ex)
                {
                    continue;
                }
                
            }

            Type[] parameters;
            if (con != null)
                parameters = con.GetParameters().Select(pi => pi.ParameterType).ToArray();
            else
                parameters = Array.Empty<Type>();

            foreach (var param in parameters)
            {
                if (param == type)
                {
                    throw new CycleException();
                }
            }
                
            var instance = Activator.CreateInstance(type, parameters.Select(pi => ResolveObject(pi)).ToArray());
            MyObject.SetDependency(instance);
           
            if (instance == null)
            {
                throw new MethodMissingException();
            }

            return instance;

        }



        public void RegisterInstance<T>(T instance)
        {
            var type = typeof(T);
            this.RegisteredTypes.Add(type, new Singleton(type.GetConstructor(new Type[] { }), instance));
        }

        public abstract class IObjectType
        {
            protected ConstructorInfo constructorInfo;
            protected static object instance;
            public abstract object GetInstance();
        }


        public class Singleton : IObjectType
        {


            public Singleton(ConstructorInfo ci)
            {
                constructorInfo = ci;
            }
            public Singleton(ConstructorInfo ci, object newInstance)
            {
                constructorInfo = ci;
                instance = newInstance;
            }


            public override object GetInstance()
            {
                if (instance == null)
                {
                    instance = constructorInfo.Invoke(new object[] { });
                }
                
                return instance;
            }

           
        }

        public class MyObject : IObjectType
        {

            public MyObject(ConstructorInfo ci)
            {
                constructorInfo = ci;
            }



            public override object GetInstance()
            {
                instance = constructorInfo.Invoke(new object[] { });
                SetDependency(instance);
                return instance;
            }
            public static void SetDependency(object i)
            {
                foreach (var m in i.GetType().GetMethods())
                {
                    if (m.GetCustomAttribute<DependencyMethod>() != null)
                    {
                        
                        var parameters = m.GetParameters().Select(pi => pi.ParameterType).ToArray();
                        List<object> instances = new List<object>();
                        foreach (var p in parameters)
                        {
                            var r = Activator.CreateInstance(p, new object[] { });
                            instances.Add(r);
                        }
                        m.Invoke(i, instances.ToArray());
                    }
                }
            }




        }



    }
}

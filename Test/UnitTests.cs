using System;
using Xunit;
using IoCEngine;
using Xunit.Abstractions;

namespace Test
{   

    public class UnitTests
    {
        abstract class IFoo { }
        class Foo : IFoo { }
        class Bar { }

        public class A
        {
            public B b;
            public D d;
            public A()
            {

            }

            public A(B b)
            {
                this.b = b;
            }

            [DependencyMethod]
            public void SetD(D d)
            {
                this.d = d;
            }
        }
        public class B { }
        public class D { }

        public interface ICalc
        {
            int Sum(int p, int q);
        }

        public class CalcImpl : ICalc
        {
            public int Sum(int p, int q)
            {
                return p + q;
            }
        }

        static int DoWork()
        {
            var calc = ServiceLocator.Current.GetInstance<SimpleContainer>().Resolve<ICalc>();
            return calc.Sum(5, 6);
        }
        static void CompositionRoot()
        {
            LoggerFactory.SetProvider(() => new ConsoleLogger());
        }

        private readonly ITestOutputHelper output;

        [Fact]
        public void TestRegisterInstance()
        {

            SimpleContainer container1 = new SimpleContainer();

            IFoo foo = new Foo();
            container1.RegisterInstance<IFoo>(foo);
            IFoo foo2 = container1.Resolve<IFoo>();
            Assert.Equal(foo, foo2);
        }

        [Fact]
        public void TestInv1()
        {
            SimpleContainer container2 = new SimpleContainer();
            container2.RegisterType<A>(false);
            A a = container2.Resolve<A>();
            Assert.NotNull(a.b);
        }
        [Fact]
        public void TestDependencyMethod()
        {
            SimpleContainer c = new SimpleContainer();
            A a = c.Resolve<A>();
            Assert.NotNull(a.b);
            Assert.NotNull(a.d);
        }
        [Fact]
        public void TestBuildUp()
        {
            SimpleContainer c = new SimpleContainer();
            A a = new A();
            c.BuildUp<A>(a);
            Assert.NotNull(a.d);
        }
        [Fact]
        public void TestServiceSingleton()
        {
            IService s1 = ServiceLocator.Current;
            IService s2 = ServiceLocator.Current;
            Assert.Equal(s1, s2);
        }
        [Fact]
        public void TestServiceSimpleContainer()
        {
            SimpleContainer c = new SimpleContainer();
            IFoo foo = new Foo();
            c.RegisterType<ICalc, CalcImpl>(true);
            ContainerProviderDelegate containerProvider = () => c;
            ServiceLocator.SetContainerProvider(containerProvider);
            Assert.Equal(11, DoWork());
        }
        [Fact]
        public void TestLocalFactory()
        {
            CompositionRoot();
            var client = new LoggerClient();
            string s = client.Execute("abcd");
            Assert.Equal(s,
                        "Log from ConsoleLogger on " + DateTime.Now + ": abcd");
        }
        


    }
}

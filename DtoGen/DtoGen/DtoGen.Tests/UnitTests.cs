using System;
using System.Collections.Generic;
using NUnit.Framework;
using ServiceStack;
using ServiceStack.Testing;
using DtoGen.ServiceModel;
using DtoGen.ServiceInterface;
using ServiceStack.Text;

namespace DtoGen.Tests
{
    [TestFixture]
    public class UnitTests
    {
        private readonly ServiceStackHost appHost;

        public UnitTests()
        {
            appHost = new BasicAppHost(typeof(MyServices).Assembly)
            {
                ConfigureContainer = container =>
                {
                    //Add your IoC dependencies here
                }
            }
            .Init();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            appHost.Dispose();
        }

        [Test]
        public void TestMethod1()
        {
            var service = appHost.Container.Resolve<MyServices>();

            var response = (HelloResponse)service.Any(new Hello { Name = "World" });

            Assert.That(response.Result, Is.EqualTo("Hello, World!"));
            var array = JsonSerializer.DeserializeFromString<List<object>>("[\"repo\", \"user\"]");
            Assert.That(array[0],Is.EqualTo("repo"));
            Assert.That(array[1],Is.EqualTo("user"));
        }
    }
}

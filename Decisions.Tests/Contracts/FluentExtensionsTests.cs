using System.Threading.Tasks;
using Decisions.Contracts;
using Decisions.Example.Support;
using MbUnit.Framework;

namespace Decisions.Tests.Contracts
{
    [TestFixture]
    class FluentExtensionsTests
    {
        [SetUp]
        void SetUp()
        {
            Injector.Resolver = new TestResolver();
        }

        [TearDown]
        void TearDown()
        {
            Injector.Resolver = null;
        }

        [AsyncTest]
        async Task Create_Applies_Context()
        {
            var context = DecisionContext.Create(a => a.Using("Example").As("User").Has("A").On(new { @id = 1 }));
            Assert.AreEqual("Example", context.Namespace);
            Assert.AreEqual("User", context.SourceId);
            Assert.AreEqual("A", context.Role);
            Assert.AreEqual(1, context.Target.id);

            var result = await context.Check();
            Assert.IsTrue(result);
        }
    }
}
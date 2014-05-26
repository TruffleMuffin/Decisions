using System.Threading.Tasks;
using Decisions.Contracts;
using Decisions.Contracts.IoC;
using Decisions.Example.Support;
using MbUnit.Framework;

namespace Decisions.Tests.Contracts
{
    [TestFixture]
    class DecisionContextTests
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

        [Test]
        void Create_Applies_Context()
        {
            var result = DecisionContext.Create(a => a.Using("Namespace").As("User").Has("Role").On(new { @id = "Entity" }));
            Assert.AreEqual("Namespace", result.Namespace);
            Assert.AreEqual("User", result.SourceId);
            Assert.AreEqual("Role", result.Role);
            Assert.AreEqual("Entity", result.Target.id);
        }

        [AsyncTest]
        async Task Check_AppliesAndExecutes_Correct()
        {
            var result = await DecisionContext.Check(a => a.Using("Example").As("User").Has("A").On(new { @id = 1 }));
            Assert.IsTrue(result);
        }
    }
}

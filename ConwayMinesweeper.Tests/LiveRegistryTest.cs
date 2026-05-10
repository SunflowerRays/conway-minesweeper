using NUnit.Framework;

namespace ConwayMinesweeper.Tests
{
    [TestFixture]
    public class LiveRegistryTests
    {
        private LiveRegistry liveRegistry;

        [SetUp]
        public void Init()
        {
            liveRegistry = new LiveRegistry();
        }

        [Test]
        public void AliveCells_IsEmptyOnInitialisation()
        {
            Assert.That(liveRegistry.aliveCells.Count, Is.EqualTo(0));
        }

        [Test]
        public void Population_UpdatesWhenCellsAreAdded()
        {
            liveRegistry.aliveCells.Add((0, 0));
            liveRegistry.aliveCells.Add((1, 1));
            liveRegistry.population = liveRegistry.aliveCells.Count;
            Assert.That(liveRegistry.population, Is.EqualTo(2));
        }
    }
}
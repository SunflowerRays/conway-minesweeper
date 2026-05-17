using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics.Metrics;

namespace ConwayMinesweeper.Tests
{

    [TestFixture]
    public class GeneratorTests
    {

        private LiveRegistry liveRegistry;
        private Grid grid;
        private Generator generator;

        [SetUp]
        public void Init()
        {
            liveRegistry = new LiveRegistry();
            grid = new Grid((0, 0), 12, 12);
            generator = new Generator(grid, liveRegistry);
        }

        [Test]
        public void UpdateState_DeadCellWithThreeNeighbours_Lives()
        {
            // Arrange
            // Three live cells around (0, 0)
            liveRegistry.aliveCells.Add((1, 0));
            liveRegistry.aliveCells.Add((0, 1));
            liveRegistry.aliveCells.Add((1, 1));

            // Act
            generator.UpdateState();

            // Assert
            Assert.That(liveRegistry.aliveCells.Contains((0, 0)), Is.EqualTo(true));
        }

        [Test]
        public void UpdateState_LiveCellWithOneNeighbour_Dies()
        {
            // Arrange
            liveRegistry.aliveCells.Add((0, 0));
            liveRegistry.aliveCells.Add((1, 0));

            // Act
            generator.UpdateState();

            // Assert
            Assert.That(liveRegistry.aliveCells.Contains((0, 0)), Is.EqualTo(false));
        }

        [Test]
        public void UpdateState_LiveCellWithTwoNeighbours_Lives()
        {
            // Arrange
            liveRegistry.aliveCells.Add((0, 0));
            liveRegistry.aliveCells.Add((1, 0));
            liveRegistry.aliveCells.Add((0, 1));

            // Act
            generator.UpdateState();

            // Assert
            Assert.That(liveRegistry.aliveCells.Contains((0, 0)), Is.EqualTo(true));
        }

        [Test]
        public void UpdateState_LiveCellWithThreeNeighbours_Lives()
        {
            // Arrange
            liveRegistry.aliveCells.Add((0, 0));
            liveRegistry.aliveCells.Add((1, 0));
            liveRegistry.aliveCells.Add((0, 1));
            liveRegistry.aliveCells.Add((1, 1));

            // Act
            generator.UpdateState();

            // Assert
            Assert.That(liveRegistry.aliveCells.Contains((0, 0)), Is.EqualTo(true));
        }

        [Test]
        public void UpdateState_CellOutsideBounds_Dies()
        {
            // Arrange
            // Grid is 12x12 centred at (0,0), so normal data exist within a range of -6 to +6
            liveRegistry.aliveCells.Add((0, 7));
            liveRegistry.aliveCells.Add((1, 7));
            liveRegistry.aliveCells.Add((0, 6));

            // Act
            generator.UpdateState();

            // Assert
            Assert.That(liveRegistry.aliveCells.Contains((0, 7)), Is.EqualTo(false));
        }


    }
}
using NUnit.Framework;
using System.Collections.Generic;

namespace ConwayMinesweeper.Tests
{

    [TestFixture]
    public class GeneratorTests
    {
        [Test]
        public void UpdateState_DeadCellWithThreeNeighbours_BecomesAlive()
        {
            // Arrange
            var liveRegistry = new LiveRegistry();
            var grid = new Grid((0, 0), 12, 12);
            var generator = new Generator(grid, liveRegistry, (0, 0));

            // Three live cells around (0, 0)
            liveRegistry.aliveCells.Add((1, 0));
            liveRegistry.aliveCells.Add((0, 1));
            liveRegistry.aliveCells.Add((1, 1));

            // Act
            generator.UpdateState();

            // Assert
            Assert.That(liveRegistry.aliveCells.Contains((0, 0)), Is.EqualTo(true));
        }
    }
}
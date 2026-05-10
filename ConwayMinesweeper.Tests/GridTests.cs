using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Linq;


namespace ConwayMinesweeper.Tests
{
    [TestFixture]
    public class GridTests
    {
        private Grid grid;

        [SetUp]
        public void Init()
        {
            grid = new Grid((0, 0), 16, 12);
        }

        [Test]
        public void GetAllCells_ReturnsCorrectNumberOfCells()
        {
            //Arrange & Act
            IEnumerable<(int x, int y)> cellsInGrid = grid.GetAllCells();

            //Assert
            Assert.That(cellsInGrid.Count(), Is.EqualTo((192)));
        }

        [Test]
        public void GetAllCells_ContainsCentreCell()
        {
            // Arrange & Act
            IEnumerable<(int x, int y)> cellsInGrid = grid.GetAllCells();

            // Assert
            Assert.That(cellsInGrid.Contains((0, 0)), Is.EqualTo(true));
        }

        [Test]
        public void Grid_StoresCorrectDimensions()
        {
            // Assert
            Assert.That(grid.gridWidth, Is.EqualTo(16));
            Assert.That(grid.gridHeight, Is.EqualTo(12));
        }

        [Test]
        public void GetAllCells_DoesNotExceedBounds()
        {
            // Arrange & Act
            IEnumerable<(int x, int y)> cellsInGrid = grid.GetAllCells();

            // Assert
            Assert.That(cellsInGrid.All(cell =>
                cell.x >= -grid.gridWidth / 2 &&
                cell.x < grid.gridWidth / 2 &&
                cell.y >= -grid.gridHeight / 2 &&
                cell.y < grid.gridHeight / 2), Is.True);
        }


    }
}

using NUnit.Framework;
using System.Collections.Generic;

namespace ConwayMinesweeper.Tests
{
    [TestFixture]
    public class MineHiderTests
    {
        private LiveRegistry liveRegistry;
        private Grid grid;
        private MineHider mineHider;

        [SetUp]
        public void Init()
        {
            liveRegistry = new LiveRegistry();
            grid = new Grid((0, 0), 16, 12);
            mineHider = new MineHider(liveRegistry);
        }

        [Test]
        public void CoverMines_PopulatesTopCells()
        {
            mineHider.coverMines(grid);
            Assert.That(mineHider.topCells.Count, Is.EqualTo(192));
        }

        [Test]
        public void Reveal_ReturnsTrueForCoveredCell()
        {
            mineHider.coverMines(grid);
            Assert.That(mineHider.reveal(0, 0), Is.True);
        }

        [Test]
        public void Reveal_ReturnsFalseForAlreadyRevealedCell()
        {
            mineHider.coverMines(grid);
            mineHider.reveal(0, 0);
            Assert.That(mineHider.reveal(0, 0), Is.False);
        }

        [Test]
        public void Reveal_RemovesCellFromTopCells()
        {
            mineHider.coverMines(grid);
            mineHider.reveal(0, 0);
            Assert.That(mineHider.topCells.Contains((0, 0)), Is.False);
        }

        [Test]
        public void Reveal_FiresWinEventWhenAllSafeCellsRevealed()
        {
            liveRegistry.aliveCells.Add((0, 0));
            mineHider.coverMines(grid);
            bool winFired = false;
            mineHider.onWin += () => winFired = true;

            foreach (var cell in new List<(int x, int y)>(mineHider.topCells))
            {
                if (!liveRegistry.aliveCells.Contains(cell))
                    mineHider.reveal(cell.x, cell.y);
            }

            Assert.That(winFired, Is.True);
        }
    }
}
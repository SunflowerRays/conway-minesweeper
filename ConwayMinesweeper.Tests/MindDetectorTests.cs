using NUnit.Framework;
using System.Collections.Generic;

namespace ConwayMinesweeper.Tests
{
    [TestFixture]
    public class MineDetectorTests
    {
        private LiveRegistry liveRegistry;
        private Grid grid;
        private MineDetector mineDetector;

        [SetUp]
        public void Init()
        {
            liveRegistry = new LiveRegistry();
            grid = new Grid((0, 0), 16, 12);
            mineDetector = new MineDetector(grid, liveRegistry);
        }

        [Test]
        public void Detector_ReturnsTrueForMineCell()
        {
            liveRegistry.aliveCells.Add((0, 0));
            var (x, y, mines) = mineDetector.detector(0, 0);
            Assert.That(mines, Is.EqualTo(-1));
        }

        [Test]
        public void Detector_ReturnsFalseForSafeCell()
        {
            liveRegistry.aliveCells.Add((0, 0));
            var (x, y, mines) = mineDetector.detector(1, 1);
            Assert.That(mines, Is.Not.EqualTo(-1));
        }

        [Test]
        public void Detector_ReturnsMinusOneForMineCell()
        {
            liveRegistry.aliveCells.Add((0, 0));
            var (x, y, mines) = mineDetector.detector(0, 0);
            Assert.That(mines, Is.EqualTo(-1));
        }

        [Test]
        public void Detector_ReturnsCorrectAdjacencyCount()
        {
            liveRegistry.aliveCells.Add((1, 0));
            liveRegistry.aliveCells.Add((0, 1));
            var (x, y, mines) = mineDetector.detector(0, 0);
            Assert.That(mines, Is.EqualTo(2));
        }

        [Test]
        public void Detector_ReturnsZeroForIsolatedSafeCell()
        {
            var (x, y, mines) = mineDetector.detector(0, 0);
            Assert.That(mines, Is.EqualTo(0));
        }
    }
}
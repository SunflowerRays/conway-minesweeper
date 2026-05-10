using NUnit.Framework;
using System.Collections.Generic;

namespace ConwayMinesweeper.Tests
{
    [TestFixture]
    public class PatternManagerTests
    {
        private LiveRegistry liveRegistry;
        private PatternManager patternManager;

        [SetUp]
        public void Init()
        {
            liveRegistry = new LiveRegistry();
            patternManager = new PatternManager(liveRegistry);
        }

        [Test]
        public void ToggleCell_AddsCellToAliveCells()
        {
            patternManager.ToggleCell(0, 0);
            Assert.That(liveRegistry.aliveCells.Contains((0, 0)), Is.True);
        }

        [Test]
        public void ToggleCell_RemovesCellFromAliveCells()
        {
            patternManager.ToggleCell(0, 0);
            patternManager.ToggleCell(0, 0);
            Assert.That(liveRegistry.aliveCells.Contains((0, 0)), Is.False);
        }

        [Test]
        public void ToggleCell_ReturnsTrueWhenCellRemoved()
        {
            patternManager.ToggleCell(0, 0);
            bool result = patternManager.ToggleCell(0, 0);
            Assert.That(result, Is.True);
        }

        [Test]
        public void ToggleCell_ReturnsFalseWhenCellAdded()
        {
            bool result = patternManager.ToggleCell(0, 0);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ClearPattern_EmptiesAliveCells()
        {
            patternManager.ToggleCell(0, 0);
            patternManager.ToggleCell(1, 1);
            patternManager.ClearPattern();
            Assert.That(liveRegistry.aliveCells.Count, Is.EqualTo(0));
        }

        [Test]
        public void GetCentre_ReturnsZeroForEmptyPattern()
        {
            var centre = patternManager.GetCentre();
            Assert.That(centre, Is.EqualTo((0, 0)));
        }

        [Test]
        public void GetCentre_ReturnsCorrectCentreForPattern()
        {
            patternManager.ToggleCell(-2, -2);
            patternManager.ToggleCell(2, 2);
            var centre = patternManager.GetCentre();
            Assert.That(centre, Is.EqualTo((0, 0)));
        }
    }
}
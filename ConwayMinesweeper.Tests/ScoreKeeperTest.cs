using NUnit.Framework;
using System.IO;

namespace ConwayMinesweeper.Tests
{
    [TestFixture]
    public class ScoreKeeperTests
    {
        private ScoreKeeper scoreKeeper;
        private string testPath;

        [SetUp]
        public void Init()
        {
            testPath = Path.GetTempPath();
            scoreKeeper = new ScoreKeeper(testPath);
        }

        [TearDown]
        public void Cleanup()
        {
            string filePath = Path.Combine(testPath, "high_scores.json");
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        [Test]
        public void SaveScore_CreatesFileIfNotExists()
        {
            scoreKeeper.saveScore(new ScoreKeeper.LatestScore
            {
                time = 10.5f,
                points = 100,
                levelCleared = true,
                playerName = "Test"
            });

            Assert.That(File.Exists(Path.Combine(testPath, "high_scores.json")), Is.True);
        }

        [Test]
        public void LoadScores_ReturnsCorrectNumberOfScores()
        {
            scoreKeeper.saveScore(new ScoreKeeper.LatestScore { time = 5f, points = 50, levelCleared = false, playerName = "A" });
            scoreKeeper.saveScore(new ScoreKeeper.LatestScore { time = 3f, points = 75, levelCleared = true, playerName = "B" });

            var scores = scoreKeeper.loadScores(2);
            Assert.That(scores.Length, Is.EqualTo(2));
        }
    }
}
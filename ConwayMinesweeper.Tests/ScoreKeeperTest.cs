using NUnit.Framework;
using System.IO;

namespace ConwayMinesweeper.Tests
{
    [TestFixture]
    public class ScoreKeeperTests
    {
        private ScoreKeeper scoreKeeper;
        private string testPath;
        private string testFile;

        [SetUp]
        public void Init()
        {
            testPath = Path.GetTempPath();
            testFile = "high_scores_test.json";
            scoreKeeper = new ScoreKeeper(testPath, testFile);
        }

        [TearDown]
        public void Cleanup()
        {
            string filePath = Path.Combine(testPath, testFile);
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

            Assert.That(File.Exists(Path.Combine(testPath, testFile)), Is.True);
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
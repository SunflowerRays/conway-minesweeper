using Newtonsoft.Json;
using System;
using System.IO;

//Newtonsoft.Json encounters less compatiblity issues with Unity than System.Text.Json.

public class ScoreKeeper
{
    private string path;



    public ScoreKeeper(string dataPath, string fileName = "high_scores.json")
    {
        path = Path.Combine(dataPath, fileName);
    }

    /// <summary>
    /// A struct containing all the score data from an attempt in minesweeper.
    /// </summary>
    public struct LatestScore
    {
        public float time;
        public int points;
        public bool levelCleared;
        public string playerName;
    }

    /// <summary>
    /// Saves the players score to the high_scores file.
    /// 
    /// </summary>
    /// <param name="score"></param>
    public void saveScore(LatestScore score)
    {
        string json = JsonConvert.SerializeObject(score);
        File.AppendAllText(path, json + "\n");
    }

    /// <summary>
    /// Loads and returns the specified number of highest scores from the score file, ordered by time in ascending
    /// order.
    /// </summary>
    /// <remarks>If the requested number of high scores exceeds the number of available scores, all available
    /// scores are returned. The method reads scores from a file and deserializes them from JSON format.</remarks>
    /// <param name="numberOfHighScores">The maximum number of high scores to return. Must be greater than or equal to zero.</param>
    /// <returns>An array of the highest scores, ordered by time. Returns an empty array if the score file does not exist or
    /// contains no scores.</returns>
    public LatestScore[] loadScores(int numberOfHighScores)
    {
        if (!File.Exists(path)) return new LatestScore[0];
        string[] lines = File.ReadAllLines(path);
        LatestScore[] scores = new LatestScore[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            scores[i] = JsonConvert.DeserializeObject<LatestScore>(lines[i]);
        }
        System.Array.Sort(scores, (a, b) => a.time.CompareTo(b.time));
        numberOfHighScores = Math.Min(numberOfHighScores, scores.Length);
        return scores[0..numberOfHighScores];
    }

    /// <summary>
    /// Deletes all stored score data by removing the underlying file, if it exists.
    /// </summary>
    /// <remarks>Use this method to reset or clear all previously saved scores. If the score file does not
    /// exist, the method performs no action. This operation cannot be undone.</remarks>
    public void clearScores()
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
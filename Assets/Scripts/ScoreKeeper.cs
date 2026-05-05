using Newtonsoft.Json;
using System;
using System.IO;

//Newtonsoft.Json encounters less compatiblity issues with Unity than System.Text.Json.

public class ScoreKeeper
{
    private string path;



    public ScoreKeeper(string dataPath)
    {
        path = Path.Combine(dataPath, "high_scores.json");
    }

    public struct LatestScore
    {
        public float time;
        public int points;
        public bool levelCleared;
        public string playerName;
    }

    public void saveScore(LatestScore score)
    {
        string json = JsonConvert.SerializeObject(score);
        File.AppendAllText(path, json + "\n");
    }

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

    public void clearScores()
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
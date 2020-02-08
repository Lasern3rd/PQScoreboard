using Newtonsoft.Json;
using System;
using System.IO;

namespace PQScoreboard
{
    public class JsonHandler
    {
        public static Scoreboard LoadFromFile(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentException("Failed to load scoreboard from file: Argument 'stream' is null.");
            }

            ScoreboardFile file;
            using (StreamReader reader = new StreamReader(stream))
            {
                file = JsonConvert.DeserializeObject<ScoreboardFile>(reader.ReadToEnd());
            }

            if (file.ExpectedNumberOfTeams < file.Teams.Length || file.Teams.Length != file.Scores.GetLength(0))
            {
                throw new ArgumentException("Failed to load scoreboard from file: Invalid number of teams.");
            }
            if (file.ExpectedNumberOfCategories < file.Categories.Length || file.Categories.Length != file.Scores.GetLength(1))
            {
                throw new ArgumentException("Failed to load scoreboard from file: Invalid number of categories.");
            }

            Scoreboard scoreboard = new Scoreboard(file.ExpectedNumberOfTeams, file.ExpectedNumberOfCategories);
            for (int i = 0; i < file.Teams.Length; ++i)
            {
                scoreboard.AddTeam(file.Teams[i]);
            }
            decimal[] scores = new decimal[file.Teams.Length];
            for (int i = 0; i < file.Categories.Length; ++i)
            {
                for (int j = file.Teams.Length - 1; j >= 0; --j)
                {
                    scores[j] = file.Scores[j, i];
                }
                scoreboard.AddCategory(file.Categories[i], scores);
            }

            return scoreboard;
        }

        public static void SaveToFile(Scoreboard scoreboard, Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentException("Failed to save scoreboard to file: Argument 'stream' is null.");
            }
            if (scoreboard == null)
            {
                throw new ArgumentException("Failed to save scoreboard to file: Argument 'scoreboard' is null.");
            }

            ScoreboardFile content = new ScoreboardFile()
            {
                ExpectedNumberOfTeams = scoreboard.ExpectedNumberOfTeams,
                ExpectedNumberOfCategories = scoreboard.ExpectedNumberOfCategories,
                Teams = scoreboard.Teams,
                Categories = scoreboard.Categories,
                Scores = scoreboard.Scores
            };

            using (StreamWriter writer = new StreamWriter(stream))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, content);
            }
        }

        private class ScoreboardFile
        {
            public int ExpectedNumberOfTeams { get; set; }

            public int ExpectedNumberOfCategories { get; set; }

            public string[] Teams { get; set; }

            public string[] Categories { get; set; }

            public decimal[,] Scores { get; set; }
        }
    }
}

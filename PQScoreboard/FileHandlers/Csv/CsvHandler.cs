using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace PQScoreboard
{
    public class CsvHandler
    {
        public static Scoreboard LoadFromFile(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentException("Failed to load scoreboard from file: Argument 'stream' is null.");
            }

            using (TextReader reader = new StreamReader(stream))
            {
                string buffer = reader.ReadToEnd();

                string[][] rows = CsvParser.Parse(buffer, true, out string[] header);

                string[] dimens = header[0].Split('\\');

                if (!int.TryParse(dimens[0], out int expectedNumberOfCategories))
                {
                    throw new ArgumentException("Failed to load scoreboard fom file: Invalid number of categories in cell (0,0).");
                }
                if (!int.TryParse(dimens[1], out int expectedNumberOfTeams))
                {
                    throw new ArgumentException("Failed to load scoreboard fom file: Invalid number of teams in cell (0,0).");
                }
                // first column contains category names
                if (header.Length > expectedNumberOfTeams + 1)
                {
                    throw new ArgumentException("Failed to load scoreboard fom file: Invalid number of columns in header.");
                }
                // header (row containing team names) is separate
                if (rows.Length > expectedNumberOfCategories)
                {
                    throw new ArgumentException("Failed to load scoreboard fom file: Invalid number of rows.");
                }
                for (int i = rows.Length - 1; i >= 0; --i)
                {
                    if (rows[i].Length != header.Length)
                    {
                        throw new ArgumentException("Failed to load scoreboard fom file: Invalid number columns in row " + (i + 1) + ".");
                    }
                }

                Scoreboard scoreboard = new Scoreboard(expectedNumberOfTeams, expectedNumberOfCategories);

                for (int i = 1; i < header.Length; ++i)
                {
                    scoreboard.AddTeam(header[i]);
                }
                decimal[] scores = new decimal[header.Length - 1];
                for (int i = 0; i < rows.Length; ++i)
                {
                    for (int j = header.Length - 2; j >= 0; --j)
                    {
                        if (!decimal.TryParse(rows[i][j + 1], NumberStyles.AllowDecimalPoint,
                            CultureInfo.InvariantCulture, out scores[j]))
                        {
                            throw new ArgumentException("Failed to load scoreboard fom file: Invalid score for team '" + header[j + 1]
                                + "' at category '" + rows[i][0] + "'.");
                        }
                    }
                    scoreboard.AddCategory(rows[i][0], scores);
                }

                return scoreboard;
            }
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

            using (TextWriter writer = new StreamWriter(stream, Encoding.UTF8))
            {
                string[] teams = scoreboard.Teams;
                string[] categories = scoreboard.Categories;
                decimal[,] scores = scoreboard.Scores;

                writer.Write(scoreboard.ExpectedNumberOfCategories + "\\" + scoreboard.ExpectedNumberOfTeams);

                foreach (string team in teams)
                {
                    writer.Write(",\"");
                    writer.Write(team);
                    writer.Write("\"");
                }
                writer.Write("\n");

                for (int i = 0; i < categories.Length; ++i)
                {
                    writer.Write("\"");
                    writer.Write(categories[i]);
                    writer.Write("\"");

                    for (int j = 0; j < teams.Length; ++j)
                    {
                        writer.Write(",");
                        writer.Write(scores[j, i].ToString(CultureInfo.InvariantCulture));
                    }
                    writer.Write("\n");
                }
            }
        }
    }
}

using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace PQScoreboard
{
    public class CsvHandler
    {
        // TODO: define exceptions

        public static Scoreboard LoadFromFile(Stream stream)
        {
            // TODO add validation
            using (TextReader reader = new StreamReader(stream))
            {
                string buffer = reader.ReadToEnd();

                string[][] rows = CsvParser.Parse(buffer, true, out string[] header);

                string[] dimens = header[0].Split('\\');

                if (!int.TryParse(dimens[0], out int numberOfCategories))
                {
                    throw new ArgumentException("invalid number of categories in cell (0,0)");
                }
                if (!int.TryParse(dimens[1], out int numberOfTeams))
                {
                    throw new ArgumentException("invalid number of teams in cell (0,0)");
                }
                if (header.Length > numberOfTeams + 1)
                {
                    throw new ArgumentException("invalid number of columns in header.");
                }
                if (rows.Length > numberOfCategories)
                {
                    throw new ArgumentException("invalid number of rows.");
                }
                for (int i = rows.Length - 1; i >= 0; --i)
                {
                    if (rows[i].Length > numberOfTeams + 1)
                    {
                        throw new ArgumentException("invalid number columns in row " + (i + 1) + ".");
                    }
                }

                Scoreboard scoreboard = new Scoreboard(numberOfTeams, numberOfCategories);

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
                            throw new ArgumentException("invalid score for team '" + header[j + 1]
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
            using (TextWriter writer = new StreamWriter(stream, Encoding.UTF8))
            {
                if (scoreboard == null)
                {
                    return;
                }

                string[] teams = scoreboard.Teams;
                string[] categories = scoreboard.Categories;
                decimal[,] scores = scoreboard.Scores;

                writer.Write(categories.Length + "\\" + teams.Length);

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

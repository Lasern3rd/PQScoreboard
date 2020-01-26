using System;
using System.Linq;

namespace PQScoreboard
{
    public class Scoreboard
    {
        private readonly string[] teams;
        private readonly string[] categories;
        private readonly decimal[,] scores;

        private int currentTeam;
        private int currentCategory;

        public Scoreboard(int numberOfTeams, int numberOfCategories)
        {
            teams = new string[numberOfTeams];
            categories = new string[numberOfCategories];
            scores = new decimal[numberOfTeams, numberOfCategories];

            currentTeam = 0;
            currentCategory = 0;
        }

        public void AddTeam(string teamName)
        {
            if (string.IsNullOrWhiteSpace(teamName))
            {
                throw new ArgumentException("Invalid team name.");
            }
            if (teams.Contains(teamName))
            {
                throw new ArgumentException("Team with name '" + teamName + "' already exists.");
            }

            if (currentTeam >= teams.Length)
            {
                // TODO
                throw new NotImplementedException();
            }

            teams[currentTeam++] = teamName;
        }

        public void AddCategory(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                throw new ArgumentException("Invalid category name.");
            }
            if (teams.Contains(categoryName))
            {
                throw new ArgumentException("Category with name '" + categoryName + "' already exists.");
            }

            if (currentCategory >= categories.Length)
            {
                // TODO
                throw new NotImplementedException();
            }

            categories[currentCategory++] = categoryName;
        }

        public void AddCategory(string categoryName, decimal[] scores)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                throw new ArgumentException("Invalid category name.");
            }
            if (categories.Contains(categoryName))
            {
                throw new ArgumentException("Category with name '" + categoryName + "' already exists.");
            }
            if (scores.Length != currentTeam)
            {
                throw new ArgumentException("Invalid length for scores. Length must be '" + currentTeam + "'.");
            }
            if (currentCategory >= categories.Length)
            {
                // TODO
                throw new NotImplementedException();
            }

            categories[currentCategory] = categoryName;

            for (int i = currentTeam - 1; i >= 0; --i)
            {
                this.scores[i, currentCategory] = scores[i];
            }

            ++currentCategory;
        }

        public string GetTeamName(int team)
        {
            if (team < 0 || team >= currentTeam)
            {
                throw new ArgumentException("Unknown team id " + team + ".");
            }

            return teams[team];
        }

        public string GetCategoryName(int category)
        {
            if (category < 0 || category >= currentCategory)
            {
                throw new ArgumentException("Unknown category id " + category + ".");
            }

            return categories[category];
        }

        public void SetScore(string teamName, string categoryName, decimal value)
        {
            int team = Array.IndexOf(teams, teamName);
            if (team < 0)
            {
                throw new ArgumentException("Unknown team '" + teamName + "'.");
            }
            int category = Array.LastIndexOf(categories, categoryName);
            if (category < 0)
            {
                throw new ArgumentException("Unknown category '" + categoryName + "'.");
            }

            scores[team, category] = value;
        }

        public bool SetScore(int team, int category, decimal value)
        {
            if (team < 0 || team >= currentTeam)
            {
                throw new ArgumentException("Unknown team id " + team + ".");
            }
            if (category < 0 || category >= currentCategory)
            {
                throw new ArgumentException("Unknown category id " + category + ".");
            }

            decimal oldScore = scores[team, category];
            scores[team, category] = value;

            return oldScore != value;
        }

        public decimal GetScore(int team, int category)
        {
            if (team < 0 || team >= currentTeam)
            {
                throw new ArgumentException("Unknown team id " + team + ".");
            }
            if (category < 0 || category >= currentCategory)
            {
                throw new ArgumentException("Unknown category id " + category + ".");
            }

            return scores[team, category];
        }

        public decimal GetTotalScore(int team)
        {
            if (team < 0 || team >= currentTeam)
            {
                throw new ArgumentException("Unknown team id " + team + ".");
            }

            decimal totalScore = 0m;

            for (int i = currentCategory - 1; i >= 0; --i)
            {
                totalScore += scores[team, i];
            }
            return totalScore;
        }


        #region properties

        public bool IsValid
        {
            get
            {
                return currentTeam > 0 && currentCategory > 0;
            }
        }

        public int ExpectedNumberOfTeams
        {
            get
            {
                return teams.Length;
            }
        }

        public string[] Teams
        {
            get
            {
                return teams.Take(currentTeam).ToArray();
            }
        }

        public int ExpectedNumberOfCategories
        {
            get
            {
                return categories.Length;
            }
        }

        public string[] Categories
        {
            get
            {
                return categories.Take(currentCategory).ToArray();
            }
        }

        public decimal[,] Scores
        {
            get
            {
                decimal[,] result = new decimal[currentTeam, currentCategory];
                for (int i = currentTeam - 1; i >= 0; --i)
                {
                    for (int j = currentCategory - 1; j >= 0; --j)
                    {
                        result[i, j] = scores[i, j];
                    }
                }
                return result;
            }
        }

        #endregion
    }
}

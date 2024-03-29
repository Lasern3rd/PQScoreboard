﻿using System;
using System.Linq;

namespace PQScoreboard
{
    public class Scoreboard
    {
        private string[] teams;
        private string[] categories;
        private decimal[,] scores;

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

        #region public methods

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
                int newSize = Math.Max(currentTeam * 4 / 3, currentTeam + 3);
                teams = Expand(teams, newSize);
                scores = Expand(scores, newSize, categories.Length);
            }

            teams[currentTeam++] = teamName;
        }

        public void RenameTeam(int team, string teamName)
        {
            if (team < 0 || team >= currentTeam)
            {
                throw new ArgumentException("Unknown team id " + team + ".");
            }
            if (string.IsNullOrWhiteSpace(teamName))
            {
                throw new ArgumentException("Invalid team name.");
            }
            if (teams.Contains(teamName))
            {
                throw new ArgumentException("Team with name '" + teamName + "' already exists.");
            }

            teams[team] = teamName;
        }

        public void RemoveTeam(int team)
        {
            if (team < 0 || team >= currentTeam)
            {
                throw new ArgumentException("Unknown team id " + team + ".");
            }
            
            --currentTeam;

            for (int i = team; i < currentTeam; ++i)
            {
                teams[i] = teams[i + 1];

                for (int j = currentCategory - 1; j >= 0; --j)
                {
                    scores[i, j] = scores[i + 1, j];
                }
            }
        }

        public void AddCategory(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                throw new ArgumentException("Invalid category name.");
            }

            if (currentCategory >= categories.Length)
            {
                int newSize = Math.Max(currentCategory * 4 / 3, currentCategory + 3);
                categories = Expand(categories, newSize);
                scores = Expand(scores, teams.Length, newSize);
            }

            categories[currentCategory++] = categoryName;
        }

        public void AddCategory(string categoryName, decimal[] scores)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                throw new ArgumentException("Invalid category name.");
            }
            if (scores.Length != currentTeam)
            {
                throw new ArgumentException("Invalid length for scores. Length must be '" + currentTeam + "'.");
            }
            if (currentCategory >= categories.Length)
            {
                int newSize = Math.Max(currentCategory * 4 / 3, currentCategory + 3);
                categories = Expand(categories, newSize);
                this.scores = Expand(this.scores, teams.Length, newSize);
            }

            categories[currentCategory] = categoryName;

            for (int i = currentTeam - 1; i >= 0; --i)
            {
                this.scores[i, currentCategory] = scores[i];
            }

            ++currentCategory;
        }

        public void RenameCategory(int category, string categoryName)
        {
            if (category < 0 || category >= currentCategory)
            {
                throw new ArgumentException("Unknown category id " + category + ".");
            }
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                throw new ArgumentException("Invalid category name.");
            }

            categories[category] = categoryName;
        }

        public void RemoveCategory(int category)
        {
            if (category < 0 || category >= currentCategory)
            {
                throw new ArgumentException("Unknown category id " + category + ".");
            }

            --currentCategory;

            for (int j = category; j < currentCategory; ++j)
            {
                categories[j] = categories[j + 1];

                for (int i = currentTeam - 1; i >= 0; --i)
                {
                    scores[i, j] = scores[i, j + 1];
                }
            }
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

        #endregion

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

        public int NumberOfTeams
        {
            get
            {
                return currentTeam;
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

        public int NumberOfCategories
        {
            get
            {
                return currentCategory;
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

        public decimal[] TotalScores
        {
            get
            {
                decimal[] result = new decimal[currentTeam];
                for (int i = currentTeam - 1; i >= 0; --i)
                {
                    for (int j = currentCategory - 1; j >= 0; --j)
                    {
                        result[i] += scores[i, j];
                    }
                }
                return result;
            }
        }

        #endregion

        #region private methods

        private string[] Expand(string[] array, int size)
        {
            string[] newArray = new string[size];
            Array.Copy(array, 0, newArray, 0, array.Length);
            return newArray;
        }

        private decimal[,] Expand(decimal[,] array, int size1, int size2)
        {
            decimal[,] newArray = new decimal[size1, size2];
            for (int i = array.GetLength(0) - 1; i >= 0; --i)
            {
                for (int j = array.GetLength(1) - 1; j >= 0; --j)
                {
                    newArray[i, j] = array[i, j];
                }
            }
            return newArray;
        }

        #endregion
    }
}

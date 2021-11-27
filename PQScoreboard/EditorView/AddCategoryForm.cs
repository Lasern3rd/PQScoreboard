using System;
using System.Windows.Forms;

namespace PQScoreboard
{
    public partial class AddCategoryForm : Form
    {

        public AddCategoryForm(string[] teams)
        {
            InitializeComponent();

            DataGridViewScores.RowHeadersVisible = false;
            DataGridViewScores.ColumnHeadersVisible = true;
            DataGridViewScores.AllowUserToAddRows = false;
            DataGridViewScores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            DataGridViewScores.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;

            if (teams == null || teams.Length == 0)
            {
                DataGridViewScores.Enabled = false;
                return;
            }

            DataGridViewScores.Columns.Clear();
            DataGridViewScores.Rows.Clear();

            foreach (string team in teams)
            {
                DataGridViewScores.Columns.Add(team, team);
                DataGridViewScores.Columns[DataGridViewScores.Columns.Count - 1].SortMode =
                    DataGridViewColumnSortMode.NotSortable;
            }

            DataGridViewScores.Rows.Add();

            for (int i = teams.Length - 1; i >= 0; --i)
            {
                DataGridViewScores.Rows[0].Cells[i].Value = 0;
            }

            ButtonOk.Enabled = false;
        }

        #region properties

        public string CategoryName
        {
            get
            {
                return TextboxCategoryName.Text;
            }
        }

        public decimal[] Scores
        {
            get
            {
                decimal[] scores = new decimal[DataGridViewScores.Rows[0].Cells.Count];

                for (int i = DataGridViewScores.Rows[0].Cells.Count - 1; i >= 0; --i)
                {
                    if (!decimal.TryParse(DataGridViewScores.Rows[0].Cells[i].Value.ToString(), out scores[i]))
                    {
                        scores[i] = 0;
                    }
                }

                return scores;
            }
        }

        #endregion

        private void ButtonOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void TextboxCategoryName_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    e.Handled = true;
                    SelectNextControl(ActiveControl, true, true, true, true);
                    break;

                case Keys.Escape:
                    e.Handled = true;
                    DialogResult = DialogResult.Cancel;
                    Close();
                    break;
            }
        }

        private void TextboxCategoryName_TextChanged(object sender, EventArgs e)
        {
            ButtonOk.Enabled = !string.IsNullOrEmpty(TextboxCategoryName.Text);
        }
    }
}

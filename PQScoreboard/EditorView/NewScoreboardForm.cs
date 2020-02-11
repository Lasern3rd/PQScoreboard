using System;
using System.Windows.Forms;

namespace PQScoreboard
{
    public partial class NewScoreboardForm : Form
    {
        public NewScoreboardForm()
        {
            InitializeComponent();
        }

        #region properties

        public int NumberOfTeams
        {
            get
            {
                return decimal.ToInt32(NumericInputNumberOfTeams.Value);
            }
        }

        public int NumberOfCategories
        {
            get
            {
                return decimal.ToInt32(NumericInputNumberOfCategories.Value);
            }
        }

        #endregion

        #region interface actions

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

        #endregion
    }
}

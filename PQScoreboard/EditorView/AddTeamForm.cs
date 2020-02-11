using System;
using System.Windows.Forms;

namespace PQScoreboard
{
    public partial class AddTeamForm : Form
    {
        public AddTeamForm()
        {
            InitializeComponent();
        }

        #region properties

        public string TeamName
        {
            get
            {
                return TextboxTeamName.Text;
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

        private void TextboxTeamName_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    e.Handled = true;
                    DialogResult = DialogResult.OK;
                    Close();
                    break;

                case Keys.Escape:
                    e.Handled = true;
                    DialogResult = DialogResult.Cancel;
                    Close();
                    break;
            }
        }

        #endregion
    }
}

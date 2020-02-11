using System;
using System.Windows.Forms;

namespace PQScoreboard
{
    public partial class ModifyTeamForm : Form
    {
        public ModifyTeamForm(string teamName)
        {
            InitializeComponent();

            TeamName = teamName;
        }

        #region properties

        public ModifyTeamResult Result { get; private set; }

        public string TeamName
        {
            get
            {
                return TextBoxTeamName.Text;
            }
            private set
            {
                TextBoxTeamName.Text = value;
            }
        }

        #endregion

        #region interface actions

        private void ButtonRename_Click(object sender, EventArgs e)
        {
            Result = ModifyTeamResult.Rename;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonRemove_Click(object sender, EventArgs e)
        {
            Result = ModifyTeamResult.Remove;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        #endregion

        public enum ModifyTeamResult
        {
            Rename, Remove
        }
    }
}

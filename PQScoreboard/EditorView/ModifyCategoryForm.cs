using System;
using System.Windows.Forms;

namespace PQScoreboard
{
    public partial class ModifyCategoryForm : Form
    {
        public ModifyCategoryForm(string categoryName)
        {
            InitializeComponent();

            CategoryName = categoryName;
        }

        #region properties

        public ModifyCategoryResult Result { get; private set; }

        public string CategoryName
        {
            get
            {
                return TextBoxCategoryName.Text;
            }
            private set
            {
                TextBoxCategoryName.Text = value;
            }
        }

        #endregion

        #region interface actions

        private void ButtonRename_Click(object sender, EventArgs e)
        {
            Result = ModifyCategoryResult.Rename;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonRemove_Click(object sender, EventArgs e)
        {
            Result = ModifyCategoryResult.Remove;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        #endregion

        public enum ModifyCategoryResult
        {
            Rename, Remove
        }
    }
}

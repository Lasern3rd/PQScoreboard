using log4net;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PQScoreboard
{
    public partial class EditorForm : Form
    {
        private const string backupFileName = "backup_";
        private static readonly ILog log = LogManager.GetLogger(typeof(EditorForm));

        private Scoreboard scoreboard;
        private bool hasUnsavedChanges;
        private string fileName;
        private ResultForm resultForm;

        public EditorForm()
        {
            log.Debug("EditorForm::ctor {");

            InitializeComponent();
            DataGridViewScores.RowHeadersVisible = true;
            DataGridViewScores.ColumnHeadersVisible = true;
            DataGridViewScores.AllowUserToAddRows = false;
            DataGridViewScores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            DataGridViewScores.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;

            SystemEvents.DisplaySettingsChanged += (s, e) =>
            {
                UpdateScreens();
            };
            UpdateScreens();

            scoreboard = null;
            hasUnsavedChanges = false;
            fileName = null;
            UpdateControls();
            UpdateScores();

            log.Debug("EditorForm::ctor }");
        }

        #region private functions

        private void UpdateScreens()
        {
            string selected = ComboBoxScreen.SelectedItem?.ToString();

            ComboBoxScreen.Items.Clear();
            for (int i = 0; i < Screen.AllScreens.Length; ++i)
            {
                string deviceName = Screen.AllScreens[i].DeviceName;
                ComboBoxScreen.Items.Add(deviceName);

                if (selected == deviceName)
                {
                    ComboBoxScreen.SelectedIndex = i;
                }
            }

            if (ComboBoxScreen.SelectedIndex < 0)
            {
                ComboBoxScreen.SelectedIndex = 0;
            }
        }

        private void UpdateScores()
        {
            // TODO: implement soft update

            DataGridViewScores.Columns.Clear();
            DataGridViewScores.Rows.Clear();

            if (scoreboard == null)
            {
                DataGridViewScores.Enabled = false;
                return;
            }

            string[] teams = scoreboard.Teams;

            if (teams.Length == 0)
            {
                DataGridViewScores.Enabled = false;
                return;
            }

            DataGridViewScores.Enabled = true;

            foreach (string team in teams)
            {
                DataGridViewScores.Columns.Add(team, team);
                DataGridViewScores.Columns[DataGridViewScores.Columns.Count - 1].SortMode =
                    DataGridViewColumnSortMode.NotSortable;
            }

            string[] categories = scoreboard.Categories;
            if (categories.Length == 0)
            {
                return;
            }

            DataGridViewScores.Rows.Add(categories.Length);

            for (int i = categories.Length - 1; i >= 0; --i)
            {
                DataGridViewScores.Rows[i].HeaderCell.Value = categories[i];
            }

            DataGridViewScores.Rows.Add();
            DataGridViewScores.Rows[categories.Length].HeaderCell.Value = "\u03a3";
            DataGridViewScores.Rows[categories.Length].ReadOnly = true;

            decimal[,] scores = scoreboard.Scores;

            for (int i = scores.GetLength(0) - 1; i >= 0; --i)
            {
                decimal totalScore = 0m;

                for (int j = scores.GetLength(1) - 1; j >= 0; --j)
                {
                    DataGridViewScores.Rows[j].Cells[i].Value = scores[i, j];
                    totalScore += scores[i, j];
                }

                DataGridViewScores.Rows[categories.Length].Cells[i].Value = totalScore;
            }
        }

        private void UpdateControls()
        {
            if (scoreboard == null)
            {
                MenuEdit.Enabled = false;
                MenuFileSave.Enabled = false;
                ButtonAnimate.Enabled = false;
            }
            else
            {
                MenuEdit.Enabled = true;
                MenuFileSave.Enabled = true;

                ButtonAnimate.Enabled = scoreboard.IsValid;
            }
        }

        private void LoadFromFile()
        {
            log.Debug("EditorForm::LoadFromFile() {");

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = string.IsNullOrEmpty(fileName) ? Environment.SpecialFolder.MyDocuments.ToString() : fileName;
                openFileDialog.Filter = "CSV files (*.csv)|*.csv|JSON files (*.json)|*.json";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    fileName = openFileDialog.FileName;
                    switch (Path.GetExtension(fileName))
                    {
                        case ".csv":
                            scoreboard = CsvHandler.LoadFromFile(openFileDialog.OpenFile());
                            break;

                        case ".json":
                            scoreboard = JsonHandler.LoadFromFile(openFileDialog.OpenFile());
                            break;

                        default:
                            throw new ArgumentException("Unknown file format: '" + Path.GetExtension(fileName) + "'.");
                    }
                    hasUnsavedChanges = false;
                    NumericInputAnimationLength.Value = GetSuggestedAnimationLength();
                    UpdateScores();
                    UpdateControls();
                }
            }

            log.Debug("EditorForm::LoadFromFile() }");
        }

        private bool SaveToFile()
        {
            log.Debug("EditorForm::SaveToFile() {");

            if (scoreboard == null)
            {
                log.Debug("EditorForm::SaveToFile() } // scoreboard == null");
                return true;
            }

            bool cancelled = false;

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "CSV files (*.csv)|*.csv|JSON files (*.json)|*.json";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.DefaultExt = "csv";
                saveFileDialog.AddExtension = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    fileName = saveFileDialog.FileName;
                    switch (Path.GetExtension(fileName))
                    {
                        default:
                        case ".csv":
                            CsvHandler.SaveToFile(scoreboard, saveFileDialog.OpenFile());
                            break;

                        case ".json":
                            JsonHandler.SaveToFile(scoreboard, saveFileDialog.OpenFile());
                            break;
                    }
                    hasUnsavedChanges = false;
                }
                else
                {
                    cancelled = true;
                }
            }

            log.Debug("EditorForm::SaveToFile() }");
            return !cancelled;
        }

        private bool PromptSaveDialogIfUnsavedChanged()
        {
            log.Debug("EditorForm::PromptSaveDialogIfUnsavedChanged() {");

            bool cancelled = false;
            if (hasUnsavedChanges)
            {
                try
                {
                    DialogResult result = MessageBox.Show("There are unsaved changes. Save?", "Information",
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);

                    switch (result)
                    {
                        case DialogResult.Yes:
                            cancelled = !SaveToFile();
                            break;

                        case DialogResult.No:
                            break;

                        default:
                        case DialogResult.Cancel:
                            cancelled = true;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Failed to close scoreboard: Failed to save existing scoreboard.", ex);
                    MessageBox.Show("Failed to close scoreboard: Failed to save existing scoreboard: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            log.Debug("EditorForm::PromptSaveDialogIfUnsavedChanged() }");
            return cancelled;
        }

        private void ModifyCategory(int category)
        {
            log.Debug("EditorForm::ModifyCategory() {");

            try
            {
                string categoryName = scoreboard.GetCategoryName(category);

                ModifyCategoryForm modifyCategoryForm = new ModifyCategoryForm(categoryName);
                modifyCategoryForm.StartPosition = FormStartPosition.CenterParent;
                DialogResult result = modifyCategoryForm.ShowDialog(this);

                if (result != DialogResult.OK)
                {
                    log.Debug("EditorForm::ModifyCategory() } // cancelled");
                    return;
                }

                switch (modifyCategoryForm.Result)
                {
                    case ModifyCategoryForm.ModifyCategoryResult.Rename:
                        string newCategoryName = modifyCategoryForm.CategoryName;
                        if (string.Equals(categoryName, newCategoryName))
                        {
                            log.Debug("EditorForm::ModifyCategory() } // new category name == old category name");
                            return;
                        }
                        scoreboard.RenameCategory(category, modifyCategoryForm.CategoryName);
                        hasUnsavedChanges = true;
                        UpdateScores();
                        UpdateControls();
                        break;

                    case ModifyCategoryForm.ModifyCategoryResult.Remove:
                        if (DialogResult.Yes != MessageBox.Show("Really remove category '" + categoryName + "'?", "Information",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                        {
                            log.Debug("EditorForm::ModifyCategory() } // cancelled remove team");
                            return;
                        }
                        scoreboard.RemoveCategory(category);
                        hasUnsavedChanges = true;
                        UpdateScores();
                        UpdateControls();
                        break;

                    default:
                        throw new NotImplementedException("Not implemented.");
                }
            }
            catch (Exception ex)
            {
                log.Error("Failed to modify category.", ex);
                MessageBox.Show("Failed to modify category: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            log.Debug("EditorForm::ModifyCategory() }");
        }

        private void ModifyTeam(int team)
        {
            log.Debug("EditorForm::ModifyTeam() {");

            try
            {
                string teamName = scoreboard.GetTeamName(team);

                ModifyTeamForm modifyTeamForm = new ModifyTeamForm(teamName);
                modifyTeamForm.StartPosition = FormStartPosition.CenterParent;
                DialogResult result = modifyTeamForm.ShowDialog(this);

                if (result != DialogResult.OK)
                {
                    log.Debug("EditorForm::ModifyTeam() } // cancelled");
                    return;
                }

                switch (modifyTeamForm.Result)
                {
                    case ModifyTeamForm.ModifyTeamResult.Rename:
                        string newTeamName = modifyTeamForm.TeamName;
                        if (string.Equals(teamName, newTeamName))
                        {
                            log.Debug("EditorForm::ModifyTeam() } // new team name == old team name");
                            return;
                        }
                        scoreboard.RenameTeam(team, modifyTeamForm.TeamName);
                        hasUnsavedChanges = true;
                        UpdateScores();
                        UpdateControls();
                        break;

                    case ModifyTeamForm.ModifyTeamResult.Remove:
                        if (DialogResult.Yes != MessageBox.Show("Really remove team '" + teamName + "'?", "Information",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                        {
                            log.Debug("EditorForm::ModifyTeam() } // cancelled remove team");
                            return;
                        }
                        scoreboard.RemoveTeam(team);
                        hasUnsavedChanges = true;
                        UpdateScores();
                        UpdateControls();
                        break;

                    default:
                        throw new NotImplementedException("Not implemented.");
                }
            }
            catch (Exception ex)
            {
                log.Error("Failed to modify team.", ex);
                MessageBox.Show("Failed to modify team: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            log.Debug("EditorForm::ModifyTeam() }");
        }

        private void SaveBackup()
        {
            try
            {
                using (StreamWriter file = File.CreateText(backupFileName + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + ".json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, scoreboard);
                }
            }
            catch (Exception ex)
            {
                log.Error("Failed to save backup.", ex);
            }
        }

        private decimal GetSuggestedAnimationLength()
        {
            // 'we are the champions' starts at 34 secs
            // hence we want f(1) = 3, f(ExpectedNoCtg) = 34
            decimal m = 31m / (Math.Max(scoreboard.ExpectedNumberOfCategories, 2) - 1m);
            return Math.Max(3m, (scoreboard.NumberOfCategories - 1) * m/2 + 3m);
        }

        #endregion

        #region interface actions

        private void MenuFileNew_Click(object sender, EventArgs e)
        {
            log.Debug("EditorForm::MenuFileNew_Click() {");

            if (hasUnsavedChanges)
            {
                try
                {
                    DialogResult result = MessageBox.Show("There are unsaved changes. Save?", "Information",
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);

                    switch (result)
                    {
                        case DialogResult.Yes:
                            if (!SaveToFile())
                            {
                                log.Debug("EditorForm::MenuFileNew_Click() } // cancelled (save existing)");
                                return;
                            }
                            break;

                        case DialogResult.No:
                            break;

                        default:
                        case DialogResult.Cancel:
                            log.Debug("EditorForm::MenuFileNew_Click() } // cancelled (save existing)");
                            return;
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Failed to create new scoreboard: Failed to save existing scoreboard.", ex);
                    MessageBox.Show("Failed to create new scoreboard: Failed to save existing scoreboard: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                    log.Debug("EditorForm::MenuFileNew_Click() }");
                    return;
                }
            }

            try
            {
                NewScoreboardForm newScoreboardForm = new NewScoreboardForm();
                newScoreboardForm.StartPosition = FormStartPosition.CenterParent;
                DialogResult result = newScoreboardForm.ShowDialog(this);

                if (result != DialogResult.OK)
                {
                    log.Debug("EditorForm::MenuFileNew_Click() } // cancelled");
                    return;
                }

                scoreboard = new Scoreboard(newScoreboardForm.NumberOfTeams, newScoreboardForm.NumberOfCategories);
                hasUnsavedChanges = false;
                NumericInputAnimationLength.Value = 3m;

                UpdateScores();
                UpdateControls();

            }
            catch (Exception ex)
            {
                log.Error("Failed to create new scoreboard.", ex);
                MessageBox.Show("Failed to create new scoreboard: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            log.Debug("EditorForm::MenuFileNew_Click() }");
        }

        private void MenuFileOpen_Click(object sender, EventArgs e)
        {
            log.Debug("EditorForm::MenuFileOpen_Click() {");

            if (hasUnsavedChanges)
            {
                try
                {
                    DialogResult result = MessageBox.Show("There are unsaved changes. Save?", "Information",
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);

                    switch (result)
                    {
                        case DialogResult.Yes:
                            if (!SaveToFile())
                            {
                                log.Debug("EditorForm::MenuFileOpen_Click() } // cancelled (save existing)");
                                return;
                            }
                            break;

                        case DialogResult.No:
                            break;

                        default:
                        case DialogResult.Cancel:
                            log.Debug("EditorForm::MenuFileOpen_Click() } // cancelled (save existing)");
                            return;
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Failed to open scoreboard: Failed to save existing scoreboard.", ex);
                    MessageBox.Show("Failed to open scoreboard: Failed to save existing scoreboard: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                    log.Debug("EditorForm::MenuFileOpen_Click() }");
                    return;
                }
            }

            try
            {
                LoadFromFile();
            }
            catch (Exception ex)
            {
                log.Error("Failed to open scoreboard.", ex);
                MessageBox.Show("Failed to open scoreboard: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            log.Debug("EditorForm::MenuFileOpen_Click() }");
        }

        private void MenuFileNewFromTemplate_Click(object sender, EventArgs e)
        {
            log.Debug("EditorForm::MenuFileNewFromTemplate_Click() {");

            if (hasUnsavedChanges)
            {
                try
                {
                    DialogResult result = MessageBox.Show("There are unsaved changes. Save?", "Information",
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);

                    switch (result)
                    {
                        case DialogResult.Yes:
                            if (!SaveToFile())
                            {
                                log.Debug("EditorForm::MenuFileNewFromTemplate_Click() } // cancelled (save existing)");
                                return;
                            }
                            break;

                        case DialogResult.No:
                            break;

                        default:
                        case DialogResult.Cancel:
                            log.Debug("EditorForm::MenuFileNewFromTemplate_Click() } // cancelled (save existing)");
                            return;
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Failed to import template: Failed to save existing scoreboard.", ex);
                    MessageBox.Show("Failed to import template: Failed to save existing scoreboard: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                    log.Debug("EditorForm::MenuFileNewFromTemplate_Click() }");
                    return;
                }
            }

            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = string.IsNullOrEmpty(fileName) ? Environment.SpecialFolder.MyDocuments.ToString() : fileName;
                    openFileDialog.Filter = "CSV files (*.csv)|*.csv|JSON files (*.json)|*.json";
                    openFileDialog.FilterIndex = 1;
                    openFileDialog.RestoreDirectory = true;

                    Scoreboard template;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        fileName = openFileDialog.FileName;
                        switch (Path.GetExtension(fileName))
                        {
                            case ".csv":
                                template = CsvHandler.LoadFromFile(openFileDialog.OpenFile());
                                break;

                            case ".json":
                                template = JsonHandler.LoadFromFile(openFileDialog.OpenFile());
                                break;

                            default:
                                throw new ArgumentException("Unknown file format: '" + Path.GetExtension(fileName) + "'.");
                        }
                        hasUnsavedChanges = false;

                        scoreboard = new Scoreboard(template.ExpectedNumberOfTeams, template.ExpectedNumberOfCategories);
                        foreach (string team in template.Teams)
                        {
                            scoreboard.AddTeam(team);
                        }
                        NumericInputAnimationLength.Value = GetSuggestedAnimationLength();
                        UpdateScores();
                        UpdateControls();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Failed to import template.", ex);
                MessageBox.Show("Failed to import template: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            log.Debug("EditorForm::MenuFileNewFromTemplate_Click() }");
        }

        private void MenuFileImportFromRemote_Click(object sender, EventArgs e)
        {
            log.Debug("EditorForm::MenuFileImportFromRemote_Click() {");

            if (hasUnsavedChanges)
            {
                try
                {
                    DialogResult result = MessageBox.Show("There are unsaved changes. Save?", "Information",
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);

                    switch (result)
                    {
                        case DialogResult.Yes:
                            if (!SaveToFile())
                            {
                                log.Debug("EditorForm::MenuFileImportFromRemote_Click() } // cancelled (save existing)");
                                return;
                            }
                            break;

                        case DialogResult.No:
                            break;

                        default:
                        case DialogResult.Cancel:
                            log.Debug("EditorForm::MenuFileImportFromRemote_Click() } // cancelled (save existing)");
                            return;
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Failed to import from remote: Failed to save existing scoreboard.", ex);
                    MessageBox.Show("Failed to import from remote: Failed to save existing scoreboard: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                    log.Debug("EditorForm::MenuFileImportFromRemote_Click() }");
                    return;
                }
            }

            try
            {
                ImportFromRemoteForm importFromRemoteForm = new ImportFromRemoteForm();
                importFromRemoteForm.StartPosition = FormStartPosition.CenterParent;
                DialogResult result = importFromRemoteForm.ShowDialog(this);

                if (result != DialogResult.OK)
                {
                    log.Debug("EditorForm::MenuFileImportFromRemote_Click() } // cancelled");
                    return;
                }

                scoreboard = importFromRemoteForm.Scoreboard;

                hasUnsavedChanges = true;
                NumericInputAnimationLength.Value = GetSuggestedAnimationLength();
                UpdateScores();
                UpdateControls();
            }
            catch (Exception ex)
            {
                log.Error("Failed to import from remote.", ex);
                MessageBox.Show("Failed to import from remote: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            log.Debug("EditorForm::MenuFileImportFromRemote_Click() }");
        }

        private void MenuFileSave_Click(object sender, EventArgs e)
        {
            log.Debug("EditorForm::MenuFileSave_Click() {");

            try
            {
                SaveToFile();
            }
            catch (Exception ex)
            {
                log.Error("Failed to save scoreboard.", ex);
                MessageBox.Show("Failed to save scoreboard: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            log.Debug("EditorForm::MenuFileSave_Click() }");
        }

        private void MenuFileClose_Click(object sender, EventArgs e)
        {
            log.Debug("EditorForm::MenuFileClose_Click() {");

            if (hasUnsavedChanges)
            {
                try
                {
                    DialogResult result = MessageBox.Show("There are unsaved changes. Save?", "Information",
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);

                    switch (result)
                    {
                        case DialogResult.Yes:
                            if (!SaveToFile())
                            {
                                log.Debug("EditorForm::MenuFileClose_Click() } // cancelled (save existing)");
                                return;
                            }
                            break;

                        case DialogResult.No:
                            break;

                        default:
                        case DialogResult.Cancel:
                            log.Debug("EditorForm::MenuFileClose_Click() } // cancelled (save existing)");
                            return;
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Failed to close scoreboard: Failed to save existing scoreboard.", ex);
                    MessageBox.Show("Failed to close scoreboard: Failed to save existing scoreboard: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            try
            {
                scoreboard = null;
                hasUnsavedChanges = false;
                UpdateScores();
                UpdateControls();
            }
            catch (Exception ex)
            {
                log.Error("Failed to close scoreboard.", ex);
                MessageBox.Show("Failed to close scoreboard: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            log.Debug("EditorForm::MenuFileClose_Click() }");
        }

        private void MenuFileExit_Click(object sender, EventArgs e)
        {
            log.Debug("EditorForm::MenuFileExit_Click() {");

            if (PromptSaveDialogIfUnsavedChanged())
            {
                log.Debug("EditorForm::MenuFileExit_Click() } // cancelled");
                return;
            }
            Close();

            log.Debug("EditorForm::MenuFileExit_Click() }");
        }

        private void EditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            log.Debug("EditorForm::EditorForm_FormClosed() {");

            if (PromptSaveDialogIfUnsavedChanged())
            {
                e.Cancel = true;
                log.Debug("EditorForm::EditorForm_FormClosed() } // cancelled");
            }

            log.Debug("EditorForm::EditorForm_FormClosed() }");
        }

        private void MenuEditAddTeam_Click(object sender, EventArgs e)
        {
            log.Debug("EditorForm::MenuEditAddTeam_Click() {");

            if (scoreboard == null)
            {
                log.Debug("EditorForm::MenuEditAddTeam_Click() } // scoreboard == null");
                return;
            }

            AddTeamForm addTeamForm = new AddTeamForm();
            addTeamForm.StartPosition = FormStartPosition.CenterParent;
            DialogResult result = addTeamForm.ShowDialog(this);

            if (result != DialogResult.OK)
            {
                log.Debug("EditorForm::MenuEditAddTeam_Click() } // cancelled");
                return;
            }

            try
            {
                scoreboard.AddTeam(addTeamForm.TeamName);
                hasUnsavedChanges = true;
            }
            catch (ArgumentException ex)
            {
                log.Error("Failed add team.", ex);
                MessageBox.Show("Failed to add team: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                log.Debug("EditorForm::MenuEditAddTeam_Click() }");
                return;
            }

            UpdateScores();
            UpdateControls();

            log.Debug("EditorForm::MenuEditAddTeam_Click() }");
        }

        private void MenuEditAddCategory_Click(object sender, EventArgs e)
        {
            log.Debug("EditorForm::MenuEditAddCategory_Click() {");

            if (scoreboard == null)
            {
                log.Debug("EditorForm::MenuEditAddCategory_Click() } // scoreboard == null");
                return;
            }

            AddCategoryForm addCategoryForm = new AddCategoryForm(scoreboard.Teams);
            addCategoryForm.StartPosition = FormStartPosition.CenterParent;
            DialogResult result = addCategoryForm.ShowDialog(this);

            if (result != DialogResult.OK)
            {
                log.Debug("EditorForm::MenuEditAddCategory_Click() } // cancelled");
                return;
            }

            try
            {
                scoreboard.AddCategory(addCategoryForm.CategoryName, addCategoryForm.Scores);
                hasUnsavedChanges = true;
            }
            catch (ArgumentException ex)
            {
                log.Error("Failed add category.", ex);
                MessageBox.Show("Failed to add category: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                log.Debug("EditorForm::MenuEditAddCategory_Click() }");
                return;
            }

            NumericInputAnimationLength.Value = 3m + Math.Max(0, scoreboard.NumberOfCategories - 1) * 30m / scoreboard.ExpectedNumberOfCategories;

            UpdateScores();
            UpdateControls();

            log.Debug("EditorForm::MenuEditAddCategory_Click() }");
        }

        private void ButtonAnimate_Click(object sender, EventArgs e)
        {
            log.Debug("EditorForm::ButtonAnimate_Click() {");

            if (scoreboard == null)
            {
                log.Debug("EditorForm::ButtonAnimate_Click() } // scoreboard == null");
                return;
            }

            SaveBackup();

            if (resultForm != null)
            {
                log.Debug("Closing previous result view.");
                resultForm.StopAnimationAndClose();
            }

            try
            {
                resultForm = new ResultForm(CheckBoxDarkMode.Checked, decimal.ToInt32(NumericInputNumberOfFireworks.Value));
                resultForm.WindowState = FormWindowState.Normal;
                resultForm.FormBorderStyle = FormBorderStyle.None;
                resultForm.StartPosition = FormStartPosition.Manual;
                Screen screen = Screen.AllScreens.FirstOrDefault(s => s.DeviceName == ComboBoxScreen.SelectedItem.ToString());
                resultForm.Bounds = (screen ?? Screen.PrimaryScreen).Bounds;

                resultForm.StartAnimation(scoreboard, (double)NumericInputAnimationLength.Value, CheckBoxFireworks.Checked);

                resultForm.Show();
            }
            catch (Exception ex)
            {
                log.Error("Failed show animated results.", ex);
                MessageBox.Show("Failed show animated results: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                resultForm.StopAnimationAndClose();
            }

            log.Debug("EditorForm::ButtonAnimate_Click() }");
        }

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            resultForm?.StopAnimationAndClose();
        }

        #endregion

        #region interface view events

        private void DataGridViewScores_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            int team = e.ColumnIndex;
            int category = e.RowIndex;
            if (team < 0 || category < 0 || category == DataGridViewScores.Rows.Count - 1)
            {
                // column or row header update or total scores row
                return;
            }

            if (!decimal.TryParse(DataGridViewScores.Rows[category].Cells[team].Value.ToString(), out decimal score))
            {
                DataGridViewScores.Rows[category].Cells[team].Value =
                                    scoreboard.GetScore(team, category);

                string errorText = "Failed to set score for team '" + scoreboard.GetTeamName(team) + "' for category '"
                    + scoreboard.GetCategoryName(category) + "': '" + DataGridViewScores.Rows[category].Cells[team].Value.ToString()
                    + "' is not a valid score.";

                log.Error("EditorForm::DataGridViewScores_CellValueChanged() } // " + errorText);
                MessageBox.Show(errorText, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (scoreboard.SetScore(team, category, score))
            {
                hasUnsavedChanges = true;

                // update total score
                DataGridViewScores.Rows[DataGridViewScores.Rows.Count - 1].Cells[team].Value =
                    scoreboard.GetTotalScore(team);
            }
        }

        private void DataGridViewScores_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 && e.ColumnIndex >= 0)
            {
                ModifyTeam(e.ColumnIndex);
            }
            else if (e.ColumnIndex == -1 && e.RowIndex >= 0)
            {
                ModifyCategory(e.RowIndex);
            }
        }

        #endregion
    }
}

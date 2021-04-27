using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PQScoreboard
{
    public partial class ImportFromRemoteForm : Form
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EditorForm));

        private static readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            }
        };

        private static string previousUrl = string.Empty;
        private static CookieContainer previousCookieContainer = new CookieContainer();

        public ImportFromRemoteForm()
        {
            InitializeComponent();
            TextBoxUrl.Text = previousUrl;

            ProgressBarImport.Visible = false;

            if (string.IsNullOrEmpty(previousUrl) || previousCookieContainer.Count == 0)
            {
                CheckBoxExistingAuth.Enabled = false;
            }
            else
            {
                CheckBoxExistingAuth.Enabled = true;
                CheckBoxExistingAuth.Checked = true;
            }
        }

        #region properties

        public Scoreboard Scoreboard { get; private set; }

        #endregion

        #region helper functions

        #endregion

        #region interface actions

        private async void ButtonOk_Click(object sender, EventArgs e)
        {
            try
            {
                ProgressBarImport.Visible = true;
                ButtonOk.Visible = false;
                ButtonCancel.Visible = false;
                ProgressBarImport.Value = 0;

                string url = TextBoxUrl.Text;

                if (url.EndsWith("/"))
                {
                    url = url.Substring(0, url.Length - 1);
                }

                if (previousUrl != url)
                {
                    previousCookieContainer = new CookieContainer();
                }

                HttpClient client = new HttpClient(new HttpClientHandler()
                {
                    CookieContainer = previousCookieContainer
                });
                HttpResponseMessage response;

                ProgressBarImport.Value = 25;

                if (!CheckBoxExistingAuth.Checked)
                {
                    HttpContent content = new StringContent(JsonConvert.SerializeObject(new LoginData()
                    {
                        Name = TextBoxName.Text,
                        Password = TextBoxPassword.Text
                    }, serializerSettings), Encoding.UTF8, "application/json");

                    response = await client.PostAsync(url + "/mods", content);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception("Failed to import from remote: Login failed: (" + response.StatusCode + ")" + response.ReasonPhrase);
                    }
                }
                else
                {
                    await Task.Delay(100);
                }

                ProgressBarImport.Value = 50;

                response = await client.GetAsync(url + "/standings");
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Failed to import from remote: (" + response.StatusCode + ")" + response.ReasonPhrase);
                }
                await Task.Delay(100);

                ProgressBarImport.Value = 75;

                Scoreboard = JsonHandler.LoadFromFile(await response.Content.ReadAsStreamAsync());

                await Task.Delay(100);

                ProgressBarImport.Value = 100;

                previousUrl = url;

                await Task.Delay(100);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                log.Error("Failed to import from remote.", ex);
                MessageBox.Show("Failed to import from remote: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ProgressBarImport.Visible = false;
                ButtonOk.Visible = true;
                ButtonCancel.Visible = true;
            }
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void TextBoxUrl_KeyDown(object sender, KeyEventArgs e)
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

        private void CheckBoxExistingAuth_CheckedChanged(object sender, EventArgs e)
        {
            bool enable = !CheckBoxExistingAuth.Checked;
            LabelUrl.Enabled = enable;
            LabelName.Enabled = enable;
            LabelPassword.Enabled = enable;
            TextBoxUrl.Enabled = enable;
            TextBoxName.Enabled = enable;
            TextBoxPassword.Enabled = enable;
        }

        #endregion
    }
}

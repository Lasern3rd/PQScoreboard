using log4net.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PQScoreboard
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // load logging config
            try
            {
                using (Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.log4net)))
                {
                    XmlConfigurator.Configure(stream);
                }
            }
            catch (Exception)
            {
                // ignore for now
            }

            // load program config
            try
            {
                using (StreamReader reader = new StreamReader("config.json"))
                {
                    Config.Values = JsonConvert.DeserializeObject<Config>(reader.ReadToEnd());
                }
                Config.Values.Validate();
            }
            catch (Exception ex)
            {
                Config.Values = new Config();
                MessageBox.Show("Failed to load config: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Application.Run(new EditorForm());
        }
    }
}

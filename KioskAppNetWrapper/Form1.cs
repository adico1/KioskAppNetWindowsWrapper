using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KioskAppNetWrapper
{
    public partial class mainWindow : Form
    {
        public mainWindow()
        {
            InitializeComponent();

            if (Uri.TryCreate(
                SettingsHelper.ReadSetting("kioskUrl"), UriKind.Absolute, out Uri kioskUrl))
            {
                webBrowser1.Url = kioskUrl;
            }
        }
    }
}

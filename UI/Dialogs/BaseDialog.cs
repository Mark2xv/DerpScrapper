using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DerpScrapper.UI.Dialogs
{
    public partial class BaseDialog : Form
    {
        public UI.DialogPanels.Library panel;

        public BaseDialog(UI.DialogPanels.Library libraryPanel)
        {
            InitializeComponent();
            this.Controls.Add(libraryPanel);
            this.panel = libraryPanel;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!this.panel.saveAllowed())
                MessageBox.Show(this.panel.errorMessage);
            else
            {
                this.panel.storeObject();
                this.Close();
            }
        }

        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }
    }
}

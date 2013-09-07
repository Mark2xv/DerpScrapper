using DerpScrapper.DBO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DerpScrapper
{
    public partial class AddNewLibrary : Form
    {
        public bool SetImportPath
        {
            get
            {
                return ImportPath != null && ImportPath != string.Empty;
            }
        }

        public string ImportPath { get; private set; }
        public DBO.Library NewLibrary { get; private set; }

        private bool haveTyped = false;

        public AddNewLibrary()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyUp += AddNewLibrary_KeyUp;

            this.Shown += (s, e) => {
                this.Activate();
                this.BringToFront();
                this.txtName.Select();
                this.txtName.Focus();
            };


            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            this.ImportPath = "D:\\Anime";
            this.txtName.Text = "Anime";
        }

        void AddNewLibrary_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter || e.KeyData == Keys.Return)
            {
                this.btnOK_Click(null, null);
            }
            else if (e.KeyData == Keys.Escape)
            {
                this.btnCancel_Click(null, null);
            }
            else
            {
                haveTyped = true;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (this.txtName.Text == string.Empty)
            {
                MessageBox.Show("Please enter a name for the new collection.");
                return;
            }

            DBO.Library newLib = new DBO.Library();
            newLib.Name = this.txtName.Text;
            if (!newLib.LibraryNameExists())
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.NewLibrary = newLib;
                this.Close();
            }
            else
            {
                MessageBox.Show("A collection with that name already exists!\r\nPlease choose a different name.");
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            Ookii.Dialogs.VistaFolderBrowserDialog dlg = new Ookii.Dialogs.VistaFolderBrowserDialog();
            dlg.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            var result = dlg.ShowDialog(this);
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                bool validPath = false; 
                foreach(string drive in Environment.GetLogicalDrives()) 
                {
                    if(dlg.SelectedPath.StartsWith(drive))
                    {  
                        validPath = true;
                        break;
                    }
                }

                if (validPath && System.IO.Directory.Exists(dlg.SelectedPath))
                {
                    ImportPath = dlg.SelectedPath;
                    lblImportPath.Text = dlg.SelectedPath;

                    if (!haveTyped)
                    {
                        int strPos = ImportPath.LastIndexOf("\\");
                        txtName.Text = ImportPath.Substring(strPos + 1);
                    }
                }
            }
        }
    }
}

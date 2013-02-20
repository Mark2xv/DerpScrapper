using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DerpScrapper.UI
{
    public partial class Main : Form
    {

        TabPage addLibraryPage;

        public Main()
        {
            InitializeComponent();

            // Fix for tabcontrol insert method
            IntPtr h = this.libraryTabControl.Handle;

            /* Create the add library tab with generic add image */
            var imageList = new ImageList();
            Image im = UI.Resources.add;
            imageList.Images.Add("addImage", im);
            imageList.ImageSize = new System.Drawing.Size(16, 16);
            libraryTabControl.ImageList = imageList;

            addLibraryPage = new TabPage("Add library");
            addLibraryPage.ImageKey = "addImage";
            addLibraryPage.ImageIndex = imageList.Images.IndexOfKey("addImage");
            libraryTabControl.TabPages.Add(addLibraryPage);
            
            /* Retrieve all existing tabs and add if available */
            List<DBO.Library> libraryListing = DBO.Library.GetAll();
            if (libraryListing.Count > 0)
            {
                foreach (DBO.Library lib in libraryListing)
                {
                    this.addLibraryTabPage(lib);
                }

                libraryTabControl.SelectedIndex = 0;
            }
            else
            {
                /* No libraries yet so force them to create one! Because in sovjet c#, c# forces you!*/
                Form addLib = new UI.Dialogs.BaseDialog(new UI.DialogPanels.Library());
                addLib.FormClosing += new FormClosingEventHandler(addLib_FormClosing_firstRun);
                addLib.ShowDialog();
            }
        }

        void addLib_FormClosing_firstRun(object sender, FormClosingEventArgs e)
        {
            List<DBO.Library> libraryListing = DBO.Library.GetAll();
            if (libraryListing.Count > 0)
            {
                foreach (DBO.Library lib in libraryListing)
                {
                    this.addLibraryTabPage(lib);
                }

                libraryTabControl.SelectedIndex = 0;
            }
            else
            {
                Application.Exit();
            }

        }

        void addLib_FormClosing(object sender, FormClosingEventArgs e)
        {
            List<DBO.Library> libraryListing = DBO.Library.GetAll();
            foreach (DBO.Library lib in libraryListing)
            {
                bool found = false;
                foreach (TabPage loadedTab in libraryTabControl.TabPages)
                {
                    if (loadedTab.Text == lib.Name)
                    {
                        found = true;
                        break;
                    }
                }

                if(!found)
                    this.addLibraryTabPage(lib);
            }

            libraryTabControl.SelectedIndex = (int)(libraryTabControl.TabPages.Count - 1);
        }

        private void libraryTabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage == addLibraryPage)
            {
                e.Cancel = true;
                Form addLib = new UI.Dialogs.BaseDialog(new UI.DialogPanels.Library());
                addLib.FormClosing += new FormClosingEventHandler(addLib_FormClosing);
                addLib.ShowDialog();
            }
        }

        private void Main_Shown(object sender, EventArgs e)
        {

        }

        private void addLibraryTabPage(DBO.Library lib)
        {
            TabPage libraryTabPage = new TabPage(lib.Name);
            /* Todo: create a library panel (user control) */
            libraryTabPage.Controls.Add(new UI.Tabpanels.Library(lib));
            libraryTabControl.TabPages.Insert((int)(libraryTabControl.TabPages.Count - 1), libraryTabPage);
        }
    }
}

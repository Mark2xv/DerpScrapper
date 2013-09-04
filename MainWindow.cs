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
    public partial class MainWindow : Form
    {
        private ImageList imageList;
        private TabPage addNewLibraryTab;

        public MainWindow()
        {
            InitializeComponent();

            addNewLibraryTab = tabs.TabPages[0];

            imageList = new ImageList();
            imageList.Images.Add("imgAdd", Resources.Resources.add);

            tabs.ImageList = imageList;
            addNewLibraryTab.ImageKey = "imgAdd";

            tabs.Selecting += tabs_Selecting;
            this.Shown += MainWindow_Shown;
        }

        private void MainWindow_Shown(object sender, EventArgs e)
        {
            var libraries = Library.GetAll();
            if (libraries.Count == 0)
            {
                // Show add new lib 
                forceNewLibraryPage();
            }
            else
            {

            }
        }

        private void forceNewLibraryPage()
        {
            AddNewLibrary addNewDialog = new AddNewLibrary();
            var result = addNewDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // Do stuff
                addNewDialog.NewLibrary.Insert();
                LibraryTab newTab = new LibraryTab(addNewDialog.NewLibrary.Name, addNewDialog.NewLibrary);
                this.tabs.TabPages.Insert(0, newTab);
                this.tabs.SelectTab(newTab);

                if (addNewDialog.SetImportPath)
                {
                    WorkThreadManager.Instance.AddNewTask(
                        importLibraryItemsFromPath, /* Func */
                        new object[] { addNewDialog.NewLibrary, addNewDialog.ImportPath, newTab }, /* Args */
                        true, /* Uses DBO */
                        importLibraryDone, /* Callback */
                        importLibraryProgress /* Progress callback */
                    );
                }
            }
            else
            {
                MessageBox.Show("You need to add at least one collection to continue.");
                this.Close();
            }
        }

        private object importLibraryItemsFromPath(ProgressReporter reporter, object input)
        {
            var args = (object[]) input;

            var library = (Library) args[0];
            var importPath = (string) args[1];
            var libTabForCallback = (LibraryTab) args[2];

            DirectoryInfo[] dirs = new DirectoryInfo(importPath).GetDirectories();
            int at = 0;
            foreach (var dirInfo in dirs)
            {
                at++;

                Serie serie = new Serie();

                serie.LibraryId = library.Id;
                serie.Name = dirInfo.Name;
                serie.FolderPath = dirInfo.FullName;
                serie.FolderIsNetworkMount = false;
                
                serie.Insert();

                reporter.ReportProgress(new object[] {
                    at,
                    dirs.Length,
                    serie,
                    libTabForCallback
                });
                
            }

            return libTabForCallback;
        }

        private void importLibraryDone(object retval)
        {
            ((LibraryTab) retval).LoadingDone();
        }

        private void importLibraryProgress(object progressData)
        {
            object[] args = (object[])progressData;
            int at = (int)args[0];
            int length = (int)args[1];
            Serie serie = (Serie)args[2];
            LibraryTab tab = (LibraryTab)args[3];

            tab.AddSerie(serie);
        }

        public void tabs_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage == addNewLibraryTab)
            {
                AddNewLibrary addNewDialog = new AddNewLibrary();
                var result = addNewDialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    // Do stuff
                    addNewDialog.NewLibrary.Insert();
                    LibraryTab newTab = new LibraryTab(addNewDialog.NewLibrary.Name, addNewDialog.NewLibrary);
                    this.tabs.TabPages.Add(newTab);
                    this.tabs.SelectTab(newTab);

                    if (addNewDialog.SetImportPath)
                    {
                        WorkThreadManager.Instance.AddNewTask(
                            importLibraryItemsFromPath, /* Func */
                            new object[] { addNewDialog.NewLibrary, addNewDialog.ImportPath, newTab }, /* Args */
                            true, /* Uses DBO */
                            importLibraryDone, /* Callback */
                            importLibraryProgress /* Progress callback */
                        );
                    }
                }
            }
        }

        public new void Dispose()
        {
            tabs.Selecting -= tabs_Selecting;
            base.Dispose();
        }

        
    }
}

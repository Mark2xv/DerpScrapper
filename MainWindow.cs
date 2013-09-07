using DerpScrapper.DBO;
using DerpScrapper.Library;
using DerpScrapper.Scraper;
using DerpScrapper.Scrapers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DerpScrapper
{
    public partial class MainWindow : Form, IMessageFilter
    {
        private const string WindowTitleFormat = "DerpScraper - {0} - {1} items";
        private ImageList ImageList;
        private TabPage AddNewLibraryTab;

        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(Point pt);
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        public MainWindow()
        {
            InitializeComponent();
            this.Text = "DerpScraper";

            AddNewLibraryTab = LibraryTabs.TabPages[0];

            ImageList = new ImageList();
            ImageList.Images.Add("imgAdd", Resources.Resources.add);

            LibraryTabs.ImageList = ImageList;
            AddNewLibraryTab.ImageKey = "imgAdd";

            LibraryTabs.Selecting += LibraryTabs_Selecting;
            this.Shown += MainWindow_Shown;
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == 0x20a)
            {
                // WM_MOUSEWHEEL, find the control at screen position m.LParam
                Point pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
                IntPtr hWnd = WindowFromPoint(pos);
                if (hWnd != IntPtr.Zero && hWnd != m.HWnd && Control.FromHandle(hWnd) != null)
                {
                    SendMessage(hWnd, m.Msg, m.WParam, m.LParam);
                    return true;
                }
            }
            return false;
        }

        private void MainWindow_Shown(object sender, EventArgs e)
        {
            var libraries = DBO.Library.GetAll();
            if (libraries.Count == 0)
            {
                // Show add new lib 
                ForceNewLibraryPage();
            }
            else
            {
                //Add Library Tabs to TabControls
                foreach (var library in libraries)
                {
                    LibraryTab libraryTab = new LibraryTab(library.Name, library);
                    this.LibraryTabs.TabPages.Insert(0, libraryTab);
                }
            }
        }

        private void ForceNewLibraryPage()
        {
            AddNewLibrary addNewDialog = new AddNewLibrary();
            var dialogResult = addNewDialog.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                // Do stuff
                addNewDialog.NewLibrary.Insert();
                LibraryTab newTab = new LibraryTab(addNewDialog.NewLibrary.Name, addNewDialog.NewLibrary);
                this.LibraryTabs.TabPages.Insert(0, newTab);
                this.LibraryTabs.SelectTab(newTab);

                if (addNewDialog.SetImportPath)
                {
                    WorkThreadManager.Instance.AddNewTask(
                        ImportLibraryItemsFromPath, /* Func */
                        new object[] { addNewDialog.NewLibrary, addNewDialog.ImportPath, newTab }, /* Args */
                        true, /* Uses DBO */
                        ImportLibrary_Done, /* Callback */
                        ImportLibrary_Progress /* Progress callback */
                    );
                }
                else
                {
                    newTab.LoadingDone();
                }
            }
            else
            {
                MessageBox.Show("You need to add at least one collection to continue.");
                this.Close();
            }
        }

        private object ImportLibraryItemsFromPath(ProgressReporter reporter, object input)
        {
            var args = (object[]) input;

            var library = (DBO.Library) args[0];
            var importPath = (string) args[1];
            var libTabForCallback = (LibraryTab) args[2];

            DirectoryInfo[] dirs = new DirectoryInfo(importPath).GetDirectories();
            int at = 0;
            foreach (var dirInfo in dirs)
            {
                at++;

                // Parse name into something usable
                string cleanName = ScraperUtility.CleanUpName(dirInfo.Name.Replace('_', ' '), false);

                Serie serie = new Serie();

                serie.LibraryId = library.Id;
                serie.Name = cleanName;
                serie.FolderPath = dirInfo.FullName;
                serie.FolderIsNetworkMount = false;
                
                serie.Insert();

                reporter.ReportProgress(new object[] {
                    at,
                    dirs.Length,
                    serie,
                    libTabForCallback
                });
                break;
            }

            return libTabForCallback;
        }

        private void ImportLibrary_Done(object retval)
        {
            ((LibraryTab) retval).LoadingDone();
        }

        private void ImportLibrary_Progress(object progressData)
        {
            object[] args = (object[])progressData;
            int at = (int)args[0];
            int length = (int)args[1];
            Serie serie = (Serie)args[2];
            LibraryTab tab = (LibraryTab)args[3];

            var item = tab.AddItem(serie);

            WorkThreadManager.Instance.AddNewTask(RetrieveMetadataForSerie, new object[] { serie, item }, true, RetrieveMetadataForSerie_Done);
        }

        private object RetrieveMetadataForSerie(ProgressReporter progress, object argsAr)
        {
            var args = (object[]) argsAr;

            var serie = (Serie) args[0];
            var tab = (LibraryItem) args[1];

            TVDBScraper scraper = new TVDBScraper();
            var serieInfoTask = scraper.FindSerie(serie.Name);
            serieInfoTask.Wait();
            var result = serieInfoTask.Result;

            if (result.FailedHit)
            {
                // No hits found at all
            }
            else if (result.UncertainHit)
            {
                // Multiple hits found - show overlay on image
            }


            return null;
        }

        private void RetrieveMetadataForSerie_Done(object args)
        {

        }

        private void LibraryTabs_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage == AddNewLibraryTab)
            {
                AddNewLibrary addNewDialog = new AddNewLibrary();
                var result = addNewDialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    // Do stuff
                    addNewDialog.NewLibrary.Insert();
                    LibraryTab newTab = new LibraryTab(addNewDialog.NewLibrary.Name, addNewDialog.NewLibrary);
                    this.LibraryTabs.TabPages.Add(newTab);
                    this.LibraryTabs.SelectTab(newTab);

                    if (addNewDialog.SetImportPath)
                    {
                        WorkThreadManager.Instance.AddNewTask(
                            ImportLibraryItemsFromPath, /* Func */
                            new object[] { addNewDialog.NewLibrary, addNewDialog.ImportPath, newTab }, /* Args */
                            true, /* Uses DBO */
                            ImportLibrary_Done, /* Callback */
                            ImportLibrary_Progress /* Progress callback */
                        );
                    }
                }
            }
            else
            {
                var tab = (LibraryTab)e.TabPage;
                this.Text = string.Format(WindowTitleFormat, tab.Library.Name, 0);
            }
        }

        public new void Dispose()
        {
            LibraryTabs.Selecting -= LibraryTabs_Selecting;
            base.Dispose();
        }

        
    }
}

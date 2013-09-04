using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DerpScrapper.DBO;
using System.Windows.Forms;
using System.Drawing;

namespace DerpScrapper
{
    class LibraryTab : System.Windows.Forms.TabPage
    {
        Panel loaderPanel;
        PictureBox loading;
        Label loadingText;

        ListView seriesList;

        public bool HasLibrary 
        { 
            get 
            {
                return Library != null;
            } 
        }

        public Library Library { get; private set; }

        public LibraryTab(string name, Library lib = null)
            : base(name)
        {
            if (lib != null)
            {
                Library = lib;
            }


            this.Controls.AddRange(new Control[] {
                    loaderPanel = new Panel() {
                        Anchor = AnchorStyles.None,
                        BackColor = Color.FromArgb(125,0,0,0)
                    },
                    seriesList = new ListView() {
                        View = View.LargeIcon,
                        Dock = DockStyle.Fill
                    }
                }
            );
            SetDoubleBuffered(seriesList);

            ImageList list = new ImageList();
            list.ImageSize = new System.Drawing.Size(69, 100);
            list.Images.Add(Resources.Resources.thumb_asdf_0506);
            seriesList.LargeImageList = list;

            loaderPanel.Controls.AddRange(new Control[] { 
                loading = new PictureBox() {
                    Size = new System.Drawing.Size(24, 24),
                    Image = Resources.Resources.spinner,
                    Anchor = AnchorStyles.None
                },
                loadingText = new Label() { 
                    Text = "Busy constructing the collection...",
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                    Anchor = AnchorStyles.None,
                    BackColor = Color.Transparent,
                    ForeColor = Color.White
                }
            });

            loading.Center();
            loadingText.Center();
            loaderPanel.Center();

            loading.Location = new Point(loading.Location.X, loading.Location.Y - 40);
        }

        public void AddSerie(Serie serie)
        {
            this.seriesList.Items.Add(serie.Name, 0);
        }

        public void LoadingDone()
        {
            this.Controls.Remove(loaderPanel);
            loaderPanel.Dispose();
        }

        private static void SetDoubleBuffered(System.Windows.Forms.Control c)
        {
            //Taxes: Remote Desktop Connection and painting
            //http://blogs.msdn.com/oldnewthing/archive/2006/01/03/508694.aspx
            if (System.Windows.Forms.SystemInformation.TerminalServerSession)
                return;

            System.Reflection.PropertyInfo aProp =
                  typeof(System.Windows.Forms.Control).GetProperty(
                        "DoubleBuffered",
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance);

            aProp.SetValue(c, true, null);
        }
    }
}

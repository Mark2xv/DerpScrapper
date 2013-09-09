using DerpScrapper.DBO;
using DerpScrapper.Library;
using DerpScrapper.Scrapers;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DerpScrapper
{
    class LibraryTab : System.Windows.Forms.TabPage
    {
        Panel loaderPanel;
        PictureBox loading;
        Label loadingText;

        FlowLayoutPanel seriesListPanel;

        private bool LoadingState = false;

        public bool HasLibrary 
        { 
            get 
            {
                return Library != null;
            } 
        }

        public DBO.Library Library { get; private set; }

        public LibraryTab(string name, DBO.Library lib = null)
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
                    seriesListPanel = new FlowLayoutPanel {
                        Dock = DockStyle.Fill,
                        FlowDirection = FlowDirection.LeftToRight,
                        WrapContents = true,
                        AutoScroll = true
                    }
                }
            );            

            loaderPanel.Controls.AddRange(new Control[] { 
                loading = new PictureBox() {
                    Size = new System.Drawing.Size(24, 24),
                    Image = Resources.Resources.spinner,
                    Anchor = AnchorStyles.None
                },
                loadingText = new Label() { 
                    Text = "Busy building your collection ...",
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                    Anchor = AnchorStyles.None,
                    BackColor = Color.Transparent,
                    ForeColor = Color.White
                }
            });

            seriesListPanel.MouseEnter += (s,e) => {
                seriesListPanel.Focus();
            };

            loading.Center();
            loadingText.Center();
            loaderPanel.Center();

            LoadingState = true;

            loading.Location = new Point(loading.Location.X, loading.Location.Y - 40);
        }

        public LibraryItem AddItem(SerieInfo serie)
        {
            LibraryItem libItem;
            this.seriesListPanel.Controls.Add(libItem = new LibraryItem(serie));
            libItem.Active = !LoadingState;
            return libItem;
        }

        //public LibraryItem AddItem(string queryName, List<UncertainSerieHit> uncertainHits)
        //{
        //    LibraryItem libItem;
        //    this.seriesListPanel.Controls.Add(libItem = new LibraryItem(queryName, uncertainHits));
        //    libItem.Active = !LoadingState;
        //    return libItem;
        //}

        public void LoadingDone()
        {
            this.Controls.Remove(loaderPanel);
            loaderPanel.Dispose();
            LoadingState = false;

            foreach (Control c in seriesListPanel.Controls)
            {
                if (c is LibraryItem)
                {
                    ((LibraryItem)c).Active = true;
                }
            }
        }
    }
}

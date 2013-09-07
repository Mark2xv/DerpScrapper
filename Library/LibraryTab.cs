using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DerpScrapper.DBO;
using System.Windows.Forms;
using System.Drawing;
using DerpScrapper.Scrapers;
using DerpScrapper.Library;

namespace DerpScrapper
{
    class LibraryTab : System.Windows.Forms.TabPage
    {
        Panel loaderPanel;
        PictureBox loading;
        Label loadingText;

        FlowLayoutPanel seriesListPanel;

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

            loading.Location = new Point(loading.Location.X, loading.Location.Y - 40);
        }

        public LibraryItem AddItem(Serie serie)
        {
            LibraryItem libItem;
            this.seriesListPanel.Controls.Add(libItem = new LibraryItem(serie));
            return libItem;
        }

        public LibraryItem AddItem(string queryName, List<UncertainSerieHit> uncertainHits)
        {
            LibraryItem libItem;
            this.seriesListPanel.Controls.Add(libItem = new LibraryItem(queryName, uncertainHits));
            return libItem;
        }

        public void LoadingDone()
        {
            this.Controls.Remove(loaderPanel);
            loaderPanel.Dispose();
        }
    }
}

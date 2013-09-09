using DerpScrapper.DBO;
using DerpScrapper.Scrapers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DerpScrapper.Library
{
    public class LibraryItem : Panel
    {
        private bool _active;
        public bool Active
        {
            set
            {
                if (value)
                {
                    this.Click += LibraryItem_Click;
                    Image.Click += LibraryItem_Click;
                    Label.Click += LibraryItem_Click;
                }
                else
                {
                    this.Click -= LibraryItem_Click;
                    Image.Click -= LibraryItem_Click;
                    Label.Click -= LibraryItem_Click;
                }
                _active = value;
            }
            get
            {
                return _active;
            }
        }

        private bool UncertainMode;
        private bool UnknownMode;

        private SerieInfo Serie;
        private List<UncertainSerieHit> UncertainHits;

        private PictureBox Image;
        private Label Label;

        public LibraryItem(SerieInfo serie, string imageUrl = "")
        {
            Image = new PictureBox()
            {
                Size = new Size(Resources.Resources.thumb_asdf_0506.Width, Resources.Resources.thumb_asdf_0506.Height)
            };
            Image.SizeMode = PictureBoxSizeMode.StretchImage;
            this.Controls.Add(Image);

            this.Controls.Add(Label = new Label()
            {
                Location = new Point(0, Image.Size.Height + 10)
            });

            this.Size = new Size(Image.Size.Width, Image.Size.Height + Label.Size.Height + 10);
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;

            this.Reload(serie, imageUrl);

            Active = true;
        }

        public LibraryItem(string queryName, List<UncertainSerieHit> hits)
        {
            this.Controls.Add(Image = new PictureBox()
            {
                Size = new Size(Resources.Resources.thumb_asdf_0506.Width, Resources.Resources.thumb_asdf_0506.Height)
            });

            this.Controls.Add(Label = new Label()
            {
                Text = queryName,
                Location = new Point(0, Image.Size.Height + 10)
            });
        }

        public virtual void LibraryItem_Click(object sender, EventArgs e)
        {
            if (Active)
            {
                if (this.UnknownMode)
                {
                    FixMismatch fixMatch = new FixMismatch(Serie.Serie);
                    Console.WriteLine("CLICK: FIX NON-MATCH");
                    if (fixMatch.ShowDialog() == DialogResult.OK)
                    {
                        // Reload
                        
                    }
                }
                else if (this.UncertainMode)
                {
                    SelectHit selectHitDialog = new SelectHit(Serie, UncertainHits);
                    Console.WriteLine("CLICK: FIX MIS-MATCH");
                    if (selectHitDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Reload
                        Console.WriteLine("Reload item {0} (rId={1}) to {2} (rId={3})", this.Serie.Serie.Name, this.Serie.Resource.ExternalSerieId, selectHitDialog.currentSelected.Serie.Serie.Name, selectHitDialog.currentSelected.Serie.Resource.ExternalSerieId);

                        WorkThreadManager.Instance.AddNewTask(
                            MainWindow.Instance.ReloadItem, 
                            new object[] { Serie, selectHitDialog.currentSelected.Serie.Resource.ExternalSerieId, this }, 
                            true, 
                            MainWindow.Instance.ReloadItemDone
                        );

                        this.SetBusyState();
                    }
                }
                else
                {
                    // Open Serie dialog/screen
                    Console.WriteLine("CLICK: NORMAL");
                }
            }
            else
            {
                Console.WriteLine("NOT ACTIVE");
            }
        }

        public void Reload(SerieInfo serie, string imageUrl)
        {
            this.Serie = serie;
            this.SetOKState();

            if (serie.PosterImage.Url != "")
            {
                imageUrl = serie.PosterImage.Url;
                Console.WriteLine("Rerouted imageUrl to " + imageUrl);
            }

            if (imageUrl == "")
            {
                Image.Image = Resources.Resources.thumb_asdf_0506;
            }
            else
            {
                Image.ImageLocation = imageUrl;
            }

            Label.Text = serie.Serie.Name;
        }

        public void SetOKState()
        {
            this.BackColor = Color.Green;
            this.Cursor = Cursors.Default;

            this.UncertainMode = false;
            this.UnknownMode = false;
        }

        public void SetBusyState()
        {
            this.BackColor = Color.Orange;
            this.Cursor = Cursors.Default;

            this.UncertainMode = false;
            this.UnknownMode = false;
        }

        public void SetUnknownState()
        {
            this.BackColor = Color.Red;
            this.Cursor = Cursors.Help;

            this.UncertainMode = false;
            this.UnknownMode = true;
        }

        public void SetMultipleHitsState(List<UncertainSerieHit> hits)
        {
            this.BackColor = Color.Blue;
            this.Cursor = Cursors.Help;

            this.UncertainMode = true;
            this.UnknownMode = false;

            this.UncertainHits = hits;
        }
    }
}

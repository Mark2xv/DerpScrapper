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

        private Serie Serie;
        private List<UncertainSerieHit> UncertainHits;

        private PictureBox Image;
        private Label Label;

        public LibraryItem(Serie serie, string imageUrl = "")
        {
            Image = new PictureBox()
            {
                Size = new Size(Resources.Resources.thumb_asdf_0506.Width, Resources.Resources.thumb_asdf_0506.Height)
            };

            if (imageUrl == "")
            {
                Image.Image = Resources.Resources.thumb_asdf_0506;
            }
            else
            {
                Console.WriteLine(imageUrl);
                Image.ImageLocation = imageUrl;
            }

            this.Controls.Add(Image);

            this.Controls.Add(Label = new Label()
            {
                Text = serie.Name,
                Location = new Point(0, Image.Size.Height + 10)
            });

            this.Size = new Size(Image.Size.Width, Image.Size.Height + Label.Size.Height + 10);
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Serie = serie;

            Active = true;
        }

        public virtual void LibraryItem_Click(object sender, EventArgs e)
        {
            if (Active)
            {
                if (this.UnknownMode)
                {
                    FixMismatch fixMatch = new FixMismatch(Serie);
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

        public void SetOKState()
        {
            this.BackColor = Color.Green;
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

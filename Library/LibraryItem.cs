using DerpScrapper;
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
        PictureBox Image;
        Label Label;
        public LibraryItem(Serie serie)
        {
            this.Controls.Add(Image = new PictureBox()
            {
                Image = Resources.Resources.thumb_asdf_0506,
                Size = new Size(Resources.Resources.thumb_asdf_0506.Width, Resources.Resources.thumb_asdf_0506.Height)
            });

            this.Controls.Add(Label = new Label()
            {
                Text = serie.Name,
                Location = new Point(0, Image.Size.Height + 10)
            });

            this.Size = new Size(Image.Size.Width, Image.Size.Height + Label.Size.Height + 10);

            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
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
    }
}

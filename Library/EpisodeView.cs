using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DerpScrapper.DBO;

namespace DerpScrapper.Library
{
    public partial class EpisodeView : UserControl
    {
        public EpisodeView(Episode ep)
        {
            InitializeComponent();

            var image = ep.Image;
            if(image != null) 
            {
                Console.WriteLine(image.RemoteURL);
                PictureEpisode.ImageLocation = image.RemoteURL;
            }
            LabelEpisodeName.Text = ep.EpisodeName;
        }
    }
}

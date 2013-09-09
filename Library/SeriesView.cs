using DerpScrapper.DBO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DerpScrapper.Library
{
    public partial class SeriesView : Form
    {
        public SeriesView(SerieInfo serie)
        {
            InitializeComponent();
            this.Icon = Resources.Resources.books;

            LabelSerieName.Text = serie.Serie.Name;
            LabelSerieSynopsis.Text = serie.Serie.Plot;
            SerieImage.ImageLocation = serie.PosterImage.Url;

            foreach (var seasonGroup in serie.Episodes.GroupBy(p => p.SeasonNumber))
            {
                Label label = new Label();
                label.Text = "Season " + seasonGroup.Key;
                FlowLayout.Controls.Add(label);

                EpisodeView lastViewAdded = null;
                foreach(var episode in seasonGroup) {
                    EpisodeView epView = new EpisodeView(episode);
                    FlowLayout.Controls.Add(epView);
                    lastViewAdded = epView;
                }
                FlowLayout.SetFlowBreak(lastViewAdded, true);
            }

        }

        private void SeriesView_Load(object sender, EventArgs e)
        {

        }

        public void SelectItem(EpisodeView epItem)
        {
            foreach (Control control in FlowLayout.Controls)
            {
                if (control is EpisodeView)
                {
                    var epCtrl = (EpisodeView) control;
                    if (control == epItem)
                    {
                        epCtrl.BorderStyle = BorderStyle.Fixed3D;
                        epCtrl.BackColor = Color.Green;
                    }
                    else
                    {
                        epCtrl.BorderStyle = BorderStyle.None;
                        epCtrl.BackColor = Color.Transparent;
                    }
                }
                else
                {
                    // nothing
                }
            }
        }
    }
}

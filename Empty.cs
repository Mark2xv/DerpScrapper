using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DerpScrapper.DBO;
using DerpScrapper.DownloadSite_Scrapers;
using DerpScrapper.Downloader;
using DerpScrapper.Scrapers;
using System.IO;

namespace DerpScrapper
{
    public partial class Empty : Form
    {
        public Empty()
        {
            InitializeComponent();
            this.Shown += new EventHandler(Empty_Shown);
        }

        void Empty_Shown(object sender, EventArgs e)
        {
            StartWorkStuff();
        }

        public void CallbackForThreads(object retVal)
        {
            DerpThing thing = (DerpThing)retVal;

            string hits = "";
            foreach(var hit in thing.List) {
                hits += hit.name + " @ " + hit.url + "\r\n\r\n";
            }

            switch (thing.idx)
            {
                case 1:
                    label1.Text = thing.Name;
                    textBox1.Text = hits;
                    break;
                case 2:
                    label2.Text = thing.Name;
                    textBox2.Text = hits;
                    break;
                case 3:
                    label3.Text = thing.Name;
                    textBox3.Text = hits;
                    break;
                case 4:
                    label4.Text = thing.Name;
                    textBox4.Text = hits;
                    break;
            }
        }

        public void StartWorkStuff()
        {
            Func<object, object> task1 = (object a) =>
            {
                string query = (string)a;

                Serie serie = Serie.GetByName(query);
                SerieInfo info;
                if (serie == null)
                {
                    TVDBScraper tvDb = new TVDBScraper();
                    info = tvDb.FindAllInformationForSerie(query);

                    int serieId = info.serie.Insert();
                    info.metadata.SerieId = serieId;
                    info.resource.SerieId = serieId;

                    info.metadata.Insert();
                    info.resource.Insert();

                    foreach (SerieGenre gen in info.genres)
                    {
                        gen.SerieId = serieId;
                        gen.Insert();
                    }

                    foreach (Episode ep in info.episodes)
                    {
                        ep.SerieId = serieId;
                        ep.Insert();
                    }

                    serie = info.serie;
                }
                else
                {
                    info = new SerieInfo(serie);
                }

                var nyaaScraper = new BakaBT();
                var downloads = nyaaScraper.GetDownloadsForEntireSerie(info);

                foreach (var dl in downloads)
                {
                    UTorrent.AddTorrent(new Uri(dl.url));
                }

                return new DerpThing() { Name = query, List = downloads, idx = 1 };
            };


            WorkThreadManager.Instance.AddNewTask(task1, "pokemon", true, CallbackForThreads);
        }

        class DerpThing
        {
            public string Name;
            public List<PossibleDownloadHit> List;

            public int idx;
        }
    }
}

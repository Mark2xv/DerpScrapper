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

                TVDBScraper tvDb = new TVDBScraper();
                SerieInfo serieInfo = tvDb.FindAllInformationForSerie(query);

                int serieId = serieInfo.serie.Insert();
                serieInfo.metadata.SerieId = serieId;
                serieInfo.resource.SerieId = serieId;

                serieInfo.metadata.Insert();
                serieInfo.resource.Insert();

                foreach (SerieGenre gen in serieInfo.genres)
                {
                    gen.SerieId = serieId;
                    gen.Insert();
                }

                foreach (Episode ep in serieInfo.episodes)
                {
                    ep.SerieId = serieId;
                    ep.Insert();
                }

                var x = new Nyaa();
                return new DerpThing() { Name = query, List = null, idx = 1 };
            };

            Func<object, object> task2 = (object a) =>
            {
                Serie serie = new Serie();
                serie["Name"] = (string)a;

                var x = new Nyaa();
                var list = x.GetDownloadsForEntireSerie(serie, null);
                return new DerpThing() { Name = serie["Name"].ToString(), List = list, idx = 2 };
            };

            Func<object, object> task3 = (object a) =>
            {
                Serie serie = new Serie();
                serie["Name"] = (string)a;

                var x = new Nyaa();
                var list = x.GetDownloadsForEntireSerie(serie, null);
                return new DerpThing() { Name = serie["Name"].ToString(), List = list, idx = 3 };
            };

            Func<object, object> task4 = (object a) =>
            {
                Serie serie = new Serie();
                serie["Name"] = (string)a;

                var x = new Nyaa();
                var list = x.GetDownloadsForEntireSerie(serie, null);
                return new DerpThing() { Name = serie["Name"].ToString(), List = list, idx = 4 };
            };

            WorkThreadManager.Instance.AddNewTask(task1, "Sword art online", true, CallbackForThreads);
            //WorkThreadManager.Instance.AddNewTask(task2, "Dog Days", false, CallbackForThreads);
            //WorkThreadManager.Instance.AddNewTask(task3, "Hyouka", true, CallbackForThreads);
            //WorkThreadManager.Instance.AddNewTask(task4, "Psycho-Pass", false, CallbackForThreads);
        }

        class DerpThing
        {
            public string Name;
            public List<PossibleDownloadHit> List;

            public int idx;
        }
    }
}

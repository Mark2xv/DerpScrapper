using DerpScrapper.DBO;
using DerpScrapper.Scrapers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DerpScrapper.Library
{
    public partial class SelectHit : Form
    {
        private List<UncertainSerieHit> Options;
        private List<SelectNewHit> Selectables;
        public SelectNewHit currentSelected;

        public SelectHit(SerieInfo serie, List<UncertainSerieHit> hits)
        {
            InitializeComponent();
            this.Icon = Resources.Resources.books; 
            Options = hits;
            Selectables = new List<SelectNewHit>();

            foreach (var hit in hits)
            {
                var select = new SelectNewHit(this, hit);
                select.Width = 300;
                select.Selected = false;
                SelectableItems.Controls.Add(select);
                Selectables.Add(select);
            }

            DialogResult = System.Windows.Forms.DialogResult.Cancel;


            if (serie.Serie.FolderPath != "")
            {
                LabelPath.Text = serie.Serie.FolderPath;
            }
            else
            {
                Controls.Remove(LabelPath);
            }
        }

        public void SelectItem(SelectNewHit item)
        {
            Console.WriteLine(item.UncertainHit.Name);
            item.BackColor = Color.Green;
            currentSelected = item;

            foreach (var selectable in this.Selectables)
            {
                if (selectable != item)
                {
                    selectable.BackColor = Color.Transparent;
                }
            }
        }

        

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            if (currentSelected != null)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select the serie");
            }
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }

    public class SelectNewHit : LibraryItem
    {
        public bool Selected = false;
        public UncertainSerieHit UncertainHit;
        private SelectHit BaseForm;

        public SelectNewHit(SelectHit parent, UncertainSerieHit uncertainHit)
            : base(new SerieInfo() { 
                Serie = new Serie() 
                {
                    Name = uncertainHit.Name 
                },
                Resource = new SerieResource()
                {
                    ExternalSerieId = uncertainHit.Id
                }
            }, uncertainHit.Image.ToString())
        {
            this.BaseForm = parent;
            this.UncertainHit = uncertainHit;
        }

        public override void LibraryItem_Click(object sender, EventArgs e)
        {
            this.BaseForm.SelectItem(this);
        }
    }
}

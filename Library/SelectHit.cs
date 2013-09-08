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
        SelectNewHit currentSelected;

        public SelectHit(Serie serie, List<UncertainSerieHit> hits)
        {
            InitializeComponent();
            this.Icon = Resources.Resources.books; 
            Options = hits;
            Selectables = new List<SelectNewHit>();

            foreach (var hit in hits)
            {
                var select = new SelectNewHit(this, hit);
                select.Width = this.SelectableItems.Width;
                select.Selected = false;
                SelectableItems.Controls.Add(select);
                Selectables.Add(select);
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

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            switch (m.Msg)
            {
                case 0x84: //WM_NCHITTEST
                    var result = (HitTest) m.Result.ToInt32();
                    if (result == HitTest.Left || result == HitTest.Right)
                        m.Result = new IntPtr((int) HitTest.Caption);
                    if (result == HitTest.TopLeft || result == HitTest.TopRight)
                        m.Result = new IntPtr((int) HitTest.Top);
                    if (result == HitTest.BottomLeft || result == HitTest.BottomRight)
                        m.Result = new IntPtr((int) HitTest.Bottom);

                    break;
            }
        }
        enum HitTest
        {
            Caption = 2,
            Transparent = -1,
            Nowhere = 0,
            Client = 1,
            Left = 10,
            Right = 11,
            Top = 12,
            TopLeft = 13,
            TopRight = 14,
            Bottom = 15,
            BottomLeft = 16,
            BottomRight = 17,
            Border = 18
        }
    }

    public class SelectNewHit : LibraryItem
    {
        public bool Selected = false;
        public UncertainSerieHit UncertainHit;
        SelectHit BaseForm;

        public SelectNewHit(SelectHit parent, UncertainSerieHit uncertainHit)
            : base(new Serie() { Name = uncertainHit.Name}, uncertainHit.Image.ToString() )
        {
            this.BaseForm = parent;
            this.UncertainHit = uncertainHit;
        }

        public override void LibraryItem_Click(object sender, EventArgs e)
        {
            Console.WriteLine("NEW HIT THINGY CLICK");
            this.BaseForm.SelectItem(this);
        }
    }
}

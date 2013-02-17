using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DerpScrapper
{
    public partial class TempForm : Form
    {
        TabPage addPage;

        public TempForm()
        {
            InitializeComponent();

            var imageList = new ImageList();
            Image im = Image.FromFile(@"C:\Users\Mark\Desktop\plus_icon.png");
            imageList.Images.Add("addImage", im);
            imageList.ImageSize = new System.Drawing.Size(16, 16);
            tbc_main.ImageList = imageList;

            addPage = new TabPage("");
            addPage.ImageKey = "addImage";
            addPage.ImageIndex = imageList.Images.IndexOfKey("addImage");

            tbc_main.TabPages.Add(addPage);
        }

        private void tbc_main_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.Action == TabControlAction.Selecting && (e.TabPage == addPage || ((TabControl)e.TabPage.Parent).TabPages.Count == 1))
            {
                e.Cancel = true;
                Console.WriteLine("Add Lib tab clicked (or will show otherwise)");
                new AddLibDialog().ShowDialog();
            }
        }

        private void TempForm_Shown(object sender, EventArgs e)
        {
            this.tbc_main_Selecting(null, new TabControlCancelEventArgs(addPage, 0, false, TabControlAction.Selecting));
        }
    }

    class AddLibDialog : Form
    {
        TextBox txt_libName;
        Button OK, Cancel;

        public AddLibDialog()
        {
            this.SuspendLayout();

            this.ClientSize = new Size(200, 300);

            this.Controls.AddRange(new Control[] {
                txt_libName = new TextBox() {
                    Left = 10,
                    Top = 10,
                    Anchor = AnchorStyles.Left | AnchorStyles.Top
                },
                OK = new Button() { 
                    Text = "OK", 
                    Left = 10,
                    Top = this.ClientSize.Height - 25,
                    Width = 50,
                    Anchor = AnchorStyles.Left | AnchorStyles.Bottom
                },
                Cancel = new Button() { 
                    Text = "Cancel", 
                    Left = this.ClientSize.Width - 60,
                    Top = this.ClientSize.Height - 25,
                    Width = 50,
                    Anchor = AnchorStyles.Right | AnchorStyles.Bottom
                }
            });
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;

            this.ResumeLayout(false);
            this.PerformLayout();

            OK.Click += (a,b) => { okPressed(); };
            Cancel.Click += (a, b) => { cancelPressed(); };
            
        }

        private void okPressed()
        {
        }

        private void cancelPressed()
        {

        }
    }
}

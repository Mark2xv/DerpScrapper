using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DerpScrapper.UI.Tabpanels
{
    public partial class Library : UserControl
    {
        public DBO.Library libraryObject = null;
        protected List<DBO.Serie> series = new List<DBO.Serie>();

        public Library(DBO.Library library)
        {
            InitializeComponent();
            libraryObject = library;            
        }
    }
}

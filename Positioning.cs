using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DerpScrapper
{
    public static class Positioning
    {
        /// <summary>
        /// Centers the control both horizontially and vertically 
        /// according to the parent control that contains it.
        /// </summary>
        /// <param name="control"></param>
        public static void Center(this Control control)
        {
            control.CenterHorizontally();
            control.CenterVertically();
        }

        /// <summary>
        /// Centers the control horizontially according 
        /// to the parent control that contains it.
        /// </summary>
        public static void CenterHorizontally(this Control control)
        {
            Rectangle parentRect = control.Parent.ClientRectangle;
            control.Left = (parentRect.Width - control.Width) / 2;
        }

        /// <summary>
        /// Centers the control vertically according 
        /// to the parent control that contains it.
        /// </summary>
        public static void CenterVertically(this Control control)
        {
            Rectangle parentRect = control.Parent.ClientRectangle;
            control.Top = (parentRect.Height - control.Height) / 2;
        }
    }
}

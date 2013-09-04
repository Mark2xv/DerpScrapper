using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DerpScrapper
{
    public class WPFWindow : Window
    {
        private Canvas canvas = new Canvas();
        private Grid grid = new Grid();

        public WPFWindow()
        {
            this.AllowsTransparency = true;
            this.WindowStyle = WindowStyle.None;
            this.Background = new SolidColorBrush( Color.FromArgb(255,0,0,0) );
            this.Topmost = true;

            this.DragMove();

            this.Width = 400;
            this.Height = 300;
            canvas.Width = this.Width;
            canvas.Height = this.Height;
            canvas.Background = new SolidColorBrush( Color.FromArgb(255,0,0,0) );
            this.Content = canvas;

            canvas.Children.Add(grid);

            //grid.Children.Add(new Label() { Content = "BLAAT", Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)) }.set);
        }
    }
}

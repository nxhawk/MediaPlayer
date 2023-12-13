using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MediaPlayer.Views.Dialog
{
    /// <summary>
    /// Interaction logic for DontDeleteThis.xaml
    /// </summary>
    public partial class DontDeleteThis : Window
    {
        public DontDeleteThis()
        {
            InitializeComponent();
            Uri icon = new Uri("pack://application:,,,/Assets/Images/title.png", UriKind.RelativeOrAbsolute);
            Icon = BitmapFrame.Create(icon);
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}

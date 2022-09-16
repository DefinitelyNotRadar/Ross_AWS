using Ross.Map;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfControlLibrary1;

namespace Ross
{
    public partial class MainWindow : Window
    {
        private void MapLayout_OnRadioJammingMode(object sender, Tabl e)
        {
           
        }

        private void MapLayout_OnRadioIntelligenceMode(object sender, Tabl e)
        {

        }

        private void MapLayout_OnPreparationMode(object sender, Tabl e)
        {

        }

        private void MapLayout_OnPoll(object sender, Tabl e)
        {
            
        }

        private void ToggleButton_Map_Click(object sender, RoutedEventArgs e)
        {
            if (mapLayout.IsVisible)
                mapLayout.Hide();
            else
                mapLayout.Show();
        }

        private void MapLayout_Closing(object sender, CancelEventArgs e)
        {
            ToggleButton_Map.IsChecked = false;
        }
    }
}

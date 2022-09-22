using Ross.Map;
using Ross.Map._EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ross
{
    public partial class MainWindow : Window
    {
        private void InitializeMapLayout()
        {
            mapLayout = new MapLayout();
            mapLayout.Closing += MapLayout_Closing;
            mapLayout.OnPoll += MapLayout_OnPoll;
            mapLayout.OnPreparationMode += MapLayout_OnPreparationMode;
            mapLayout.OnRadioIntelligenceMode += MapLayout_OnRadioIntelligenceMode;
            mapLayout.OnRadioJammingMode += MapLayout_OnRadioJammingMode;
            mapLayout.OnCoordASPPropertyGridSelecteted += MapLayout_OnCoordASPPropertyGridSelecteted;
            mapLayout.OnCoordControlPoinChanged += MapLayout_OnCoordControlPoinChanged;
        }
    }
}

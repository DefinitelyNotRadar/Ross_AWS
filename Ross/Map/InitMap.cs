using EvaTable;
using Ross.Map;
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
            mapLayout.OnNeedToRedrawMapObjects += MapLayout_OnNeedToRedrawMapJojects;
            mapLayout.OnTableItemDoubleClicked += MapLayout_OnTableItemDoubleClicked;
            mapLayout.RouteChanged += MapLayout_RouteChanged;
            mapLayout.RouteDeleted += MapLayout_RouteDeleted;
            mapLayout.RouteClear += MapLayout_RouteClear;
        }

    }
}

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
            var IsSeccess = SelectedByConnectionTypeClient.SendMode(0);

            if (IsSeccess)
            {
                var newTabl = e;//.Clone();
                mapLayout.SetStationInEvaTable(newTabl, e);
            }
        }

        private void MapLayout_OnRadioIntelligenceMode(object sender, Tabl e)
        {
            var IsSeccess = SelectedByConnectionTypeClient.SendMode(0);

            if (IsSeccess)
            {
                var newTabl = e;//.Clone();
                mapLayout.SetStationInEvaTable(newTabl, e);
            }
        }

        private void MapLayout_OnPreparationMode(object sender, Tabl e)
        {
            var IsSeccess = SelectedByConnectionTypeClient.SendMode(0);

            if (IsSeccess)
            {
                var newTabl = e;//.Clone();
                mapLayout.SetStationInEvaTable(newTabl, e);
            }
        }

        private void MapLayout_OnPoll(object sender, Tabl e)
        {
            Poll();
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

        private void DrawAllObjects()
        {
            //mapLayout.RastrMap.mapControl.RemoveAllObjects();

            //DrawAllASP();

            //DrawAllFWS();

            //DrawAllFHSS();
        }

        private void DrawAllASP()
        {
            foreach(var asp in lASP)
            {
                mapLayout.DrawStation(asp.Coordinates);
            }
        }

        private void DrawAllFWS()
        {
            foreach (var fws in lReconFWS)
                mapLayout.DrawSourceFWS(fws.Coordinates, DLLSettingsControlPointForMap.Model.ColorsForMap.Yellow);
            foreach (var fws in lSuppressFWS)
                mapLayout.DrawSourceFWS(fws.Coordinates, DLLSettingsControlPointForMap.Model.ColorsForMap.Red);
        }

        private void DrawAllFHSS()
        {
            foreach(var fhss in lSourceFHSS)
            {
                mapLayout.DrawSourceFHSS(fhss.Coordinates, DLLSettingsControlPointForMap.Model.ColorsForMap.Yellow);
            }

            //foreach (var fhss in lReconFHSS)
            //    mapLayout.DrawSourceFHSS(fhss., DLLSettingsControlPointForMap.Model.ColorsForMap.Yellow);
            //foreach (var fhss in lSuppressFHSS)
            //    mapLayout.DrawSourceFHSS(fhss., DLLSettingsControlPointForMap.Model.ColorsForMap.Red);
        }
    }
}

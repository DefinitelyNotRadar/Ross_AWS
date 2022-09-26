using EvaTable;
using Ross.Map;
using Ross.Map._EventArgs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ross
{
    public partial class MainWindow : Window
    {

        private void MapLayout_OnCoordControlPoinChanged(object sender, CoordEventArgs e)
        {
            Properties.Local.Common.Latitude = e.Data.Latitude;
            Properties.Local.Common.Longitude = e.Data.Longitude;
        }

        private void MapLayout_OnCoordASPPropertyGridSelecteted(object sender, CoordEventArgs e)
        {
            ucASP.SetCoordsASPToPG(e.Data);
        }



        private void MapLayout_OnRadioJammingMode(object sender, Tabl e)
        {
            var IsSeccess = SelectedByConnectionTypeClient.SendMode(0);

            if (IsSeccess)
            {
                var newTabl = e.Clone();
                mapLayout.SetStationInEvaTable(newTabl, e);
            }
        }

        private void MapLayout_OnRadioIntelligenceMode(object sender, Tabl e)
        {
            var IsSeccess = SelectedByConnectionTypeClient.SendMode(0);

            if (IsSeccess)
            {
                var newTabl = e.Clone();
                mapLayout.SetStationInEvaTable(newTabl, e);
            }
        }

        private void MapLayout_OnPreparationMode(object sender, Tabl e)
        {
            var IsSeccess = SelectedByConnectionTypeClient.SendMode(0);

            if (IsSeccess)
            {
                var newTabl = e.Clone();
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
            {
                mapLayout.Show();
                DrawAllObjects();
            }
        }

        private void MapLayout_Closing(object sender, CancelEventArgs e)
        {
            ToggleButton_Map.IsChecked = false;
        }

        private void UpdateEvaTable()
        {
            Dispatcher.Invoke(() =>
            {
                mapLayout.ClearEvaTable();
                foreach (var asp in lASP)
                    mapLayout.AddStationInEvaTable(new Tabl() { Name = asp.Caption, Id = asp.Id, StateASP = asp.IsConnect == ModelsTablesDBLib.Led.Green ? StateASP.On : StateASP.Off, ModASP = (ModASP)asp.Mode });
            });
        }

        private void DrawAllObjects()
        {
            mapLayout?.RastrMap?.mapControl?.RemoveAllObjects();

            DrawAllASP();

            DrawAllFWS();

            DrawAllFHSS();

            UpdateEvaTable();
        }

        private void DrawAllASP()
        {
            foreach(var asp in lASP)
            {
                mapLayout.DrawStation(asp.Coordinates, asp.Caption);
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

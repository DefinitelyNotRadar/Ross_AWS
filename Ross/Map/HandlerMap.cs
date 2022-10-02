using EvaTable;
using ModelsTablesDBLib;
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
            var IsSeccess = false;
            if (e.Id == SelectedByConnectionTypeClient1.IdMaster || e.Id == SelectedByConnectionTypeClient1.IdSlave)
                IsSeccess = SelectedByConnectionTypeClient1.SelectedConnectionObject.SendMode(2);
            else IsSeccess = SelectedByConnectionTypeClient2.SelectedConnectionObject.SendMode(2);

            if (IsSeccess)
            {
                var newTabl = e.Clone();
                mapLayout.SetStationInEvaTable(newTabl, e);
            }
        }

        private void MapLayout_OnRadioIntelligenceMode(object sender, Tabl e)
        {
            var IsSeccess = false;
            if (e.Id == SelectedByConnectionTypeClient1.IdMaster || e.Id == SelectedByConnectionTypeClient1.IdSlave)
                IsSeccess = SelectedByConnectionTypeClient1.SelectedConnectionObject.SendMode(1);
            else IsSeccess = SelectedByConnectionTypeClient2.SelectedConnectionObject.SendMode(1);

            if (IsSeccess)
            {
                var newTabl = e.Clone();
                mapLayout.SetStationInEvaTable(newTabl, e);
            }
        }

        private void MapLayout_OnPreparationMode(object sender, Tabl e)
        {
            var IsSeccess = false;
            if (e.Id == SelectedByConnectionTypeClient1.IdMaster || e.Id == SelectedByConnectionTypeClient1.IdSlave)
                IsSeccess = SelectedByConnectionTypeClient1.SelectedConnectionObject.SendMode(0);
            else IsSeccess = SelectedByConnectionTypeClient2.SelectedConnectionObject.SendMode(0);


            if (IsSeccess)
            {
                var newTabl = e.Clone();
                mapLayout.SetStationInEvaTable(newTabl, e);
            }
        }

        private void MapLayout_OnPoll(object sender, Tabl e)
        {
            if (e.Id == SelectedByConnectionTypeClient1.IdMaster || e.Id == SelectedByConnectionTypeClient1.IdSlave)
                    Poll_Station(SelectedByConnectionTypeClient1.SelectedConnectionObject);
            else Poll_Station(SelectedByConnectionTypeClient2.SelectedConnectionObject);
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
                {
                    Role roleStation = asp.Role == RoleStation.Master ? Role.Master : (asp.Role == RoleStation.Slave ? Role.Slave : Role.Single);  
                    mapLayout.AddStationInEvaTable(new Tabl() { Name = asp.Caption, Id = asp.Id, Role = roleStation, StateASP = asp.IsConnect == ModelsTablesDBLib.Led.Green ? StateASP.On : StateASP.Off, ModASP = (ModASP)asp.Mode }); 
                }
            });
        }

        private void DrawAllObjects()
        {
            if (this.mapLayout == null) return;   
            UpdateEvaTable();

            if (this.mapLayout.RastrMap == null || this.mapLayout.RastrMap.mapControl == null || !mapLayout.RastrMap.IsLoaded) return;

            if(mapLayout.RastrMap.mapControl.MapObjects.Count > 0 || mapLayout.RastrMap.mapControl.PolyObjects.Count > 0)
                mapLayout?.RastrMap?.mapControl.RemoveAllObjects();

            DrawAllASP();

            DrawAllFWS();

            DrawAllFHSS();
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

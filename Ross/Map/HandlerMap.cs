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
    using System.Threading;
    using System.Windows.Threading;

    using TableEvents;

    public partial class MainWindow : Window
    {

     

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


        #region Context Menu
        private void MapLayout_OnCoordControlPoinChanged(object sender, CoordEventArgs e)
        {
            Properties.Local.Common.Latitude = e.Data.Latitude;
            Properties.Local.Common.Longitude = e.Data.Longitude;
        }

        private void MapLayout_OnCoordASPPropertyGridSelecteted(object sender, CoordEventArgs e)
        {
            ucASP.SetCoordsASPToPG(e.Data);
        }
        #endregion


        #region Eva Table handler

        private async void MapLayout_OnRadioJammingMode(object sender, Tabl e)
        {
            var IsSeccess = false;
            foreach (var item in SelectedStationModels)
            {
                if ((e.Id == item.IdMaster || e.Id == item.IdSlave) && item.SelectedConnectionObject.IsConnected)
                {
                    IsSeccess = await item.SelectedConnectionObject.SendMode(2).ConfigureAwait(false);
                    break;
                }
            }

            if (IsSeccess)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
                    {
                        e.ModASP = ModASP.Jumming;
                    });
            }
        }

        private async void MapLayout_OnRadioIntelligenceMode(object sender, Tabl e)
        {
            var IsSeccess = false;
            foreach(var item in SelectedStationModels)
            {
                if ((e.Id == item.IdMaster || e.Id == item.IdSlave) && item.SelectedConnectionObject.IsConnected)
                { 
                    IsSeccess = await item.SelectedConnectionObject.SendMode(1).ConfigureAwait(false);
                    break;
                }
            }

            if (IsSeccess)
            {
                Dispatcher.BeginInvoke( DispatcherPriority.Normal,(ThreadStart)delegate
                    {
                        e.ModASP = ModASP.RadioReconnaissance;
                    });
            }

            //mapLayout.GetItemFromEvaTable(e.Id).ModASP = ModASP.RadioReconnaissance;
        }

        private async void MapLayout_OnPreparationMode(object sender, Tabl e)
        {
            var IsSeccess = false;
            foreach (var item in SelectedStationModels)
            {
                if ((e.Id == item.IdMaster || e.Id == item.IdSlave) && item.SelectedConnectionObject.IsConnected)
                {
                    IsSeccess = await item.SelectedConnectionObject.SendMode(0).ConfigureAwait(false);
                    break;
                }
            }

            if (IsSeccess)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
                    {
                        e.ModASP = ModASP.Preparation;
                    });
            }
        }

        private async void MapLayout_OnPoll(object sender, Tabl e)
        {
            foreach (var item in SelectedStationModels)
            {
                if ((e.Id == item.IdMaster || e.Id == item.IdSlave) && item.SelectedConnectionObject.IsConnected)
                   await Poll_Station(item.SelectedConnectionObject);
            }
        }



        #endregion

        #region Draw

        private void DrawAllObjects()
        {
            if (this.mapLayout == null) return;
            UpdateEvaTable();

            if (this.mapLayout.RastrMap == null || this.mapLayout.RastrMap.mapControl == null || !mapLayout.RastrMap.IsLoaded) return;

            if (mapLayout.RastrMap.mapControl.MapObjects.Count > 0
                || mapLayout.RastrMap.mapControl.PolyObjects.Count > 0)
            {
                mapLayout?.RastrMap?.mapControl.RemoveAllObjects();
            }



            DrawAllASP();

            DrawSectors();

            DrawAllFWS();

            DrawAllFHSS();


            mapLayout.DrawPolygonOfLineOfSight();

        }


        private void UpdateEvaTable()
        {

                    mapLayout.ClearEvaTable();
                    foreach (var asp in lASP)
                    {
                        Role roleStation = asp.Role == RoleStation.Master ? Role.Master : (asp.Role == RoleStation.Slave ? Role.Slave : Role.Single);
                        mapLayout.AddStationInEvaTable(new Tabl() { Name = asp.CallSign, Id = asp.Id, Role = roleStation, StateASP = asp.IsConnect == Led.Green ? StateASP.On : StateASP.Off, ModASP = (ModASP)asp.Mode, Letters = asp.Letters });
                    }
          
        }

        private void DrawSectors()
        {
            foreach (var asp in lASP)
            {                
                    mapLayout.DrawSectors(asp.Coordinates, new short[5] {asp.LPA10, asp.LPA13, asp.LPA24, asp.LPA510, asp.LPA59}, 0);
            }

        }


        private void DrawAllASP()
        {
                    foreach (var asp in lASP)
                    {
                        mapLayout.DrawStation(asp.Coordinates, asp.Caption);
                    }
                    mapLayout.GetStatusBarModel().AJSValue = lASP.Count;
                    mapLayout.SetASP(lASP);  
        }

        private void DrawAllFWS()
        {
                    foreach (var fws in lReconFWS)
                        mapLayout.DrawSourceFWS(fws.Coordinates, DLLSettingsControlPointForMap.Model.ColorsForMap.Yellow);
                    foreach (var fws in lSuppressFWS)
                        mapLayout.DrawSourceFWS(fws.Coordinates, DLLSettingsControlPointForMap.Model.ColorsForMap.Red);

                    mapLayout.GetStatusBarModel().RESFWSTDValue = lReconFWS.Count;
                    mapLayout.GetStatusBarModel().RESFWSJValue = lSuppressFWS.Count;
        }

        private void DrawAllFHSS()
        {
                    foreach (var fhss in lSourceFHSS)
                    {
                        mapLayout.DrawSourceFHSS(fhss.Coordinates, DLLSettingsControlPointForMap.Model.ColorsForMap.Yellow);
                    }

                    mapLayout.GetStatusBarModel().RESFHSSTDValue = lReconFHSS.Count;
                    mapLayout.GetStatusBarModel().RESFHSSJValue = lSuppressFHSS.Count;

            //foreach (var fhss in lReconFHSS)
            //    mapLayout.DrawSourceFHSS(fhss., DLLSettingsControlPointForMap.Model.ColorsForMap.Yellow);
            //foreach (var fhss in lSuppressFHSS)
            //    mapLayout.DrawSourceFHSS(fhss., DLLSettingsControlPointForMap.Model.ColorsForMap.Red);
        }

        private void MapLayout_OnNeedToRedrawMapJojects(object sender, EventArgs e)
        {
            DrawAllObjects();
        }
        #endregion
    }
}

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
    using Ross.JSON;
    using RouteControl.Model;
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
            Properties.Local.Common.Latitude = Math.Round(e.Data.Latitude, 6);
            Properties.Local.Common.Longitude = Math.Round(e.Data.Longitude,6);
            SerializerJSON.Serialize(Properties.Local, "LocalProperties");
        }

        private void MapLayout_OnCoordASPPropertyGridSelecteted(object sender, CoordEventArgs e)
        {
            ucASP.SetCoordsASPToPG(e.Data);
        }

        private void MapLayout_RouteChanged(object sender, Route e)
        {
            bool isRouteExist = false;
            foreach (var route in lTableRoute)
            {
                if (route.Caption.Equals(e.Name))
                {
                    List<CoordRoute> coordRoutes = new List<CoordRoute>();
                    foreach (var point in e.ListPoints)
                    {
                        coordRoutes.Add(new CoordRoute() { Coordinates = new Coord() { Latitude = point.Latitude, Longitude = point.Longitude } });
                    }

                    route.Caption = e.Name;
                    route.ListCoordinates = coordRoutes;
                    clientDB?.Tables[NameTable.TableRoute].Change(route);
                    isRouteExist = true;
                }
            }

            if(!isRouteExist)
            {
                List<CoordRoute> coordRoutes = new List<CoordRoute>();
                foreach (var point in e.ListPoints)
                {
                    coordRoutes.Add(new CoordRoute() { Coordinates = new Coord() { Latitude = point.Latitude, Longitude = point.Longitude } });
                }
                clientDB?.Tables[NameTable.TableRoute].Add(new TableRoute() { Caption = e.Name, ListCoordinates = coordRoutes});
            }

        }


        private void MapLayout_RouteDeleted(object sender, Route e)
        {
            foreach (var route in lTableRoute)
            {
                if (route.Caption.Equals(e.Name))
                {
                    clientDB?.Tables[NameTable.TableRoute].Delete(route);
                }
            }        
        }


        private void MapLayout_RouteClear(object sender, Route e)
        {
            clientDB?.Tables[NameTable.TableRoute].Clear();
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
                _ = Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
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


        private void MapLayout_OnTableItemDoubleClicked(object sender, Tabl e)
        {
            foreach(var item in lASP)
            {
                if (e.Id == item.Id)
                    mapLayout.NavigateTo(item.Coordinates);
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


            DrawRoss();

            DrawAllASP();

            DrawSectors();

            DrawAllFWS();

            DrawAllFHSS();


            mapLayout.DrawTasks();
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

        private void DrawRoss()
        {
            mapLayout.DrawRoss(new UIMapRast.Models.WGSCoordinate() { Latitude = Properties.Local.Common.Latitude, Longitude = Properties.Local.Common.Longitude });
        }

        private void UpdateEvaTableConnection(TableASP asp)
        {
            var newStation = ConvertToEvaTable(asp);
            var old = mapLayout.GetItemFromEvaTable(asp.Id);

            if (old == null) return;

            mapLayout.SetStationInEvaTable(newStation, old);
        }

        private Tabl ConvertToEvaTable(TableASP asp)
        {
            Tabl tabl = new Tabl() { Name = asp.CallSign, Id = asp.Id, StateASP = asp.IsConnect == Led.Green ? StateASP.On : StateASP.Off, ModASP = (ModASP)asp.Mode, Letters = asp.Letters };
            tabl.Role = asp.Role == RoleStation.Master ? Role.Master : (asp.Role == RoleStation.Slave ? Role.Slave : Role.Single);
            return tabl;
        }

        private void DrawSectors()
        {
            mapLayout.ClearSectors();

            for (int i = 0; i < lASP.Count; i++)
            {
                if (lASP[i].Coordinates.Latitude <= -90 || lASP[i].Coordinates.Latitude >= 90 || lASP[i].Coordinates.Longitude <= -180 || lASP[i].Coordinates.Longitude >= 180)
                    continue;
                mapLayout.DrawSectors(lASP[i].Coordinates, new short[4] { lASP[i].LPA510 ,lASP[i].BPSS, lASP[i].LPA13, lASP[i].LPA24}, i);
            }
        }


        private void DrawAllASP()
        {
            foreach (var asp in lASP)
            {
                if (asp.Coordinates.Latitude <= -90 || asp.Coordinates.Latitude >= 90 || asp.Coordinates.Longitude <= -180 || asp.Coordinates.Longitude >= 180)
                    continue;
                mapLayout.DrawStation(asp.Coordinates, asp.Caption);
            }
            mapLayout.GetStatusBarModel().AJSValue = lASP.Count;
            mapLayout.SetASP(lASP);  
        }

        private void DrawAllFWS()
        {


            foreach (var fws in lReconFWS)
            {
                if (fws.Coordinates.Latitude <= -90 || fws.Coordinates.Latitude >= 90 || fws.Coordinates.Longitude <= -180 || fws.Coordinates.Longitude >= 180)
                    continue;
                mapLayout.DrawSourceFWS(fws.Coordinates, DLLSettingsControlPointForMap.Model.ColorsForMap.Yellow, fws.FreqKHz);

   
            }

            foreach (var fws in lSuppressFWS)
            {
                if (fws.Coordinates.Latitude <= -90 || fws.Coordinates.Latitude >= 90 || fws.Coordinates.Longitude <= -180 || fws.Coordinates.Longitude >= 180)
                    continue;
                mapLayout.DrawSourceFWS(fws.Coordinates, DLLSettingsControlPointForMap.Model.ColorsForMap.Red, fws.FreqKHz);
            }


            //foreach(var aspFws in clientDB?.Tables[NameTable.TableSuppressFWS].)

            
            mapLayout.GetStatusBarModel().RESFWSTDValue = lReconFWS.Count;
            mapLayout.GetStatusBarModel().RESFWSJValue = lSuppressFWS.Count;
        }

        private void DrawAllFHSS()
        {
            foreach (var fhss in lSourceFHSS)
            {
                if (fhss.Coordinates.Latitude <= -90 || fhss.Coordinates.Latitude >= 90 || fhss.Coordinates.Longitude <= -180 || fhss.Coordinates.Longitude >= 180)
                    continue;
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

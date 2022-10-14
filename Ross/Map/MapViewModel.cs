using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DLLSettingsControlPointForMap.Model;
using DtedLibrary;
using LineOfSightZoneControl;
using LineOfSightZoneControl.Models;
using Mapsui.Geometries;
using Mapsui.Styles;
using WpfMapControl;
using Point = Mapsui.Geometries.Point;
using RouteControl;
using RouteControl.Model;

namespace Ross.Map
{
    public class MapViewModel : INotifyPropertyChanged
    {
        private StatusBarModel statusBar = new StatusBarModel();
        private GridLength settingsControlWidth = new GridLength(0, GridUnitType.Pixel);
        private GridLength calculationJobHeight = new GridLength(0, GridUnitType.Pixel);


        public MapViewModel(RasterMapControl rasterMapControl,  List<Point> polygonReturn)
        {
            InitTask1(rasterMapControl,  polygonReturn);
        }

        public StatusBarModel StatusBar
        {
            get => statusBar;
            set
            {
                if (statusBar == value) return;
                statusBar = value;
                OnPropertyChanged();
            }
        }

        public GridLength SettingsControlWidth
        {
            get => settingsControlWidth;
            set
            {
                if(settingsControlWidth == value) return;
                settingsControlWidth = value;
                OnPropertyChanged();
            }
        }

        public GridLength CalculationJobHeight
        {
            get => calculationJobHeight;
            set
            {
                if (calculationJobHeight == value) return;
                calculationJobHeight = value;
                OnPropertyChanged();
            }
        }


        #region Task 1

        private ViewModel viewModelForTask1;
        private List<Point> polygon;
        private RasterMapControl RasterMapControl;

        private Color orangeColor = new Color(255, 140, 0, 100);
        private Color greenColor = new Color(124, 252, 0, 100);

        public ViewModel ViewModelForTask1
        {
            get => viewModelForTask1;
            set
            {
                if (viewModelForTask1 == value) return;
                viewModelForTask1 = value;
            }
        }


        private void InitTask1(RasterMapControl rasterMapControl, List<Point> polygonReturn)
        {
            this.polygon = polygonReturn;
            viewModelForTask1 = new ViewModel(rasterMapControl);
            this.RasterMapControl = rasterMapControl;
            viewModelForTask1.PropertyChanged += ViewModelForTask1_PropertyChanged;
            viewModelForTask1.ReturnModel.PropertyChanged += ReturnModel_PropertyChanged;
        }

        private void ViewModelForTask1_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(viewModelForTask1.SelectedPolygon)))
            {
               

                foreach (var item in RasterMapControl.PolyObjects)
                {
                    if (item.Geometry is Polygon polygon)
                        if (polygon.ExteriorRing.Vertices.Count >= 360)
                        {
                            RasterMapControl.RemoveObject(item);
                            break;
                        }
                }

                if (viewModelForTask1.SelectedPolygon.Count <= 0)
                {
                    polygon.Clear();
                    RasterMapControl.AddPolygon(polygon, orangeColor);
                }
                else
                {
                    polygon.AddRange(viewModelForTask1.SelectedPolygon);
                    RasterMapControl.AddPolygon(polygon, greenColor);
                }                        
            }
        }

        private void ReturnModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(viewModelForTask1.ReturnModel.Points)))
            {
                foreach (var item in RasterMapControl.PolyObjects)
                {
                    if (item.Geometry is Polygon polygon)
                        if (polygon.ExteriorRing.Vertices.Count >= 360)
                        {
                            RasterMapControl.RemoveObject(item);
                            break;
                        }
                }

                polygon.Clear();
                RasterMapControl.AddPolygon(viewModelForTask1.ReturnModel.Points, orangeColor);
            }
        }

        #endregion

        #region Task 2
        private Route route;

        public Route Route
        {
            get => route;
            set
            {
                if (route == value) return;
                route = value;
                OnPropertyChanged();
            }
        }



        

        #endregion

        #region Task 3



        #endregion

        #region OnPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
            }
            catch { }
        }

        #endregion


    }
}

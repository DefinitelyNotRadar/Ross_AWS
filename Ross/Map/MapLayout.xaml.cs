using DLLSettingsControlPointForMap;
using System.Windows;
using DLLSettingsControlPointForMap.Model;

namespace Ross.Map
{
    /// <summary>
    /// Логика взаимодействия для MapLayout.xaml
    /// </summary>
    public partial class MapLayout : Window
    {
        public MapLayout()
        {
            InitializeComponent();
            Properties.OnDefaultButtonClick += Properties_OnDefaultButtonClick;
            Properties.OnApplyButtonClick += Properties_OnApplyButtonClick;
        }

        public SettingsControlForMap MapProperties
        {
            get => Properties;
            set
            {
                Properties = value;
            }
        }

        private void Properties_OnApplyButtonClick(object sender, DLLSettingsControlPointForMap.Model.LocalProperties e)
        {
            RastrMap.PathMap = e.PathMap.FileMap;       
        }

        private void Properties_OnDefaultButtonClick(object sender, DLLSettingsControlPointForMap.Model.LocalProperties e)
        {
            Properties.Local.Common.CoordinateSystem = GeographicCoordinateSystem.CK42;
            Properties.Local.Common.CoordinateView = CoordView.Dd;
            Properties.Local.ColorsMap.ColorIRIFUSS = ColorsForMap.Yellow;
            Properties.Local.ColorsMap.ColorIRIFWS = ColorsForMap.Yellow;
            Properties.Local.ColorsMap.ColorSectorRI = ColorsForMap.Yellow;
            Properties.Local.ColorsMap.ColorSectorRJ = ColorsForMap.Yellow;
        }

        public void TranslateMapLayout(ModelsTablesDBLib.Languages languages)
        {
           
        }

       

        private void Properties_OnLanguageChanged(object sender, ModelsTablesDBLib.Languages e)
        {
            TranslateMapLayout(e);
        }

        private void SettingToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ColumnSettings.Width = new GridLength(0, GridUnitType.Pixel);
        }

        private void SettingToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            ColumnSettings.Width = new GridLength(0, GridUnitType.Auto);

        }

        private void Properties_OnPathMapChanged(object sender, PathMap e)
        {
            RastrMap.PathMap = e.FileMap;
        }
    }
}

using DLLSettingsControlPointForMap;
using System.Windows;
using DLLSettingsControlPointForMap.Model;
using Ross.JSON;

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
            LoadSettings();
        }

        public SettingsControlForMap MapProperties
        {
            get => Properties;
            set
            {
                Properties = value;
            }
        }

        private void Properties_OnApplyButtonClick(object sender, LocalProperties e)
        {
            SerializerJSON.Serialize<LocalProperties>(e, "MapProperties");
        }

        private void Properties_OnDefaultButtonClick(object sender, LocalProperties e)
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

        private void LoadSettings()
        {
            try
            {
                MapProperties.Local = SerializerJSON.Deserialize<LocalProperties>("MapProperties");
            }
            catch
            {
                
            }
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
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
    }
}

using DLLSettingsControlPointForMap;
using System.Windows;
using DLLSettingsControlPointForMap.Model;
using Ross.JSON;
using System.Windows.Input;

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
            InitHotKeys();
        }

        public SettingsControlForMap MapProperties
        {
            get => Properties;
            set
            {
                Properties = value;
            }
        }


        #region Hot Keys

        private void InitHotKeys()
        {
            var newCmd = new RoutedCommand();
            newCmd.InputGestures.Add(new KeyGesture(Key.C, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(newCmd, OpenEvaTable));
        }

        private void OpenEvaTable(object sender, ExecutedRoutedEventArgs e)
        {
           if(evaTable.Visibility == Visibility.Visible)
                evaTable.Visibility = Visibility.Collapsed;
           else evaTable.Visibility = Visibility.Visible;
        }

        #endregion


        #region Properties

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

        private void Properties_OnPathMapChanged(object sender, PathMap e)
        {
        }

        #endregion


        #region Map

        public void TranslateMapLayout(ModelsTablesDBLib.Languages languages)
        {

        }

        #endregion


        private void ToggleButton_Setting_Unchecked(object sender, RoutedEventArgs e)
        {
            ColumnSettings.Width = new GridLength(0, GridUnitType.Pixel);
        }

        private void ToggleButton_Setting_Checked(object sender, RoutedEventArgs e)
        {
            ColumnSettings.Width = new GridLength(0, GridUnitType.Auto);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
    }
}

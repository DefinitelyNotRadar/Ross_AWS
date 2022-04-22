using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DLLSettingsControlPointForMap.Model;
using Ross.AddPanel;
using Ross.JSON;
using Ross.Map;

namespace Ross
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        private MapLayout mapLayout;

        private SizeValue sizeSetting { get; set; }
        private SizeValue sizeTopTable { get; set; }
        private SizeValue sizeLeftDownTable { get; set; }
        private SizeValue sizeRightDownTable { get; set; }


        public MainWindow()
        {
            InitializeComponent();

            ucSRangesRecon.AddSRange(new ModelsTablesDBLib.TableSectorsRanges());
            ucASP.AddASP(new ModelsTablesDBLib.TableASP());


            SetLocalProperties();

            SetLanguageTables(Properties.Local.Common.Language);
            SetLanguageConnectionPanel(Properties.Local.Common.Language);
            mapLayout = new MapLayout();
            SetLanguageMapLayout(Properties.Local.Common.Language);

            InitMarkSizeWnd();
        }

        private void ToggleButton_Map_Click(object sender, RoutedEventArgs e)
        {

            if (mapLayout.IsVisible)
                mapLayout.Hide();
            else
                mapLayout.Show();
        }

        private void Properties_OnUpdateLocalProperties(object sender, ModelsTablesDBLib.LocalProperties e)
        {
  
        }

        private void SetLocalProperties()
        {
            try
            {
                Properties.Local = SerializerJSON.Deserialize<ModelsTablesDBLib.LocalProperties>("LocalProperties");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        MarkSizeWnd markSizeWnd;

        private void InitMarkSizeWnd()
        {
            markSizeWnd = SerializerJSON.Deserialize<MarkSizeWnd>(AppDomain.CurrentDomain.BaseDirectory + "SizePanel.json");

            if (markSizeWnd == null)
            {
                markSizeWnd = new MarkSizeWnd();
                sizeSetting = new SizeValue(DefaultSize.WidthDPanelSetting);
                sizeTopTable = new SizeValue(DefaultSize.HeightDPanelJammer);
                sizeLeftDownTable = new SizeValue(DefaultSize.WidthDPanelJamming);
                sizeRightDownTable = new SizeValue(DefaultSize.WidthDPanelJamming);

            }

            else
            {
                sizeSetting = markSizeWnd.sizeSetting;
                sizeTopTable = markSizeWnd.sizeTopTable;
                sizeLeftDownTable = markSizeWnd.sizeLeftDownTable;
                sizeRightDownTable = markSizeWnd.sizeRightDownTable;
            }
        }

        private void Properties_OnPasswordChecked(object sender, bool e)
        {
                mapLayout.MapProperties.Local.Common.Access = e? AccessTypes.Admin : AccessTypes.User;
        }

        private void Properties_OnUpdateLocalProperties_1(object sender, ModelsTablesDBLib.LocalProperties e)
        {
            SerializerJSON.Serialize<ModelsTablesDBLib.LocalProperties>(e, "LocalProperties");
        }
    }
}

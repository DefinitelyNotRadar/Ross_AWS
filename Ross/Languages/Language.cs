using ModelsTablesDBLib;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows;
using ValuesCorrectLib;

namespace Ross
{
    

    public partial class MainWindow : Window
    {

        public  string GetResourceTitle (string key)
        {
            try
            {                
                return (string)this.Resources[key];
            }
            catch
            {
                return "";
            }
            
        }
       
        private void SetLanguageTables(Languages language)
        {
            ResourceDictionary dict = new ResourceDictionary();
            try
            {
                switch (language)
                {
                    case Languages.Eng:
                        dict.Source = new Uri("/Ross;component/Languages/TranslatorTables/TranslatorTables.EN.xaml",
                                      UriKind.Relative);
                        break;
                    case Languages.Rus:
                        dict.Source = new Uri("/Ross;component/Languages/TranslatorTables/TranslatorTables.RU.xaml",
                                           UriKind.Relative);
                        break;
                    case Languages.Azr:
                        dict.Source = new Uri("/Ross;component/Languages/TranslatorTables/TranslatorTables.AZ.xaml",
                                           UriKind.Relative);
                        break;
                    default:
                        dict.Source = new Uri("/Ross;component/Languages/TranslatorTables/TranslatorTables.RU.xaml",
                                          UriKind.Relative);
                        break;
                }

                this.Resources.MergedDictionaries.Add(dict);
            }
            catch (Exception ex)
            { }
        }

        private void SetLanguageConnectionPanel(Languages language)
        {
            ResourceDictionary dict = new ResourceDictionary();
            try
            {
                switch (language)
                {
                    case Languages.Eng:
                        dict.Source = new Uri("/Ross;component/Languages/TranslatorConnectionPanel/StringResource.EN.xaml",
                                      UriKind.Relative);
                        break;
                    case Languages.Rus:
                        dict.Source = new Uri("/Ross;component/Languages/TranslatorConnectionPanel/StringResource.RU.xaml",
                                           UriKind.Relative);
                        break;
                    case Languages.Azr:
                        dict.Source = new Uri("/Ross;component/Languages/TranslatorConnectionPanel/StringResource.AZ.xaml",
                                           UriKind.Relative);
                        break;
                    default:
                        dict.Source = new Uri("/Ross;component/Languages/TranslatorConnectionPanel/StringResource.RU.xaml",
                                          UriKind.Relative);
                        break;
                }

                this.Resources.MergedDictionaries.Add(dict);
            }
            catch (Exception ex)
            { }
        }


        private void SetLanguageMapLayout(Languages languages)
        {
            ResourceDictionary dict = new ResourceDictionary();
            try
            {
                switch (languages)
                {
                    case Languages.Eng:
                        dict.Source = new Uri("/Ross;component/Languages/TranslatorMapWindow/StringResource.EN.xaml",
                                      UriKind.Relative);

                        break;
                    case Languages.Rus:
                        dict.Source = new Uri("/Ross;component/Languages/TranslatorMapWindow/StringResource.RU.xaml",
                                           UriKind.Relative);
                        break;
                    default:
                        dict.Source = new Uri("/Ross;component/Languages/TranslatorMapWindow/StringResource.RU.xaml",
                                      UriKind.Relative);
                        Properties.Local.Common.Language = Languages.Rus;
                        break;
                }

                mapLayout.Resources.MergedDictionaries.Add(dict);
            }
            catch (Exception ex)
            {

            }
        }


        private void Properties_OnLanguageChange(object sender, Languages e)
        {
            SetLanguageTables(e);
            SetLanguageConnectionPanel(e);
            SetLanguageMapLayout(e);

        }

    }
}

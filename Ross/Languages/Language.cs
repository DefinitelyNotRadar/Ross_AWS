using System;
using System.Windows;
using ASPControl;
using ModelsTablesDBLib;
using SectorsRangesControl;
using SpecFreqControl;
using SuppressFHSSControl;
using SuppressFWSControl;

namespace Ross
{
    public partial class MainWindow : Window
    {
        public string GetResourceTitle(string key)
        {
            try
            {
                return (string)Resources[key];
            }
            catch
            {
                return "";
            }
        }

        private void SetLanguageTables(Languages language)
        {
            var dict = GetTableLanguageDict(language);
            if (dict != null)
                Resources.MergedDictionaries.Add(dict);
        }

        private ResourceDictionary GetTableLanguageDict(Languages language)
        {
            var dict = new ResourceDictionary();
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
            }
            catch (Exception ex)
            {
                return null;
            }

            return dict;
        }


        private void SetLanguageConnectionPanel(Languages language)
        {
            var dict = new ResourceDictionary();
            try
            {
                switch (language)
                {
                    case Languages.Eng:
                        dict.Source = new Uri(
                            "/Ross;component/Languages/TranslatorConnectionPanel/StringResource.EN.xaml",
                            UriKind.Relative);
                        break;
                    case Languages.Rus:
                        dict.Source = new Uri(
                            "/Ross;component/Languages/TranslatorConnectionPanel/StringResource.RU.xaml",
                            UriKind.Relative);
                        break;
                    case Languages.Azr:
                        dict.Source = new Uri(
                            "/Ross;component/Languages/TranslatorConnectionPanel/StringResource.AZ.xaml",
                            UriKind.Relative);
                        break;
                    default:
                        dict.Source = new Uri(
                            "/Ross;component/Languages/TranslatorConnectionPanel/StringResource.RU.xaml",
                            UriKind.Relative);
                        break;
                }

                Resources.MergedDictionaries.Add(dict);
            }
            catch (Exception ex)
            {
            }
        }

        private void SetLanguageMapLayout(Languages languages)
        {
            var dict = new ResourceDictionary();
            try
            {
                switch (languages)
                {
                    case Languages.Eng:
                        dict.Source = new Uri("/Ross;component/Languages/TranslatorMapWindow/StringResource.EN.xaml",
                            UriKind.Relative);
                        mapLayout.Properties.Local.Common.Language = DLLSettingsControlPointForMap.Model.Languages.EN;

                        break;
                    case Languages.Rus:
                        dict.Source = new Uri("/Ross;component/Languages/TranslatorMapWindow/StringResource.RU.xaml",
                            UriKind.Relative);
                        mapLayout.Properties.Local.Common.Language = DLLSettingsControlPointForMap.Model.Languages.RU;
                        break;
                    default:
                        dict.Source = new Uri("/Ross;component/Languages/TranslatorMapWindow/StringResource.RU.xaml",
                            UriKind.Relative);
                        Properties.Local.Common.Language = Languages.Rus;
                        mapLayout.Properties.Local.Common.Language = DLLSettingsControlPointForMap.Model.Languages.RU;
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


        private void UcSRangesRecon_OnIsWindowPropertyOpen(object sender, SectorsRangesProperty e)
        {
            e.SetLanguagePropertyGrid(Properties.Local.Common.Language);
        }

        private void UcSpecFreqKnown_OnIsWindowPropertyOpen(object sender, SpecFreqProperty e)
        {
            e.SetLanguagePropertyGrid(Properties.Local.Common.Language);
        }

        private void UcSpecFreqImportant_OnIsWindowPropertyOpen(object sender, SpecFreqProperty e)
        {
            e.SetLanguagePropertyGrid(Properties.Local.Common.Language);
        }

        private void UcSpecFreqForbidden_OnIsWindowPropertyOpen(object sender, SpecFreqProperty e)
        {
            e.SetLanguagePropertyGrid(Properties.Local.Common.Language);
        }

        private void UcSRangesSuppr_OnIsWindowPropertyOpen(object sender, SectorsRangesProperty e)
        {
            e.SetLanguagePropertyGrid(Properties.Local.Common.Language);
        }

        private void UcASP_OnIsWindowPropertyOpen(object sender, ASPProperty e)
        {
            e.SetLanguagePropertyGrid(Properties.Local.Common.Language);
        }

        private void UcSuppressFHSS_OnIsWindowPropertyOpen(object sender, SuppressFHSSProperty e)
        {
            e.SetLanguagePropertyGrid(Properties.Local.Common.Language);
        }

        private void UcSuppressFHSS_OnIsWindowPropertyOpenExc(object sender, ExcludedFreqProperty e)
        {
            e.SetLanguagePropertyGrid(Properties.Local.Common.Language);
        }

        private void UcSuppressFWS_OnIsWindowPropertyOpen(object sender, SuppressFWSProperty e)
        {
            e.SetLanguagePropertyGrid(Properties.Local.Common.Language);
        }
    }
}
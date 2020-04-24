using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;

namespace InventarioTPV
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            SelectRegion("es-VE");
            InitializeComponent();
        }

        #region IDIOMAS
        public static void SelectRegion(string culture)
        {
            if (String.IsNullOrEmpty(culture))
                return;

            //Copy all MergedDictionarys into a auxiliar list.
            var dictionaryList = Application.Current.Resources.MergedDictionaries.ToList();

            string xamlsPath = "Recursos/Strings/Idiomas/";

            //Search for the specified culture.     
            string requestedCulture = string.Format(xamlsPath + "{0}.xaml", culture);
            var resourceDictionary = dictionaryList.
                FirstOrDefault(d => d.Source.OriginalString == requestedCulture);

            if (resourceDictionary == null)
            {
                //If not found, select our default language.             
                requestedCulture = xamlsPath + "es-VE.xaml";
                resourceDictionary = dictionaryList.
                    FirstOrDefault(d => d.Source.OriginalString == requestedCulture);
            }

            //If we have the requested resource, remove it from the list and place at the end.     
            //Then this language will be our string table to use.      
            if (resourceDictionary != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            }

            //Inform the threads of the new culture.     
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
        }
        #endregion
    }
}

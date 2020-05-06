using HandyControl.Data;
using HandyControl.Tools;
using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Data.SQLite;

namespace InventarioTPV
{
    public partial class MainWindow
    {
        public const string BaseDatos = "Recursos\\db.db";

        public MainWindow()
        {
            InitializeComponent();

            CargarSkin();
            SelectRegion("es-VE");
            ConfigHelper.Instance.SetLang("en");

            CodigoTest();
        }

        public void CodigoTest()
        {

            SQLiteConnectionStringBuilder strConn = new SQLiteConnectionStringBuilder()
            {
                DataSource = BaseDatos
            };

            if (!File.Exists(BaseDatos))
            {
                return;
            }

            var connection_string = String.Format("Data Source={0};ServerVersion=3;", BaseDatos);

            SQLiteConnection conn = new SQLiteConnection(connection_string);
            conn.Open();

            SQLiteCommand command = new SQLiteCommand
            {
                CommandText = "SELECT @Efectivo as tipopago, @Monto as monto FROM c_tasa",
                Connection = conn
            };
            command.Parameters.AddWithValue("Efectivo", "Efectivo");
            command.Parameters.AddWithValue("Monto", "Bs. 99.999.999");

            SQLiteDataReader dr = command.ExecuteReader();

            DataTable data = new DataTable();
            data.Clear();
            data.Load(dr);

            listIngresosMonedas.ItemsSource = data.DefaultView;
            dgVentas.ItemsSource = data.DefaultView;
        }

        #region IDIOMAS
        private void ElegirIdiomaClick(object sender, RoutedEventArgs e)
        {
            var seleccion = (MenuItem)e.OriginalSource;
            SelectRegion((string)seleccion.DataContext);
        }

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

        #region TEMAS

        public void UpdateSkin(SkinType skin)
        {
            Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri($"pack://application:,,,/HandyControl;component/Themes/Skin{skin.ToString()}.xaml")
            });
            Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/HandyControl;component/Themes/Theme.xaml")
            });
        }
        private void CargarSkin()
        {
            bool modoOscuro = Properties.Settings.Default.ModoOscuro;
            if (modoOscuro)
            {
                ActivarModoOscuro(null, null);
                btnModoOscuro.IsChecked = true;
            }
            else
            {
                ApagarModoOscuro(null, null);
                btnModoOscuro.IsChecked = false;
            }
        }
        private void ActivarModoOscuro(object sender, RoutedEventArgs e)
        {
            UpdateSkin(SkinType.Dark);
            Properties.Settings.Default.ModoOscuro = true;
            Properties.Settings.Default.Save();
        }

        private void ApagarModoOscuro(object sender, RoutedEventArgs e)
        {
            UpdateSkin(SkinType.Default);
            Properties.Settings.Default.ModoOscuro = false;
            Properties.Settings.Default.Save();
        }


        #endregion


    }
}

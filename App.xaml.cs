using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Inventario_y_Contabilidad
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Ha ocurrido un error: \n" + e.Exception.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            CreaLog("Error no controlado. \n" + e.Exception.ToString());
            e.Handled = true;
        }

        private void CreaLog(string info)
        {
            DirectoryInfo di;
            if (!Directory.Exists(Environment.CurrentDirectory + "\\logs"))
                di = Directory.CreateDirectory(Environment.CurrentDirectory + "\\logs");

            string directorio = Environment.CurrentDirectory + "\\logs\\log_" + String.Format("{0:ddMMyyyy_hhmmss}", DateTime.Now) + ".txt";

            StreamWriter log = new StreamWriter(directorio);
            log.Write(info);
            log.Close();
        }
    }
}

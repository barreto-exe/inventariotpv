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
        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // Process unhandled exception
            // Prevent default unhandled exception processing
            CreaLog(e.Exception.ToString());
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

using HandyControl.Data;
using System;
using System.Windows;

namespace InventarioTPV
{
    public partial class MainWindow
    {
        private void ActualizarAlturaGridVentas()
        {
            this.dgVentas.Height = this.ActualHeight - this.grdSuperior.ActualHeight;
        }

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
    }
}

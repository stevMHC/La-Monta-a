using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using La_Montaña.Vista;

namespace La_Montaña
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected void Application_Startup(object sender, StartupEventArgs e)
        {
            var loginVista = new LoginVista();
            loginVista.Show();
            loginVista.IsVisibleChanged += (s, ev) =>
            {
                if (loginVista.IsVisible == false && loginVista.IsLoaded)
                {
                    var mainView = new MainView();
                    mainView.Show();
                    loginVista.Close();
                }
            };
        }
    }
}

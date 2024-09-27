using System;
using System.Windows.Forms;

namespace MGLASER
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Exibe o SplashScreen imediatamente
            user splashScreen = new user();
            splashScreen.Show();

            // Instancia o form principal (Form1)
            Form1 formPrincipal = new Form1();

            // Carrega o form principal de forma assíncrona
            formPrincipal.Load += (sender, e) =>
            {
                // Quando o Form1 for carregado, feche o SplashScreen
                splashScreen.Close();
            };

            // Executa o form principal
            Application.Run(formPrincipal);
        }
    }
}

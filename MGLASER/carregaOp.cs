/*
 * By: Pedro Borges - Delta Sollutions/Conecthus
 * 03/09/2024
 */

//Tela para carregamento de arquivos XLS e TXT - Seriais

using Prj_RBS;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MGLASER
{
    public partial class carregaOp : Form
    {
        //DLLS do teclado virtual

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool Wow64DisableWow64FsRedirection(ref IntPtr ptr);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool Wow64RevertWow64FsRedirection(IntPtr ptr);

        private const UInt32 WM_SYSCOMMAND = 0x112;
        private const UInt32 SC_RESTORE = 0xf120;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        private string OnScreenKeyboadApplication = "osk.exe";


        IniFile _myIni = new IniFile(Application.StartupPath.ToString() + "\\Setup.ini");
        setupFile _setupFile = new setupFile();

        public carregaOp()
        {
            InitializeComponent();
        }

        private void carregaOp_Load(object sender, EventArgs e)
        {
            rjTextBox2.Focus();

            dirAberturaOp = _myIni.Read("camAberturaOP", "DIR");

            string processName = System.IO.Path.GetFileNameWithoutExtension(OnScreenKeyboadApplication);

            // Check whether the application is not running 
            var query = from process in Process.GetProcesses()
                        where process.ProcessName == processName
                        select process;

            var keyboardProcess = query.FirstOrDefault();

            // launch it if it doesn't exist
            if (keyboardProcess == null)
            {
                IntPtr ptr = new IntPtr(); ;
                bool sucessfullyDisabledWow64Redirect = false;

                // Disable x64 directory virtualization if we're on x64,
                // otherwise keyboard launch will fail.
                if (System.Environment.Is64BitOperatingSystem)
                {
                    sucessfullyDisabledWow64Redirect = Wow64DisableWow64FsRedirection(ref ptr);
                }

                // osk.exe is in windows/system folder. So we can directky call it without path
                using (Process osk = new Process())
                {
                    osk.StartInfo.FileName = OnScreenKeyboadApplication;
                    osk.Start();
                }

                // Re-enable directory virtualisation if it was disabled.
                if (System.Environment.Is64BitOperatingSystem)
                    if (sucessfullyDisabledWow64Redirect)
                        Wow64RevertWow64FsRedirection(ptr);
            }
            else
            {
                // Bring keyboard to the front if it's already running
                var windowHandle = keyboardProcess.MainWindowHandle;
                SendMessage(windowHandle, WM_SYSCOMMAND, new IntPtr(SC_RESTORE), new IntPtr(0));
            }
        }

        string dirAberturaOp;
        private void rjButton1_Click(object sender, EventArgs e)
        {
            //Abrir arquivo de OP. dirAberturaOp -> ao inves de abrir no C:\\ abre no caminho especificado no .ini 

            if (rjTextBox2.Texts != "")
            {
                OpenFileDialog opf = new OpenFileDialog();
                opf.InitialDirectory = dirAberturaOp;
                opf.Filter = "all files (*.txt)|*txt";

                if (opf.ShowDialog() == DialogResult.OK)
                {
                    rjTextBox4.Texts = opf.FileName;
                }

                string caminho = Path.Combine(rjTextBox4.Texts);
                string operador = Path.Combine(rjTextBox2.Texts);

                //Salva no arquivo .ini
                IniFile file = new IniFile(Application.StartupPath.ToString() + "\\Setup.ini");
                file.Write("cam", caminho, "DIR");
                file.Write("nome", operador, "OP");
                file.Write("dtlog", textBox1.Text, "OP");
                file.Write("allmarked", "0", "LASER");
                file.Write("count", "0", "LASER");

                MessageBox.Show("Seriais prontas para gravação.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Preencha os campos NOME e SENHA!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void FecharTecladoVirtual()
        {
            //Usado para que no momento que transição de telas, utilize para fechar por completo o teclado virtual que foi aberto na tela de OP

            string[] tecladoVirtualNomes = { "osk" };

            foreach (string nome in tecladoVirtualNomes)
            {

                var processosTeclado = Process.GetProcessesByName(nome);

                foreach (var processo in processosTeclado)
                {
                    try
                    {
                        processo.Kill();
                        processo.WaitForExit();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao tentar fechar o teclado virtual: {ex.Message}");
                    }
                }
            }
        }

        private void carregaOp_FormClosing(object sender, FormClosingEventArgs e)
        {
            //no close dor form, fecha o teclado virtual
            FecharTecladoVirtual();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            textBox1.Text = DateTime.Now.ToString();
        }

        private async void rjButton2_Click(object sender, EventArgs e)
        {
            if (rjTextBox2.Texts != "")
            {

                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Excel Files (*.xls)|*.xls";
                openFileDialog.Title = "Selecione um arquivo Excel";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Caminho do arquivo Excel selecionado
                    string excelFilePath = openFileDialog.FileName;

                    // Define o caminho de saída para o arquivo .txt
                    string txtFilePath = Path.ChangeExtension(excelFilePath, ".txt");

                    rjTextBox3.Visible = true;
                    // Instancia e executa a conversão
                    ExcelToTxtConverter converter = new ExcelToTxtConverter();
                    converter.ConvertXlsToTxt(excelFilePath, txtFilePath);

                    rjTextBox3.BackColor = System.Drawing.Color.Green;
                    rjTextBox3.Texts = "Processo finalizado, seriais prontas.";

                    await Task.Delay(3000);

                    rjTextBox3.Visible = false;


                }

                string operador = Path.Combine(rjTextBox2.Texts);

                //Salva no arquivo .ini
                IniFile file = new IniFile(Application.StartupPath.ToString() + "\\Setup.ini");

                file.Write("nome", operador, "OP");
                file.Write("dtlog", textBox1.Text, "OP");
                file.Write("allmarked", "0", "LASER");
                file.Write("count", "0", "LASER");
            }
            else
            {
                MessageBox.Show("Preencha os campos NOME e SENHA!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


        }
    }
}

using Prj_RBS;
using System;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MGLASER
{
    public partial class modoManual : Form
    {
        public modoManual()
        {
            InitializeComponent();
        }

        string caminhoConnect = Application.StartupPath.ToString() + "/Setup.ini";
        IniFile _myIni = new IniFile(Application.StartupPath.ToString() + "\\Setup.ini");

        private void modoManual_Load(object sender, EventArgs e)
        {
            if (Directory.Exists("/home"))
            {
                string[] lines = File.ReadAllLines(caminhoConnect);

                foreach (string line in lines)
                {
                    string[] str = line.Split('=');
                    if (str[0] == "com") { serialPort1.PortName = str[1]; }
                    if (str[0] == "baudRate") { serialPort1.BaudRate = int.Parse(str[1]); }
                }
            }
            else
            {
                //Pega as informações de COM e BAUDRATE
                serialPort1.BaudRate = int.Parse(_myIni.Read("baudRate", "CONECT"));
                serialPort1.PortName = _myIni.Read("com", "CONECT");
            }

            try
            {
                //abre a Porta Serial se ela estiver fechada
                if (!serialPort1.IsOpen)
                {
                    serialPort1.Open();
                    button1.Visible = true;
                }
                else
                {
                    button1.Visible = true;
                    button1.BackColor = Color.Red;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Verifique a Conexão Serial. O Software continuará em modo Offline.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        string rxData;
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // string rxData recebe informações da Serial
            rxData = serialPort1.ReadExisting();
            this.Invoke(new EventHandler(dataReceived));
        }

        private void dataReceived(object sender, EventArgs e)
        {
            //TextBox usado para mostrar informações recebidas em rxData
            textBox1.AppendText(rxData);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private async void pictureBox1_Click(object sender, EventArgs e)
        {
            //Animação para simular clique no PictureBox

            pictureBox1.Visible = false;
            pictureBox5.Visible = true;

            serialPort1.Write("G0");

            await Task.Delay(700);

            pictureBox5.Visible = false;
            pictureBox1.Visible = true;
        }

        private async void pictureBox2_Click(object sender, EventArgs e)
        {
            //Animação para simular clique no PictureBox
            pictureBox2.Visible = false;
            pictureBox6.Visible = true;

            serialPort1.Write("G180");

            await Task.Delay(700);

            pictureBox6.Visible = false;
            pictureBox2.Visible = true;
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox4_MouseDown(object sender, MouseEventArgs e)
        {
            //Aqui, quando o botão recebe Clique e permanencia no click, ele executa a ação de subida SOBE
            serialPort1.Write("SOBE");
            pictureBox4.Visible = false;
            pictureBox7.Visible = true;

        }

        private void pictureBox4_MouseUp(object sender, MouseEventArgs e)
        {
            //Aqui, quando é solto o cursor, a ação de parar e feita PARASOBE
            serialPort1.Write("PARASOBE");
            pictureBox7.Visible = false;
            pictureBox4.Visible = true;

        }

        private void pictureBox3_MouseDown(object sender, MouseEventArgs e)
        {
            //Aqui, quando o botão recebe Clique e permanencia no click, ele executa a ação de descida DESCE
            serialPort1.Write("DESCE");
            pictureBox3.Visible = false;
            pictureBox8.Visible = true;

        }

        private void pictureBox3_MouseUp(object sender, MouseEventArgs e)
        {
            //Aqui, quando é solto o cursor, a ação de descer e feita PARASOBE
            serialPort1.Write("PARADESCE");
            pictureBox8.Visible = false;
            pictureBox3.Visible = true;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }
    }
}

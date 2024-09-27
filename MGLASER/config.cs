using Prj_RBS;
using System;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;

namespace MGLASER
{
    public partial class config : Form
    {
        public config()
        {
            InitializeComponent();
        }

        setupFile _setupFile = new setupFile();

        public string portName;
        public int baudRate;

        private void rjButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!serialPort1.IsOpen)
                {
                    string portaSelecionada = comboBox1.SelectedItem.ToString();
                    int baudRateSelecionado = int.Parse(comboBox2.SelectedItem.ToString());

                    serialPort1.PortName = portaSelecionada;
                    serialPort1.BaudRate = baudRateSelecionado;
                    serialPort1.Open();

                    MessageBox.Show("Conectado", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    rjButton4.BackColor = Color.Green;

                    serialPort1.Write("START");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Erro de Conexão: " + ex.ToString(), "Erro", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        string rxData;
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            rxData = serialPort1.ReadExisting();
            this.Invoke(new EventHandler(dataReceived));
        }

        private void dataReceived(object sender, EventArgs e)
        {
            textBox1.AppendText(rxData);
            richTextBox1.ForeColor = Color.Green;
            richTextBox1.AppendText(rxData);
        }

        private void rjButton2_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                MessageBox.Show("Desconectado", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                rjButton4.BackColor = Color.Red;
            }
        }

        private void config_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                comboBox1.Items.Add(port);
            }

            try
            {
                _setupFile.swVer();
                _setupFile.configSerialPort();

                comboBox1.Text = _setupFile.portName;
                comboBox2.Text = _setupFile.baudRate.ToString();

                portName = _setupFile.portName;
                baudRate = _setupFile.baudRate;
            }
            catch (Exception)
            {
                MessageBox.Show("Falha na leitura da Porta Serial", "Aviso");
            }
        }

        private void rjButton3_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen)
                {
                    IniFile file = new IniFile(Application.StartupPath.ToString() + "\\Setup.ini");
                    if (comboBox1.Text != "") { file.Write("com", comboBox1.Text, "CONECT"); }
                    if (comboBox2.Text != "") { file.Write("baudRate", comboBox2.Text, "CONECT"); }
                    MessageBox.Show("Porta: " + comboBox1.Text + " | Taxa: " + comboBox2.Text + " - Informações salvas com sucesso!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Faça a conexão para que as informações sejam salvas.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Erro ao salvar, feche o programa, abra e tente novamente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void rjButton5_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                richTextBox1.ForeColor = Color.Red;
                serialPort1.Write(rjTextBox1.Texts);

                richTextBox1.Text = rjTextBox1.Texts;
            }

        }

        private void rjButton8_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        private void rjTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                if (serialPort1.IsOpen)
                {
                    richTextBox1.ForeColor = Color.Red;
                    serialPort1.Write(rjTextBox1.Texts);

                    richTextBox1.Text = rjTextBox1.Texts;
                }
                else
                {
                    MessageBox.Show("Sem Conexão!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
    }
}

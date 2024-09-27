using MBPLib2;
using Prj_RBS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MGLASER
{
    public partial class principal : Form
    {
        int vlrInicioCount;
        int valorDeInicio;
        int tpimg;
        int qtdAtual;
        int lineCount;
        int qtdSeriais;
        int contPecas = 0;
        string ipLaser;
        string opCam;
        int laserCiclo = 0;
        int pecasGravadas = 0;
        bool liberaAposJob = false;
        bool gravacaoLiberada = false;
        bool gravando = false;
        string selectedPath;
        string caminhoConnect = Application.StartupPath.ToString() + "/Setup.ini";
        IniFile _myIni = new IniFile(Application.StartupPath.ToString() + "\\Setup.ini");
        bool stopLaser = false;

        public principal()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (textBox1.Text.Contains("0_OK") || textBox1.Text.Contains("180_OK") && liberaAposJob == true)
            {
                textBox1.Text = "";
                //Aqui eu bloqueio a possibilidade de fazer o giro da base enquanto ocorre a gravação.
                serialPort1.Write("LIB_OFF");

                textBox1.Text = "";
                rjButton1.Text = "";

                //torno a camera inativa, para se caso ocorra de estar presa na gravação anterior, aqui desabilita
                try
                {
                    axMBActX1.Operation.IsCameraFinderView = false;
                }
                catch (Exception)
                {
                    MessageBox.Show("Verifique o Marcador a Laser, comunicação Falhou.");
                }

                gravando = false;
                gravacaoLiberada = false;

                try
                {
                    //Envia o conteudo dos textBoxs para a Laser. Block é referente ao campo, a numeração é o campo a ser mudado.
                    //Na BlockList (dentro do software Marker Builder) estão presente as numerações referente a cada uma das strings.
                    //O numero serial sempre deverá ser a string que está dentro da lista -> 0, 4, 8, 12, 16, 20, 24, 28 (isso dentro do software da gravadora)

                    //Deixando esse IF para que o usuario só consiga fazer o: axMBActX1.SaveControllerJob(nJob); se caso puxar o modelo do Berço, momento onde é carregada as seriais.
                    if (comboBox2.Text != "")
                    {
                        if (serial_1.Texts != "")
                        {
                            axMBActX1.Block(0).Text = serial_1.Texts;
                        }

                        if (serial_2.Texts != "")
                        {
                            axMBActX1.Block(4).Text = serial_2.Texts;
                        }

                        if (serial_3.Texts != "")
                        {
                            axMBActX1.Block(8).Text = serial_3.Texts;
                        }

                        if (serial_4.Texts != "")
                        {
                            axMBActX1.Block(12).Text = serial_4.Texts;
                        }

                        if (serial_5.Texts != "")
                        {
                            axMBActX1.Block(16).Text = serial_5.Texts;
                        }

                        if (serial_6.Texts != "")
                        {
                            axMBActX1.Block(20).Text = serial_6.Texts;
                        }

                        if (serial_7.Texts != "")
                        {
                            axMBActX1.Block(24).Text = serial_7.Texts;
                        }
                        if (serial_8.Texts != "")
                        {
                            axMBActX1.Block(28).Text = serial_8.Texts;
                        }

                        //Aqui eu salvo isso para que seja executado, é como salvar um arquivo que foi editado.
                        axMBActX1.SaveControllerJob(nJob);

                        //Apois enviar as seriais e fazer o Save, libera a flag para a gravação.
                        gravacaoLiberada = true;
                    }

                    // serial_1.Texts != "" -> Ele não irá gravar se não existir seriais. O campo serial_1 é usado pois se existir 1 serial, ela irá para ele, logo, se ele estiver vazio, é porque acabaram as seriais.
                    if (gravacaoLiberada == true && serial_1.Texts != "")
                    {
                        try
                        {
                            textBox1.Text = "";

                            try
                            {
                                //Inicio da gravação a laser
                                axMBActX1.Operation.StartMarking();
                                gravacaoLiberada = false;
                                timer1.Stop();

                            }
                            catch (System.Runtime.InteropServices.COMException error)
                            {
                                Console.WriteLine(error.Message);
                            }
                        }
                        catch (System.Runtime.InteropServices.COMException error)
                        {
                            MessageBox.Show("Erro no gravação: " + error.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (System.Runtime.InteropServices.COMException error)
                {
                    Console.WriteLine("Erro ao enviar Seriais: " + error.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void principal_Load(object sender, EventArgs e)
        {
            textBox1.ReadOnly = true;

            comboBox2.Focus();

            if (Directory.Exists("/home"))
            {
                string[] lines = System.IO.File.ReadAllLines(caminhoConnect);

                foreach (string line in lines)
                {
                    string[] str = line.Split('=');
                    if (str[0] == "com") { serialPort1.PortName = str[1]; }
                    if (str[0] == "baudRate") { serialPort1.BaudRate = int.Parse(str[1]); }
                }
            }
            else
            {
                serialPort1.BaudRate = int.Parse(_myIni.Read("baudRate", "CONECT"));
                serialPort1.PortName = _myIni.Read("com", "CONECT");
                ipLaser = _myIni.Read("ip", "LASER");
                ipLaser.Trim();
                opCam = _myIni.Read("cam", "DIR");
                rjTextBox12.Texts = opCam;
                rjTextBox1.Texts = _myIni.Read("nome", "OP");
                rjTextBox2.Texts = _myIni.Read("dtlog", "OP");
                selectedPath = _myIni.Read("arquivos", "GRAV");
                tpimg = int.Parse(_myIni.Read("tpimage", "LASER"));

                //rjTextBox14.Texts = _myIni.Read("allmarked", "LASER");
                //valorDeInicio = int.Parse(_myIni.Read("allmarked", "LASER"));
                //pecasGravadas = valorDeInicio;

                //rjTextBox5.Texts = _myIni.Read("count", "LASER");
                //vlrInicioCount = int.Parse(_myIni.Read("count", "LASER"));
                //laserCiclo = vlrInicioCount;
            }

            try
            {
                // conta apenas o que nao contem '*'
                var lines = File.ReadLines(opCam).Where(line => !line.Contains('*'));

                // Conta as linhas filtradas
                lineCount = lines.Count();

                //joga o valor no textBox
                rjTextBox13.Texts = $"{lineCount}";

                //joga na variavel que vou usar abaixo
                qtdSeriais = lineCount;


            }
            catch (Exception)
            {

                MessageBox.Show("Lembre-se de Carregar o arquivo da OP.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            try
            {
                if (!serialPort1.IsOpen)
                {
                    serialPort1.Open();
                    textBox1.Text = "";
                    label1.Visible = true;
                    label1.ForeColor = Color.Green;
                }
                else
                {
                    label1.Visible = true;
                    label1.ForeColor = Color.Red;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Verifique a Conexão Serial. O Software continuará em modo Offline.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                label1.ForeColor = Color.Red;
            }

            try
            {
                //muda modelo para mdx2500 - Por padrao comeca no mdx2000
                axMBActX1.InitMBActX(MBPLib2.MarkingUnitTypes.
                MARKINGUNIT_MDX2500);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            MessageBox.Show("Aguarde, realizando a conexão.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);

            int ControllerSerialNo = 743461; /*Numero do Controller*/
            try
            {
                //realiza a conexao e controle
                if (axMBActX1.Comm.IsOnline)
                {
                    axMBActX1.Comm.Offline();
                }

                //conexao usb
                axMBActX1.Comm.ConnectionType = MBPLib2.ConnectionTypes.CONNECTION_USB;
                axMBActX1.Comm.UsbControllerSerialNo = ControllerSerialNo;
                bool is_success = axMBActX1.Comm.Online();

                //Coloca em modo CONTROLLER
                if (axMBActX1.Comm.IsOnline)
                {
                    axMBActX1.Context = ContextTypes.CONTEXT_CONTROLLER;
                    label2.Visible = true;
                    label2.ForeColor = Color.Green;
                }
                else
                {
                    MessageBox.Show("Erro ao tentar assumir controle da máquina.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                MessageBox.Show("Erro ao tentar estabelecer conexão com a Gravadora a Laser.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                label2.Visible = true;
                label2.ForeColor = Color.Red;
            }

            if (serialPort1.IsOpen)
            {
                textBox1.Text = "";
                serialPort1.Write("START");
                textBox1.Text = "";
            }

            timerEmergencia.Start();
            timerEmergenciaOFF.Start();
            liberaAposJob = false;
            timer1.Start();
            stopLaser = false;

            rjTextBox14.Texts = _myIni.Read("allmarked", "LASER");
            valorDeInicio = int.Parse(_myIni.Read("allmarked", "LASER"));
            pecasGravadas = valorDeInicio;

            rjTextBox5.Texts = _myIni.Read("count", "LASER");
            vlrInicioCount = int.Parse(_myIni.Read("count", "LASER"));
            laserCiclo = vlrInicioCount;
        }

        private void marcaSeriaisUS()
        {
            //apos a gravacao, a serial é marcada com US (AA000US) USADO, para que nao seja mais gravada

            string iniPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Setup.ini");
            string filePath = GetFilePathFromIni(iniPath);
            string[] serials = System.IO.File.ReadAllLines(filePath);
            List<string> usedSerials = new List<string>();

            if (!string.IsNullOrEmpty(serial_1.Texts)) usedSerials.Add(serial_1.Texts);
            if (!string.IsNullOrEmpty(serial_2.Texts)) usedSerials.Add(serial_2.Texts);
            if (!string.IsNullOrEmpty(serial_3.Texts)) usedSerials.Add(serial_3.Texts);
            if (!string.IsNullOrEmpty(serial_4.Texts)) usedSerials.Add(serial_4.Texts);
            if (!string.IsNullOrEmpty(serial_5.Texts)) usedSerials.Add(serial_5.Texts);
            if (!string.IsNullOrEmpty(serial_6.Texts)) usedSerials.Add(serial_6.Texts);
            if (!string.IsNullOrEmpty(serial_7.Texts)) usedSerials.Add(serial_7.Texts);
            if (!string.IsNullOrEmpty(serial_8.Texts)) usedSerials.Add(serial_8.Texts);

            foreach (var serial in usedSerials)
            {
                int index = Array.IndexOf(serials, serial);
                if (index >= 0)
                {
                    serials[index] += "*" + laserCiclo;
                }
            }

            System.IO.File.WriteAllLines(filePath, serials);
        }

        string GetFilePathFromIni(string iniPath)
        {
            //Pega as informações seriais do arquivo .txt

            string[] lines = System.IO.File.ReadAllLines(iniPath);

            foreach (string line in lines)
            {
                if (line.StartsWith("cam="))
                {
                    return line.Substring(4);
                }
            }
            throw new Exception("Caminho do arquivo não encontrado no arquivo de Configuração (Setup.ini)");
        }

        private void carregaSerias()
        {
            try
            {
                //Carrega as seriais do arquivo .txt, especificado no .ini, nos TextBox

                string iniPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Setup.ini");
                string filePath = GetFilePathFromIni(iniPath);
                string[] serials = System.IO.File.ReadAllLines(filePath);
                List<string> availableSerials = new List<string>();

                foreach (string serial in serials)
                {
                    if (serial.Length == 5)
                    {
                        availableSerials.Add(serial);
                        label21.Visible = false;
                    }
                    else
                    {
                        //Aqui é para indicar o FIM DE CÓDIGO
                        serial_1.Texts = "";
                        serial_2.Texts = "";
                        serial_3.Texts = "";
                        serial_4.Texts = "";
                        serial_5.Texts = "";
                        serial_6.Texts = "";
                        serial_7.Texts = "";
                        serial_8.Texts = "";

                        gravacaoLiberada = false;
                        liberaAposJob = false;

                        label21.Visible = true;
                        label9.Visible = false;
                        label19.Visible = false;
                        label20.Visible = false;
                    }
                }

                //Cada berço poderá ter uma quantia diferente de seriais, esse FOR torna tudo (seriais) flexivel, podendo escolher quantas carregar
                for (int i = 0; i < availableSerials.Count && i < 8; i++)
                {
                    if (comboBox2.Text == "1 PEÇA")
                    {
                        switch (i)
                        {
                            //em todos esses cases, o avilableSerials carrega as serias se acordo com a quantia escolhida
                            case 0:
                                serial_1.Texts = availableSerials[i];
                                break;
                        }

                        //O contPecas é para a contagem de Peças gravadas, nesse caso vamos de 1 a 8
                        contPecas = 1;
                    }

                    if (comboBox2.Text == "2 PEÇAS")
                    {
                        switch (i)
                        {
                            case 0:
                                serial_1.Texts = availableSerials[i];
                                break;

                            case 1:
                                serial_2.Texts = availableSerials[i];
                                break;
                        }

                        contPecas = 2;
                    }

                    if (comboBox2.Text == "3 PEÇAS")
                    {
                        switch (i)
                        {
                            case 0:
                                serial_1.Texts = availableSerials[i];
                                break;

                            case 1:
                                serial_2.Texts = availableSerials[i];
                                break;

                            case 2:
                                serial_3.Texts = availableSerials[i];
                                break;
                        }

                        contPecas = 3;
                    }

                    if (comboBox2.Text == "4 PEÇAS")
                    {
                        switch (i)
                        {
                            case 0:
                                serial_1.Texts = availableSerials[i];
                                break;

                            case 1:
                                serial_2.Texts = availableSerials[i];
                                break;

                            case 2:
                                serial_3.Texts = availableSerials[i];
                                break;

                            case 3:
                                serial_4.Texts = availableSerials[i];
                                break;
                        }

                        contPecas = 4;
                    }

                    if (comboBox2.Text == "5 PEÇAS")
                    {
                        switch (i)
                        {
                            case 0:
                                serial_1.Texts = availableSerials[i];
                                break;

                            case 1:
                                serial_2.Texts = availableSerials[i];
                                break;

                            case 2:
                                serial_3.Texts = availableSerials[i];
                                break;

                            case 3:
                                serial_4.Texts = availableSerials[i];
                                break;

                            case 4:
                                serial_5.Texts = availableSerials[i];
                                break;
                        }

                        contPecas = 5;
                    }

                    if (comboBox2.Text == "6 PEÇAS")
                    {
                        switch (i)
                        {
                            case 0:
                                serial_1.Texts = availableSerials[i];
                                break;

                            case 1:
                                serial_2.Texts = availableSerials[i];
                                break;

                            case 2:
                                serial_3.Texts = availableSerials[i];
                                break;

                            case 3:
                                serial_4.Texts = availableSerials[i];
                                break;

                            case 4:
                                serial_5.Texts = availableSerials[i];
                                break;

                            case 5:
                                serial_6.Texts = availableSerials[i];
                                break;
                        }

                        contPecas = 6;
                    }

                    if (comboBox2.Text == "7 PEÇAS")
                    {
                        switch (i)
                        {
                            case 0:
                                serial_1.Texts = availableSerials[i];
                                break;

                            case 1:
                                serial_2.Texts = availableSerials[i];
                                break;

                            case 2:
                                serial_3.Texts = availableSerials[i];
                                break;

                            case 3:
                                serial_4.Texts = availableSerials[i];
                                break;

                            case 4:
                                serial_5.Texts = availableSerials[i];
                                break;

                            case 5:
                                serial_6.Texts = availableSerials[i];
                                break;

                            case 6:
                                serial_7.Texts = availableSerials[i];
                                break;
                        }

                        contPecas = 7;
                    }

                    if (comboBox2.Text == "8 PEÇAS")
                    {
                        switch (i)
                        {
                            case 0:
                                serial_1.Texts = availableSerials[i];
                                break;

                            case 1:
                                serial_2.Texts = availableSerials[i];
                                break;

                            case 2:
                                serial_3.Texts = availableSerials[i];
                                break;

                            case 3:
                                serial_4.Texts = availableSerials[i];
                                break;

                            case 4:
                                serial_5.Texts = availableSerials[i];
                                break;

                            case 5:
                                serial_6.Texts = availableSerials[i];
                                break;

                            case 6:
                                serial_7.Texts = availableSerials[i];
                                break;

                            case 7:
                                serial_8.Texts = availableSerials[i];
                                break;
                        }

                        contPecas = 8;
                    }
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Lembre-se de carregar os arquivos da OP", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        string rxData;

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            rxData = serialPort1.ReadExisting();

            this.Invoke(new EventHandler(dataReceived));
        }

        private void dataReceived(object sender, EventArgs e)
        {
            textBox1.AppendText(rxData);
        }

        private async void rjButton2_Click_1(object sender, EventArgs e)
        {
            //Carrega JOB ainda em modo controller, para depois, abrir em modo edição

            try
            {
                if (comboBox1.SelectedItem != null)
                {
                    string jobNo = comboBox1.SelectedItem.ToString().PadLeft(3, '0'); // Formata com 3 dígitos, ex: 001

                    rjButton1.Text = "Aguarde, as informações estão sendo enviadas.";
                    rjButton1.BackColor = Color.Green;
                    // Define o trabalho atual com o número selecionado
                    axMBActX1.Operation.SetCurrentJobNo(Convert.ToInt32(jobNo));

                    await Task.Delay(3000);

                    // Atualiza o ComboBox4 com o arquivo que contém o JobNo selecionado no nome
                    LoadFilesToComboBox(selectedPath, jobNo);

                    // Exibe o painel para da open e ativar o controller
                    roundedPanel7.Visible = true;

                    rjButton1.Text = "";
                    rjButton1.BackColor = ColorTranslator.FromHtml("#202020");
                }
                else
                {
                    MessageBox.Show("Por favor, selecione um trabalho (Job) na lista.", "Inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    liberaAposJob = false;
                }
            }
            catch (System.Runtime.InteropServices.COMException error)
            {
                MessageBox.Show("Erro ao definir o trabalho: " + error.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                liberaAposJob = false;
            }
            catch (FormatException)
            {
                MessageBox.Show("O valor selecionado não é um número válido.", "Inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                liberaAposJob = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocorreu um erro: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                liberaAposJob = false;
            }
            roundedPanel7.Visible = true;



        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            rjTextBox3.Texts = comboBox1.Text;
            comboBox3.Text = comboBox1.Text;
        }

        private void timerEmergencia_Tick(object sender, EventArgs e)
        {
            if (textBox1.Text.Contains("EMERG_ON"))
            {
                textBox1.Text = "";
                rjButton1.Text = "EMERGÊNCIA ACIONADA";
                rjButton1.ForeColor = Color.White;
                rjButton1.BackColor = Color.Red;
                label20.Visible = false;
                gravando = true;

                if (gravando == true)
                {
                    stopLaser = true;
                    gravando = false;
                    axMBActX1.Operation.StopMarking();
                    timerEmergencia.Stop();
                    timer1.Stop();
                }
            }
        }

        private async void timerEmergenciaOFF_Tick(object sender, EventArgs e)
        {
            if (textBox1.Text.Contains("EMERG_OFF"))
            {
                textBox1.Text = "";
                rjButton1.Text = "MÁQUINA LIBERADA";
                rjButton1.BackColor = Color.Green;

                serialPort1.Write("START");

                label9.Visible = false;
                label19.Visible = false;
                label20.Visible = false;
                label21.Visible = false;

                stopLaser = false;
                gravando = false;

                textBox1.Text = "";
                await Task.Delay(3000);
                textBox1.Text = "";

                rjButton1.BackColor = ColorTranslator.FromHtml("#202020");
                rjTextBox1.Texts = "";
                rjButton1.Text = "";

                timer1.Start();
                timerEmergencia.Start();
            }
        }

        private void rjButton5_Click(object sender, EventArgs e)
        {

            //comando da gravadora, utilizado para ligar e desligar a luz verde

            if (axMBActX1.Comm.IsOnline)
            {
                if (rjButton5.Text == "LIGAR ILUMINAÇÃO LASER")
                {
                    axMBActX1.Operation.SetLighting(true);
                    rjButton5.Text = "DESLIGAR ILUMINAÇÃO LASER";
                }
                else
                {
                    axMBActX1.Operation.SetLighting(false);
                    rjButton5.Text = "LIGAR ILUMINAÇÃO LASER";
                }
            }
        }

        private void rjButton4_Click(object sender, EventArgs e)
        {
            //Comando da gravadora, utilizado para emitir um laser que serve como guia para a area de aplicação

            if (axMBActX1.Comm.IsOnline)
            {
                axMBActX1.Operation.StartGuideLaserMarking(MBPLib2.GuideLaserTypes.GUIDELASER_ONETIME);
            }
            else
            {
                MessageBox.Show("Sem Conexão com a Laser", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void rjButton6_Click(object sender, EventArgs e)
        {
            // axMBActX1.Operation.StartMarking();
        }

        private void comboBox2_TextChanged(object sender, EventArgs e)
        {
            //Conforme escolher, os textBox que não precisam estar somem.

            if (comboBox2.Text == "1 PEÇA")
            {
                serial_1.Visible = true;
                serial_2.Visible = false;
                serial_3.Visible = false;
                serial_4.Visible = false;
                serial_5.Visible = false;
                serial_6.Visible = false;
                serial_7.Visible = false;
                serial_8.Visible = false;

                carregaSerias();
            }

            if (comboBox2.Text == "2 PEÇAS")
            {
                serial_1.Visible = true;
                serial_2.Visible = true;
                serial_3.Visible = false;
                serial_4.Visible = false;
                serial_5.Visible = false;
                serial_6.Visible = false;
                serial_7.Visible = false;
                serial_8.Visible = false;

                carregaSerias();
            }

            if (comboBox2.Text == "3 PEÇAS")
            {
                serial_1.Visible = true;
                serial_2.Visible = true;
                serial_3.Visible = true;
                serial_4.Visible = false;
                serial_5.Visible = false;
                serial_6.Visible = false;
                serial_7.Visible = false;
                serial_8.Visible = false;

                carregaSerias();
            }

            if (comboBox2.Text == "4 PEÇAS")
            {
                serial_1.Visible = true;
                serial_2.Visible = true;
                serial_3.Visible = true;
                serial_4.Visible = true;
                serial_5.Visible = false;
                serial_6.Visible = false;
                serial_7.Visible = false;
                serial_8.Visible = false;

                carregaSerias();
            }

            if (comboBox2.Text == "5 PEÇAS")
            {
                serial_1.Visible = true;
                serial_2.Visible = true;
                serial_3.Visible = true;
                serial_4.Visible = true;
                serial_5.Visible = true;
                serial_6.Visible = false;
                serial_7.Visible = false;
                serial_8.Visible = false;

                carregaSerias();
            }

            if (comboBox2.Text == "6 PEÇAS")
            {
                serial_1.Visible = true;
                serial_2.Visible = true;
                serial_3.Visible = true;
                serial_4.Visible = true;
                serial_5.Visible = true;
                serial_6.Visible = true;
                serial_7.Visible = false;
                serial_8.Visible = false;

                carregaSerias();
            }

            if (comboBox2.Text == "7 PEÇAS")
            {
                serial_1.Visible = true;
                serial_2.Visible = true;
                serial_3.Visible = true;
                serial_4.Visible = true;
                serial_5.Visible = true;
                serial_6.Visible = true;
                serial_7.Visible = true;
                serial_8.Visible = false;

                carregaSerias();
            }

            if (comboBox2.Text == "8 PEÇAS")
            {
                serial_1.Visible = true;
                serial_2.Visible = true;
                serial_3.Visible = true;
                serial_4.Visible = true;
                serial_5.Visible = true;
                serial_6.Visible = true;
                serial_7.Visible = true;
                serial_8.Visible = true;

                carregaSerias();
            }
        }

        string nomeJob;
        int nJob;

        private void rjButton10_Click(object sender, EventArgs e)
        {
            //Selecao do Numero do JOB(0000) e no do Arquivo do JOB(nomearquivo.M2A). Entra com o modo editing e depois seleciona para enviar

            axMBActX1.Context = ContextTypes.CONTEXT_EDITING;

            if (axMBActX1.Comm.IsOnline)
            {
                if (comboBox4.Text != null)
                {
                    nomeJob = comboBox4.SelectedItem.ToString();
                    axMBActX1.OpenJob(nomeJob);
                }

                if (comboBox3.Text != null)
                {
                    nJob = int.Parse(comboBox3.SelectedItem.ToString());
                    axMBActX1.OpenControllerJob(nJob);
                }

                liberaAposJob = true;
            }
            else
            {
                MessageBox.Show("Você não está conectado com a Gravadora.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            roundedPanel7.Visible = false;
        }

        private void rjButton9_Click(object sender, EventArgs e)
        {
            string jobNo = comboBox1.SelectedItem.ToString().PadLeft(3, '0');
            //Clique para chamar os nomes no comboBox que irá abrir em RoundedPanel
            LoadFilesToComboBox(selectedPath, jobNo);
            roundedPanel7.Visible = true;
        }

        private void CarregaJobs(string directoryPath)
        {
            try
            {
                richTextBox1.Clear();

                // Obtém todos os arquivos do diretório
                string[] files = Directory.GetFiles(directoryPath);

                // Adiciona o nome dos arquivos no ComboBox
                foreach (string file in files)
                {
                    // Adiciona apenas o nome do arquivo, sem o caminho completo

                    richTextBox1.AppendText(Path.GetFileName(file) + Environment.NewLine);


                    // comboBox1.Items.Add(Path.GetFileName(file));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar arquivos: " + ex.Message);
            }
        }

        private void LoadFilesToComboBox(string directoryPath, string jobNo)
        {
            try
            {
                comboBox4.Items.Clear();
                string[] files = Directory.GetFiles(directoryPath);

                // Busca arquivos que contenham o JobNo no nome
                foreach (string file in files)
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);

                    // Verifica se o nome do arquivo contém o JobNo
                    if (fileName.Contains($"_{jobNo}"))
                    {
                        comboBox4.Items.Add(Path.GetFileName(file));
                    }
                }

                // Seleciona o primeiro item automaticamente, se encontrado
                if (comboBox4.Items.Count > 0)
                {
                    comboBox4.SelectedIndex = 0;
                }
                else
                {
                    MessageBox.Show($"Nenhum arquivo encontrado com o número {jobNo}.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar arquivos: " + ex.Message);
            }
        }

        private void rjButton12_Click(object sender, EventArgs e)
        {
            roundedPanel7.Visible = false;
        }

        private void axMBActX1_EvMarkingStart(object sender, EventArgs e)
        {
            label9.Visible = true;
            marcaSeriaisUS();

            try
            {
                axMBActX1.Operation.IsCameraFinderView = true;
            }
            catch (Exception)
            {
                MessageBox.Show("Verifique o Marcador a Laser, comunicação Falhou.");
            }

            label19.Visible = false;
        }

        private async void axMBActX1_EvMarkingEnd(object sender, AxMBPLib2._DMBActXEvents_EvMarkingEndEvent e)
        {
            label19.Visible = true;

            if (stopLaser == false)
            {
                textBox1.Text = "";

                serialPort1.Write("LIB_ON");

                laserCiclo++;
                rjTextBox5.Texts = laserCiclo.ToString();

                pecasGravadas += contPecas;
                rjTextBox14.Texts = pecasGravadas.ToString();

                //decrementa a qtd de Seriais no campo TOTAL SERIAIS
                qtdAtual = int.Parse(rjTextBox13.Texts);
                qtdSeriais = qtdAtual - contPecas;
                rjTextBox13.Texts = $"{qtdSeriais}";

                label9.Visible = false;


                carregaSerias();
            }
            if (stopLaser == true)
            {
                serialPort1.Write("LIB_ON");
                label9.Visible = false;
                label19.Visible = false;
                label20.Visible = true;
                stopLaser = false;

            }

            await Task.Delay(500);

            Thread.Sleep(tpimg);

            try
            {
                axMBActX1.Operation.IsCameraFinderView = false;
            }
            catch (Exception)
            {
                MessageBox.Show("Verifique o Marcador a Laser, comunicação Falhou.");
            }

            textBox1.Text = "";


            timer1.Start();
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            rjTextBox4.Texts = comboBox4.Text;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            //label19.Visible = true;
            //await Task.Delay(3000);
            //label19.Visible = false;

            //label9.Visible = true;
            //await Task.Delay(3000);
            //label9.Visible = false;

            //label20.Visible = true;
            //await Task.Delay(3000);
            //label20.Visible = false;

            //label21.Visible = true;
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            if (label21.Visible == true)
            {
                if (serialPort1.IsOpen)
                {
                    serialPort1.Write("LIB_OFF");
                }

                textBox1.Text = "";
                rjButton1.BackColor = Color.DarkGoldenrod;
                rjButton1.Text = "CARREGUE NOVAS SERIAIS(OP) PARA PROSSEGUIR COM AS GRAVAÇÕES.";
                timer3.Stop();

            }
        }

        private void rjButton8_Click(object sender, EventArgs e)
        {

        }

        IniFile file = new IniFile(Application.StartupPath.ToString() + "\\Setup.ini");
        private void rjTextBox14__TextChanged(object sender, EventArgs e)
        {
            if (rjTextBox14.Texts != "")
            {
                file.Write("allmarked", rjTextBox14.Texts, "LASER");
            }
        }

        private void rjTextBox5__TextChanged(object sender, EventArgs e)
        {
            if (rjTextBox5.Texts != "")
            {
                file.Write("count", rjTextBox5.Texts, "LASER");
            }
        }

        private void rjButton8_Click_1(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Write("G180");
            }
        }

        private void rjButton11_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Write("G0");
            }
        }

        private void principal_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }
        }

        private void rjButton13_Click(object sender, EventArgs e)
        {

            roundedPanel8.Visible = true;

            CarregaJobs(selectedPath);

        }

        private void circularPanel1_Click(object sender, EventArgs e)
        {
            roundedPanel8.Visible = false;
        }

        private void rjButton16_Click(object sender, EventArgs e)
        {
            int linhaInicial = (int)numericUpDown1.Value;
            int linhaFinal = (int)numericUpDown2.Value;

            var linhas = File.ReadAllLines(opCam).ToList();

            if (linhaInicial > 0 && linhaFinal >= linhaInicial && linhaFinal <= linhas.Count)
            {
                for (int i = linhaInicial - 1; i < linhaFinal; i++)
                {
                    int posAsterisco = linhas[i].IndexOf('*');
                    if (posAsterisco != -1)
                    {
                        linhas[i] = linhas[i].Substring(0, posAsterisco);
                    }
                }
                File.WriteAllLines(opCam, linhas);

                MessageBox.Show("Marcação removida com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                carregaSerias();
            }
            else
            {
                MessageBox.Show("Intervalo de linhas inválido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void rjButton15_Click(object sender, EventArgs e)
        {
            roundedPanel9.Visible = true;
        }

        private void circularPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void circularPanel2_Click(object sender, EventArgs e)
        {
            roundedPanel9.Visible = false;
        }
    }
}

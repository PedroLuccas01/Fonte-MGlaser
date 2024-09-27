/*
 * By: Pedro Borges - Delta Sollutions/Conecthus
 * 03/09/2024
 */


using Prj_RBS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace MGLASER
{
    public partial class Form1 : Form
    {

        string op;

        DateTime dataHora = DateTime.Now;
        IniFile _myIni = new IniFile(Application.StartupPath.ToString() + "\\Setup.ini");

        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Close();
            FecharTecladoVirtual();
        }

        private void FecharTecladoVirtual()
        {
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

        private void circularPanel1_Click(object sender, EventArgs e)
        {
            Close();
            FecharTecladoVirtual();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // rjTextBox2.Texts = dataHora.ToString();



            label9.Text = dataHora.ToString();

            // FecharOutrosForms();
            panel_pai.Controls.Clear();
            principal p = new principal();
            p.TopLevel = false;
            panel_pai.Controls.Add(p);
            p.Show();

            op = _myIni.Read("cam", "DIR");

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            FecharOutrosForms();
            panel_pai.Controls.Clear();
            carregaOp op = new carregaOp();
            op.TopLevel = false;
            panel_pai.Controls.Add(op);
            op.Show();
        }

        private void FecharOutrosForms()
        {
            List<Form> formsParaFechar = new List<Form>();

            foreach (Form form in Application.OpenForms)
            {
                if (form != this && form.Name != "Form1")
                {
                    formsParaFechar.Add(form);
                }
            }

            foreach (Form form in formsParaFechar)
            {
                form.Close();
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            FecharOutrosForms();
            panel_pai.Controls.Clear();
            modoManual mm = new modoManual();
            mm.TopLevel = false;
            panel_pai.Controls.Add(mm);
            mm.Show();

        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

            FecharOutrosForms();
            panel_pai.Controls.Clear();
            principal p = new principal();
            p.TopLevel = false;
            panel_pai.Controls.Add(p);
            p.Show();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            FecharTecladoVirtual();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            FecharOutrosForms();
            panel_pai.Controls.Clear();
            user u = new user();
            u.TopLevel = false;
            panel_pai.Controls.Add(u);
            u.Show();
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            label9.Text = DateTime.Now.ToString();
        }

        private void pictureBox5_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Modo Automático", pictureBox5);
        }

        private void pictureBox2_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Modo Manual", pictureBox2);
        }

        private void pictureBox4_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Carregar OP", pictureBox4);
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            FecharOutrosForms();
            panel_pai.Controls.Clear();
            config c = new config();
            c.TopLevel = false;
            panel_pai.Controls.Add(c);
            c.Show();
        }

        private void pictureBox3_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Configurações", pictureBox3);
        }
    }
}

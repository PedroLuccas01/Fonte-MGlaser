using System.Windows.Forms;

namespace Prj_RBS
{
    class setupFile
    {
        //Win
        FileIniSetup _fileIniSetup;
        //  string _fileIniSetupLin;

        //Public variables used between forms
        public string portName;
        public int baudRate;
        public bool win = true;


        public void swVer()
        {
            _fileIniSetup = new FileIniSetup(Application.StartupPath.ToString() + "\\Setup.ini");
        }

        public void configSerialPort()
        {
            portName = _fileIniSetup.Read("com", "CONECT");
            baudRate = int.Parse(_fileIniSetup.Read("baudRate", "CONECT"));

        }
    }
}



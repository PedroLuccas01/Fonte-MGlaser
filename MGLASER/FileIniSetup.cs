using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Prj_RBS
{
    internal class FileIniSetup
    {

        string Path;
        string EXE = Assembly.GetExecutingAssembly().GetName().Name;

        public FileIniSetup(string IniPath = null)
        {
            Path = new FileInfo(IniPath ?? EXE + ".ini").FullName.ToString();
        }

        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        public string Read(string Key, string Section = null)
        {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(Section ?? EXE, Key, "", RetVal, 255, Path);
            return RetVal.ToString();
        }

    }
}
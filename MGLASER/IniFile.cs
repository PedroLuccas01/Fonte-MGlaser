﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace Prj_RBS
{
    class IniFile

    {
        string Path;
        string EXE = Assembly.GetExecutingAssembly().GetName().Name;

        [DllImport("kernel32")]
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        public IniFile(string IniPath = null)
        {
            Path = new FileInfo(IniPath ?? EXE + ".ini").FullName.ToString();
        }

        public string Read(string Key, string Section = null)
        {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(Section ?? EXE, Key, "", RetVal, 255, Path);
            return RetVal.ToString();
        }

        public void Write(string Key, string Value, string Section = null)
        {
            WritePrivateProfileString(Section ?? EXE, Key, Value, Path);
        }

        public void DeleteKey(string Key, string Section = null)
        {
            Write(Key, null, Section ?? EXE);
        }

        public void DeleteSection(string Section = null)
        {
            Write(null, null, Section ?? EXE);
        }

        public bool KeyExists(string Key, string Section = null)
        {
            return Read(Key, Section).Length > 0;
        }

    }

    //{
    //    private string filePath;
    //
    //    public IniFile(string filePath)
    //    {
    //        this.filePath = filePath;
    //    }
    //
    //    [DllImport("kernel32")]
    //    private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
    //
    //    [DllImport("kernel32")]
    //    private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
    //
    //    public void Write(string section, string key, string value)
    //    {
    //        WritePrivateProfileString(section, key, value, filePath);
    //    }
    //
    //    public string Read(string section, string key, string defaultValue = "")
    //    {
    //        var retVal = new StringBuilder(255);
    //        GetPrivateProfileString(section, key, defaultValue, retVal, 255, filePath);
    //        return retVal.ToString();
    //    }
    //}
}
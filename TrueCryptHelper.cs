using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace diNo
{
    public class TrueCryptHelper
    {
        public void Decrypt()
        {
            List<char> driveLetters = new List<char>(26); // Allocate space for alphabet
            for (int i = 69; i < 91; i++) // increment from ASCII values for A-Z
            {
                driveLetters.Add(Convert.ToChar(i)); // Add uppercase letters to possible drive letters
            }

            foreach (string drive in Directory.GetLogicalDrives())
            {
                driveLetters.Remove(drive[0]); // removed used drive letters from possible drive letters
            }

            char firstFreeDrive = driveLetters.First();

            Process tc = new Process();
            tc.StartInfo.FileName = Application.StartupPath + "\\TrueCrypt\\TrueCrypt.exe";
            tc.StartInfo.Arguments = string.Format("/v \"{0}\" /l \"{1}\" /q", "C:\\Projects\\UnsereSV\\test.tc", firstFreeDrive); // for quiet!

            tc.Start();
        }

        public void Encrypt(string drive)
        {
            Process tc = new Process();
            tc.StartInfo.FileName = Application.StartupPath + "\\TrueCrypt\\TrueCrypt.exe";
            tc.StartInfo.Arguments = string.Format("/d \"{0}\" /q", drive); // for quiet!

            tc.Start();
        }


    }
}

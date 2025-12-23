using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace QwertyGarden
{
    public static class SettingsDataIO
    {
        public static int VERSION = 1;

        public static void SaveSettings(SettingsData settingsData)
        {
            string fileName = Application.persistentDataPath + "/settings_v" + VERSION + ".dat";
            using (FileStream fs = File.Create(fileName))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                bw.Write(settingsData.SFX);
                bw.Write(settingsData.Music);
            }
        }

        public static void LoadSettings(SettingsData settingsData)
        {
            string fileName = Application.persistentDataPath + "/settings_v" + VERSION + ".dat";
            if (File.Exists(fileName))
            {
                using (var stream = File.Open(fileName, FileMode.Open))
                {
                    using (BinaryReader br = new BinaryReader(stream))
                    {
                        settingsData.SFX = br.ReadBoolean();
                        settingsData.Music = br.ReadBoolean();
                    }
                }
            }
        }

    }
}

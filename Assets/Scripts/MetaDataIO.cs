using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace QwertyGarden
{
    public static class MetaDataIO
    {
        public static int VERSION = 1;

        public static void SaveMeta(MetaData metaData)
        {
            string fileName = Application.persistentDataPath + "/meta_v" + VERSION + ".dat";
            using (FileStream fs = File.Create(fileName))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                bw.Write(metaData.Coins);
                bw.Write((int)metaData.GameType);
                bw.Write((int)metaData.MenuState);
                bw.Write(metaData.KeyboardIndex);
                bw.Write(metaData.LastTimeStamp);
                bw.Write(metaData.GrowTime);
            }
        }

        public static void LoadMeta(MetaData metaData)
        {
            string fileName = Application.persistentDataPath + "/meta_v" + VERSION + ".dat";
            if (File.Exists(fileName))
            {
                using (var stream = File.Open(fileName, FileMode.Open))
                {
                    using (BinaryReader br = new BinaryReader(stream))
                    {
                        metaData.Coins = br.ReadDecimal();
                        metaData.GameType = (GAME_TYPE)br.ReadInt32();
                        metaData.MenuState = (MENU_STATE)br.ReadInt32();
                        metaData.KeyboardIndex = br.ReadInt32();
                        metaData.LastTimeStamp = br.ReadSingle();
                        metaData.GrowTime = br.ReadSingle();
                    }
                }
            }
        }

    }
}

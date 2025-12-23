using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace QwertyGarden
{
    public static class KeyboardDataIO
    {
        public static int VERSION = 1;
        public static void SaveKeyboard(KeyboardData keyboardData, int index)
        {
            string fileName = Application.persistentDataPath + "/keyboarddata_v" + VERSION + "_kb" + index + ".dat";
            using (FileStream fs = File.Create(fileName))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                bw.Write(keyboardData.KeyboardType);
                bw.Write(keyboardData.FlowerIndex.Length);
                for (int i = 0; i < keyboardData.FlowerIndex.Length; i++)
                    bw.Write(keyboardData.FlowerIndex[i]);
                for (int i = 0; i < keyboardData.CharacterCount.Length; i++)
                    bw.Write(keyboardData.CharacterCount[i]);
                for (int i = 0; i < keyboardData.FlowerProgress.Length; i++)
                    bw.Write(keyboardData.FlowerProgress[i]);
                bw.Write(keyboardData.TypedWord);
                bw.Write(keyboardData.WrongCount);
            }
        }

        public static bool KeyboardDataExists(int index)
        {
            string fileName = Application.persistentDataPath + "/keyboarddata_v" + VERSION + "_kb" + index + ".dat";
            return File.Exists(fileName);
        }

        public static void LoadKeyboard(KeyboardData keyboardData, int index)
        {
            string fileName = Application.persistentDataPath + "/keyboarddata_v" + VERSION + "_kb" + index + ".dat";
            if (File.Exists(fileName))
            {
                using (var stream = File.Open(fileName, FileMode.Open))
                {
                    using (BinaryReader br = new BinaryReader(stream))
                    {
                        keyboardData.KeyboardType = br.ReadInt32();

                        int numFlowerIndices = br.ReadInt32();
                        for (int i = 0; i < numFlowerIndices; i++)
                            keyboardData.FlowerIndex[i] = br.ReadInt32();

                        int numCharacterCount = br.ReadInt32();
                        for (int i = 0; i < numCharacterCount; i++)
                            keyboardData.CharacterCount[i] = br.ReadInt32();

                        int numFlowerProgress = br.ReadInt32();
                        for (int i = 0; i < numFlowerProgress; i++)
                            keyboardData.FlowerProgress[i] = br.ReadInt32();

                        keyboardData.TypedWord = br.ReadString();
                        keyboardData.WrongCount = br.ReadInt32();
                    }
                }
            }
        }

    }
}

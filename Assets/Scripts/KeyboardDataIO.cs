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
                bw.Write(keyboardData.WordIndex);
                bw.Write(keyboardData.KeyboardType);
                bw.Write(26);
                for (int i = 0; i < 26; i++)
                    bw.Write(keyboardData.FlowerType[i]);
                for (int i = 0; i < 26; i++)
                    bw.Write(keyboardData.CharacterCount[i]);
                for (int i = 0; i < 26; i++)
                    bw.Write(keyboardData.FlowerProgress[i]);
                for (int i = 0; i < 26; i++)
                    bw.Write(keyboardData.FlowerCount[i]);
                for (int i = 0; i < 26; i++)
                    bw.Write(keyboardData.GrowTime[i]);

                bw.Write(keyboardData.TypedWord);

                bw.Write(keyboardData.WrongLetter);
                bw.Write(keyboardData.WordCount);
                bw.Write(keyboardData.WordTime);
                bw.Write(keyboardData.CorrectCount);
                bw.Write(keyboardData.MistakeCount);

                bw.Write(1234567);
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
                        keyboardData.WordIndex = br.ReadInt32();
                        keyboardData.KeyboardType = br.ReadInt32();

                        int maxFlowers = br.ReadInt32();
                        for (int i = 0; i < maxFlowers; i++)
                            keyboardData.FlowerType[i] = br.ReadInt32();

                        for (int i = 0; i < maxFlowers; i++)
                            keyboardData.CharacterCount[i] = br.ReadInt32();

                        for (int i = 0; i < maxFlowers; i++)
                            keyboardData.FlowerProgress[i] = br.ReadInt32();

                        for (int i = 0; i < maxFlowers; i++)
                            keyboardData.FlowerCount[i] = br.ReadInt32();

                        for (int i = 0; i < maxFlowers; i++)
                            keyboardData.GrowTime[i] = br.ReadSingle();

                        keyboardData.TypedWord = br.ReadString();

                        keyboardData.WrongLetter = br.ReadInt32();
                        keyboardData.WordCount = br.ReadInt32();
                        keyboardData.WordTime = br.ReadSingle();
                        keyboardData.CorrectCount = br.ReadInt32();
                        keyboardData.MistakeCount = br.ReadInt32();

                        int magic = br.ReadInt32();
                        Debug.Log("LoadKeyboard(" + index + ") magic " + magic);
                    }
                }
            }
        }

    }
}

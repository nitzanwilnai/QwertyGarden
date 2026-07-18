using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace QwertyGarden
{
    public static class KeyboardDataIO
    {
        public static int VERSION = 5;
        public static void SaveKeyboard(KeyboardData keyboardData, int index)
        {
            string fileName = Application.persistentDataPath + "/keyboarddata_v" + VERSION + "_kb" + index + ".dat";
            using (FileStream fs = File.Create(fileName))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                bw.Write(keyboardData.WordIndex);
                bw.Write(keyboardData.KeyboardType);
                bw.Write(Balance.LETTERS);
                for (int i = 0; i < Balance.LETTERS; i++)
                    bw.Write(keyboardData.FlowerType[i]);
                for (int i = 0; i < Balance.LETTERS; i++)
                    bw.Write(keyboardData.NewFlowerType[i]);
                for (int i = 0; i < Balance.LETTERS; i++)
                    bw.Write(keyboardData.CharacterCount[i]);
                for (int i = 0; i < Balance.LETTERS; i++)
                    bw.Write(keyboardData.FlowerProgress[i]);

                for (int i = 0; i < Balance.MAX_FLOWER_TYPES; i++) // SHOULD NOT BE 26 !!!
                    bw.Write(keyboardData.FlowerCount[i]);

                for (int i = 0; i < Balance.LETTERS; i++)
                    bw.Write(keyboardData.PrestigeCount[i]);
                for (int i = 0; i < Balance.LETTERS; i++)
                    bw.Write(keyboardData.GrowTime[i]);

                bw.Write(keyboardData.TypedWord);

                bw.Write(keyboardData.WrongLetter);
                bw.Write(keyboardData.WPMWordCount);
                bw.Write(keyboardData.WPMCharacterCount);
                bw.Write(keyboardData.WPMWordTime);
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

        public static bool LoadKeyboard(KeyboardData keyboardData, int index)
        {
            bool success = false;

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
                            keyboardData.NewFlowerType[i] = br.ReadInt32();

                        for (int i = 0; i < maxFlowers; i++)
                            keyboardData.CharacterCount[i] = br.ReadInt32();

                        for (int i = 0; i < maxFlowers; i++)
                            keyboardData.FlowerProgress[i] = br.ReadInt32();

                        for (int i = 0; i < Balance.MAX_FLOWER_TYPES; i++)
                            keyboardData.FlowerCount[i] = br.ReadInt32();

                        for (int i = 0; i < maxFlowers; i++)
                            keyboardData.PrestigeCount[i] = br.ReadInt32();

                        for (int i = 0; i < maxFlowers; i++)
                            keyboardData.GrowTime[i] = br.ReadSingle();

                        keyboardData.TypedWord = br.ReadString();

                        keyboardData.WrongLetter = br.ReadInt32();
                        keyboardData.WPMWordCount = br.ReadInt32();
                        keyboardData.WPMCharacterCount = br.ReadInt32();
                        keyboardData.WPMWordTime = br.ReadSingle();
                        keyboardData.CorrectCount = br.ReadInt32();
                        keyboardData.MistakeCount = br.ReadInt32();

                        int magic = br.ReadInt32();
                        Debug.Log("LoadKeyboard(" + index + ") magic " + magic);

                        success = true;
                    }
                }
            }
            return success;
        }

        public static bool LoadKeyboardV4(KeyboardData keyboardData, int index)
        {
            bool success = false;

            string fileName = Application.persistentDataPath + "/keyboarddata_v4_kb" + index + ".dat";
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
                            keyboardData.NewFlowerType[i] = br.ReadInt32();

                        for (int i = 0; i < maxFlowers; i++)
                            keyboardData.CharacterCount[i] = br.ReadInt32();

                        for (int i = 0; i < maxFlowers; i++)
                            keyboardData.FlowerProgress[i] = br.ReadInt32();

                        for (int i = 0; i < Balance.MAX_FLOWER_TYPES; i++)
                            keyboardData.FlowerCount[i] = br.ReadInt32();

                        for (int i = 0; i < maxFlowers; i++)
                            keyboardData.PrestigeCount[i] = br.ReadInt32();

                        for (int i = 0; i < maxFlowers; i++)
                            keyboardData.GrowTime[i] = br.ReadSingle();

                        keyboardData.TypedWord = br.ReadString();

                        keyboardData.WrongLetter = br.ReadInt32();
                        keyboardData.WPMWordCount = br.ReadInt32();
                        keyboardData.WPMCharacterCount = br.ReadInt32();
                        keyboardData.WPMWordTime = br.ReadSingle();
                        keyboardData.CorrectCount = br.ReadInt32();
                        keyboardData.MistakeCount = br.ReadInt32();

                        int magic = br.ReadInt32();
                        Debug.Log("LoadKeyboardV4(" + index + ") magic " + magic);

                        success = true;
                    }
                }
            }
            return success;
        }

        public static bool LoadKeyboardV3(KeyboardData keyboardData, int index)
        {
            bool success = false;

            string fileName = Application.persistentDataPath + "/keyboarddata_v3_kb" + index + ".dat";
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
                            keyboardData.NewFlowerType[i] = br.ReadInt32();

                        for (int i = 0; i < maxFlowers; i++)
                            keyboardData.CharacterCount[i] = br.ReadInt32();

                        for (int i = 0; i < maxFlowers; i++)
                            keyboardData.FlowerProgress[i] = br.ReadInt32();

                        for (int i = 0; i < maxFlowers; i++)
                            keyboardData.FlowerCount[i] = br.ReadInt32();

                        for (int i = 0; i < maxFlowers; i++)
                            keyboardData.PrestigeCount[i] = br.ReadInt32();

                        for (int i = 0; i < maxFlowers; i++)
                            keyboardData.GrowTime[i] = br.ReadSingle();

                        keyboardData.TypedWord = br.ReadString();

                        keyboardData.WrongLetter = br.ReadInt32();
                        keyboardData.WPMWordCount = br.ReadInt32();
                        keyboardData.WPMCharacterCount = br.ReadInt32();
                        keyboardData.WPMWordTime = br.ReadSingle();
                        keyboardData.CorrectCount = br.ReadInt32();
                        keyboardData.MistakeCount = br.ReadInt32();

                        int magic = br.ReadInt32();
                        Debug.Log("LoadKeyboard(" + index + ") magic " + magic);

                        success = true;
                    }
                }
            }
            return success;
        }

        public static bool LoadKeyboardV2(KeyboardData keyboardData, int index)
        {
            bool success = false;

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
                            keyboardData.PrestigeCount[i] = br.ReadInt32();

                        for (int i = 0; i < maxFlowers; i++)
                            keyboardData.GrowTime[i] = br.ReadSingle();

                        keyboardData.TypedWord = br.ReadString();

                        keyboardData.WrongLetter = br.ReadInt32();
                        keyboardData.WPMWordCount = br.ReadInt32();
                        keyboardData.WPMCharacterCount = br.ReadInt32();
                        keyboardData.WPMWordTime = br.ReadSingle();
                        keyboardData.CorrectCount = br.ReadInt32();
                        keyboardData.MistakeCount = br.ReadInt32();

                        int magic = br.ReadInt32();
                        Debug.Log("LoadKeyboard(" + index + ") magic " + magic);

                        success = true;
                    }
                }
            }
            return success;
        }

        public static bool LoadKeyboardV1(KeyboardData keyboardData, int index)
        {
            bool success = false;

            int version = 1;
            string fileName = Application.persistentDataPath + "/keyboarddata_v" + version + "_kb" + index + ".dat";
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
                            keyboardData.PrestigeCount[i] = br.ReadInt32();

                        for (int i = 0; i < maxFlowers; i++)
                            keyboardData.GrowTime[i] = br.ReadSingle();

                        keyboardData.TypedWord = br.ReadString();

                        keyboardData.WrongLetter = br.ReadInt32();
                        keyboardData.WPMWordCount = br.ReadInt32();
                        keyboardData.WPMWordTime = br.ReadSingle();
                        keyboardData.CorrectCount = br.ReadInt32();
                        keyboardData.MistakeCount = br.ReadInt32();

                        int magic = br.ReadInt32();
                        Debug.Log("LoadKeyboard(" + index + ") magic " + magic);

                        success = true;
                    }
                }
            }
            return success;
        }
    }
}

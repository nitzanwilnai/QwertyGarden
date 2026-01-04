/*
  TypingGarden — Non-Commercial, No-Modification License
  Copyright © 2025 Nitzan Wilnai
  Source Code: https://github.com/nitzanwilnai/TypingGarden

  Permission is granted to view and run this code for non-commercial purposes only.
  Modification, redistribution of altered versions, and commercial use are strictly prohibited.

  See the LICENSE file for full legal terms.
*/

using UnityEngine;
using System.IO;
using Unity.VisualScripting;
using System;

namespace QwertyGarden
{
    public class Balance
    {
        public int NumFlowers;
        public string[] FlowerPrefab;
        public string[] FlowerCard;
        public string[] FlowerIcon;
        public string[] FlowerName;
        public int[] FlowerSeedCost;
        public int[] FlowerSellValue;
        public float[] FlowerGrowTime;
        public string[][] FlowerFrames;


        public int MaxFlowerFrames = 10;
        public int MaxKeyboards = 128;
        public int StartingCoins;
        public string[] LessonWords;
        public string[] Words;
        public int[][] WordsForLetters;
        public int[] LetterMultiplier;

        public void LoadBalance()
        {
            TextAsset asset = Resources.Load("balance") as TextAsset;
            LoadBalance(asset.bytes);
        }

        public void LoadBalance(byte[] array)
        {
            Stream s = new MemoryStream(array);
            using (BinaryReader br = new BinaryReader(s))
            {
                int version = br.ReadInt32();

                StartingCoins = br.ReadInt32();
                NumFlowers = br.ReadInt32();
                FlowerPrefab = new string[NumFlowers];
                FlowerCard = new string[NumFlowers];
                FlowerIcon = new string[NumFlowers];
                FlowerName = new string[NumFlowers];
                FlowerSeedCost = new int[NumFlowers];
                FlowerSellValue = new int[NumFlowers];
                FlowerGrowTime = new float[NumFlowers];
                FlowerFrames = new string[NumFlowers][];
                for (int i = 0; i < NumFlowers; i++)
                {
                    FlowerPrefab[i] = br.ReadString();
                    FlowerCard[i] = br.ReadString();
                    FlowerIcon[i] = br.ReadString();
                    FlowerName[i] = br.ReadString();
                    FlowerSeedCost[i] = br.ReadInt32();
                    FlowerSellValue[i] = br.ReadInt32();
                    FlowerGrowTime[i] = br.ReadSingle();
                    FlowerFrames[i] = new string[br.ReadInt32()];
                    for (int j = 0; j < FlowerFrames[i].Length; j++)
                        FlowerFrames[i][j] = br.ReadString();
                }
                loadLesson(br);

                loadWords(br);
            }
        }

        private void loadWords(BinaryReader br)
        {
            int numWords = br.ReadInt32();
            Words = new string[numWords];
            for (int wordIdx = 0; wordIdx < numWords; wordIdx++)
                Words[wordIdx] = br.ReadString();

            WordsForLetters = new int[26][];
            for (int letterIdx = 0; letterIdx < 26; letterIdx++)
            {
                int numWordsForLetter = br.ReadInt32();
                WordsForLetters[letterIdx] = new int[numWordsForLetter];
                for (int wordIdx = 0; wordIdx < numWordsForLetter; wordIdx++)
                    WordsForLetters[letterIdx][wordIdx] = br.ReadInt32();
            }

            LetterMultiplier = new int[26];
            for (int letterIdx = 0; letterIdx < 26; letterIdx++)
                LetterMultiplier[letterIdx] = br.ReadInt32();
        }

        private void loadLesson(BinaryReader br)
        {
            int numLessonWords = br.ReadInt32();
            LessonWords = new string[numLessonWords];
            for (int i = 0; i < numLessonWords; i++)
                LessonWords[i] = br.ReadString();
        }
    }
}
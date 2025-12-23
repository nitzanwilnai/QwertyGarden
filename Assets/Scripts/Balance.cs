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

namespace QwertyGarden
{
    public class Balance
    {
        public int NumFlowerFrames = 10;
        public int MaxKeyboards = 128;
        public string[] LessonWords;
        public string[] Words;
        public int[][] WordsForLetters;

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
/*
  TypingGarden — Non-Commercial, No-Modification License
  Copyright © 2025 Nitzan Wilnai
  Source Code: https://github.com/nitzanwilnai/TypingGarden

  Permission is granted to view and run this code for non-commercial purposes only.
  Modification, redistribution of altered versions, and commercial use are strictly prohibited.

  See the LICENSE file for full legal terms.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework.Internal;
using UnityEditor;
using UnityEngine;


namespace QwertyGarden
{
    public class BalanceParser : MonoBehaviour
    {
#if UNITY_EDITOR
        [MenuItem("TypingGarden/Balance/Parse Local")]
        public static void ParseLocal()
        {
            Debug.Log("Parse balance started!");

            byte[] array = parse();
            // save array
            string path = "Assets/Resources/balance.bytes";
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            using (FileStream fs = File.Create(path))
            using (BinaryWriter bw = new BinaryWriter(fs))
                bw.Write(array);

            Debug.Log("Parse balance finished!");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        static byte[] parse()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(stream))
                {
                    int version = 2;
                    bw.Write(version);

                    parseLesson(bw);

                    parseWords(bw);

                }
                return stream.ToArray();
            }
        }

        static void parseLesson(BinaryWriter bw)
        {
            TextAsset textFile = Resources.Load<TextAsset>("typinglesson");
            string allWords = textFile.text;
            string[] words = allWords.Split();

            bw.Write(words.Length);
            for (int i = 0; i < words.Length; i++)
                bw.Write(words[i].ToUpper());
        }

        static void parseWords(BinaryWriter bw)
        {
            // TextAsset textFile = Resources.Load<TextAsset>("words_10000");
            TextAsset textFile = Resources.Load<TextAsset>("typinggarden");
            string allWords = textFile.text;
            string[] words = allWords.Split();
            Debug.Log("words.length " + words.Length);

            for (int i = 0; i < words.Length; i++)
                words[i] = words[i].ToUpper();

            Span<int> wordCount = stackalloc int[128];
            int longestWord = 0;
            for (int i = 0; i < 128; i++)
                wordCount[i] = 0;
            for (int i = 0; i < words.Length; i++)
            {
                wordCount[words[i].Length]++;
                if (longestWord < words[i].Length)
                    longestWord = words[i].Length;
            }

            Debug.Log("longestWord " + longestWord);
            longestWord++;

            for (int i = 0; i < longestWord; i++)
                if (wordCount[i] > 0)
                    Debug.Log("word length " + i + " = " + wordCount[i]);

            string[][] m_wordsForLength;
            m_wordsForLength = new string[longestWord][];
            for (int i = 0; i < longestWord; i++)
                m_wordsForLength[i] = new string[wordCount[i]];

            Span<int> wordCounter = stackalloc int[longestWord];
            for (int i = 0; i < longestWord; i++)
                wordCounter[i] = 0;

            for (int i = 0; i < words.Length; i++)
            {
                int length = words[i].Length;
                // Debug.Log("words[" + i + "] " + words[i] + " Length " + words[i].Length);
                // Debug.Log("WordCounter[length] " + wordCounter[length] + " m_wordsForLength[" + length + "].Length " + m_wordsForLength[length].Length);
                m_wordsForLength[length][wordCounter[length]++] = words[i];
            }

            // Span<int> letterCounter = stackalloc int[26];
            // for (int wordIdx = 0; wordIdx < words.Length; wordIdx++)
            // {
            //     for (int letterIdx = 0; letterIdx < words[wordIdx].Length; letterIdx++)
            //     {
            //         for (char c = 'A'; c <= 'Z'; c++)
            //             if (words[wordIdx][letterIdx] == c)
            //                 letterCounter[(int)c - 65]++;
            //     }
            // }

            // for (int i = 0; i < 26; i++)
            //     Debug.Log((char)(i + 65) + " count " + letterCounter[i]);

            int[][] wordsForLetters = new int[26][];
            Span<int> letterCounter = stackalloc int[26];
            for (int i = 0; i < 26; i++)
                letterCounter[i] = 0;
            for (int wordIdx = 0; wordIdx < words.Length; wordIdx++)
                for (char c = 'A'; c <= 'Z'; c++)
                    if (words[wordIdx].Contains(c))
                        letterCounter[(int)c - 65]++;

            // count words
            for (int i = 0; i < 26; i++)
                wordsForLetters[i] = new int[letterCounter[i]];

            Span<int> letterWordCount = stackalloc int[wordCount.Length];
            for (int i = 0; i < letterWordCount.Length; i++)
                letterWordCount[i] = 0;

            for (int wordIdx = 0; wordIdx < words.Length; wordIdx++)
                for (char c = 'A'; c <= 'Z'; c++)
                    if (words[wordIdx].Contains(c))
                    {
                        wordsForLetters[(int)c - 65][letterWordCount[(int)c - 65]++] = wordIdx;
                        // Debug.Log("Words["+wordIdx+"] " + words[wordIdx] + " contains letter " + c);
                    }

            for (int i = 0; i < 26; i++)
                if (letterWordCount[i] != letterCounter[i])
                    Debug.Log("letterWordCount[" + i + "] " + letterWordCount[i] + " != letterCounter[" + i + "]) " + letterCounter[i]);

            for (int i = 0; i < 26; i++)
                Debug.Log((char)(i + 65) + " word count " + wordsForLetters[i].Length);

            bw.Write(words.Length);
            for (int wordIdx = 0; wordIdx < words.Length; wordIdx++)
                bw.Write(words[wordIdx]);
            for (int letterIdx = 0; letterIdx < 26; letterIdx++)
            {
                bw.Write(wordsForLetters[letterIdx].Length);
                for (int wordIdx = 0; wordIdx < wordsForLetters[letterIdx].Length; wordIdx++)
                    bw.Write(wordsForLetters[letterIdx][wordIdx]);
            }


            // count letters
            for (int i = 0; i < 26; i++)
                letterCounter[i] = 0;

            for (int wordIdx = 0; wordIdx < words.Length; wordIdx++)
            {
                bool includeWord = false;
                for (int letterIdx = 0; letterIdx < words[wordIdx].Length; letterIdx++)
                {
                    if (words[wordIdx][letterIdx] == 'Z')
                        includeWord = true;
                    if (words[wordIdx][letterIdx] == 'Q')
                        includeWord = true;
                    if (words[wordIdx][letterIdx] == 'J')
                        includeWord = true;
                    if (words[wordIdx][letterIdx] == 'X')
                        includeWord = true;
                    if (words[wordIdx][letterIdx] == 'W')
                        includeWord = true;
                }

                if (includeWord)
                    for (int letterIdx = 0; letterIdx < words[wordIdx].Length; letterIdx++)
                        for (char c = 'A'; c <= 'Z'; c++)
                            if (words[wordIdx][letterIdx] == c)
                                letterCounter[(int)c - 65]++;
            }

            for (int i = 0; i < 26; i++)
                Debug.Log((char)(i + 65) + " letter count " + letterCounter[i]);


            /*
                        // write the letters A to Z
                        bw.Write(26);
                        for (int i = 0; i < 26; i++)
                            bw.Write(((char)(i + 65)).ToString());

                        for (int length = 2; length < longestWord; length++)
                        {
                            bw.Write(wordCount[length]);
                            for (int wordIdx = 0; wordIdx < wordCount[length]; wordIdx++)
                                bw.Write(m_wordsForLength[length][wordIdx]);
                        }
            */
        }

        public static void AddObjectsFromDirectory(string path, List<UnityEngine.Object> items, System.Type type)
        {
            if (Directory.Exists(path))
            {
                string[] assets = Directory.GetFiles(path);
                foreach (string assetPath in assets)
                    if (assetPath.Contains(".asset") && !assetPath.Contains(".meta"))
                        items.Add(AssetDatabase.LoadAssetAtPath(assetPath, type));

                string[] directories = Directory.GetDirectories(path);
                foreach (string directory in directories)
                    if (Directory.Exists(directory))
                        AddObjectsFromDirectory(directory, items, type);
            }
        }
#endif
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

namespace QwertyGarden
{
    public static class KeyboardLogic
    {
        public static void InitKeyboardData(KeyboardData keyboardData)
        {
            keyboardData.CharacterCount = new int[26];
            keyboardData.FlowerProgress = new int[26];
            keyboardData.FlowerCount = new int[26];
            keyboardData.PrestigeCount = new int[26];
            keyboardData.GrowTime = new float[26];
            keyboardData.FlowerType = new int[26];
            keyboardData.NewFlowerType = new int[26];
            keyboardData.TypedWord = "";
        }

        public static void StartGame(KeyboardData keyboardData)
        {
            for (int i = 0; i < 26; i++)
                keyboardData.CharacterCount[i] = 0;

            ContinueGame(keyboardData);
        }

        public static void ContinueGame(KeyboardData keyboardData)
        {
            for (int i = 0; i < 26; i++)
                keyboardData.FlowerProgress[i] = 0;

            for (int i = 0; i < 26; i++)
                keyboardData.FlowerCount[i] = 0;

            for (int i = 0; i < 26; i++)
                keyboardData.GrowTime[i] = 0.0f;

            keyboardData.WrongLetter = 0;
        }

        public static int GetWPM(KeyboardData keyboardData)
        {
            return Mathf.RoundToInt(keyboardData.WPMCharacterCount / 5 / (keyboardData.WPMWordTime / 60.0f));
        }

        public static int GetAccuracy(KeyboardData keyboardData)
        {
            return Mathf.FloorToInt((keyboardData.CorrectCount - keyboardData.MistakeCount) / (float)keyboardData.CorrectCount * 100.0f);
        }

        public static float GetWPMBonus(MetaData metaData, KeyboardData keyboardData)
        {
            if (keyboardData.WPMWordTime <= 0.0f)
                return 1.0f;
            if (metaData.WPM)
                return 2.0f;
            return 1.0f + keyboardData.WPMWordCount / (keyboardData.WPMWordTime / 60.0f) / 100.0f;
        }

        public static float GetAccuracyBonus(MetaData metaData, KeyboardData keyboardData)
        {
            if (keyboardData.CorrectCount == 0)
                return 0.0f;
            if (metaData.WPM)
                return 2.0f;
            return 1.0f + (keyboardData.CorrectCount - keyboardData.MistakeCount) / (float)keyboardData.CorrectCount;
        }

        public static void ResetWPMAndAccuracy(KeyboardData keyboardData)
        {
            keyboardData.WPMWordCount = 0;
            keyboardData.WPMCharacterCount = 0;
            keyboardData.WPMWordTime = 0.0f;
            keyboardData.CorrectCount = 0;
            keyboardData.MistakeCount = 0;
        }

        public static void IncrementCharacterCount(MetaData metaData, KeyboardData keyboardData, Balance balance, char c)
        {
            int keyIndex = c - 65;
            int flowerType = keyboardData.FlowerType[keyIndex];
            keyboardData.CharacterCount[keyIndex]++;
            keyboardData.FlowerProgress[keyIndex]++;
            if (keyboardData.FlowerProgress[keyIndex] >= balance.MaxFlowerFrames)
            {
                if (keyboardData.PrestigeCount[keyIndex] >= metaData.Prestige)
                {
                    keyboardData.FlowerProgress[keyIndex] = 0;
                    keyboardData.PrestigeCount[keyIndex] = 0;
                }
                else
                {
                    keyboardData.FlowerProgress[keyIndex] = balance.MaxFlowerFrames - 1;
                    keyboardData.PrestigeCount[keyIndex]++;
                }
                keyboardData.FlowerCount[flowerType]++;
                // metaData.Coins += balance.FlowerSellValue[flowerType];
            }
        }

        public static void TryAddCharacter(MetaData metaData, KeyboardData keyboardData, Balance balance, char c, ref bool wordComplete, ref bool incorrectCharacter, string currentWord, ref float startTime)
        {
            if (keyboardData.TypedWord.Length == 0)
                startTime = Time.realtimeSinceStartup;

            if (currentWord[keyboardData.TypedWord.Length] != c)
            {
                incorrectCharacter = true;

                if (keyboardData.WrongLetter == 0)
                {
                    keyboardData.MistakeCount++;
                }

                // decrease flower
                // if (keyboardData.WrongCount == 0)
                // {
                //     int index = currentWord[keyboardData.TypedWord.Length] - 65;
                //     keyboardData.FlowerProgress[index]--;
                //     if (keyboardData.FlowerProgress[index] < 0)
                //         keyboardData.FlowerProgress[index] = 0;
                // }
                keyboardData.WrongLetter++;
            }
            else
            {
                IncrementCharacterCount(metaData, keyboardData, balance, c);
                keyboardData.TypedWord += c;
                keyboardData.WrongLetter = 0;

                keyboardData.CorrectCount++;

                if (string.Compare(keyboardData.TypedWord, currentWord) == 0)
                {
                    wordComplete = true;
                    keyboardData.WrongLetter = 0;

                    keyboardData.WPMWordCount++;
                    keyboardData.WPMCharacterCount += keyboardData.TypedWord.Length;
                    keyboardData.WPMWordTime += Time.realtimeSinceStartup - startTime;
                }
            }
        }

        public static int GetTypedKeyIndex(out char c)
        {
            c = ' ';
            if (Keyboard.current.aKey.wasReleasedThisFrame)
            {
                c = 'a';
                return 0;
            }
            if (Keyboard.current.bKey.wasReleasedThisFrame)
            {
                c = 'b';
                return 1;
            }
            if (Keyboard.current.cKey.wasReleasedThisFrame)
            {
                c = 'c';
                return 2;
            }
            if (Keyboard.current.dKey.wasReleasedThisFrame)
            {
                c = 'd';
                return 3;
            }
            if (Keyboard.current.eKey.wasReleasedThisFrame)
            {
                c = 'e';
                return 4;
            }
            if (Keyboard.current.fKey.wasReleasedThisFrame)
            {
                c = 'f';
                return 5;
            }
            if (Keyboard.current.gKey.wasReleasedThisFrame)
            {
                c = 'g';
                return 6;
            }
            if (Keyboard.current.hKey.wasReleasedThisFrame)
            {
                c = 'h';
                return 7;
            }
            if (Keyboard.current.iKey.wasReleasedThisFrame)
            {
                c = 'i';
                return 8;
            }
            if (Keyboard.current.jKey.wasReleasedThisFrame)
            {
                c = 'j';
                return 9;
            }
            if (Keyboard.current.kKey.wasReleasedThisFrame)
            {
                c = 'k';
                return 10;
            }
            if (Keyboard.current.lKey.wasReleasedThisFrame)
            {
                c = 'l';
                return 11;
            }
            if (Keyboard.current.mKey.wasReleasedThisFrame)
            {
                c = 'm';
                return 12;
            }
            if (Keyboard.current.nKey.wasReleasedThisFrame)
            {
                c = 'n';
                return 13;
            }
            if (Keyboard.current.oKey.wasReleasedThisFrame)
            {
                c = 'o';
                return 14;
            }
            if (Keyboard.current.pKey.wasReleasedThisFrame)
            {
                c = 'p';
                return 15;
            }
            if (Keyboard.current.qKey.wasReleasedThisFrame)
            {
                c = 'q';
                return 16;
            }
            if (Keyboard.current.rKey.wasReleasedThisFrame)
            {
                c = 'r';
                return 17;
            }
            if (Keyboard.current.sKey.wasReleasedThisFrame)
            {
                c = 's';
                return 18;
            }
            if (Keyboard.current.tKey.wasReleasedThisFrame)
            {
                c = 't';
                return 19;
            }
            if (Keyboard.current.uKey.wasReleasedThisFrame)
            {
                c = 'u';
                return 20;
            }
            if (Keyboard.current.vKey.wasReleasedThisFrame)
            {
                c = 'v';
                return 21;
            }
            if (Keyboard.current.wKey.wasReleasedThisFrame)
            {
                c = 'w';
                return 22;
            }
            if (Keyboard.current.xKey.wasReleasedThisFrame)
            {
                c = 'x';
                return 23;
            }
            if (Keyboard.current.yKey.wasReleasedThisFrame)
            {
                c = 'y';
                return 24;
            }
            if (Keyboard.current.zKey.wasReleasedThisFrame)
            {
                c = 'z';
                return 25;
            }
            return -1;
        }
    }
}
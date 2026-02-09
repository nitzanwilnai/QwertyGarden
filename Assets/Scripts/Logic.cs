using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace QwertyGarden
{
    public static class Logic
    {
        public static void InitMetaData(MetaData metaData, Balance balance)
        {
            metaData.Coins = balance.StartingCoins;
            metaData.SFX = true;
            metaData.Music = true;
            metaData.WPM = true;
            metaData.TotalCollectedCount = 0;
            metaData.FlowerCollectedCount = new int[Balance.MAX_FLOWER_TYPES];
        }

        public static void SetMenuState(MetaData metaData, MENU_STATE newMenuState)
        {
            metaData.MenuState = newMenuState;
        }

        public static double GetSellValue(MetaData metaData, KeyboardData keyboardData, Balance balance)
        {
            double totalSellValue = 0;
            for (int flowerType = 0; flowerType < balance.NumFlowers; flowerType++)
                totalSellValue += balance.FlowerSellValue[flowerType] * keyboardData.FlowerCount[flowerType];

            float accuracyBonus = Logic.GetAccuracyBonus(metaData, keyboardData);
            if (accuracyBonus == 0.0f)
                accuracyBonus = 2.0f;
            float wpmBonus = Logic.GetWPMBonus(metaData, keyboardData);
            return System.Math.Floor(totalSellValue * accuracyBonus * wpmBonus);
        }

        public static void SellCollectedFlowers(MetaData metaData, KeyboardData keyboardData, Balance balance)
        {
            metaData.Coins += GetSellValue(metaData, keyboardData, balance);

            for (int flowerType = 0; flowerType < balance.NumFlowers; flowerType++)
                keyboardData.FlowerCount[flowerType] = 0;
        }

        public static bool IsFlagSet(int flags, int index)
        {
            return (flags & (1 << index)) > 0;
        }

        public static void SetFlag(ref int flags, int index)
        {
            flags |= 1 << index;
        }

        public static int CountDigits(double value)
        {
            if (value == 0.0) return 1;
            value = System.Math.Abs(value);

            return (int)System.Math.Floor(System.Math.Log10(value)) + 1;
        }

        public static double PowerOf10(double value)
        {
            if (value < 1.0)
                return 0.0;

            double result = 1.0;

            while (value >= 10.0)
            {
                value *= 0.1;
                result *= 10.0;
            }

            return result;
        }

        public static void TryShowPrestige(MetaData metaData)
        {
            if (metaData.Coins >= 1000)
                metaData.ShowPrestige = true;
        }

        public static double GetPrestigeCost(MetaData metaData, Balance balance)
        {
            double prestigeCost = balance.PrestigeCost;

            for (int i = 0; i < metaData.Prestige; i++)
                prestigeCost *= balance.PrestigeMultiplier;

            return prestigeCost;
        }

        public static bool TryPrestige(MetaData metaData, KeyboardData keyboardData, Balance balance)
        {
            double prestigeCost = GetPrestigeCost(metaData, balance);
            double totalSellValue = GetSellValue(metaData, keyboardData, balance);

            if (prestigeCost < metaData.Coins + totalSellValue)
            {
                metaData.Coins = 0;
                metaData.Prestige++;

                for (int i = 0; i < Balance.LETTERS; i++)
                    keyboardData.FlowerType[i] = 0;

                for (int i = 0; i < Balance.LETTERS; i++)
                    keyboardData.NewFlowerType[i] = 0;

                for (int i = 0; i < Balance.LETTERS; i++)
                    keyboardData.FlowerProgress[i] = 0;

                for (int i = 0; i < 26; i++)
                    keyboardData.FlowerCount[i] = 0;

                for (int i = 0; i < Balance.LETTERS; i++)
                    keyboardData.GrowTime[i] = 0.0f;

                return true;
            }
            return false;
        }

        public static void StartCozy(KeyboardData keyboardData, MetaData metaData, Balance balance)
        {
            commonStartCozy(metaData, keyboardData, balance, 0);
        }

        public static double CalculateGardenCost(KeyboardData keyboardData, Balance balance, Span<int> newFlowerTypes)
        {
            double totalCost = 0;
            for (int keyIdx = 0; keyIdx < Balance.LETTERS; keyIdx++)
            {
                int oldFlowerType = keyboardData.FlowerType[keyIdx];
                int newFlowerType = newFlowerTypes[keyIdx];
                if (newFlowerType != oldFlowerType)
                {
                    double newCost = balance.FlowerSeedCost[newFlowerType];
                    totalCost += newCost;
                }
            }
            return totalCost;
        }

        public static double CalculateCurrentGardenCost(KeyboardData keyboardData, Balance balance)
        {
            double totalCost = 0;
            for (int keyIdx = 0; keyIdx < Balance.LETTERS; keyIdx++)
            {
                int oldFlowerType = keyboardData.FlowerType[keyIdx];
                int newFlowerType = keyboardData.NewFlowerType[keyIdx];
                if (newFlowerType != oldFlowerType)
                {
                    double newCost = balance.FlowerSeedCost[newFlowerType];
                    totalCost += newCost;
                }
            }
            return totalCost;
        }

        public static bool TryEditCozy(KeyboardData keyboardData, MetaData metaData, Balance balance)
        {
            double totalCost = CalculateCurrentGardenCost(keyboardData, balance);

            if (totalCost <= metaData.Coins)
            {
                for (int keyIdx = 0; keyIdx < Balance.LETTERS; keyIdx++)
                    if (keyboardData.FlowerType[keyIdx] != keyboardData.NewFlowerType[keyIdx])
                        keyboardData.FlowerProgress[keyIdx] = 0;

                for (int keyIdx = 0; keyIdx < Balance.LETTERS; keyIdx++)
                    keyboardData.FlowerType[keyIdx] = keyboardData.NewFlowerType[keyIdx];

                for (int keyIdx = 0; keyIdx < 26; keyIdx++)
                    keyboardData.FlowerCount[keyIdx] = 0;

                keyboardData.WrongLetter = 0;

                commonStartCozy(metaData, keyboardData, balance, totalCost);

                return true;
            }
            return false;
        }

        static void commonStartCozy(MetaData metaData, KeyboardData keyboardData, Balance balance, double totalCost)
        {
            metaData.Coins -= totalCost;

            int randomIndex = Mathf.FloorToInt(UnityEngine.Random.value * balance.Words.Length);
            keyboardData.WordIndex = randomIndex;

            ResetWPMAndAccuracy(keyboardData);

            assignNextGameWord(keyboardData, balance);
        }

        public static void GameTyping(MetaData metaData, KeyboardData keyboardData, Balance balance, char c, out bool wordComplete, out bool incorrectCharacter, ref float startTime)
        {
            wordComplete = false;
            incorrectCharacter = false;

            string currentWord = balance.Words[keyboardData.WordIndex];

            TryAddCharacter(metaData, keyboardData, balance, c, ref wordComplete, ref incorrectCharacter, currentWord, ref startTime);

            if (wordComplete)
                assignNextGameWord(keyboardData, balance);
        }

        public static void assignNextGameWord(KeyboardData keyboardData, Balance balance)
        {
            int lowestValue = int.MaxValue;
            int lowestUsedLetter = 0;

            for (int keyIdx = 0; keyIdx < Balance.LETTERS; keyIdx++)
            {
                if (keyboardData.CharacterCount[keyIdx] < lowestValue)
                {
                    lowestValue = keyboardData.CharacterCount[keyIdx];
                    lowestUsedLetter = keyIdx;
                }

            }

            // if (lowestUsedLetter == -1)
            {
                lowestValue = int.MaxValue;
                // search backwards because rare letters are at the end (WXYZ)
                for (int letterIdx = 25; letterIdx >= 0; letterIdx--)
                {
                    if (keyboardData.CharacterCount[letterIdx] < lowestValue)
                    {
                        lowestUsedLetter = letterIdx;
                        lowestValue = keyboardData.CharacterCount[letterIdx];
                    }
                }
            }

            string s1 = "Character count: ";
            for (int keyIdx = 0; keyIdx < Balance.LETTERS; keyIdx++)
                s1 += (char)(keyIdx + 65) + " " + keyboardData.CharacterCount[keyIdx] + "\t";
            Debug.Log(s1);

            // assign random word for lowest used letter
            int randomWord = Mathf.FloorToInt(UnityEngine.Random.value * balance.WordsForLetters[lowestUsedLetter].Length);
            keyboardData.WordIndex = balance.WordsForLetters[lowestUsedLetter][randomWord];
            keyboardData.TypedWord = "";
            Debug.Log("balance.WordsForLetters[" + lowestUsedLetter + "][" + randomWord + "] " + balance.WordsForLetters[lowestUsedLetter][randomWord]);
            Debug.Log("assignNextGameWord() lowestUsedLetter = " + (char)(lowestUsedLetter + 65) + " lowestValue " + lowestValue + " new word " + balance.Words[keyboardData.WordIndex]);
        }

        public static void InitKeyboardData(KeyboardData keyboardData)
        {
            keyboardData.CharacterCount = new int[Balance.LETTERS];
            keyboardData.FlowerProgress = new int[Balance.LETTERS];
            keyboardData.FlowerCount = new int[Balance.MAX_FLOWER_TYPES];
            keyboardData.PrestigeCount = new int[Balance.LETTERS];
            keyboardData.GrowTime = new float[Balance.LETTERS];
            keyboardData.FlowerType = new int[Balance.LETTERS];
            keyboardData.NewFlowerType = new int[Balance.LETTERS];
            keyboardData.TypedWord = "";
        }

        public static void StartGame(KeyboardData keyboardData)
        {
            for (int i = 0; i < Balance.LETTERS; i++)
                keyboardData.CharacterCount[i] = 0;

            ContinueGame(keyboardData);
        }

        public static void ContinueGame(KeyboardData keyboardData)
        {
            for (int i = 0; i < Balance.LETTERS; i++)
                keyboardData.FlowerProgress[i] = 0;

            for (int i = 0; i < Balance.MAX_FLOWER_TYPES; i++)
                keyboardData.FlowerCount[i] = 0;

            for (int i = 0; i < Balance.LETTERS; i++)
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
                metaData.FlowerCollectedCount[flowerType]++;
                metaData.TotalCollectedCount++;
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
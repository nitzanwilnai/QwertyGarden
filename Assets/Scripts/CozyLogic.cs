using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace QwertyGarden
{
    public static class CozyLogic
    {
        public static void StartCozy(KeyboardData keyboardData, MetaData metaData, Balance balance)
        {
            commonStartCozy(metaData, keyboardData, balance, 0);
        }

        public static double CalculateGardenCost(KeyboardData keyboardData, Balance balance, Span<int> newFlowerTypes)
        {
            double totalCost = 0;
            for (int keyIdx = 0; keyIdx < 26; keyIdx++)
            {
                int oldFlowerType = keyboardData.FlowerType[keyIdx];
                int newFlowerType = newFlowerTypes[keyIdx];
                if (newFlowerType != oldFlowerType)
                {
                    int newCost = balance.FlowerSeedCost[newFlowerType];
                    totalCost += newCost;
                }
            }
            return totalCost;
        }

        public static double CalculateCurrentGardenCost(KeyboardData keyboardData, Balance balance)
        {
            double totalCost = 0;
            for (int keyIdx = 0; keyIdx < 26; keyIdx++)
            {
                int oldFlowerType = keyboardData.FlowerType[keyIdx];
                int newFlowerType = keyboardData.NewFlowerType[keyIdx];
                if (newFlowerType != oldFlowerType)
                {
                    int newCost = balance.FlowerSeedCost[newFlowerType];
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
                for (int keyIdx = 0; keyIdx < 26; keyIdx++)
                    if (keyboardData.FlowerType[keyIdx] != keyboardData.NewFlowerType[keyIdx])
                        keyboardData.FlowerProgress[keyIdx] = 0;

                for (int keyIdx = 0; keyIdx < 26; keyIdx++)
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

            KeyboardLogic.ResetWPMAndAccuracy(keyboardData);

            assignNextGameWord(keyboardData, balance);
        }

        public static void GameTyping(MetaData metaData, KeyboardData keyboardData, Balance balance, char c, out bool wordComplete, out bool incorrectCharacter, ref float startTime)
        {
            wordComplete = false;
            incorrectCharacter = false;

            string currentWord = balance.Words[keyboardData.WordIndex];

            KeyboardLogic.TryAddCharacter(metaData, keyboardData, balance, c, ref wordComplete, ref incorrectCharacter, currentWord, ref startTime);

            if (wordComplete)
                assignNextGameWord(keyboardData, balance);
        }

        public static void assignNextGameWord(KeyboardData keyboardData, Balance balance)
        {
            int lowestValue = int.MaxValue;
            int lowestUsedLetter = 0;

            for (int keyIdx = 0; keyIdx < 26; keyIdx++)
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
            for (int keyIdx = 0; keyIdx < 26; keyIdx++)
                s1 += (char)(keyIdx + 65) + " " + keyboardData.CharacterCount[keyIdx] + "\t";
            Debug.Log(s1);

            // assign random word for lowest used letter
            int randomWord = Mathf.FloorToInt(UnityEngine.Random.value * balance.WordsForLetters[lowestUsedLetter].Length);
            keyboardData.WordIndex = balance.WordsForLetters[lowestUsedLetter][randomWord];
            keyboardData.TypedWord = "";
            Debug.Log("balance.WordsForLetters[" + lowestUsedLetter + "][" + randomWord + "] " + balance.WordsForLetters[lowestUsedLetter][randomWord]);
            Debug.Log("assignNextGameWord() lowestUsedLetter = " + (char)(lowestUsedLetter + 65) + " lowestValue " + lowestValue + " new word " + balance.Words[keyboardData.WordIndex]);
        }
    }
}
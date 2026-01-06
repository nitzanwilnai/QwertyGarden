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

        public static bool TryStartCozy(KeyboardData keyboardData, MetaData metaData, Balance balance)
        {
            Span<int> flowerCount = stackalloc int[balance.NumFlowers];
            for (int keyIdx = 0; keyIdx < 26; keyIdx++)
            {
                int flowerType = keyboardData.FlowerType[keyIdx];
                flowerCount[flowerType]++;
            }
            double totalCost = 0;
            for (int flowerType = 0; flowerType < balance.NumFlowers; flowerType++)
                if (flowerCount[flowerType] > 0)
                    totalCost += balance.FlowerSeedCost[flowerType] * flowerCount[flowerType];

            if (totalCost <= metaData.Coins)
            {
                commonStartCozy(metaData, keyboardData, balance, totalCost);

                return true;
            }
            return false;
        }

        public static bool TryEditCozy(KeyboardData keyboardData, MetaData metaData, Balance balance, int[] newFlowerType)
        {
            double totalCost = 0;
            for (int keyIdx = 0; keyIdx < 26; keyIdx++)
            {
                if (newFlowerType[keyIdx] != keyboardData.FlowerType[keyIdx])
                {
                    int flowerType = newFlowerType[keyIdx];
                    totalCost += balance.FlowerSeedCost[flowerType];
                }
            }

            if (totalCost <= metaData.Coins)
            {
                for (int keyIdx = 0; keyIdx < 26; keyIdx++)
                    keyboardData.FlowerType[keyIdx] = newFlowerType[keyIdx];

                commonStartCozy(metaData, keyboardData, balance, totalCost);

                return true;
            }
            return false;
        }

        static void commonStartCozy(MetaData metaData, KeyboardData keyboardData, Balance balance, double totalCost)
        {
            metaData.Coins -= totalCost;
            KeyboardLogic.StartGame(keyboardData);

            int randomIndex = Mathf.FloorToInt(UnityEngine.Random.value * balance.Words.Length);
            keyboardData.WordIndex = randomIndex;

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

        static void assignedNextWordWeighted(KeyboardData keyboardData, Balance balance)
        {
            int largestCount = 0;
            int totalWeight = 0;
            for (int keyIdx = 0; keyIdx < 26; keyIdx++)
            {
                if (largestCount < keyboardData.CharacterCount[keyIdx])
                    largestCount = keyboardData.CharacterCount[keyIdx];
            }

            // reverse character count to use as weights
            Span<int> weights = stackalloc int[26];
            for (int keyIdx = 0; keyIdx < 26; keyIdx++)
                weights[keyIdx] = largestCount - keyboardData.CharacterCount[keyIdx];

            for (int keyIdx = 0; keyIdx < 26; keyIdx++)
                weights[keyIdx] = weights[keyIdx] * 10000;

            for (int keyIdx = 0; keyIdx < 26; keyIdx++)
                totalWeight += keyboardData.CharacterCount[keyIdx];

            int randomWeight = Mathf.FloorToInt(UnityEngine.Random.value * totalWeight);
            int randomIndex = 0;
            totalWeight = 0;
            for (int keyIdx = 0; keyIdx < 26; keyIdx++)
            {
                totalWeight += weights[keyIdx];
                if (totalWeight > randomWeight)
                {
                    randomIndex = keyIdx;
                    break;
                }
            }

            string s1 = "Character count: ";
            string s2 = "Weights ";
            for (int keyIdx = 0; keyIdx < 26; keyIdx++)
            {
                s1 += (char)(keyIdx + 65) + " " + keyboardData.CharacterCount[keyIdx] + "\t";
                s2 += (char)(keyIdx + 65) + " " + weights[keyIdx] + "\t";
            }

            Debug.Log(s1);
            Debug.Log(s2);

            int randomWord = Mathf.FloorToInt(UnityEngine.Random.value * balance.WordsForLetters[randomIndex].Length);
            keyboardData.WordIndex = balance.WordsForLetters[randomIndex][randomWord];
            keyboardData.TypedWord = "";
            Debug.Log("balance.WordsForLetters[" + randomIndex + "][" + randomWord + "] " + balance.WordsForLetters[randomIndex][randomWord]);
            Debug.Log("assignNextGameWord() lowestUsedLetter = " + (char)(randomIndex + 65) + " new word " + balance.Words[keyboardData.WordIndex]);
        }

        static void assignNextGameWord(KeyboardData keyboardData, Balance balance)
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
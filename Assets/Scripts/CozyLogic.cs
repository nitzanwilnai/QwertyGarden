using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace QwertyGarden
{
    public static class CozyLogic
    {
        public static bool TryStartCozy(KeyboardData keyboardData, MetaData metaData, Balance balance)
        {
            Span<int> flowerCount = stackalloc int[balance.NumFlowers];
            for (int keyIdx = 0; keyIdx < 26; keyIdx++)
            {
                int flowerType = keyboardData.FlowerType[keyIdx];
                flowerCount[flowerType]++;
            }
            decimal totalCost = 0;
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
            decimal totalCost = 0;
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

        static void commonStartCozy(MetaData metaData, KeyboardData keyboardData, Balance balance, decimal totalCost)
        {
            metaData.Coins -= totalCost;
            KeyboardLogic.StartGame(keyboardData);

            int randomIndex = Mathf.FloorToInt(UnityEngine.Random.value * balance.Words.Length);
            keyboardData.WordIndex = randomIndex;

            assignNextGameWord(keyboardData, balance);
        }

        public static void GameTyping(MetaData metaData, KeyboardData keyboardData, Balance balance, char c, out bool wordComplete, out bool incorrectCharacter)
        {
            wordComplete = false;
            incorrectCharacter = false;

            string currentWord = balance.Words[keyboardData.WordIndex];

            KeyboardLogic.TryAddCharacter(metaData, keyboardData, balance, c, ref wordComplete, ref incorrectCharacter, currentWord);

            if (wordComplete)
                assignNextGameWord(keyboardData, balance);
        }

        static void assignNextGameWord(KeyboardData keyboardData, Balance balance)
        {
            int lowestValue = int.MaxValue;
            int lowestUsedLetter = -1;

            for (int letterIdx = 25; letterIdx >= 0; letterIdx--)
            {
                if (keyboardData.FlowerProgress[letterIdx] < balance.MaxFlowerFrames - 1 && keyboardData.FlowerProgress[letterIdx] < lowestValue)
                {
                    lowestUsedLetter = letterIdx;
                    lowestValue = keyboardData.CharacterCount[letterIdx];
                }
            }

            if (lowestUsedLetter == -1)
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

            keyboardData.WrongCount = 0;

            // assign random word for lowest used letter
            int randomWord = Mathf.FloorToInt(UnityEngine.Random.value * balance.WordsForLetters[lowestUsedLetter].Length);
            keyboardData.WordIndex = balance.WordsForLetters[lowestUsedLetter][randomWord];
            keyboardData.TypedWord = "";
            Debug.Log("balance.WordsForLetters[" + lowestUsedLetter + "][" + randomWord + "] " + balance.WordsForLetters[lowestUsedLetter][randomWord]);
            Debug.Log("assignNextGameWord() lowestUsedLetter = " + (char)(lowestUsedLetter + 65) + " lowestValue " + lowestValue + " new word " + balance.Words[keyboardData.WordIndex]);
        }
    }
}
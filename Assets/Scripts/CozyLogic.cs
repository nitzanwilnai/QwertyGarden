using UnityEngine;

namespace QwertyGarden
{
    public static class CozyLogic
    {
        public static void StartGame(KeyboardData keyboardData, GameData gameData, Balance balance)
        {
            KeyboardLogic.StartGame(keyboardData);

            int randomIndex = Mathf.FloorToInt(UnityEngine.Random.value * balance.Words.Length);
            gameData.WordIndex = randomIndex;

            assignNextGameWord(keyboardData, gameData, balance);
        }

        public static void GameTyping(MetaData metaData, KeyboardData keyboardData, GameData gameData, Balance balance, char c, out bool wordComplete, out bool incorrectCharacter)
        {
            wordComplete = false;
            incorrectCharacter = false;

            string currentWord = balance.Words[gameData.WordIndex];

            KeyboardLogic.TryAddCharacter(metaData, keyboardData, balance, c, ref wordComplete, ref incorrectCharacter, currentWord);

            if (wordComplete)
                assignNextGameWord(keyboardData, gameData, balance);
        }

        static void assignNextGameWord(KeyboardData keyboardData, GameData gameData, Balance balance)
        {
            int lowestValue = int.MaxValue;
            int lowestUsedLetter = -1;

            for (int letterIdx = 25; letterIdx >= 0; letterIdx--)
            {
                if (keyboardData.FlowerProgress[letterIdx] < balance.NumFlowerFrames - 1 && keyboardData.FlowerProgress[letterIdx] < lowestValue)
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
            int randomWord = Mathf.FloorToInt(Random.value * balance.WordsForLetters[lowestUsedLetter].Length);
            gameData.WordIndex = balance.WordsForLetters[lowestUsedLetter][randomWord];
            keyboardData.TypedWord = "";
            Debug.Log("balance.WordsForLetters[" + lowestUsedLetter + "][" + randomWord + "] " + balance.WordsForLetters[lowestUsedLetter][randomWord]);
            Debug.Log("assignNextGameWord() lowestUsedLetter = " + (char)(lowestUsedLetter + 65) + " lowestValue " + lowestValue + " new word " + balance.Words[gameData.WordIndex]);
        }
    }
}
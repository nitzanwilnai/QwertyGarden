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
            keyboardData.FlowerIndex = new int[26];
            keyboardData.TypedWord = "";
        }

        public static void StartGame(KeyboardData keyboardData)
        {
            for (int i = 0; i < 26; i++)
                keyboardData.CharacterCount[i] = 0;

            for (int i = 0; i < 26; i++)
                keyboardData.FlowerProgress[i] = 0;

            keyboardData.WrongCount = 0;
        }

        public static void IncrementCharacterCount(MetaData metaData, KeyboardData keyboardData, Balance balance, char c)
        {
            int index = c - 65;
            keyboardData.CharacterCount[index]++;
            keyboardData.FlowerProgress[index]++;
            if (keyboardData.FlowerProgress[index] >= balance.NumFlowerFrames)
            {
                keyboardData.FlowerProgress[index] = balance.NumFlowerFrames - 1;
                metaData.Coins++;
            }
        }

        public static void TryAddCharacter(MetaData metaData, KeyboardData keyboardData, Balance balance, char c, ref bool wordComplete, ref bool incorrectCharacter, string currentWord)
        {
            if (currentWord[keyboardData.TypedWord.Length] != c)
            {
                incorrectCharacter = true;

                // decrease flower
                if (keyboardData.WrongCount == 0)
                {
                    int index = currentWord[keyboardData.TypedWord.Length] - 65;
                    keyboardData.FlowerProgress[index]--;
                    if (keyboardData.FlowerProgress[index] < 0)
                        keyboardData.FlowerProgress[index] = 0;
                }
                keyboardData.WrongCount++;
            }
            else
            {
                IncrementCharacterCount(metaData, keyboardData, balance, c);
                keyboardData.TypedWord += c;
                keyboardData.WrongCount = 0;

                if (string.Compare(keyboardData.TypedWord, currentWord) == 0)
                {
                    wordComplete = true;
                    keyboardData.WrongCount = 0;
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
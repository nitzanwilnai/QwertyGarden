using UnityEngine;

namespace QwertyGarden
{
    public class KeyboardData
    {
        public int WordIndex;
        public int KeyboardType;
        public int[] FlowerType; // which flower sits on which key
        public int[] CharacterCount; // how many times this character has been typed
        public int[] FlowerProgress; // flower progress 0-9 (duh)
        public int[] FlowerCount; // how many times this flower has been collected
        public string TypedWord;
        public float[] GrowTime;

        public int WrongLetter;
        public int WordCount;
        public float WordTime;
        public int CorrectCount;
        public int MistakeCount;
    }
}
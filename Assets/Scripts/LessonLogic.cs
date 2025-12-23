using UnityEngine;

namespace QwertyGarden
{
    public static class LessonLogic
    {
        public static void InitLessonData(LessonData lessonData, Balance balance)
        {
            lessonData.LessonWeights = new int[balance.LessonWords.Length];
            for (int i = 0; i < balance.LessonWords.Length; i++)
                lessonData.LessonWeights[i] = 100;

            lessonData.LessonCorrectCount = new int[balance.LessonWords.Length];
        }

        public static void StartLesson(KeyboardData keyboardData, LessonData lessonData, Balance balance)
        {
            KeyboardLogic.StartGame(keyboardData);

            lessonData.LessonProgress = 0;

            assignNextLessonWord(keyboardData, lessonData, balance);
        }


        public static void LessonTyping(MetaData metaData, KeyboardData keyboardData, LessonData lessonData, Balance balance, char c, out bool wordComplete, out bool incorrectCharacter)
        {
            wordComplete = false;
            incorrectCharacter = false;

            string currentWord = balance.LessonWords[lessonData.LessonWordIndex];

            KeyboardLogic.TryAddCharacter(metaData, keyboardData, balance, c, ref wordComplete, ref incorrectCharacter, currentWord);

            if (incorrectCharacter)
            {
                lessonData.LessonWeights[lessonData.LessonWordIndex]++;
                if (lessonData.LessonWeights[lessonData.LessonWordIndex] > 100)
                    lessonData.LessonWeights[lessonData.LessonWordIndex] = 100;
            }
            else if (wordComplete)
            {
                // decrease weight of this word
                lessonData.LessonWeights[lessonData.LessonWordIndex]--;
                if (lessonData.LessonWeights[lessonData.LessonWordIndex] < 1)
                    lessonData.LessonWeights[lessonData.LessonWordIndex] = 1;

                wordComplete = true;

                lessonData.LessonCorrectCount[lessonData.LessonWordIndex]++;

                if (lessonData.LessonWordIndex == lessonData.LessonProgress)
                    tryProgressLesson(lessonData, balance);

                // assign new word
                assignNextLessonWord(keyboardData, lessonData, balance);
            }
        }

        static void tryProgressLesson(LessonData lessonData, Balance balance)
        {
            if (lessonData.LessonCorrectCount[lessonData.LessonProgress] >= 10 && lessonData.LessonProgress < balance.LessonWords.Length - 1)
                lessonData.LessonProgress++;
        }

        static void assignNextLessonWord(KeyboardData keyboardData, LessonData lessonData, Balance balance)
        {

            int totalWeight = 0;
            for (int i = 0; i < lessonData.LessonProgress + 1; i++)
                totalWeight += lessonData.LessonWeights[i];

            int randomWeight = Mathf.FloorToInt(UnityEngine.Random.value * totalWeight);
            totalWeight = 0;
            for (int i = 0; i < lessonData.LessonProgress + 1; i++)
            {
                totalWeight += lessonData.LessonWeights[i];
                if (randomWeight < totalWeight)
                {
                    lessonData.LessonWordIndex = i;
                    break;
                }
            }
            keyboardData.TypedWord = "";
        }
    }
}
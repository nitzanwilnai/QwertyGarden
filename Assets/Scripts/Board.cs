using System;
using CommonTools;
using NUnit.Framework.Constraints;
using ParticleSystemDOD;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace QwertyGarden
{
    public class Board : MonoBehaviour
    {
        public enum WORD_STATE { READY, SLIDE_OUT, SLIDE_IN };
        public WORD_STATE WordState;

        public float WordSlideSpeed;
        public int WordSlideLimit;

        public Transform SpriteParent;
        PlantKey[] m_flowerKeys;
        KeyboardRef m_keyboardRef;

        public GameObject UI;

        public string[] Words;

        public Flower FlowerPrefab;
        Flower[] m_flowers;

        public Color WordColor;
        public Color GuessedColor;

        TextMeshPro m_wordText;

        public ParticleSystemBoard ParticleSystemBoard;
        public Color CorrectParticleColor;
        public Color WrongParticleColor;

        RectTransform m_coinTarget;
        Vector3[] m_coinTargetCorners = new Vector3[4];
        Animation m_coinTargetAnimation;
        TextMeshProUGUI m_coinsText;
        public SpinningCoin SpinningCoinPrefab;
        public int MaxSpinningCoins;
        Transform m_spinningCoinParent;
        public AnimationCurve SpinningCoinSpeed;
        public AnimationCurve SpinningCoinY;
        public AnimationCurve SpinningCoinScale;
        SpinningCoin[] m_spinningCoinPool;
        float[] m_spinningCoinTime;
        Vector3[] m_spinningCoinOriginIndex;
        decimal m_localCoinCount;
        decimal m_targetCoinCount;
        float m_coinCountTime;
        public float CoinCountTime;

        GameObject[] m_collectedFlowers;
        TextMeshProUGUI[] m_collectdFlowerText;

        TextMeshProUGUI m_totalCostText;
        TextMeshProUGUI m_changeText;
        TextMeshProUGUI m_currentCoinText;
        Animation m_changeAnimation;

        LessonData lessonData;
        GameData gameData;
        MetaData metaData;
        Balance balance;
        KeyboardData keyboardData;
        Camera worldCamera;

        public void Init(MetaData metaData, LessonData lessonData, GameData gameData, Balance balance, Camera camera)
        {
            this.lessonData = lessonData;
            this.gameData = gameData;
            this.balance = balance;
            this.metaData = metaData;

            worldCamera = camera;

            GUIRef guiRef = UI.GetComponent<GUIRef>();
            m_coinTarget = guiRef.GetGameObject("Coin").GetComponent<RectTransform>();
            m_coinTargetAnimation = guiRef.GetAnimation("Coin");
            m_coinsText = guiRef.GetTextGUI("Coin");

            m_collectdFlowerText = new TextMeshProUGUI[balance.NumFlowers];
            Transform uiFlowerParent = guiRef.GetGameObject("Flowers").transform;
            m_collectedFlowers = new GameObject[balance.NumFlowers];
            for (int flowerType = 0; flowerType < balance.NumFlowers; flowerType++)
            {
                GameObject go = Instantiate(AssetManager.Instance.InGameUIFlower, uiFlowerParent);
                GUIRef flowerGUIRef = go.GetComponent<GUIRef>();
                m_collectdFlowerText[flowerType] = flowerGUIRef.GetTextGUI("Count");
                m_collectdFlowerText[flowerType].text = "";
                flowerGUIRef.GetImage("Flower").sprite = AssetManager.Instance.LoadFlowerIcon(balance.FlowerName[flowerType], balance.FlowerIcon[flowerType]);
                go.SetActive(false);
                m_collectedFlowers[flowerType] = go;
            }

            GUIRef receeiptGUIRef = guiRef.GetGameObject("Invoice").GetComponent<GUIRef>();
            m_totalCostText = receeiptGUIRef.GetTextGUI("Total");
            m_currentCoinText = receeiptGUIRef.GetTextGUI("Cash");
            m_changeText = receeiptGUIRef.GetTextGUI("Change");
            m_changeAnimation = receeiptGUIRef.GetAnimation("Change");

            UI.SetActive(false);

            m_flowerKeys = new PlantKey[26];
            m_flowers = new Flower[26];

            // ParticleSystemBoard.Init(ParticleParent);

            m_spinningCoinPool = new SpinningCoin[MaxSpinningCoins];
            m_spinningCoinTime = new float[MaxSpinningCoins];
            m_spinningCoinOriginIndex = new Vector3[MaxSpinningCoins];
            for (int i = 0; i < MaxSpinningCoins; i++)
            {
                SpinningCoin spinningCoin = Instantiate(SpinningCoinPrefab);
                m_spinningCoinPool[i] = spinningCoin;
                m_spinningCoinPool[i].gameObject.SetActive(false);
                m_spinningCoinTime[i] = 1.0f;
            }

            SpriteParent.gameObject.SetActive(false);
        }

        void LoadTextFile()
        {
            TextAsset textFile = Resources.Load<TextAsset>("words_3000");
            string allWords = textFile.text;
            string[] words = allWords.Split();
            Debug.Log("words.length " + words.Length);
            Span<int> wordCount = stackalloc int[128];
            for (int i = 0; i < 128; i++)
                wordCount[i] = 0;
            for (int i = 0; i < words.Length; i++)
                wordCount[words[i].Length]++;

            for (int i = 0; i < 128; i++)
                if (wordCount[i] > 0)
                    Debug.Log("word length " + i + " = " + wordCount[i]);
        }

        public void PlayLesson(KeyboardData keyboardData)
        {
            LessonLogic.StartLesson(keyboardData, lessonData, balance);

            Show(keyboardData);
            m_wordText.text = balance.LessonWords[lessonData.LessonWordIndex];
        }

        public void PlayCozy(KeyboardData keyboardData)
        {
            Show(keyboardData);
            m_wordText.text = balance.Words[keyboardData.WordIndex];
        }

        void Show(KeyboardData keyboardData)
        {
            this.keyboardData = keyboardData;

            m_keyboardRef = Instantiate(AssetManager.Instance.KeyboardRefs[keyboardData.KeyboardType], SpriteParent);
            for (int i = 0; i < m_flowerKeys.Length; i++)
            {
                m_flowerKeys[i] = m_keyboardRef.LettersGO[i].GetComponent<PlantKey>();
                m_flowerKeys[i].Randomize();
            }
            m_keyboardRef.transform.localPosition = new Vector3(0.0f, -1.0f, 0.0f);

            m_wordText = m_keyboardRef.WordText;

            for (int keyIndex = 0; keyIndex < 26; keyIndex++)
            {
                int flowerType = keyboardData.FlowerType[keyIndex];
                Flower flower = AssetManager.Instance.LoadFlowerPrefab(balance.FlowerPrefab[flowerType], m_keyboardRef.FlowerParent);
                m_flowers[keyIndex] = flower;
                m_flowers[keyIndex].ResetFlower(balance.NumFlowerFrames);

                int progress = keyboardData.FlowerProgress[keyIndex];
                flower.GrowFlower(progress);
            }

            for (int i = 0; i < MaxSpinningCoins; i++)
                m_spinningCoinPool[i].transform.SetParent(m_keyboardRef.CoinParent);

            for (int i = 0; i < m_flowers.Length; i++)
            {
                float scaleY = UnityEngine.Random.value * 0.1f + 0.95f;
                float scaleX = UnityEngine.Random.value < 0.5f ? scaleY : -scaleY;
                m_flowers[i].transform.localScale = new Vector3(scaleX, scaleY, 1.0f);

                Vector3 position = m_flowerKeys[i].transform.localPosition;
                position.x += UnityEngine.Random.value * 0.1f + -0.05f;
                position.y += UnityEngine.Random.value * 0.1f + -0.05f;
                m_flowers[i].transform.localPosition = position;
            }

            m_localCoinCount = m_targetCoinCount = metaData.Coins;
            m_coinsText.text = m_localCoinCount.ToString("N0");

            SpriteParent.gameObject.SetActive(true);
            UI.SetActive(true);

            WordState = WORD_STATE.SLIDE_IN;
            m_wordText.transform.localPosition = new Vector3(WordSlideLimit, 0.0f, 0.0f);
        }

        public void Hide()
        {
            UI.SetActive(false);
            SpriteParent.gameObject.SetActive(false);

            for (int i = 0; i < 26; i++)
                GameObject.Destroy(m_flowers[i].gameObject);

            for (int i = 0; i < MaxSpinningCoins; i++)
                m_spinningCoinPool[i].transform.SetParent(transform);

            GameObject.Destroy(m_keyboardRef.gameObject);
        }

        public void Tick(float dt)
        {
            float keyboardScale = 0.9f;
            float ratio = (float)Screen.width / (float)Screen.height;
            if (ratio < 1.7)
                keyboardScale = 0.9f - (1.0f / ratio) * 0.3f;
            m_keyboardRef.transform.localScale = new Vector3(keyboardScale, keyboardScale, 1.0f);

            if (WordState == WORD_STATE.SLIDE_IN)
            {
                Vector3 wordPos = m_wordText.transform.localPosition;
                wordPos.x -= dt * WordSlideSpeed;
                if (wordPos.x <= 0.0f)
                {
                    wordPos.x = 0.0f;
                    WordState = WORD_STATE.READY;
                }
                m_wordText.transform.localPosition = wordPos;
            }
            else if (WordState == WORD_STATE.SLIDE_OUT)
            {
                Vector3 wordPos = m_wordText.transform.localPosition;
                wordPos.x -= dt * WordSlideSpeed;
                if (wordPos.x <= -WordSlideLimit)
                {
                    wordPos.x = WordSlideLimit;
                    WordState = WORD_STATE.SLIDE_IN;
                    if (metaData.GameType == GAME_TYPE.COZY)
                        m_wordText.text = balance.Words[keyboardData.WordIndex];
                    else if (metaData.GameType == GAME_TYPE.LESSON)
                        m_wordText.text = balance.LessonWords[lessonData.LessonWordIndex];
                    m_wordText.color = WordColor;
                }
                m_wordText.transform.localPosition = wordPos;
            }
            else if (WordState == WORD_STATE.READY)
            {
                char c;
                int keyIndex = KeyboardLogic.GetTypedKeyIndex(out c);
                if (keyIndex > -1)
                {
                    if (metaData.GameType == GAME_TYPE.LESSON)
                        lessonTextInput(keyIndex, Char.ToUpper(c));
                    else if (metaData.GameType == GAME_TYPE.COZY)
                        gameTextInput(keyIndex, Char.ToUpper(c));
                }
                if (Keyboard.current.escapeKey.wasReleasedThisFrame)
                {
                    Game.Instance.SetMenuState(MENU_STATE.GARDEN_SELECTION);
                }
            }

            // Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, m_coinTarget.position);
            // Vector3 worldPos = mainCamera.ScreenToWorldPoint(
            //     new Vector3(screenPos.x, screenPos.y, mainCamera.nearClipPlane)
            // );
            // Vector3 targetPos = new Vector3(worldPos.x, worldPos.y, transform.position.z);

            m_coinTarget.GetWorldCorners(m_coinTargetCorners);
            // 0=BL, 1=TL, 2=TR, 3=BR
            Vector3 center = (m_coinTargetCorners[0] + m_coinTargetCorners[2]) * 0.5f;
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, center);
            Vector3 worldPos = worldCamera.ScreenToWorldPoint(
                new Vector3(screenPos.x, screenPos.y, worldCamera.nearClipPlane)
            );

            Vector3 targetPos = new Vector3(worldPos.x, worldPos.y, transform.position.z);


            for (int coinIdx = 0; coinIdx < MaxSpinningCoins; coinIdx++)
            {
                if (m_spinningCoinTime[coinIdx] < 1.0f)
                {
                    m_spinningCoinTime[coinIdx] += dt * 0.5f;
                    if (m_spinningCoinTime[coinIdx] >= 1.0f)
                    {
                        // m_spinningCoinTime[coinIdx] = 1.0f;
                        m_spinningCoinPool[coinIdx].gameObject.SetActive(false);
                        m_targetCoinCount = metaData.Coins;
                        m_coinCountTime = CoinCountTime;
                    }
                    else
                    {
                        float speed = SpinningCoinSpeed.Evaluate(m_spinningCoinTime[coinIdx]);
                        float y = SpinningCoinY.Evaluate(m_spinningCoinTime[coinIdx]);
                        float scale = SpinningCoinScale.Evaluate(m_spinningCoinTime[coinIdx]);
                        Debug.Log("m_spinningCoinTime[" + coinIdx + "] " + m_spinningCoinTime[coinIdx] + " scale " + scale);
                        Vector3 originPos = m_spinningCoinOriginIndex[coinIdx];
                        Vector3 pos = (targetPos - originPos) * speed + originPos;
                        pos.z = -90.0f;
                        // pos.y += y;
                        m_spinningCoinPool[coinIdx].transform.position = pos;
                        m_spinningCoinPool[coinIdx].transform.localScale = new Vector3(scale, scale, 1.0f);
                    }

                }
            }

            if (m_coinCountTime > 0.0f)
                m_coinCountTime -= dt;
            if (m_coinCountTime <= 0.0f)
            {
                if (m_localCoinCount < m_targetCoinCount)
                {
                    m_localCoinCount++;
                    m_coinsText.text = m_localCoinCount.ToString("N0");
                    m_coinTargetAnimation.Play("Grow");
                    m_coinCountTime = CoinCountTime;
                }
            }
        }

        void flyCoin(int letterIndex)
        {
            for (int coinIdx = 0; coinIdx < MaxSpinningCoins; coinIdx++)
            {
                if (m_spinningCoinTime[coinIdx] >= 1.0f)
                {
                    m_spinningCoinOriginIndex[coinIdx] = m_flowerKeys[letterIndex].transform.position;
                    m_spinningCoinOriginIndex[coinIdx].x += UnityEngine.Random.value * 1.0f - 0.5f;
                    m_spinningCoinOriginIndex[coinIdx].y += UnityEngine.Random.value * 1.0f - 0.5f;
                    m_spinningCoinTime[coinIdx] = 0.0f;
                    m_spinningCoinPool[coinIdx].gameObject.SetActive(true);
                    break;
                }
            }
        }

        private void gameTextInput(int keyIndex, char c)
        {
            if (WordState == WORD_STATE.READY)
            {
                bool wordComplete;
                bool incorrectCharacter;

                int prevProgress = keyboardData.FlowerProgress[keyIndex];

                string currentWord = balance.Words[keyboardData.WordIndex];

                CozyLogic.GameTyping(metaData, keyboardData, balance, c, out wordComplete, out incorrectCharacter);
                updateWord(keyboardData.TypedWord, wordComplete, incorrectCharacter, currentWord);

                for (int i = 0; i < 26; i++)
                    if (keyboardData.FlowerProgress[i] >= balance.NumFlowerFrames)
                        Debug.LogError("keyboardData.FlowerProgress[" + i + "] " + keyboardData.FlowerProgress[i]);
                KeyboardDataIO.SaveKeyboard(keyboardData, metaData.KeyboardIndex);

                // int charIndex = incorrectCharacter ? currentWord[keyboardData.TypedWord.Length] - 65 : char.ToUpper(c) - 65;
                // Vector2 pos = PlantKeys[charIndex].transform.position;
                // ParticleSystemBoard.Emit(particleColor, pos, ParticleSystemBoard.particleBalance.NumSprites);


                // if (keyboardData.FlowerProgress[keyIndex] > prevProgress)
                m_flowers[keyIndex].GrowFlower(keyboardData.FlowerProgress[keyIndex]);

                if (!incorrectCharacter && prevProgress >= balance.NumFlowerFrames - 1)
                {
                    int flowerType = keyboardData.FlowerType[keyIndex];
                    // int numCoins = balance.FlowerSellValue[flowerType];
                    // if (numCoins > 10)
                    //     numCoins = 10;
                    // for (int i = 0; i < numCoins; i++)
                    //     flyCoin(keyIndex);

                    m_collectdFlowerText[flowerType].text = keyboardData.FlowerCount[flowerType].ToString("N0");
                    if (!m_collectedFlowers[flowerType].activeSelf)
                        m_collectedFlowers[flowerType].SetActive(true);
                }
            }


        }

        private void lessonTextInput(int keyIndex, char c)
        {
            if (WordState == WORD_STATE.READY)
            {
                int prevProgress = keyboardData.FlowerProgress[keyIndex];
                string currentWord = balance.LessonWords[lessonData.LessonWordIndex];
                bool wordComplete;
                bool incorrectCharacter;
                LessonLogic.LessonTyping(metaData, keyboardData, lessonData, balance, char.ToUpper(c), out wordComplete, out incorrectCharacter);

                updateWord(keyboardData.TypedWord, wordComplete, incorrectCharacter, currentWord);

                KeyboardDataIO.SaveKeyboard(keyboardData, metaData.KeyboardIndex);

                // if (wordComplete)
                // {
                //     Debug.Log("wordComplete new word balance.LessonWords[" + lessonData.LessonWordIndex + "]" + balance.LessonWords[lessonData.LessonWordIndex]);
                //     m_wordText.text = balance.LessonWords[lessonData.LessonWordIndex];
                // }

                //bug! we don't change flower on inocrrect character?
                if (incorrectCharacter)
                {
                    Debug.Log("Incorrect character");
                }
                else
                {
                    if (keyboardData.FlowerProgress[keyIndex] > prevProgress)
                        m_flowers[keyIndex].GrowFlower(keyboardData.FlowerProgress[keyIndex]);

                }
                if (!incorrectCharacter && keyboardData.FlowerProgress[keyIndex] >= balance.NumFlowerFrames - 1)
                {
                    flyCoin(keyIndex);
                }
            }
        }

        private void updateWord(string typedWord, bool wordComplete, bool incorrectCharacter, string currentWord)
        {
            if (wordComplete)
            {
                WordState = WORD_STATE.SLIDE_OUT;
            }
            else if (incorrectCharacter)
            {
                Debug.Log("Incorrect character");
            }
            else
            {
                // guessed correct but word not complete
            }

            if (WordState == WORD_STATE.READY)
            {
                string wrongColorString = ColorUtility.ToHtmlStringRGBA(WrongParticleColor);
                string wordColorString = ColorUtility.ToHtmlStringRGBA(WordColor);
                string guessedColorString = ColorUtility.ToHtmlStringRGBA(GuessedColor);
                string s = "";
                for (int i = 0; i < currentWord.Length; i++)
                {
                    if (i < typedWord.Length)
                        s += "<color=#" + guessedColorString + ">";
                    else if (incorrectCharacter && i == typedWord.Length)
                        s += "<color=#" + wrongColorString + ">";
                    else
                        s += "<color=#" + wordColorString + ">";


                    s += currentWord[i];
                    s += "</color>";
                }
                m_wordText.text = s;
            }
            else if (WordState == WORD_STATE.SLIDE_OUT)
            {
                m_wordText.text = currentWord;
                m_wordText.color = GuessedColor;
            }
        }
    }
}
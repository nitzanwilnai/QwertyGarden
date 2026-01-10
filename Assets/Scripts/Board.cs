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
    public class CollectedFlowersGUI
    {
        public RectTransform[] FlowerRT;
        public Vector3[][] FlowerCorners;

        public int[] SourceKeyIndex;
        public int[] TargetFlowerType;

        public GameObject[] FlowersUI;
        public TextMeshProUGUI[] FlowersText;
        public Animation[] FlowersAnim;
        public GameObject[] FlyingFlowers;
    }

    public class InoviceGUI
    {
        public GameObject InoviceGO;
        public TextMeshProUGUI SubTotal;
        public TextMeshProUGUI CurrentCoinText;
        public TextMeshProUGUI BalanceText;
        public TextMeshProUGUI AccuracyBonus;
        public TextMeshProUGUI WPMBonus;
    }

    public class InvoiceLineGUI
    {
        public GameObject GO;
        public TextMeshProUGUI Item;
        public TextMeshProUGUI Value;
    }

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

        GameObject m_collectedGO;
        GameObject m_notCollectedGO;

        public string[] Words;

        public Flower FlowerPrefab;
        Flower[] m_flowers;

        public Color WordColor;
        public Color GuessedColor;

        TextMeshPro m_wordText;

        public ParticleSystemBoard ParticleSystemBoard;
        public Color CorrectParticleColor;
        public Color WrongParticleColor;

        TextMeshProUGUI m_coinsText;
        TextMeshProUGUI m_WPMText;
        TextMeshProUGUI m_settingsText;

        double m_localCoinCount;
        double m_targetCoinCount;
        float m_coinCountTime;
        public float CoinCountTime;

        public GameObject FlyingFlowerPrefab;
        CollectedFlowersGUI m_collectedFlowers = new CollectedFlowersGUI();

        InoviceGUI m_inoviceGUI = new InoviceGUI();
        bool m_showInvoice;
        InvoiceAnimation m_invoiceAnimation;
        InvoiceLineGUI[] m_invoiceLines;

        float m_wordStartTime;

        TutorialGUI m_tutorialGUI = new TutorialGUI();
        GameObject m_instructionGO;

        double m_pressedKeyTimer;

        LessonData lessonData;
        MetaData metaData;
        Balance balance;
        KeyboardData keyboardData;
        Camera worldCamera;

        public void Init(MetaData metaData, LessonData lessonData, Balance balance, Camera camera)
        {
            this.lessonData = lessonData;
            this.balance = balance;
            this.metaData = metaData;

            worldCamera = camera;

            GUIRef guiRef = UI.GetComponent<GUIRef>();
            m_coinsText = guiRef.GetTextGUI("Coin");
            m_WPMText = guiRef.GetTextGUI("WPM");

            m_collectedGO = guiRef.GetGameObject("Collected");
            m_notCollectedGO = guiRef.GetGameObject("NotCollected");
            m_instructionGO = guiRef.GetGameObject("Instructions");
            m_settingsText = guiRef.GetTextGUI("Settings");

            CommonVisual.InitTutorialGUI(guiRef, m_tutorialGUI);

            Transform uiFlowerParent = guiRef.GetGameObject("Flowers").transform;
            m_collectedFlowers.FlowersText = new TextMeshProUGUI[balance.NumFlowers];
            m_collectedFlowers.FlowersAnim = new Animation[balance.NumFlowers];
            m_collectedFlowers.FlowersUI = new GameObject[balance.NumFlowers];
            m_collectedFlowers.SourceKeyIndex = new int[balance.NumFlowers];
            m_collectedFlowers.TargetFlowerType = new int[balance.NumFlowers];
            m_collectedFlowers.FlyingFlowers = new GameObject[balance.NumFlowers];
            for (int flowerType = 0; flowerType < balance.NumFlowers; flowerType++)
            {
                GameObject uiFlower = Instantiate(AssetManager.Instance.InGameUIFlower, uiFlowerParent);
                GUIRef flowerGUIRef = uiFlower.GetComponent<GUIRef>();
                m_collectedFlowers.FlowersText[flowerType] = flowerGUIRef.GetTextGUI("Count");
                m_collectedFlowers.FlowersText[flowerType].text = "";
                flowerGUIRef.GetImage("Flower").sprite = AssetManager.Instance.LoadFlowerIcon(balance.FlowerName[flowerType], balance.FlowerIcon[flowerType]);
                uiFlower.SetActive(false);
                m_collectedFlowers.FlowersUI[flowerType] = uiFlower;
                m_collectedFlowers.FlowersAnim[flowerType] = flowerGUIRef.GetAnimation("Grow");
            }

            m_inoviceGUI.InoviceGO = guiRef.GetGameObject("Invoice");
            m_invoiceAnimation = m_inoviceGUI.InoviceGO.GetComponent<InvoiceAnimation>();
            GUIRef receeiptGUIRef = m_inoviceGUI.InoviceGO.GetComponent<GUIRef>();
            Transform flowerLineParent = receeiptGUIRef.GetGameObject("InvoiceItems").transform;
            m_inoviceGUI.SubTotal = receeiptGUIRef.GetTextGUI("Total");
            m_inoviceGUI.CurrentCoinText = receeiptGUIRef.GetTextGUI("Cash");
            m_inoviceGUI.BalanceText = receeiptGUIRef.GetTextGUI("Balance");
            m_inoviceGUI.AccuracyBonus = receeiptGUIRef.GetTextGUI("AccuracyBonus");
            m_inoviceGUI.WPMBonus = receeiptGUIRef.GetTextGUI("WPMBonus");
            m_invoiceLines = new InvoiceLineGUI[balance.NumFlowers];
            m_invoiceAnimation.Init(balance);
            for (int flowerType = 0; flowerType < balance.NumFlowers; flowerType++)
            {
                InvoiceLineGUI invoiceLine = new InvoiceLineGUI();
                GameObject invoiceLineGO = GameObject.Instantiate(AssetManager.Instance.ShopReceiptItem, flowerLineParent);
                GUIRef receiptGUIRef = invoiceLineGO.GetComponent<GUIRef>();
                invoiceLine.GO = invoiceLineGO;
                invoiceLine.Item = receiptGUIRef.GetTextGUI("Name");
                invoiceLine.Value = receiptGUIRef.GetTextGUI("Value");
                invoiceLine.GO.transform.SetSiblingIndex(m_invoiceAnimation.ItemLineIndexStart + flowerType);
                m_invoiceLines[flowerType] = invoiceLine;
                invoiceLine.GO.SetActive(false);
                m_invoiceAnimation.InvoiceLines[m_invoiceAnimation.ItemLineIndexStart + flowerType] = invoiceLine.GO;
            }

            UI.SetActive(false);

            m_flowerKeys = new PlantKey[26];
            m_flowers = new Flower[26];

            // ParticleSystemBoard.Init(ParticleParent);

            SpriteParent.gameObject.SetActive(false);
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

            KeyboardLogic.ResetWPMAndAccuracy(keyboardData);
            KeyboardDataIO.SaveKeyboard(keyboardData, metaData.KeyboardIndex);

            m_showInvoice = false;
            m_instructionGO.SetActive(true);
            m_tutorialGUI.GO.SetActive(true);
            m_inoviceGUI.InoviceGO.SetActive(false);

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
                m_flowers[keyIndex].ResetFlower(balance.MaxFlowerFrames);

                int progress = keyboardData.FlowerProgress[keyIndex];
                flower.GrowFlower(progress);
            }

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

            updateUI();

            SpriteParent.gameObject.SetActive(true);
            UI.SetActive(true);

            WordState = WORD_STATE.SLIDE_IN;
            m_wordText.transform.localPosition = new Vector3(WordSlideLimit, 0.0f, 0.0f);

            CommonVisual.CheckTutorialFlag(metaData, m_tutorialGUI);
        }

        void updateUI()
        {
            m_localCoinCount = m_targetCoinCount = metaData.Coins;
            double totalSellValue = MetaLogic.GetSellValue(keyboardData, balance);
            m_coinsText.text = MetaLogic.ToShortScale(m_localCoinCount + totalSellValue) + " <sprite name=\"coin\">";

            int wpm = KeyboardLogic.GetWPM(keyboardData);
            int accuracy = KeyboardLogic.GetAccuracy(keyboardData);
            m_WPMText.text = "WPM: " + wpm + "\nAccuracy: " + accuracy + "%";

            bool collected = false;
            for (int flowerType = 0; flowerType < balance.NumFlowers; flowerType++)
            {
                m_collectedFlowers.FlowersText[flowerType].text = keyboardData.FlowerCount[flowerType].ToString("N0");
                m_collectedFlowers.FlowersUI[flowerType].SetActive(keyboardData.FlowerCount[flowerType] > 0);
                if (keyboardData.FlowerCount[flowerType] > 0)
                    collected = true;
            }
            m_notCollectedGO.SetActive(!collected);
        }

        public void Hide()
        {
            UI.SetActive(false);
            SpriteParent.gameObject.SetActive(false);

            for (int i = 0; i < 26; i++)
                GameObject.Destroy(m_flowers[i].gameObject);

            GameObject.Destroy(m_keyboardRef.gameObject);
        }

        public void Tick(float dt)
        {
            resizeKeyboardForResolution();

            if (m_showInvoice)
            {
                if (Keyboard.current != null)
                {
                    if (Keyboard.current.mKey.wasReleasedThisFrame)
                    {
                        Game.Instance.SetMenuState(MENU_STATE.MAIN_MENU);
                    }
                    else if (Keyboard.current.spaceKey.wasReleasedThisFrame)
                    {
                        Game.Instance.SetMenuState(MENU_STATE.EDIT_GARDEN);
                    }
                    else if (Keyboard.current.enterKey.wasReleasedThisFrame)
                    {
                        m_showInvoice = false;

                        m_instructionGO.SetActive(true);
                        m_tutorialGUI.GO.SetActive(true);

                        m_inoviceGUI.InoviceGO.SetActive(false);
                        m_tutorialGUI.Darken.SetActive(false);

                        KeyboardLogic.ResetWPMAndAccuracy(keyboardData);
                        KeyboardDataIO.SaveKeyboard(keyboardData, metaData.KeyboardIndex);
                        updateUI();
                    }
                }

                if (m_coinCountTime > 0.0f)
                    m_coinCountTime -= dt;
                if (m_coinCountTime <= 0.0f)
                {
                    if (m_localCoinCount < m_targetCoinCount)
                    {
                        double diff = System.Math.Floor(m_targetCoinCount - m_localCoinCount);
                        m_localCoinCount += MetaLogic.PowerOf10(diff);
                        m_coinsText.text = MetaLogic.ToShortScale(m_localCoinCount) + " <sprite name=\"coin\">";
                        m_coinCountTime = CoinCountTime;
                    }
                }
            }
            else
            {
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
                    if (Keyboard.current != null)
                    {
                        char c;
                        int keyIndex = KeyboardLogic.GetTypedKeyIndex(out c);

                        if (CommonVisual.CheckTutorialKey(m_tutorialGUI))
                        {

                        }
                        else if (!m_tutorialGUI.TutorialShown)
                        {
                            if (keyIndex > -1)
                            {
                                m_pressedKeyTimer = Time.realtimeSinceStartupAsDouble;

                                if (metaData.GameType == GAME_TYPE.LESSON)
                                    lessonTextInput(keyIndex, Char.ToUpper(c));
                                else if (metaData.GameType == GAME_TYPE.COZY)
                                    gameTextInput(keyIndex, Char.ToUpper(c));
                            }
                            else if (Keyboard.current.spaceKey.wasReleasedThisFrame)
                            {
                                double timeSinceLastPressedKey = Time.realtimeSinceStartupAsDouble - m_pressedKeyTimer;
                                if (timeSinceLastPressedKey > 1.0d)
                                {
                                    m_showInvoice = true;
                                    m_instructionGO.SetActive(false);
                                    m_tutorialGUI.GO.SetActive(false);
                                    m_tutorialGUI.Darken.SetActive(true);
                                    m_inoviceGUI.InoviceGO.SetActive(true);

                                    for (int flowerType = 0; flowerType < balance.NumFlowers; flowerType++)
                                    {
                                        int totalCoinsForThisFlower = balance.FlowerSellValue[flowerType] * keyboardData.FlowerCount[flowerType];
                                        m_invoiceLines[flowerType].Item.text = balance.FlowerName[flowerType] + " x" + keyboardData.FlowerCount[flowerType];
                                        m_invoiceLines[flowerType].Value.text = MetaLogic.ToShortScale(totalCoinsForThisFlower) + " <sprite name=\"coin\">";
                                        m_invoiceAnimation.ShowInvoiceLIne[flowerType + m_invoiceAnimation.ItemLineIndexStart] = keyboardData.FlowerCount[flowerType] > 0;
                                    }
                                    float accuracyBonus = KeyboardLogic.GetAccuracyBonus(keyboardData);
                                    float wpmBonus = KeyboardLogic.GetWPMBonus(keyboardData);
                                    double totalSellValue = MetaLogic.GetSellValue(keyboardData, balance);
                                    m_inoviceGUI.AccuracyBonus.text = "x" + accuracyBonus.ToString("N2");
                                    m_inoviceGUI.WPMBonus.text = "x" + wpmBonus.ToString("N2");
                                    m_inoviceGUI.SubTotal.text = MetaLogic.ToShortScale(totalSellValue) + " <sprite name=\"coin\">";

                                    m_inoviceGUI.CurrentCoinText.text = MetaLogic.ToShortScale(metaData.Coins) + " <sprite name=\"coin\">";

                                    MetaLogic.SellCollectedFlowers(metaData, keyboardData, balance);
                                    SoundManager.Instance.PlaySFXMoney();
                                    KeyboardDataIO.SaveKeyboard(keyboardData, metaData.KeyboardIndex);
                                    MetaDataIO.SaveMeta(metaData);

                                    m_targetCoinCount = metaData.Coins;


                                    m_inoviceGUI.BalanceText.text = MetaLogic.ToShortScale(metaData.Coins) + " <sprite name=\"coin\">";

                                    m_invoiceAnimation.StartAnimation();
                                }
                            }
                            else if (Keyboard.current.digit1Key.wasReleasedThisFrame)
                            {
                                Game.Instance.SetMenuState(MENU_STATE.SETTINGS);
                                // metaData.SFX = !metaData.SFX;
                                // CommonVisual.UpdateSettingsText(metaData, m_settingsText);
                            }
                            // else if (Keyboard.current.digit2Key.wasReleasedThisFrame)
                            // {
                            //     metaData.Music = !metaData.Music;
                            //     CommonVisual.UpdateSettingsText(metaData, m_settingsText);
                            // }
                        }
                    }
                }
#if UNITY_EDITOR
                if (Keyboard.current.f1Key.wasReleasedThisFrame)
                {
                    keyboardData.FlowerCount[0]++;
                    updateUI();
                }
                if (Keyboard.current.f2Key.wasReleasedThisFrame)
                {
                    keyboardData.FlowerCount[1]++;
                    updateUI();
                }
                if (Keyboard.current.f3Key.wasReleasedThisFrame)
                {
                    keyboardData.FlowerCount[2]++;
                    updateUI();
                }
                if (Keyboard.current.f4Key.wasReleasedThisFrame)
                {
                    keyboardData.FlowerCount[3]++;
                    updateUI();
                }
#endif
            }
        }

        private void resizeKeyboardForResolution()
        {
            // float keyboardScale = 0.9f;
            float ratio = (float)Screen.width / (float)Screen.height;
            // float defaultRatio = 16.0f / 9.0f;
            float keyboardScale = 1.0f - Mathf.Clamp01((1.78f - ratio) / 0.78f) * 0.4f;
            m_keyboardRef.transform.localScale = new Vector3(keyboardScale, keyboardScale, 1.0f);
        }

        void flyFlower(int keyIndex, int flowerType)
        {
            // get origin key index
            // get target (maybe only the flower type)
            // set time to 0
            // turn on game object
        }

        private void gameTextInput(int keyIndex, char c)
        {
            if (WordState == WORD_STATE.READY)
            {
                bool wordComplete;
                bool incorrectCharacter;

                int prevProgress = keyboardData.FlowerProgress[keyIndex];

                string currentWord = balance.Words[keyboardData.WordIndex];

                CozyLogic.GameTyping(metaData, keyboardData, balance, c, out wordComplete, out incorrectCharacter, ref m_wordStartTime);
                updateWord(keyboardData.TypedWord, wordComplete, incorrectCharacter, currentWord);

                KeyboardDataIO.SaveKeyboard(keyboardData, metaData.KeyboardIndex);

                // int charIndex = incorrectCharacter ? currentWord[keyboardData.TypedWord.Length] - 65 : char.ToUpper(c) - 65;
                // Vector2 pos = PlantKeys[charIndex].transform.position;
                // ParticleSystemBoard.Emit(particleColor, pos, ParticleSystemBoard.particleBalance.NumSprites);


                // if (keyboardData.FlowerProgress[keyIndex] > prevProgress)
                m_flowers[keyIndex].GrowFlower(keyboardData.FlowerProgress[keyIndex]);

                if (!incorrectCharacter && prevProgress >= balance.MaxFlowerFrames - 1)
                {
                    int flowerType = keyboardData.FlowerType[keyIndex];
                    // int numCoins = balance.FlowerSellValue[flowerType];
                    // if (numCoins > 10)
                    //     numCoins = 10;
                    // for (int i = 0; i < numCoins; i++)
                    //     flyCoin(keyIndex);

                    SoundManager.Instance.PlaySFXFlowerCollected();

                    m_collectedFlowers.FlowersText[flowerType].text = keyboardData.FlowerCount[flowerType].ToString("N0");
                    if (!m_collectedFlowers.FlowersUI[flowerType].activeSelf)
                        m_collectedFlowers.FlowersUI[flowerType].SetActive(true);
                    m_collectedFlowers.FlowersAnim[flowerType].Play("Grow");
                }

                updateUI();
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
                LessonLogic.LessonTyping(metaData, keyboardData, lessonData, balance, char.ToUpper(c), out wordComplete, out incorrectCharacter, ref m_wordStartTime);

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
                if (!incorrectCharacter && keyboardData.FlowerProgress[keyIndex] >= balance.MaxFlowerFrames - 1)
                {
                    // used to be fly coin
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
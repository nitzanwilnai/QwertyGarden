using UnityEngine;
using UnityEngine.UI;
using CommonTools;
using System;

namespace QwertyGarden
{
    public static class CommonFlowerVisual
    {
        public static void PositionKeyboardForResolution(KeyboardImages keyboardImages)
        {
            // -130 to 0
            // 1 to 1.77
            float keyboardX = 0.0f;
            float ratio = (float)Screen.width / (float)Screen.height;
            if (ratio < 1.77f && ratio >= 1.33f)
                keyboardX = (1.0f - ((ratio - 1.33f) / 0.77f)) * -130.0f;
            else if (ratio < 1.33f)
                keyboardX = -130.0f;
            keyboardImages.transform.localPosition = new Vector3(keyboardX, 0.0f, 0.0f);
        }

        public static void UpdateCardOutline(Image[] m_flowerCardOutlines, int m_flowerType)
        {
            for (int i = 0; i < m_flowerCardOutlines.Length; i++)
                m_flowerCardOutlines[i].color = i != m_flowerType ? AssetManager.Instance.FlowerCardUnselected : AssetManager.Instance.FlowerCardSelected;
        }

        public static void AddFlowerCard(Balance balance, int flowerType, GameObject flowerCard, Image[] flowerCardOutlines)
        {
            GUIRef flowerCardGUIRef = flowerCard.GetComponent<GUIRef>();
            flowerCardGUIRef.GetTextGUI("FlowerName").text = balance.FlowerName[flowerType];
            flowerCardGUIRef.GetTextGUI("Cost").text = CommonVisual.ToShortScale(balance.FlowerSeedCost[flowerType]) + " <sprite name=\"coin\">";
            flowerCardGUIRef.GetTextGUI("Sell").text = CommonVisual.ToShortScale(balance.FlowerSellValue[flowerType]) + " <sprite name=\"coin\">";
            float seconds = balance.FlowerGrowTime[flowerType];
            TimeSpan t = TimeSpan.FromSeconds(seconds);
            string growString = "";
            if (t.Hours > 0)
                growString += t.Hours + "h ";
            if (t.Minutes > 0)
                growString += t.Minutes + "m ";
            if (t.Seconds > 0)
                growString += t.Seconds + "s ";
            flowerCardGUIRef.GetTextGUI("Time").text = balance.FlowerGrowTime[flowerType].ToString(growString);
            flowerCardGUIRef.GetImage("Flower").sprite = AssetManager.Instance.LoadFlowerIcon(balance.FlowerName[flowerType], balance.FlowerIcon[flowerType]);
            flowerCard.transform.localPosition = new Vector3(flowerType * 272.0f, 0.0f, 0.0f);
            flowerCardOutlines[flowerType] = flowerCardGUIRef.GetImage("Outline");
            flowerCardOutlines[flowerType].color = flowerType > 0 ? AssetManager.Instance.FlowerCardUnselected : AssetManager.Instance.FlowerCardSelected;
        }

    }
}
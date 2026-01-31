using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using CommonTools;
using UnityEditor;

namespace QwertyGarden
{
    public class AssetManager : Singleton<AssetManager>
    {
        // public FlowerSO[] Flowers;
        public KeyboardImages[] KeyboardImages;
        public GameObject KeyboardSelectionBox;
        public KeyboardRef[] KeyboardRefs;
        public GameObject ShopCardPrefab;
        public GameObject ShopReceiptItem;
        public GameObject InGameUIFlower;
        public GameObject UIFlowerPopup;
        public UIFlower TitleFlowerPrefab;

        public Color FlowerCardSelected;
        public Color FlowerCardUnselected;
        public Color ReceiptChangePositive;
        public Color ReceiptChangeNegative;
        public Color FlowerPopupGreen;
        public Color FlowerPopupRed;
        public Color PrestigeEnabled;
        public Color PrestigeDisabled;
        public Color KeySelected;
        public Color KeyNotSelected;
        public static bool UseAssetBundles = false;
        AssetBundle m_commonBundle;

        public void LoadCommonAssetBundle()
        {
#if UNITY_EDITOR
            if (UseAssetBundles)
                m_commonBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "AssetBundles/common"));
#else
            m_commonBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "AssetBundles/common"));
#endif
        }

        public void UnloadCommonAssetBundle()
        {
            if (m_commonBundle != null)
                m_commonBundle.Unload(true);
        }

        GameObject loadGameObject(AssetBundle assetBundle, string objName, string localPath)
        {
            // Debug.Log("loadGameObject objName " + objName + " localPath " + localPath);

            GameObject go = null;
#if UNITY_EDITOR
            if (UseAssetBundles)
                go = assetBundle.LoadAsset<GameObject>(objName);
            else
                go = (GameObject)AssetDatabase.LoadAssetAtPath(localPath, typeof(GameObject));
#else
            go = assetBundle.LoadAsset<GameObject>(objName);
#endif
            return go;
        }

        Sprite loadSprite(AssetBundle assetBundle, string objName, string localPath)
        {
            Debug.Log("loadSprite(" + objName + ")");
            Sprite sprite = null;
#if UNITY_EDITOR
            if (UseAssetBundles)
                sprite = assetBundle.LoadAsset<Sprite>(objName);
            else
                sprite = (Sprite)AssetDatabase.LoadAssetAtPath(localPath, typeof(Sprite));
#else
            sprite = assetBundle.LoadAsset<Sprite>(objName);
#endif
            return sprite;
        }

        public Sprite LoadFlowerIcon(string flowerName, string flowerIconName)
        {
            return loadSprite(m_commonBundle, flowerIconName, "Assets/Textures/Flowers/" + flowerName + "/" + flowerIconName + ".png");
        }

        public Sprite LoadFlowerCard(string flowerName, string flowerCardName)
        {
            return loadSprite(m_commonBundle, flowerCardName, "Assets/Textures/Flowers/" + flowerName + "/" + flowerCardName + ".png");
        }

        public Flower LoadFlowerPrefab(string flowerName, Transform parent)
        {
            return Instantiate(loadGameObject(m_commonBundle, flowerName, "Assets/Prefabs/Flowers/" + flowerName + ".prefab").GetComponent<Flower>(), parent);
        }

        public Sprite LoadFlowerProgress(string flowerName, string spriteName)
        {
            return loadSprite(m_commonBundle, spriteName, "Assets/Textures/Flowers/" + flowerName + "/" + spriteName + ".png");
        }
    }
}
using CommonTools;
using UnityEngine;

namespace QwertyGarden
{
    public class AssetManager : Singleton<AssetManager>
    {
        public FlowerSO[] Flowers;
        public KeyboardImages[] KeyboardImages;
        public GameObject KeyboardSelectionBox;
        public KeyboardRef[] KeyboardRefs;
    }
}
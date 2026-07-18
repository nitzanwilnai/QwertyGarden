using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace QwertyGarden
{
    public static class MetaDataIO
    {
        public static int VERSION = 7;

        public static void SaveMeta(MetaData metaData)
        {
            string fileName = Application.persistentDataPath + "/meta_v" + VERSION + ".dat";
            using (FileStream fs = File.Create(fileName))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                bw.Write(metaData.Coins);
                bw.Write((int)metaData.GameType);
                bw.Write((int)metaData.MenuState);
                bw.Write((int)metaData.PrevMenuState);
                bw.Write(metaData.KeyboardIndex);
                bw.Write(metaData.LastTimeStamp);
                bw.Write(metaData.GrowTime);
                bw.Write(metaData.TutorialFlags);

                bw.Write(metaData.ShowPrestige);
                bw.Write(metaData.Prestige);

                bw.Write(metaData.SFX);
                bw.Write(metaData.Music);
                bw.Write(metaData.WPM);
                bw.Write(metaData.Font);
                bw.Write(metaData.Smiley);

                bw.Write(metaData.TotalCollectedCount);
                for (int flowerType = 0; flowerType < Balance.MAX_FLOWER_TYPES; flowerType++)
                    bw.Write(metaData.FlowerCollectedCount[flowerType]);
                for (int flowerType = 0; flowerType < Balance.MAX_FLOWER_TYPES; flowerType++)
                    bw.Write(metaData.FlowerAchievement[flowerType]);
            }
        }

        public static bool TryLoadMeta(MetaData metaData)
        {
            bool success = false;

            string fileName = Application.persistentDataPath + "/meta_v" + VERSION + ".dat";
            if (File.Exists(fileName))
            {
                using (var stream = File.Open(fileName, FileMode.Open))
                {
                    using (BinaryReader br = new BinaryReader(stream))
                    {
                        metaData.Coins = br.ReadDouble();
                        metaData.GameType = (GAME_TYPE)br.ReadInt32();
                        metaData.MenuState = (MENU_STATE)br.ReadInt32();
                        metaData.PrevMenuState = (MENU_STATE)br.ReadInt32();
                        metaData.KeyboardIndex = br.ReadInt32();
                        metaData.LastTimeStamp = br.ReadSingle();
                        metaData.GrowTime = br.ReadSingle();
                        metaData.TutorialFlags = br.ReadInt32();

                        metaData.ShowPrestige = br.ReadBoolean();
                        metaData.Prestige = br.ReadInt32();

                        metaData.SFX = br.ReadBoolean();
                        metaData.Music = br.ReadBoolean();
                        metaData.WPM = br.ReadBoolean();
                        metaData.Font = br.ReadInt32();
                        metaData.Smiley = br.ReadBoolean();

                        metaData.TotalCollectedCount = br.ReadInt32();
                        for (
                            int flowerType = 0;
                            flowerType < Balance.MAX_FLOWER_TYPES;
                            flowerType++
                        )
                            metaData.FlowerCollectedCount[flowerType] = br.ReadInt32();
                        for (
                            int flowerType = 0;
                            flowerType < Balance.MAX_FLOWER_TYPES;
                            flowerType++
                        )
                            metaData.FlowerAchievement[flowerType] = br.ReadBoolean();

                        if (metaData.Coins < 0.0f)
                            metaData.Coins = 0.0f;

                        success = true;
                        //TEST
                        // metaData.TutorialFlags = 0;
                    }
                }
            }
            return success;
        }

        public static bool TryLoadMetaV6(MetaData metaData)
        {
            bool success = false;

            string fileName = Application.persistentDataPath + "/meta_v6.dat";
            if (File.Exists(fileName))
            {
                using (var stream = File.Open(fileName, FileMode.Open))
                {
                    using (BinaryReader br = new BinaryReader(stream))
                    {
                        metaData.Coins = br.ReadDouble();
                        metaData.GameType = (GAME_TYPE)br.ReadInt32();
                        metaData.MenuState = (MENU_STATE)br.ReadInt32();
                        metaData.PrevMenuState = (MENU_STATE)br.ReadInt32();
                        metaData.KeyboardIndex = br.ReadInt32();
                        metaData.LastTimeStamp = br.ReadSingle();
                        metaData.GrowTime = br.ReadSingle();
                        metaData.TutorialFlags = br.ReadInt32();

                        metaData.ShowPrestige = br.ReadBoolean();
                        metaData.Prestige = br.ReadInt32();

                        metaData.SFX = br.ReadBoolean();
                        metaData.Music = br.ReadBoolean();
                        metaData.WPM = br.ReadBoolean();
                        metaData.Font = br.ReadInt32();
                        metaData.Smiley = br.ReadBoolean();

                        metaData.TotalCollectedCount = br.ReadInt32();
                        for (
                            int flowerType = 0;
                            flowerType < Balance.MAX_FLOWER_TYPES;
                            flowerType++
                        )
                            metaData.FlowerCollectedCount[flowerType] = br.ReadInt32();
                        for (
                            int flowerType = 0;
                            flowerType < Balance.MAX_FLOWER_TYPES;
                            flowerType++
                        )
                            metaData.FlowerAchievement[flowerType] = br.ReadBoolean();

                        if (metaData.Coins < 0.0f)
                            metaData.Coins = 0.0f;

                        success = true;
                    }
                }
            }
            return success;
        }

        public static bool TryLoadMetaV5(MetaData metaData)
        {
            bool success = false;

            string fileName = Application.persistentDataPath + "/meta_v" + VERSION + ".dat";
            if (File.Exists(fileName))
            {
                using (var stream = File.Open(fileName, FileMode.Open))
                {
                    using (BinaryReader br = new BinaryReader(stream))
                    {
                        metaData.Coins = br.ReadDouble();
                        metaData.GameType = (GAME_TYPE)br.ReadInt32();
                        metaData.MenuState = (MENU_STATE)br.ReadInt32();
                        metaData.PrevMenuState = (MENU_STATE)br.ReadInt32();
                        metaData.KeyboardIndex = br.ReadInt32();
                        metaData.LastTimeStamp = br.ReadSingle();
                        metaData.GrowTime = br.ReadSingle();
                        metaData.TutorialFlags = br.ReadInt32();

                        metaData.ShowPrestige = br.ReadBoolean();
                        metaData.Prestige = br.ReadInt32();

                        metaData.SFX = br.ReadBoolean();
                        metaData.Music = br.ReadBoolean();
                        metaData.WPM = br.ReadBoolean();
                        metaData.Font = br.ReadInt32();

                        metaData.TotalCollectedCount = br.ReadInt32();
                        for (
                            int flowerType = 0;
                            flowerType < Balance.MAX_FLOWER_TYPES;
                            flowerType++
                        )
                            metaData.FlowerCollectedCount[flowerType] = br.ReadInt32();
                        for (
                            int flowerType = 0;
                            flowerType < Balance.MAX_FLOWER_TYPES;
                            flowerType++
                        )
                            metaData.FlowerAchievement[flowerType] = br.ReadBoolean();

                        if (metaData.Coins < 0.0f)
                            metaData.Coins = 0.0f;

                        success = true;
                        //TEST
                        // metaData.TutorialFlags = 0;
                    }
                }
            }
            return success;
        }

        public static bool TryLoadMetaV4(MetaData metaData)
        {
            bool success = false;

            string fileName = Application.persistentDataPath + "/meta_v4.dat";
            if (File.Exists(fileName))
            {
                using (var stream = File.Open(fileName, FileMode.Open))
                {
                    using (BinaryReader br = new BinaryReader(stream))
                    {
                        metaData.Coins = br.ReadDouble();
                        metaData.GameType = (GAME_TYPE)br.ReadInt32();
                        metaData.MenuState = (MENU_STATE)br.ReadInt32();
                        metaData.PrevMenuState = (MENU_STATE)br.ReadInt32();
                        metaData.KeyboardIndex = br.ReadInt32();
                        metaData.LastTimeStamp = br.ReadSingle();
                        metaData.GrowTime = br.ReadSingle();
                        metaData.TutorialFlags = br.ReadInt32();

                        metaData.ShowPrestige = br.ReadBoolean();
                        metaData.Prestige = br.ReadInt32();

                        metaData.SFX = br.ReadBoolean();
                        metaData.Music = br.ReadBoolean();
                        metaData.WPM = br.ReadBoolean();

                        metaData.TotalCollectedCount = br.ReadInt32();
                        for (
                            int flowerType = 0;
                            flowerType < Balance.MAX_FLOWER_TYPES;
                            flowerType++
                        )
                            metaData.FlowerCollectedCount[flowerType] = br.ReadInt32();
                        for (
                            int flowerType = 0;
                            flowerType < Balance.MAX_FLOWER_TYPES;
                            flowerType++
                        )
                            metaData.FlowerAchievement[flowerType] = br.ReadBoolean();

                        if (metaData.Coins < 0.0f)
                            metaData.Coins = 0.0f;

                        success = true;
                        //TEST
                        // metaData.TutorialFlags = 0;
                    }
                }
            }
            return success;
        }

        public static bool TryLoadMetaV2(MetaData metaData)
        {
            bool success = false;

            string fileName = Application.persistentDataPath + "/meta_v2.dat";
            if (File.Exists(fileName))
            {
                using (var stream = File.Open(fileName, FileMode.Open))
                {
                    using (BinaryReader br = new BinaryReader(stream))
                    {
                        metaData.Coins = br.ReadDouble();
                        metaData.GameType = (GAME_TYPE)br.ReadInt32();
                        metaData.MenuState = (MENU_STATE)br.ReadInt32();
                        metaData.PrevMenuState = (MENU_STATE)br.ReadInt32();
                        metaData.KeyboardIndex = br.ReadInt32();
                        metaData.LastTimeStamp = br.ReadSingle();
                        metaData.GrowTime = br.ReadSingle();
                        metaData.TutorialFlags = br.ReadInt32();

                        metaData.ShowPrestige = br.ReadBoolean();
                        metaData.Prestige = br.ReadInt32();

                        metaData.SFX = br.ReadBoolean();
                        metaData.Music = br.ReadBoolean();
                        metaData.WPM = br.ReadBoolean();

                        if (metaData.Coins < 0.0f)
                            metaData.Coins = 0.0f;

                        success = true;
                        //TEST
                        // metaData.TutorialFlags = 0;
                    }
                }
            }
            return success;
        }
    }
}

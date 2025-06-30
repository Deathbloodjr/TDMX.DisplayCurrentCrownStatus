using DisplayCurrentCrownStatus;
using SongSelect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace DisplayCurrentCrownStatus.Patches
{
    internal class SpriteManagement
    {
        public static Dictionary<DataConst.CrownType, Sprite> CrownSprites = new Dictionary<DataConst.CrownType, Sprite>();
        public static Dictionary<DataConst.CrownType, GameObject> CrownSpriteGameObjects = new Dictionary<DataConst.CrownType, GameObject>();

        public static void InitializeCrownSprites(SongSelectScoreDisplay scoreDisplay)
        {
            if (scoreDisplay == null || scoreDisplay.playerIndex != SongSelectScoreDisplay.PlayerIndex.Player1)
            {
                return;
            }

            var images = scoreDisplay.GetComponentsInChildren<Image>();
            for (int i = 0; i < images.Length; i++)
            {
                if (images[i] is null)
                {
                    continue;
                }
                var name = images[i].gameObject.name;
                if (name.StartsWith("IconCrown"))
                {
                    if (name == "IconCrownSilver")
                    {
                        AddSpriteToDictionary(images[i].sprite, DataConst.CrownType.Silver);
                    }
                    else if (name == "IconCrownGold")
                    {
                        AddSpriteToDictionary(images[i].sprite, DataConst.CrownType.Gold);
                    }
                    else if (name == "IconCrownRainbow")
                    {
                        AddSpriteToDictionary(images[i].sprite, DataConst.CrownType.Rainbow);
                    }
                }
            }
        }

        static void AddSpriteToDictionary(Sprite sprite, DataConst.CrownType crown)
        {
            if (!CrownSprites.ContainsKey(crown))
            {
                CrownSprites.Add(crown, sprite);
                GameObject obj = new GameObject("SaveSprite");
                var image = obj.AddComponent<Image>();
                image.sprite = sprite;
                UnityEngine.Object.DontDestroyOnLoad(obj);
                CrownSpriteGameObjects.Add(crown, obj);
                Plugin.Log.LogInfo("Sprite added for crown: " + crown.ToString());
            }
        }
    }
}

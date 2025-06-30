using Blittables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

#if IL2CPP
using Il2CppInterop.Runtime.Injection;
#endif

namespace DisplayCurrentCrownStatus.Patches
{
    internal class PlayerCrownStatus : MonoBehaviour
    {
#if IL2CPP
        static PlayerCrownStatus() => ClassInjector.RegisterTypeInIl2Cpp<PlayerCrownStatus>();
#endif

        public bool IsFC { get; set; } = true;
        public bool IsDFC { get; set; } = true;

        public int OKs { get; set; } = 0;
        public int Bads { get; set; } = 0;

        Image CrownImage { get; set; } = null;

        public void Start()
        {
            SetCrownSprite(DataConst.CrownType.Rainbow);
        }

        void SetCrownSprite(DataConst.CrownType crown)
        {
            if (crown != DataConst.CrownType.Silver &&
                crown != DataConst.CrownType.Gold &&
                crown != DataConst.CrownType.Rainbow)
            {
                return;
            }

            CrownImage = gameObject.GetComponent<Image>();
            if (CrownImage == null)
            {
                CrownImage = gameObject.AddComponent<Image>();
            }

            //Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, 1, false);
            //tex.SetPixels(SpriteManagement.CrownSprites[crown]);
            //Rect rect = new Rect(0, 0, tex.width, tex.height);
            if (SpriteManagement.CrownSprites.ContainsKey(crown))
            {
                CrownImage.sprite = SpriteManagement.CrownSprites[crown];
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void Reset(bool isEnabled)
        {
            // Change sprite to Rainbow Crown
            IsFC = true;
            IsDFC = true;
            OKs = 0;
            Bads = 0;
            SetCrownSprite(DataConst.CrownType.Rainbow);
            //gameObject.SetActive(isEnabled);
            CrownImage.gameObject.SetActive(isEnabled);
            ModLogger.Log("Currently Rainbow");
        }

        public void CheckHits(EachPlayer player)
        {
            if (player.countKa != OKs)
            {
                OKs = (int)player.countKa;
                HitOk();
            }
            if (player.countFuka != Bads)
            {
                Bads = (int)player.countFuka;
                HitBad();
            }
        }

        void HitOk()
        {
            if (IsDFC)
            {
                // Change sprite to Gold Crown
                IsDFC = false;
                ModLogger.Log("Currently Gold");
                SetCrownSprite(DataConst.CrownType.Gold);
            }
        }

        void HitBad()
        {
            if (IsFC || IsDFC)
            {
                // Change sprite to Silver Crown
                IsFC = false;
                IsDFC = false;
                ModLogger.Log("Currently Silver");
                SetCrownSprite(DataConst.CrownType.Silver);
            }
        }
    }
}

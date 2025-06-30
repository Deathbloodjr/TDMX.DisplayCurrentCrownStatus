using HarmonyLib;
using SongSelect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DisplayCurrentCrownStatus.Patches
{
    internal class DisplayCurrentCrownStatusPatch
    {
        static List<PlayerCrownStatus> PlayerStatuses = new List<PlayerCrownStatus>();

        [HarmonyPatch(typeof(SongSelectScoreDisplay))]
        [HarmonyPatch(nameof(SongSelectScoreDisplay.Start))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void SongSelectScoreDisplay_Start_Postfix(SongSelectScoreDisplay __instance)
        {
            SpriteManagement.InitializeCrownSprites(__instance);
        }

        public static void InitializePlayerStatus()
        {
            ModLogger.Log("InitializePlayerStatus");
            // idk how to properly handle player 2 yet
            //var ruleType = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.ruleType;
            for (int i = 0; i < 2; i++)
            {
                if (PlayerStatuses.Count <= i)
                {
                    PlayerStatuses.Add(CreatePlayerCrownObject(i/*, ruleType != EnsoData.RuleType.Enso*/));
                }
                if (PlayerStatuses[i] == null)
                {
                    PlayerStatuses[i] = CreatePlayerCrownObject(i/*, ruleType != EnsoData.RuleType.Enso*/);
                }
                var numPlayers = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.playerNum;
                PlayerStatuses[i].Reset(numPlayers > i);
            }
        }

        static PlayerCrownStatus CreatePlayerCrownObject(int playerNum/*, bool isVersusMatch*/)
        {
            GameObject playerStatusObject = new GameObject("Player " + (playerNum + 1) + " Crown Status");
            GameObject parent = GameObject.Find("taiko_indicator" + (playerNum == 0 ? "" : "2"));
            if (parent != null)
            {
                playerStatusObject.transform.SetParent(parent.transform);
            }

            // Might need some adjustments for player 2, or ranked matches
            playerStatusObject.transform.localPosition = new Vector2(63, 109);
            playerStatusObject.transform.localScale = new Vector2(0.5f, 0.5f);

            if (GameObject.Find("DaniDojo") is not null)
            {
                playerStatusObject.transform.localScale = Vector2.zero;
            }

            var playerCrownStatus = playerStatusObject.AddComponent<PlayerCrownStatus>();
            ModLogger.Log("PlayerCrownStatus created", LogType.Debug);
            return playerCrownStatus;
        }

        [HarmonyPatch(typeof(EnsoGraphicManager))]
        [HarmonyPatch(nameof(EnsoGraphicManager.IsPrepareFinished))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void EnsoGraphicManager_IsPrepareFinished_Postfix(EnsoGameManager __instance)
        {
            ModLogger.Log("EnsoGraphicManager_IsPrepareFinished_Postfix");
            InitializePlayerStatus();
        }

        [HarmonyPatch(typeof(EnsoGameManager))]
        [HarmonyPatch(nameof(EnsoGameManager.ProcExecMain))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void EnsoGameManager_ProcExecMain_Postfix_GetNoteResults(EnsoGameManager __instance)
        {
            var frameResult = __instance.ensoParam.GetFrameResults();
            AddHitResultFromFrameResult(frameResult);
        }

        public static void AddHitResultFromFrameResult(TaikoCoreFrameResults frameResults)
        {
            for (int i = 0; i < 2; i++)
            {
                if (frameResults.eachPlayer.Length > i)
                {
                    if (PlayerStatuses.Count > i)
                    {
                        PlayerStatuses[i].CheckHits(frameResults.eachPlayer[i]);
                    }
                    else
                    {
                        InitializePlayerStatus();
                    }
                }
            }
        }
    }
}

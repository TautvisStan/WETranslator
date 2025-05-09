using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GameTranslator
{
    [BepInPlugin(PluginGuid, PluginName, PluginVer)]
    [HarmonyPatch]
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "GeeEm.WrestlingEmpire.GameTranslator";
        public const string PluginName = "GameTranslator";
        public const string PluginVer = "0.0.1";

        internal static ManualLogSource Log;
        internal readonly static Harmony Harmony = new(PluginGuid);

        internal static string PluginPath;

        public static bool SpeechBubble = false;

        public static ConfigEntry<string> TargetLanguage;
        private void Awake()
        {
            Plugin.Log = base.Logger;

            PluginPath = Path.GetDirectoryName(Info.Location);

            TargetLanguage = Config.Bind(
                "General",
                "Target Language",
                "Lithuanian",
                new ConfigDescription("Target language to translate in", new AcceptableValueList<string>("Lithuanian", "Latvian")));
        }

        private void OnEnable()
        {
            Harmony.PatchAll();
            Logger.LogInfo($"Loaded {PluginName}!");
        }

        private void OnDisable()
        {
            Harmony.UnpatchSelf();
            Logger.LogInfo($"Unloaded {PluginName}!");
        }
        [HarmonyPatch(typeof(NEGAFEHECNL), nameof(NEGAFEHECNL.BBICLKGGIGB))]
        [HarmonyPrefix]         //Used to track when the speech bubble appears
        static void SpeakLinesPre()
        {
            if (NEGAFEHECNL.ABMMBFGLBOL != null)
            {
                SpeechBubble = NEGAFEHECNL.ABMMBFGLBOL.activeSelf;
            }
        }
        [HarmonyPatch(typeof(NEGAFEHECNL), nameof(NEGAFEHECNL.BBICLKGGIGB))]
        [HarmonyPostfix]        //handling 
        static void SpeakLinesPost()
        {
            if (NEGAFEHECNL.ABMMBFGLBOL != null && NEGAFEHECNL.EJFHLGMHAHB == 0)
            {
                if (SpeechBubble == false && NEGAFEHECNL.ABMMBFGLBOL.activeSelf == true)        //if the speech bubble appears
                {
                    Log.LogInfo($"{NEGAFEHECNL.MLLPFEKAONO[1]}|{NEGAFEHECNL.MLLPFEKAONO[2]}");
                }
                NEGAFEHECNL.BMNIHDAGPFB[1].text = "Čia yra mano replacintas tekstas";
               // NEGAFEHECNL.BMNIHDAGPFB[1].color = NEGAFEHECNL.OBFDKFIDMIL[1];
                NEGAFEHECNL.BMNIHDAGPFB[2].text = "Originalus tekstas yra konsolėje";
                //  NEGAFEHECNL.BMNIHDAGPFB[2].color = NEGAFEHECNL.OBFDKFIDMIL[2];
                LIPNHOMGGHF.JPFJOHGHHDL(NEGAFEHECNL.BMNIHDAGPFB[1], 40, 45);  //resizing the text
                LIPNHOMGGHF.JPFJOHGHHDL(NEGAFEHECNL.BMNIHDAGPFB[2], 40, 45);
            }
        }
    }
}
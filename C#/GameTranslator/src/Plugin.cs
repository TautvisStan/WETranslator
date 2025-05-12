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

        public static CliProcessPool process = null;
        private void Awake()
        {
            Plugin.Log = base.Logger;

            PluginPath = Path.GetDirectoryName(Info.Location);

            TargetLanguage = Config.Bind(
                "General",
                "Target Language",
                "Lithuanian",
                new ConfigDescription("Target language to translate in", new AcceptableValueList<string>("Lithuanian", "Latvian")));

            process = new CliProcessPool(Path.Combine(Plugin.PluginPath, "TerminalApp.exe"), 1);
            //string result = process.Execute("StartupTest");
            //Log.LogInfo($"Translator status {result}");
        }
        public static string TranslateText(string lang, string text)
        {
           

            string result = process.Execute($"{lang}|{text}");
            return result;
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
        static string line1 = "";
        static string line2 = "";
        [HarmonyPatch(typeof(NEGAFEHECNL), nameof(NEGAFEHECNL.BBICLKGGIGB))]
        [HarmonyPrefix]         //Used to track when the speech bubble appears
        static void SpeakLinesPre()
        {
            if (NEGAFEHECNL.ABMMBFGLBOL != null)
            {
                SpeechBubble = NEGAFEHECNL.ABMMBFGLBOL.activeSelf;
            }
            else
            {
                line1 = "";
                line2 = "";
            }
        }

        [HarmonyPatch(typeof(NEGAFEHECNL), nameof(NEGAFEHECNL.BBICLKGGIGB))]
        [HarmonyPostfix]        //handling dialog
        // note: runs on update
        static void SpeakLinesPost()
        {
            if (NEGAFEHECNL.ABMMBFGLBOL != null && NEGAFEHECNL.EJFHLGMHAHB == 0)
            {
                if (SpeechBubble == false && NEGAFEHECNL.ABMMBFGLBOL.activeSelf == true)        //if the speech bubble appears
                {
                    line1 = TranslateText("LT", NEGAFEHECNL.MLLPFEKAONO[1]);
                    line2 = TranslateText("LT", NEGAFEHECNL.MLLPFEKAONO[2]);
                    Log.LogInfo($"{NEGAFEHECNL.MLLPFEKAONO[1]}|{NEGAFEHECNL.MLLPFEKAONO[2]}");
                }
                if (line1 != "")
                {
                    NEGAFEHECNL.BMNIHDAGPFB[1].text = line1;
                    LIPNHOMGGHF.JPFJOHGHHDL(NEGAFEHECNL.BMNIHDAGPFB[1], 40, 45);  //resizing the text
                }
                if (line2 != "")
                {
                    NEGAFEHECNL.BMNIHDAGPFB[2].text = line2;
                    LIPNHOMGGHF.JPFJOHGHHDL(NEGAFEHECNL.BMNIHDAGPFB[2], 40, 45);  //resizing the text
                }
            }
        }

        [HarmonyPatch(typeof(Scene_News), nameof(Scene_News.KALIJLMGNNH))]
        [HarmonyPostfix]        //news pages 
        static void NewsPageDisplay(Scene_News __instance, int EJDHFNIJFHI)
        {
            string headline = "";
            if (__instance.gHeadline.activeSelf)    //week report
            {
                headline = IMNHOCBFGHJ.OLMOLOOOIJM[IMNHOCBFGHJ.ODOAPLMOJPD].PKLAJJAGGAK;  
                __instance.textHeadline.text = "Čia yra mano replacintas tekstas";
            }
            string text = IMNHOCBFGHJ.OLMOLOOOIJM[IMNHOCBFGHJ.ODOAPLMOJPD].CLCLFBAAMOM; //match report; week report
            __instance.textArticle.text = "Originalus tekstas yra konsolėje";

            Log.LogInfo($"{headline}|{text}");
        }



        [HarmonyPatch(typeof(IMNHOCBFGHJ), nameof(IMNHOCBFGHJ.OAODMFBHCGA))]
        [HarmonyPostfix]        //match news page headline 1
        static void MatchReportHeadline(ref string __result)
        {
            Log.LogInfo($"1: {__result}");
            __result = "Čia yra mano replacintas tekstas 1";
        }

        [HarmonyPatch(typeof(IMNHOCBFGHJ), nameof(IMNHOCBFGHJ.CIIDDMMENME))]
        [HarmonyPostfix]        //match news page headline 2
        static void MatchReportResult(ref string __result)
        {
            Log.LogInfo($"2: {__result}");
            __result = "Čia yra mano replacintas tekstas 2";
        }
    }
}
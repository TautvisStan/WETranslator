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
        public const string PluginVer = "0.0.2";

        internal static ManualLogSource Log;
        internal readonly static Harmony Harmony = new(PluginGuid);

        internal static string PluginPath;

        public static bool SpeechBubble = false;

        public static ConfigEntry<string> TargetLanguage;

        public static CLIProcess TranslatorProcess = null;
        private void Awake()
        {
            Plugin.Log = base.Logger;

            PluginPath = Path.GetDirectoryName(Info.Location);

            TargetLanguage = Config.Bind(
                "General",
                "Target Language",
                "Lithuanian",
                new ConfigDescription("Target language to translate in", new AcceptableValueList<string>("Lithuanian", "Spanish")));

            TranslatorProcess = new CLIProcess(Path.Combine(Plugin.PluginPath, "TranslatorApp.exe"));
            Log.LogWarning(TranslateText("LT", "Hello Code Academy!"));
        }
        public static string TranslateText(string lang, string text)
        {
            Log.LogInfo($"Translating {text} to {lang}");
            string result = TranslatorProcess.Execute($"{lang}|{text}");
            Log.LogInfo($"Received {result}");
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
            if (__instance.gHeadline.activeSelf)    //week report
            {
                //   headline = IMNHOCBFGHJ.OLMOLOOOIJM[IMNHOCBFGHJ.ODOAPLMOJPD].PKLAJJAGGAK;
                string headline = TranslateText("LT", IMNHOCBFGHJ.OLMOLOOOIJM[IMNHOCBFGHJ.ODOAPLMOJPD].PKLAJJAGGAK);
                __instance.textHeadline.text = headline;
            }
            //string text = IMNHOCBFGHJ.OLMOLOOOIJM[IMNHOCBFGHJ.ODOAPLMOJPD].CLCLFBAAMOM; //match report; week report
            string text = TranslateText("LT", IMNHOCBFGHJ.OLMOLOOOIJM[IMNHOCBFGHJ.ODOAPLMOJPD].CLCLFBAAMOM);
            __instance.textArticle.text = text;
        }

        [HarmonyPatch(typeof(IMNHOCBFGHJ), nameof(IMNHOCBFGHJ.OAODMFBHCGA))]
        [HarmonyPostfix]        //match news page headline 1
        static void MatchReportHeadline(ref string __result)
        {
            __result = TranslateText("LT", __result);
        }

        [HarmonyPatch(typeof(IMNHOCBFGHJ), nameof(IMNHOCBFGHJ.CIIDDMMENME))]
        [HarmonyPostfix]        //match news page headline 2
        static void MatchReportResult(ref string __result)
        {
            __result = TranslateText("LT", __result);
        }
    }
}
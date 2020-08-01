using System;
using UnityEngine;
using RimWorld;
using Verse;
using HarmonyLib;

namespace TestMod
{
    [StaticConstructorOnStartup]
    public static class MyMod
    {
        static MyMod() //our constructor
        {
            Harmony harmony = new Harmony("miki4920.Rimworld.NoSpeedLimit");
            harmony.PatchAll();
            Log.Message("Hello World!");
        }
    }
    [HarmonyPatch(typeof(TimeSlower))]
    [HarmonyPatch(nameof(TimeSlower.SignalForceNormalSpeed))] 
    static class TimeSlower_SignalForceNormalSpeed_Patch
    {
        static void Prefix() //pass the __result by ref to alter it.
        {
            this.forceNormalSpeedUntil = Mathf.Max(new int[]
            {
                Find.TickManager.TicksGame
            });
        }
    }
    [HarmonyPatch(typeof(TimeSlower))]
    [HarmonyPatch(nameof(TimeSlower.SignalForceNormalSpeedShort))]
    static class TimeSlower_SignalForceNormalSpeedShort_Patch
    {
        static void Prefix() //pass the __result by ref to alter it.
        {
            this.forceNormalSpeedUntil = Mathf.Max(new int[]
            {
                Find.TickManager.TicksGame
            });
        }
    }
}
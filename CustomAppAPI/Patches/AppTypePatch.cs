using HarmonyLib;
using Reptile.Phone;
using System;
namespace CustomAppAPI.Patches
{
    [HarmonyPatch(typeof(Type))]
    internal class AppTypePatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("GetType", typeof(string))]
        public static void GetType(string typeName, ref Type __result)
        {
            // insane game developer things
            foreach (Type type in CustomAppCache.CustomAppTypes)
            {
                if (typeName == $"Reptile.Phone.{CustomAppMod.GetAppKey(type)}")
                {
                    __result = type;
                    break;
                }
            }
        }
    }
}

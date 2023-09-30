using BepInEx;
using HarmonyLib;
using Reptile.Phone;
using Reptile;
using System.Reflection;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomAppAPI
{
    [BepInPlugin(PluginMetadata.GUID, PluginMetadata.Name, PluginMetadata.Version)]
    [BepInProcess("Bomb Rush Cyberfunk.exe")]
    internal class CustomAppMod : BaseUnityPlugin
    {
        private static Harmony _harmonyInstance;

        private static CustomAppMod _modInstance;
        public static CustomAppMod Instance => _modInstance;

        private static Assembly _modAssembly;
        public static Assembly ModAssembly => _modAssembly;

        public static BepInEx.Logging.ManualLogSource Log => _modInstance.Logger;

        // Unity Functions

        private void Awake()
        {
            _modInstance = this;
            _modAssembly = Assembly.GetExecutingAssembly();

            _harmonyInstance = new Harmony($"{PluginMetadata.GUID}.patch");
            _harmonyInstance.PatchAll();

            Logger.LogInfo($"Plugin {PluginMetadata.Name} patched successfully.");
        }

        private void Start()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in assemblies)
            {
                Initialize(assembly);
            }
        }

        // Custom App Initialization

        public static void Initialize(Assembly assembly)
        {
            Type[] customAppTypes = FindDerivedTypes(assembly, typeof(CustomApp)).ToArray();

            if (customAppTypes.Length <= 0)
                return;

            foreach (Type customAppType in customAppTypes)
            {
                CustomAppCache.AddType(customAppType);
                Log.LogInfo($"Loaded new custom app: {GetAppKey(customAppType)}");
            }
        }

        public static void InitializeApps(Phone phone)
        {
            CustomAppCache.ResetApps();

            Transform apps = phone.transform.Find("OpenCanvas/PhoneContainerOpen/MainScreen/Apps");

            foreach (Type app in CustomAppCache.CustomAppTypes)
            {
                GameObject customApp = new GameObject(GetAppKey(app));
                customApp.layer = Layers.Phone;
                CustomApp component = (CustomApp)customApp.AddComponent(app);

                customApp.transform.SetParent(apps, false);
                customApp.transform.localScale = Vector3.one;
                customApp.SetActive(true);

                CustomAppCache.AddApp(component);
            }
        }

        // Utility

        private static IEnumerable<Type> FindDerivedTypes(Assembly assembly, Type baseType)
        {
            return assembly.GetTypes().Where(t => baseType.IsAssignableFrom(t) && t != baseType);
        }

        public static string GetAppKey(Type type)
        {
            return $"{type.Name}";
        }
    }
}

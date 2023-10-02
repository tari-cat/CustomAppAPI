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
                CustomApp customAppComponent = (CustomApp)customApp.AddComponent(app);

                customApp.transform.SetParent(apps, false);
                customApp.transform.localScale = Vector3.one;
                customApp.SetActive(true);

                GameObject content = new GameObject("Content");
                content.layer = Layers.Phone;
                RectTransform contentRect = content.AddComponent<RectTransform>();
                contentRect.sizeDelta = new Vector2(1070, 1775); // the FUCK
                content.transform.SetParent(customApp.transform, false);
                content.transform.localScale = Vector3.one;
                content.SetActive(true);

                Notification email = GetEmailNotification(phone);

                GameObject newEmail = Instantiate(email.gameObject, customApp.transform);
                Notification notification = newEmail.GetComponent<Notification>();

                customAppComponent.SetNotification(notification);

                CustomAppCache.AddApp(customAppComponent);
            }
        }

        private static Notification GetEmailNotification(Phone phone)
        {
            AppEmail app = phone.GetAppInstance<AppEmail>();
            if (app == null)
                return null;
            Notification notif = app.GetComponent<Notification>();
            if (notif == null)
                return null;
            return notif;
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

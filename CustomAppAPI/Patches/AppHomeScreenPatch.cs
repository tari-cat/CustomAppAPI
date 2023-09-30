using HarmonyLib;
using Reptile.Phone;
using Reptile;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

namespace CustomAppAPI.Patches
{
    [HarmonyPatch(typeof(AppHomeScreen))]
    internal class AppHomeScreenPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("OnAppInit")]
        public static void OnAppInit(AppHomeScreen __instance)
        {
            Traverse traverse = Traverse.Create(__instance);
            Traverse<HomeScreenApp[]> apps = traverse.Field<HomeScreenApp[]>("m_Apps");
            Traverse<HomeScreenApp[]> availableHomeScreenApps = traverse.Field<HomeScreenApp[]>("availableHomeScreenApps");

            // give me your phone.
            Phone phone = traverse.Property<Phone>("MyPhone").Value;

            // initialize our custom apps which populates our lists
            CustomAppMod.InitializeApps(phone);

            Traverse phoneTraverse = Traverse.Create(phone);
            AudioManager audioManager = phoneTraverse.Field<AudioManager>("audioManager").Value;

            // create app instances in homescreen init -- then initialize them
            // trying to do this in any other order has been a painful experience

            List<CustomApp> customApps = CustomAppCache.CustomApps;

            foreach (CustomApp customApp in customApps)
            {
                phone.AppInstances.Add(customApp.Name, customApp);
                customApp.Init(phone, audioManager);
                customApp.gameObject.SetActive(false);
            }

            // some linq for simplicity

            List<HomeScreenApp> appsList = apps.Value.ToList();
            List<HomeScreenApp> availableHomeScreenAppsList = availableHomeScreenApps.Value.ToList();

            // create new apps

            int maxValue = (int)Enum.GetValues(typeof(HomeScreenApp.HomeScreenAppType)).Cast<HomeScreenApp.HomeScreenAppType>().Max();

            for (int i = 0; i < customApps.Count; i++)
            {
                // we gotta assign some unique enum value, pray this doesnt conflict with anything
                HomeScreenApp.HomeScreenAppType enumValue = (HomeScreenApp.HomeScreenAppType) (maxValue + (i + 1));

                CustomApp customApp = customApps[i];

                Texture2D icon = customApp.Icon;
                Rect rect = new Rect(0, 0, icon.width, icon.height);
                Sprite sprite = Sprite.Create(icon, rect, new Vector2(0.5f, 0.5f), 100);

                // create our home screen app and set all relevant values
                HomeScreenApp app = ScriptableObject.CreateInstance<HomeScreenApp>();
                Traverse appTraverse = Traverse.Create(app);
                appTraverse.Field<string>("m_AppName").Value = customApp.Name;
                appTraverse.Field<string>("m_DisplayName").Value = customApp.DisplayName;
                appTraverse.Field<Sprite>("m_AppIcon").Value = sprite;
                appTraverse.Field<HomeScreenApp.HomeScreenAppType>("appType").Value = enumValue;

                // then we're done! add it to the phone's lists of apps
                appsList.Add(app);
                availableHomeScreenAppsList.Add(app);
            }

            // and finally set the actual lists
            apps.Value = appsList.ToArray();
            availableHomeScreenApps.Value = availableHomeScreenAppsList.ToArray();
        }
    }
}

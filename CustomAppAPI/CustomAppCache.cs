using System;
using System.Collections.Generic;

namespace CustomAppAPI
{
    internal static class CustomAppCache
    {
        private static List<Type> _customAppTypes = new List<Type>();

        private static List<CustomApp> _customApps = new List<CustomApp>();

        public static List<Type> CustomAppTypes => _customAppTypes;
        public static List<CustomApp> CustomApps => _customApps;

        public static void AddType(Type type)
        {
            if (_customAppTypes.Contains(type))
                return;
            _customAppTypes.Add(type);
        }

        public static void AddApp(CustomApp customApp)
        {
            if (_customApps.Contains(customApp))
                return;
            _customApps.Add(customApp);
        }

        public static void ResetApps()
        {
            _customApps.Clear();
        }
    }
}

using Reptile;
using Reptile.Phone;
using System;
using System.IO;
using UnityEngine;

namespace CustomAppAPI
{
    public abstract class CustomApp : App
    {
        public string Name => CustomAppMod.GetAppKey(GetType());
        public abstract string DisplayName { get; }
        public abstract Texture2D Icon { get; }

        public override void Awake()
        {
            m_Unlockables = Array.Empty<AUnlockable>();
            base.Awake();
        }

        public static Texture2D LoadTexture(string filePath)
        {
            if (!File.Exists(filePath))
            {
                CustomAppMod.Log.LogError($"Texture doesn't exist at path: {filePath}");
                return null;
            }
            string fileName = Path.GetFileNameWithoutExtension(filePath);

            string ext = Path.GetExtension(filePath);

            if (ext != ".jpg" && ext != ".jpeg" && ext != ".png")
            {
                CustomAppMod.Log.LogWarning($"Cannot load texture: {fileName}{ext}. Only .png and .jpg files are accepted.");
                return null;
            }

            Texture2D tex = new Texture2D(2, 2);
            if (tex.LoadImage(File.ReadAllBytes(filePath)))
            {
                tex.Apply();

                return tex;
            }

            CustomAppMod.Log.LogWarning($"Could not load image data of {fileName}{ext}.");
            return null;
        }

        public void SetNotification(Notification notification)
        {
            m_Notification = notification;
            m_Notification.InitNotification(this);
        }
    }
}

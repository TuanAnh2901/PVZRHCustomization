using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.Rendering;
using BepInEx;

namespace CustomizeLib.BepInEx
{
    public static class TextureStore
    {
        public static void Init() => LoadTextures();

        public static Texture2D LoadImage(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("The image file at path '" + path + "' does not exist.");
            }
            byte[] array = File.ReadAllBytes(path);
            Texture2D texture2D = new(2, 2, GraphicsFormat.R8G8B8A8_UNorm, TextureCreationFlags.None);
            ImageConversion.LoadImage(texture2D, array);
            return texture2D;
        }

        public static void LoadTextures()
        {
            if (!Directory.Exists(Path.Combine(Paths.PluginPath, "Textures")))
            {
                Directory.CreateDirectory(Path.Combine(Paths.PluginPath, "Textures"));
            }
            try
            {
                foreach (string text3 in Directory.EnumerateFiles(Path.Combine(Paths.PluginPath, "Textures"), "*.png", SearchOption.AllDirectories))
                {
                    LoadImage(text3);
                    TextureDict[Path.GetFileNameWithoutExtension(text3)] = text3;
                }
            }
            catch (Exception ex2)
            {
                CustomCore.Instance.Value.Log.LogError("Error loading Texture.");
                CustomCore.Instance.Value.Log.LogError(ex2);
                return;
            }
            CustomCore.Instance.Value.Log.LogInfo("Textures loaded successfully.");
        }

        public static void Reload()
        {
            TextureDict.Clear();
            LoadTextures();
        }

        public static void ReplaceTextures()
        {
            foreach (var texture2D in from Texture2D texture2D in (Texture2D[])Resources.FindObjectsOfTypeAll<Texture2D>()
                                      where !texture2D.name.StartsWith("replaced_")
                                      select texture2D)
            {
                TryReplaceTexture2D(texture2D);
            }
        }

        public static IEnumerator ReplaceTexturesCoroutine()
        {
            for (; ; )
            {
                ReplaceTextures();
                yield return new WaitForSeconds(0.5f);
            }
        }

        public static bool TryReplaceTexture2D(Texture2D ogTexture)
        {
            if (ogTexture is not null)
            {
                if (TextureDict.TryGetValue(ogTexture.name, out var text))
                {
                    try
                    {
                        ImageConversion.LoadImage(ogTexture, File.ReadAllBytes(text));
                        ogTexture.name = "replaced_" + ogTexture.name;
                        return true;
                    }
                    catch (Exception ex)
                    {
                        CustomCore.Instance.Value.Log.LogError("Failed to replace texture: " + ogTexture.name + " at path: " + text);
                        CustomCore.Instance.Value.Log.LogError(ex);
                    }
                }
            }
            return false;
        }

        public static Dictionary<string, string> TextureDict { get; set; } = [];
    }
}
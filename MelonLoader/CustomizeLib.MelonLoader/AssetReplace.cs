using MelonLoader;
using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.Rendering;

namespace CustomizeLib.MelonLoader
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
            if (!Directory.Exists(Path.Combine("Mods", "Textures")))
            {
                Directory.CreateDirectory(Path.Combine("Mods", "Textures"));
            }
            try
            {
                foreach (string text3 in Directory.EnumerateFiles(Path.Combine("Mods", "Textures"), "*.png", SearchOption.AllDirectories))
                {
                    LoadImage(text3);
                    TextureDict[Path.GetFileNameWithoutExtension(text3)] = text3;
                }
            }
            catch (Exception ex2)
            {
                MelonLogger.Error("Error loading Texture.");
                MelonLogger.Error(ex2);
                return;
            }
            MelonLogger.Msg("Textures loaded successfully.");
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
                        MelonLogger.Error("Failed to replace texture: " + ogTexture.name + " at path: " + text);
                        MelonLogger.Error(ex);
                    }
                }
            }
            return false;
        }

        public static Dictionary<string, string> TextureDict { get; set; } = [];
    }
}
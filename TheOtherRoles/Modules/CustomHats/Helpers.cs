using System.IO;
using UnityEngine;

namespace TheOtherRoles.Modules.CustomHats;

internal static class Helpers
{
    public static Texture2D LoadTextureFromPath(string path)
    {
        if (!File.Exists(path)) return null;
        var texture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
        try
        {
            var byteTexture = Il2CppSystem.IO.File.ReadAllBytes(path);
            ImageConversion.LoadImage(texture, byteTexture, false);
        }
        catch
        {
            TheOtherRolesPlugin.Logger.LogError("Error loading texture from disk: " + path);
            return null;
        }

        return texture;
    }
}
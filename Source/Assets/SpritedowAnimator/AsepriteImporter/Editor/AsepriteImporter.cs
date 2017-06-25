// Spritedow Animation Plugin by Elendow
// http://elendow.com

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Elendow.SpritedowAnimator
{
    public class AsepriteImporter : AssetPostprocessor
    {
        public const string MAKE_ANIM_TAG = "make_anim";

        // This list contains the more common file extensions supported by Aseprite AND Unity. 
        // If you need another file extension, contact me.
        public static string[] asepriteTextureExtensions = { "bmp", "gif", "jpeg", "jpg", "png", "tga" };

        public override int GetPostprocessOrder()
        {
            return 1;
        }

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            List<TextureWithPath> textureList = new List<TextureWithPath>();
            List<AsepriteJson> asepriteJsonList = new List<AsepriteJson>();

            // Look every file imported, we need the textures and jsons
            foreach (string assetName in importedAssets)
            {
                if (assetName.Contains(".json"))
                {
                    TextAsset jsonAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetName);

                    if (jsonAsset != null)
                    {
                        // Make sure the json file is from aseprite
                        if (jsonAsset.text.Contains("http://www.aseprite.org/"))
                        {
                            AsepriteJson asepriteObject = JsonUtility.FromJson<AsepriteJson>(jsonAsset.text);
                            asepriteJsonList.Add(asepriteObject);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < asepriteTextureExtensions.Length; i++)
                    {
                        // Here we look if this file is a texture, looking for the supported extensions
                        if (assetName.Contains(asepriteTextureExtensions[i]))
                        {
                            Texture2D textureAsset = AssetDatabase.LoadAssetAtPath<Texture2D>(assetName);
                            if (textureAsset != null)
                                textureList.Add(new TextureWithPath(assetName, textureAsset));
                        }
                    }
                }
            }

            // We've got some textures and json
            if (textureList.Count > 0 && asepriteJsonList.Count > 0)
            {
                for (int i = 0; i < asepriteJsonList.Count; i++)
                {
                    string textureName = asepriteJsonList[i].meta.TextureName();
                    TextureWithPath texture = textureList.Find(x => x.path.Contains(textureName));
                    if (texture != null)
                    {
                        TextureImporter importer = AssetImporter.GetAtPath(texture.path) as TextureImporter;
                        if (importer != null)
                        {
                            importer.textureType = TextureImporterType.Sprite;
                            importer.spriteImportMode = SpriteImportMode.Multiple;
                            importer.userData = MAKE_ANIM_TAG;
                            List<SpriteMetaData> spriteList = new List<SpriteMetaData>();
                            for (int j = 0; j < asepriteJsonList[i].frames.Length; j++)
                            {
                                AsepriteFrame frame = asepriteJsonList[i].frames[j];
                                SpriteMetaData frameData = new SpriteMetaData();
                                frameData.name = frame.filename.Replace(".ase", "");
                                frameData.rect = frame.frame.ToRect();
                                spriteList.Add(frameData);
                            }
                            importer.spritesheet = spriteList.ToArray();
                            importer.SaveAndReimport();
                        }
                    }
                }
            }
        }

        private class TextureWithPath
        {
            public string path;
            public Texture2D texture;

            public TextureWithPath(string p, Texture2D t)
            {
                path = p;
                texture = t;
            }
        }
    }
}
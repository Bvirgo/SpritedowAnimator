// Spritedow Animation Plugin by Elendow
// http://elendow.com

using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

namespace Elendow.SpritedowAnimator
{
    public class AsepriteAnimationCreator : AssetPostprocessor
    {
        public override int GetPostprocessOrder()
        {
            return 10;
        }

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string assetName in importedAssets)
            {
                for (int i = 0; i < AsepriteImporter.asepriteTextureExtensions.Length; i++)
                {
                    if (assetName.Contains(AsepriteImporter.asepriteTextureExtensions[i]))
                    {
                        TextureImporter importer = AssetImporter.GetAtPath(assetName) as TextureImporter;
                        if (importer != null && importer.userData == AsepriteImporter.MAKE_ANIM_TAG)
                        {
                            Texture2D t = AssetDatabase.LoadAssetAtPath<Texture2D>(assetName);
                            importer.userData = "";
                            string[] path = assetName.Split(new string[] { t.name }, StringSplitOptions.RemoveEmptyEntries);
                            CreateAnimation(t.name, path[0], assetName);
                        }
                    }
                }
            }
        }

        private static void CreateAnimation(string fileName, string path, string texturePath)
        {
            SpriteAnimation asset = ScriptableObject.CreateInstance<SpriteAnimation>();
            asset.Name = fileName;

            AssetDatabase.CreateAsset(asset, path + fileName + ".asset");

            Sprite[] spritesInTexture = AssetDatabase.LoadAllAssetsAtPath(texturePath).OfType<Sprite>().ToArray();
            for (int i = 0; i < spritesInTexture.Length; i++)
            {
                asset.Frames.Add(spritesInTexture[i]);
                asset.FramesDuration.Add(1);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
// Spritedow Animation Plugin by Elendow
// http://elendow.com

using UnityEngine;
using System;
using System.Collections.Generic;

namespace Elendow.SpritedowAnimator
{
    [Serializable]
    public class AsepriteJson
    {
        public AsepriteMeta meta;
        public AsepriteFrame[] frames;
    }

    [Serializable]
    public struct AsepriteFrame
    {
        public string filename;
        public AsepriteRect frame;
        public bool rotated;
        public bool trimmed;
        public AsepriteSize spriteSourceSize;
        public AsepriteSize sourceSize;
        public float duration;
    }

    [Serializable]
    public struct AsepriteRect
    {
        public float x;
        public float y;
        public float w;
        public float h;

        public Rect ToRect()
        {
            return new Rect(x, y, w, h);
        }
    }

    [Serializable]
    public struct AsepriteSize
    {
        public float w;
        public float h;
    }

    [Serializable]
    public struct AsepriteMeta
    {
        public string app;
        public string version;
        public string image;
        public string format;
        public AsepriteSize size;
        public float scale;

        public string TextureName()
        {
            char[] c = new char[1];
            if (image.Contains("/"))
                c[0] = '/';
            else if(image.Contains("\\"))
                c[0] = '\\';

            string[] pathSplit = image.Split(c);
            if (pathSplit.Length > 0)
                return pathSplit[pathSplit.Length - 1];
            else
                return "";
        }
    }

}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.Xna.Framework.Graphics;

namespace DeferredLightingAndroid
{
    public class GBufferEffect : Effect
    {
        EffectParameter textureParam;
        EffectParameter normalParam;
        EffectParameter specularParam;

        public Texture2D Texture
        {
            get { return textureParam.GetValueTexture2D(); }
            set { textureParam.SetValue(value); }
        }

        public Texture2D Normal
        {
            get { return normalParam.GetValueTexture2D(); }
            set { normalParam.SetValue(value); }
        }

        public Texture2D Specular
        {
            get { return specularParam.GetValueTexture2D(); }
            set { specularParam.SetValue(value); }
        }

        static readonly byte[] Bytecode = LoadEffect();//LoadEffectResource("DeferredLighting.Resources.RenderGBuffer.dx11.mgfxo");

        private static byte[] LoadEffect()
        {
            string pathToEffects = $"Data{Path.DirectorySeparatorChar}Effects{Path.DirectorySeparatorChar}";
            string effectPath = pathToEffects + "RenderGBuffer.mgfx";
            var folderPath = effectPath.Substring(0, effectPath.LastIndexOf('/') + 1);
            var filePath = effectPath.Substring(effectPath.LastIndexOf('/') + 1);
            return GetFileBytes(folderPath, filePath);
        }
        public static byte[] GetFileBytes(string path, string fileName)
        {
            var bytes = default(byte[]);
            using (StreamReader reader = new StreamReader(Game1.AssetManager.Open(Path.Combine(path, fileName))))
            {
                using (var memstream = new MemoryStream())
                {
                    reader.BaseStream.CopyTo(memstream);
                    bytes = memstream.ToArray();
                }
            }
            return bytes;
        }
        static byte[] LoadEffectResource(string name)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Creates a new BasicEffect with default parameter settings.
        /// </summary>
        public GBufferEffect(GraphicsDevice device) : base(device, Bytecode)
        {
            CacheEffectParameters(null);
        }

        void CacheEffectParameters(GBufferEffect cloneSource)
        {
            textureParam = Parameters["Texture"];
            normalParam = Parameters["NormalMap"];
            specularParam = Parameters["SpecularMap"];
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DeferredLightingAndroid
{
    class GBufferEffectReader : ContentTypeReader<GBufferEffect>
    {
        protected override GBufferEffect Read(ContentReader input, GBufferEffect existingInstance)
        {
            IGraphicsDeviceService service = input.ContentManager.ServiceProvider.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;
            var effect = new GBufferEffect(service.GraphicsDevice);

            //var texture = input.ReadExternalReference<Texture>() as Texture2D;
            //if (texture != null)
            //{
            //    effect.Texture = texture;
            //}

            //texture = input.ReadExternalReference<Texture>() as Texture2D;
            //if (texture != null)
            //{
                //effect.Normal = texture;
            //}

            //texture = input.ReadExternalReference<Texture>() as Texture2D;
            //if (texture != null)
            //{
                //effect.Specular = texture;
            //}

            return effect;
        }
    }
}

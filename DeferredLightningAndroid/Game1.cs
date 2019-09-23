using Android.Content.Res;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;

namespace DeferredLightingAndroid
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public static AssetManager AssetManager;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Camera camera;
        private QuadRenderComponent quadRenderer;
        private Scene scene;

        private RenderTarget2D colorRT; //color and specular intensity
        private RenderTarget2D normalRT; //normals + specular power
        private RenderTarget2D depthRT; //depth
        private RenderTarget2D lightRT; //lighting

        private Effect clearBufferEffect;
        private Effect directionalLightEffect;

        private Effect pointLightEffect;
        private Effect renderGBufferEffect;
        //public static Effect RenderGBufferEffect
        //{
        //    get { return renderGBufferEffect; }
        //}
        private Model sphereModel; //point ligt volume

        private Effect finalCombineEffect;

        private Vector2 halfPixel;

        public Game1(AssetManager assetManager)
        {
            AssetManager = assetManager;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;

            scene = new Scene(this);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            camera = new Camera(this);
            camera.CameraArc = -30;
            camera.CameraDistance = 50;
            quadRenderer = new QuadRenderComponent(this);
            this.Components.Add(camera);
            this.Components.Add(quadRenderer);

            base.Initialize();

            
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {


            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            halfPixel = new Vector2()
            {
                X = 0.5f / (float)GraphicsDevice.PresentationParameters.BackBufferWidth,
                Y = 0.5f / (float)GraphicsDevice.PresentationParameters.BackBufferHeight
            };

            int backbufferWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            int backbufferHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

            colorRT = new RenderTarget2D(GraphicsDevice, backbufferWidth, backbufferHeight, false, SurfaceFormat.Color, DepthFormat.Depth24);
            normalRT = new RenderTarget2D(GraphicsDevice, backbufferWidth, backbufferHeight, false, SurfaceFormat.Color, DepthFormat.None);
            depthRT = new RenderTarget2D(GraphicsDevice, backbufferWidth, backbufferHeight, false, SurfaceFormat.Single, DepthFormat.None);
            lightRT = new RenderTarget2D(GraphicsDevice, backbufferWidth, backbufferHeight, false, SurfaceFormat.Color, DepthFormat.None);

            scene.InitializeScene(graphics.GraphicsDevice);

            clearBufferEffect = LoadEffect("ClearGBuffer.mgfx", Content, GraphicsDevice);
            directionalLightEffect = LoadEffect("DirectionalLight.mgfx", Content, GraphicsDevice); //this.Content.Load<Effect>("DirectionalLight");
            finalCombineEffect = LoadEffect("CombineFinal.mgfx", Content, GraphicsDevice); //this.Content.Load<Effect>("CombineFinal");
            pointLightEffect = LoadEffect("PointLight.mgfx", Content, GraphicsDevice); //this.Content.Load<Effect>("PointLight");
            renderGBufferEffect = LoadEffect("RenderGBuffer.mgfx", Content, GraphicsDevice, out RenderHBufferEffectBytes); //this.Content.Load<Effect>("PointLight");
            sphereModel = this.Content.Load<Model>(@"Data\Models\sphere");

            scene.OverwriteEffects(renderGBufferEffect);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            base.LoadContent();
        }
        public static byte[] RenderHBufferEffectBytes;
        public Effect LoadEffect(string effectFileName, ContentManager content, GraphicsDevice device)
        {
            string pathToEffects = $"Data{Path.DirectorySeparatorChar}Effects{Path.DirectorySeparatorChar}";
            string effectPath = pathToEffects + effectFileName;
            var folderPath = effectPath.Substring(0, effectPath.LastIndexOf('/') + 1);
            var filePath = effectPath.Substring(effectPath.LastIndexOf('/') + 1);
            byte[] bytecode = GetFileBytes(folderPath, filePath);
            return new Effect(device, bytecode);
        }
        public Effect LoadEffect(string effectFileName, ContentManager content, GraphicsDevice device, out byte[] bytecode)
        {
            string pathToEffects = $"Data{Path.DirectorySeparatorChar}Effects{Path.DirectorySeparatorChar}";
            string effectPath = pathToEffects + effectFileName;
            var folderPath = effectPath.Substring(0, effectPath.LastIndexOf('/') + 1);
            var filePath = effectPath.Substring(effectPath.LastIndexOf('/') + 1);
            bytecode = GetFileBytes(folderPath, filePath);
            return new Effect(device, bytecode);
        }
        public byte[] GetFileBytes(string path, string fileName)
        {
            var bytes = default(byte[]);
            using (StreamReader reader = new StreamReader(AssetManager.Open(Path.Combine(path, fileName))))
            {
                using (var memstream = new MemoryStream())
                {
                    reader.BaseStream.CopyTo(memstream);
                    bytes = memstream.ToArray();
                }
            }
            return bytes;
        }
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        //MG on Android does not support setting multiple render targets
        //private void SetGBuffer()
        //{
        //    GraphicsDevice.SetRenderTargets(colorRT, normalRT, depthRT);
        //}
        private void SetGBuffer()
        {
            GraphicsDevice.SetRenderTargets(colorRT, normalRT, depthRT);
        }

        private void ResolveGBuffer()
        {
            GraphicsDevice.SetRenderTarget(null);
        }

        private void ClearGBuffer()
        {
            clearBufferEffect.Techniques[0].Passes[0].Apply();
            quadRenderer.Render(Vector2.One * -1, Vector2.One);
        }

        private void DrawDirectionalLight(Vector3 lightDirection, Color color)
        {
            directionalLightEffect.Parameters["colorMap"].SetValue(colorRT);
            directionalLightEffect.Parameters["normalMap"].SetValue(normalRT);
            directionalLightEffect.Parameters["depthMap"].SetValue(depthRT);

            directionalLightEffect.Parameters["lightDirection"].SetValue(lightDirection);
            directionalLightEffect.Parameters["Color"].SetValue(color.ToVector3());

            directionalLightEffect.Parameters["cameraPosition"].SetValue(camera.Position);
            directionalLightEffect.Parameters["InvertViewProjection"].SetValue(Matrix.Invert(camera.View * camera.Projection));

            directionalLightEffect.Parameters["halfPixel"].SetValue(halfPixel);

            directionalLightEffect.Techniques[0].Passes[0].Apply();
            quadRenderer.Render(Vector2.One * -1, Vector2.One);
        }

        private void DrawPointLight(Vector3 lightPosition, Color color, float lightRadius, float lightIntensity)
        {
            //set the G-Buffer parameters
            pointLightEffect.Parameters["colorMap"].SetValue(colorRT);
            pointLightEffect.Parameters["normalMap"].SetValue(normalRT);
            pointLightEffect.Parameters["depthMap"].SetValue(depthRT);

            //compute the light world matrix
            //scale according to light radius, and translate it to light position
            Matrix sphereWorldMatrix = Matrix.CreateScale(lightRadius) * Matrix.CreateTranslation(lightPosition);
            pointLightEffect.Parameters["World"].SetValue(sphereWorldMatrix);
            pointLightEffect.Parameters["View"].SetValue(camera.View);
            pointLightEffect.Parameters["Projection"].SetValue(camera.Projection);
            //light position
            pointLightEffect.Parameters["lightPosition"].SetValue(lightPosition);

            //set the color, radius and Intensity
            pointLightEffect.Parameters["Color"].SetValue(color.ToVector3());
            pointLightEffect.Parameters["lightRadius"].SetValue(lightRadius);
            pointLightEffect.Parameters["lightIntensity"].SetValue(lightIntensity);

            //parameters for specular computations
            pointLightEffect.Parameters["cameraPosition"].SetValue(camera.Position);
            pointLightEffect.Parameters["InvertViewProjection"].SetValue(Matrix.Invert(camera.View * camera.Projection));
            //size of a halfpixel, for texture coordinates alignment
            pointLightEffect.Parameters["halfPixel"].SetValue(halfPixel);
            //calculate the distance between the camera and light center
            float cameraToCenter = Vector3.Distance(camera.Position, lightPosition);
            //if we are inside the light volume, draw the sphere's inside face
            if (cameraToCenter < lightRadius)
                GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
            else
                GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            GraphicsDevice.DepthStencilState = DepthStencilState.None;

            pointLightEffect.Techniques[0].Passes[0].Apply();
            foreach (ModelMesh mesh in sphereModel.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    GraphicsDevice.Indices = meshPart.IndexBuffer;
                    GraphicsDevice.SetVertexBuffer(meshPart.VertexBuffer);

                    GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, meshPart.NumVertices, meshPart.StartIndex, meshPart.PrimitiveCount);
                }
            }

            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            //SetGBuffer();
            //ClearGBuffer();
            //scene.DrawScene(camera, gameTime);
            //ResolveGBuffer();

            //GraphicsDevice.SetRenderTargets(colorRT, normalRT, depthRT);
            GraphicsDevice.SetRenderTargets(colorRT);
            ClearGBuffer();
            scene.DrawScene(camera, gameTime, "Color");
            ResolveGBuffer();
            GraphicsDevice.SetRenderTargets(normalRT);
            ClearGBuffer();
            scene.DrawScene(camera, gameTime, "Normal");
            ResolveGBuffer();
            GraphicsDevice.SetRenderTargets(depthRT);
            ClearGBuffer();
            scene.DrawScene(camera, gameTime, "Depth");
            ResolveGBuffer();

            DrawLights(gameTime);

            base.Draw(gameTime);
        }

        private void DrawLights(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(lightRT);
            GraphicsDevice.Clear(Color.Transparent);
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = DepthStencilState.None;

            Color[] colors = new Color[10];
            colors[0] = Color.Red; colors[1] = Color.Blue;
            colors[2] = Color.IndianRed; colors[3] = Color.CornflowerBlue;
            colors[4] = Color.Gold; colors[5] = Color.Green;
            colors[6] = Color.Crimson; colors[7] = Color.SkyBlue;
            colors[8] = Color.Red; colors[9] = Color.ForestGreen;
            float angle = (float)gameTime.TotalGameTime.TotalSeconds;
            int n = 15;

            for (int i = 0; i < n; i++)
            {
                Vector3 pos = new Vector3((float)Math.Sin(i * MathHelper.TwoPi / n + angle), 0.30f, (float)Math.Cos(i * MathHelper.TwoPi / n + angle));
                DrawPointLight(pos * 40, colors[i % 10], 15, 2);
                pos = new Vector3((float)Math.Cos((i + 5) * MathHelper.TwoPi / n - angle), 0.30f, (float)Math.Sin((i + 5) * MathHelper.TwoPi / n - angle));
                DrawPointLight(pos * 20, colors[i % 10], 20, 1);
                pos = new Vector3((float)Math.Cos(i * MathHelper.TwoPi / n + angle), 0.10f, (float)Math.Sin(i * MathHelper.TwoPi / n + angle));
                DrawPointLight(pos * 75, colors[i % 10], 45, 2);
                pos = new Vector3((float)Math.Cos(i * MathHelper.TwoPi / n + angle), -0.3f, (float)Math.Sin(i * MathHelper.TwoPi / n + angle));
                DrawPointLight(pos * 20, colors[i % 10], 20, 2);
            }

            DrawPointLight(new Vector3(0, (float)Math.Sin(angle * 0.8) * 40, 0), Color.Red, 30, 5);
            DrawPointLight(new Vector3(0, 25, 0), Color.White, 30, 1);
            DrawPointLight(new Vector3(0, 0, 70), Color.Wheat, 55 + 10 * (float)Math.Sin(5 * angle), 3);

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            GraphicsDevice.SetRenderTarget(null);

            //Combine everything
            finalCombineEffect.Parameters["colorMap"].SetValue(colorRT);
            finalCombineEffect.Parameters["lightMap"].SetValue(lightRT);
            finalCombineEffect.Parameters["halfPixel"].SetValue(halfPixel);

            finalCombineEffect.Techniques[0].Passes[0].Apply();
            quadRenderer.Render(Vector2.One * -1, Vector2.One);

            //Output FPS and 'credits'
            double fps = (1000 / gameTime.ElapsedGameTime.TotalMilliseconds);
            fps = Math.Round(fps, 0);
            this.Window.Title = "Deferred Rendering by Catalin Zima, converted to XNA4 by Roy Triesscheijn. Drawing " + (n * 4 + 3) + " lights at " + fps.ToString() + " FPS";
        }
    }
}

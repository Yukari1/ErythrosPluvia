using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Erythros_Pluvia
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Main : Game
    {
        public static GraphicsDeviceManager Graphics { get; private set; }
        public static SpriteBatch SpriteBatch { get; private set; }
        public static Microsoft.Xna.Framework.Content.ContentManager ContentManager { get; private set; }

        public static Effect RenderingEffect { get; private set; }

        RenderTarget2D RenderTarget;

        public Main()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            ContentManager = Content;

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        private float consoleEdgeSharpness = 8.0f;
        private float consoleEdgeThreshold = 0.125f;
        private float consoleEdgeThresholdMin = 0.05f;
        private float fxaaQualitySubpix = 0.75f;
        private float fxaaQualityEdgeThreshold = 0.166f;
        private float fxaaQualityEdgeThresholdMin = 0.0833f;

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            RenderingEffect = Content.Load<Effect>("Shaders/FXAA");

            RenderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            float w = GraphicsDevice.Viewport.Width;
            float h = GraphicsDevice.Viewport.Height;

            bool useQuality = true;

            if (!useQuality)
            {
                RenderingEffect.CurrentTechnique = RenderingEffect.Techniques["ppfxaa_Console"];
                RenderingEffect.Parameters["ConsoleOpt1"].SetValue(new Vector4(-2.0f / w, -2.0f / h, 2.0f / w, 2.0f / h));
                RenderingEffect.Parameters["ConsoleOpt2"].SetValue(new Vector4(8.0f / w, 8.0f / h, -4.0f / w, -4.0f / h));
                RenderingEffect.Parameters["ConsoleEdgeSharpness"].SetValue(consoleEdgeSharpness);
                RenderingEffect.Parameters["ConsoleEdgeThreshold"].SetValue(consoleEdgeThreshold);
                RenderingEffect.Parameters["ConsoleEdgeThresholdMin"].SetValue(consoleEdgeThresholdMin);
            }
            else
            {
                RenderingEffect.CurrentTechnique = RenderingEffect.Techniques["ppfxaa_PC"];
                RenderingEffect.Parameters["fxaaQualitySubpix"].SetValue(fxaaQualitySubpix);
                RenderingEffect.Parameters["fxaaQualityEdgeThreshold"].SetValue(fxaaQualityEdgeThreshold);
                RenderingEffect.Parameters["fxaaQualityEdgeThresholdMin"].SetValue(fxaaQualityEdgeThresholdMin);
            }

            RenderingEffect.Parameters["invViewportWidth"].SetValue(1f / w);
            RenderingEffect.Parameters["invViewportHeight"].SetValue(1f / h);
            RenderingEffect.Parameters["texScreen"].SetValue((Texture2D)RenderTarget);

            Util.SceneManager.CurrentScene = new Scenes.SceneTitle();
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
            Util.SceneManager.CurrentScene.OnUpdate(gameTime);

            if (Util.SceneManager.CurrentScene == null)
            {
                Exit();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(RenderTarget);

            GraphicsDevice.Clear(Color.Black);

            Util.SceneManager.CurrentScene.OnDraw(gameTime);

            GraphicsDevice.SetRenderTarget(null);

            SpriteBatch.Begin();
            SpriteBatch.Draw((Texture2D)RenderTarget, Vector2.Zero, Color.White);
            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

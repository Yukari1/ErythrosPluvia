using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;

using Erythros_Pluvia.Input;
using Erythros_Pluvia.Util;

namespace Erythros_Pluvia.Scenes
{
    class SceneLevel : IScene
    {

        #region Fields

        // name of the content file the map is stored in
        protected String mapAssetName;

        // object containing all the data needed to render the Tiled map
        protected TiledMap map;

        // the map renderer
        protected IMapRenderer mapRenderer;

        // the Matrix for the user's current viewport
        protected Matrix viewportMatrix;

        #endregion

        #region Methods

        public SceneLevel(String mapAssetName)
        {
            this.mapAssetName = mapAssetName;
        }

        public override void OnStart()
        {
            map = Content.Load<TiledMap>(mapAssetName);
            mapRenderer = new FullMapRenderer(GraphicsDevice);
            mapRenderer.SwapMap(map);
            viewportMatrix = Matrix.Identity;

            // for now, make sure there's an easy way to exit the game. We'll do more fancy stuff later
            Command exitGame = delegate
            {
                SceneManager.CurrentScene = null;
            };

            WindowsInputManager windowsInputManager = new WindowsInputManager();
            windowsInputManager.addKeyPressBinding(Keys.Escape, exitGame);

            InputManager = windowsInputManager;
        }

        public override void OnUpdate(GameTime time)
        {
            InputManager.executeCommands(time);

            mapRenderer.Update(time);
        }

        public override void OnDraw(GameTime time)
        {
            base.OnDraw(time);

            mapRenderer.Draw(viewportMatrix);
        }

        #endregion
    }
}

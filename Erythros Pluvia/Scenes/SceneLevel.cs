using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended.Maps.Tiled;

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

        #endregion

        #region Methods

        public SceneLevel(String mapAssetName)
        {
            this.mapAssetName = mapAssetName;
        }

        public override void OnStart()
        {
            this.map = Content.Load<TiledMap>(mapAssetName);

            // for now, make sure there's an easy way to exit the game. We'll do more fancy stuff later
            Command exitGame = delegate
            {
                SceneManager.CurrentScene = null;
            };

            WindowsInputManager windowsInputManager = new WindowsInputManager();
            windowsInputManager.addKeyPressBinding(Keys.Escape, exitGame);

            InputManager = windowsInputManager;
        }

        #endregion
    }
}

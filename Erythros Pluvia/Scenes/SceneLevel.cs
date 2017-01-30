using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;

using Erythros_Pluvia.Entities;
using Erythros_Pluvia.Input;
using Erythros_Pluvia.Util;

namespace Erythros_Pluvia.Scenes
{
    public class SceneLevel : IScene
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

        // the X coordinate of the tile the player will start on
        protected int playerStartTileX;

        // the Y coordinate of the tile the player will start on
        protected int playerStartTileY;

        // the camera tracking the player
        protected Camera2D camera;

        // the player entity
        protected PlayerEntity player;

        // the player's movement speed
        protected float playerMovementSpeed;

        #endregion

        #region Properties

        /// <summary>
        /// The entity representing the playable character in the scene
        /// </summary>
        public PlayerEntity Player
        {
            get { return player; }
        }

        /// <summary>
        /// Object containing all the data necessary for 2D camera transformations
        /// </summary>
        public Camera2D Camera
        {
            get { return camera; }
        }



        #endregion

        #region Methods

        /// <summary>
        /// Construct a new instance of SceneLevel
        /// </summary>
        /// <param name="mapAssetName">The name of the map Content file</param>
        /// <param name="playerStartTileX">The player's starting tile X</param>
        /// <param name="playerStartTileY">The player's starting tile Y</param>
        public SceneLevel(String mapAssetName, int playerStartTileX, int playerStartTileY)
        {
            this.mapAssetName = mapAssetName;
            this.playerStartTileX = playerStartTileX;
            this.playerStartTileY = playerStartTileY;
        }

        public override void OnStart()
        {

            // load the map
            map = Content.Load<TiledMap>(mapAssetName);
            mapRenderer = new FullMapRenderer(GraphicsDevice);
            mapRenderer.SwapMap(map);
            viewportMatrix = Matrix.Identity;
            camera = new Camera2D(GraphicsDevice);

            // initialize the player entity
            Texture2D playerTexture = Content.Load<Texture2D>("Sprites/Player/Player");
            Sprite sprite = new Sprite(playerTexture, 0, 0);
            player = new PlayerEntity(playerStartTileX * map.TileWidth, playerStartTileY * map.TileHeight, sprite);
            playerMovementSpeed = 5.0f;

            // for now, make sure there's an easy way to exit the game. We'll do more fancy stuff later
            Command exitGame = delegate
            {
                SceneManager.CurrentScene = null;
            };
            WindowsInputManager windowsInputManager = new WindowsInputManager();
            windowsInputManager.addKeyPressBinding(Keys.Escape, exitGame);

            // while we're at it, let's enable some movement, for debugging/testing purposes
            Command movePlayerUpCommand = delegate
            {
                this._movePlayerUp();
            };
            windowsInputManager.addKeyPressBinding(Keys.W, movePlayerUpCommand);

            Command movePlayerRightCommand = delegate
            {
                this._movePlayerRight();
            };
            windowsInputManager.addKeyPressBinding(Keys.D, movePlayerRightCommand);

            Command movePlayerDownCommand = delegate
            {
                this._movePlayerDown();
            };
            windowsInputManager.addKeyPressBinding(Keys.S, movePlayerDownCommand);

            Command movePlayerLeftCommand = delegate
            {
                this._movePlayerLeft();
            };
            windowsInputManager.addKeyPressBinding(Keys.A, movePlayerLeftCommand);

            Command stopPlayerMovement = delegate
            {
                this._stopPlayerMovement();
            };
            windowsInputManager.addKeyReleaseBinding(Keys.W, stopPlayerMovement);
            windowsInputManager.addKeyReleaseBinding(Keys.D, stopPlayerMovement);
            windowsInputManager.addKeyReleaseBinding(Keys.S, stopPlayerMovement);
            windowsInputManager.addKeyReleaseBinding(Keys.A, stopPlayerMovement);

            InputManager = windowsInputManager;

            _updateDisplay();
        }

        public override void OnUpdate(GameTime time)
        {
            InputManager.executeCommands(time);

            player.EndUpdate(time);

            _updateDisplay();

            mapRenderer.Update(time);
        }

        public override void OnDraw(GameTime time)
        {
            SpriteBatch.Begin();

            base.OnDraw(time);

            mapRenderer.Draw(camera.GetViewMatrix());

            player.Draw(SpriteBatch);

            SpriteBatch.End();
        }

        private void _movePlayerUp()
        {
            Vector2 playerVelocity = player.Velocity;

            playerVelocity.X = 0.0f;
            playerVelocity.Y = -playerMovementSpeed;

            player.Velocity = playerVelocity;
        }

        private void _movePlayerRight()
        {
            Vector2 playerVelocity = player.Velocity;

            playerVelocity.X = playerMovementSpeed;
            playerVelocity.Y = 0.0f;

            player.Velocity = playerVelocity;
        }

        private void _movePlayerDown()
        {
            Vector2 playerVelocity = player.Velocity;

            playerVelocity.X = 0.0f;
            playerVelocity.Y = playerMovementSpeed;

            player.Velocity = playerVelocity;
        }

        private void _movePlayerLeft()
        {
            Vector2 playerVelocity = player.Velocity;

            playerVelocity.X = -playerMovementSpeed;
            playerVelocity.Y = 0.0f;

            player.Velocity = playerVelocity;
        }

        private void _stopPlayerMovement()
        {
            Vector2 playerVelocity = player.Velocity;

            playerVelocity.X = 0.0f;
            playerVelocity.Y = 0.0f;

            player.Velocity = playerVelocity;
        }

        private void _updateDisplay()
        {
            Vector2 cameraPosition = camera.Position;
            cameraPosition.X = this._getUpdatedCameraPosition(player.Position.X, map.WidthInPixels, GraphicsDevice.Viewport.Width, player.Sprite.Width);
            cameraPosition.Y = this._getUpdatedCameraPosition(player.Position.Y, map.HeightInPixels, GraphicsDevice.Viewport.Height, player.Sprite.Height);
            camera.Position = cameraPosition;


            Point spritePosition = player.Sprite.Position;
            spritePosition.X = this._getUpdatedSpritePosition(player.Position.X, map.WidthInPixels, GraphicsDevice.Viewport.Width, player.Sprite.Width);
            spritePosition.Y = this._getUpdatedSpritePosition(player.Position.Y, map.HeightInPixels, GraphicsDevice.Viewport.Height, player.Sprite.Height);
            player.Sprite.Position = spritePosition;
        }

        private float _getUpdatedCameraPosition(float playerWorldPosition, int mapSize, int viewportSize, int spriteSize)
        {
            return MathHelper.Clamp(playerWorldPosition - (viewportSize / 2) - (spriteSize / 2), 0, mapSize - viewportSize);
        }

        private int _getUpdatedSpritePosition(float playerWorldPosition, int mapSize, int viewportSize, int spriteSize)
        {
            if (playerWorldPosition >= 0.0f && playerWorldPosition < (viewportSize / 2 - spriteSize / 2))
            {
                return (int)playerWorldPosition;
            }
            else if (playerWorldPosition >= (viewportSize / 2 - spriteSize / 2) && playerWorldPosition < (mapSize - (viewportSize / 2 + spriteSize / 2)))
            {
                return viewportSize / 2 - spriteSize / 2;
            }
            else
            {
                return (int)playerWorldPosition + viewportSize - mapSize;
            }
        }

        #endregion
    }
}
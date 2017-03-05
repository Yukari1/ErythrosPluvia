using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Erythros_Pluvia.Util
{
    public class Sprite
    {
        #region Fields

        /// <summary>
        /// The bounding box contains the x and y position of the Sprite, plus the size of the sprite on screen
        /// </summary>
        private Microsoft.Xna.Framework.Rectangle _boundingBox;

        #endregion

        #region Properties

        /// <summary>
        /// The texture to use for this sprite
        /// </summary>
        public Texture2D Texture { get; set; }

        /// <summary>
        /// The depth at which to draw the sprite
        /// </summary>
        public float Depth { get; set; }

        /// <summary>
        /// The bounding box for the sprite
        /// </summary>
        public Microsoft.Xna.Framework.Rectangle BoundingBox
        {
            get
            {
                return _boundingBox;
            }

            set
            {
                _boundingBox = value;
            }
        }

        /// <summary>
        /// Gets/Sets the position of the Sprite
        /// </summary>
        public Point Position
        {
            get
            {
                return new Point(_boundingBox.X, _boundingBox.Y);
            }

            set
            {
                _boundingBox.X = value.X;
                _boundingBox.Y = value.Y;
            }
        }

        /// <summary>
        /// Gets/Sets the width of the Sprite
        /// </summary>
        public int Width
        {
            get
            {
                return _boundingBox.Width;
            }

            set
            {
                _boundingBox.Width = value;
            }
        }

        /// <summary>
        /// Gets/Sets the height of the Sprite
        /// </summary>
        public int Height
        {
            get
            {
                return _boundingBox.Height;
            }

            set
            {
                _boundingBox.Height = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Construct a new instance of Sprite
        /// </summary>
        /// <param name="texture">The texture the sprite will use</param>
        /// <param name="x">The x position of the sprite on the display window</param>
        /// <param name="y">The y position of the sprite on the display window</param>
        /// <param name="depth">The depth at which to draw the sprite</param>
        public Sprite(Texture2D texture, int x, int y, float depth = 0.0f)
        {
            this.Texture = texture;
            this._boundingBox = new Microsoft.Xna.Framework.Rectangle(x, y, texture.Width, texture.Height);
            this.Depth = depth;
        }

        /// <summary>
        /// Construct a new instance of Sprite
        /// </summary>
        /// <param name="texture">The texture the sprite will use</param>
        /// <param name="x">The x position of the sprite on the display window</param>
        /// <param name="y">The y position of the sprite on the display window</param>
        /// <param name="width">The width of the sprite</param>
        /// <param name="height">The height of the sprite</param>
        /// <param name="depth">The depth at which to draw the sprite</param>
        public Sprite(Texture2D texture, int x, int y, int width, int height, float depth = 0.0f)
        {
            this.Texture = texture;
            this._boundingBox = new Microsoft.Xna.Framework.Rectangle(x, y, width, height);
            this.Depth = depth;
        }

        /// <summary>
        /// Draw the sprite to the sprite batch
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to which the sprite will be drawn</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, _boundingBox, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, Depth);
        }

        #endregion
    }
}

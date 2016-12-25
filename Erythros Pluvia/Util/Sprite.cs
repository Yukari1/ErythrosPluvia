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
        public Rectangle BoundingBox;

        /// <summary>
        /// The source inside the Texture to draw
        /// </summary>
        public Rectangle Source;

        /// <summary>
        /// A List of all the Textures for the sprite
        /// </summary>
        public Texture2D[] Textures;

        /// <summary>
        /// The ID of the Texture to draw
        /// </summary>
        public int TextureID = 0;

        /// <summary>
        /// The Center of the sprite
        /// </summary>
        public Vector2 Origin;

        /// <summary>
        /// The rotation of the sprite
        /// </summary>
        public float Rotation = 0;

        /// <summary>
        /// The Depth at which to draw the sprite in
        /// </summary>
        public float Depth = 0;

        /// <summary>
        /// The color of the sprite
        /// </summary>
        public Color Color = Color.White;

        /// <summary>
        /// What effects should we emplace?
        /// </summary>
        public SpriteEffects Effects = SpriteEffects.None;

        #endregion

        #region Properties

        /// <summary>
        /// Gets/Sets the position of the Sprite
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return new Vector2(BoundingBox.X, BoundingBox.Y);
            }

            set
            {
                BoundingBox.X = (int)value.X;
                BoundingBox.Y = (int)value.Y;
            }
        }

        #endregion

        #region Methods

        public virtual void Update()
        {
            
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Textures[TextureID], BoundingBox, Source, Color, Rotation, Origin, Effects, Depth);
        }

        #endregion
    }
}

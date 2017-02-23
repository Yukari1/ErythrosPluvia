using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Erythros_Pluvia.Util;

using MonoGame.Extended.Tiled;

namespace Erythros_Pluvia.Entities
{

    /// <summary>
    /// Base class for dynamic entities within the game.
    /// </summary>
    public class IEntity
    {
        #region Properties

        protected Vector2 velocity;

        /// <summary>
        /// Current velocity of the entity.
        /// </summary>
        public Vector2 Velocity
        {
            set
            {
                velocity = value;
            }

            get
            {
                return velocity;
            }
        }

        /// <summary>
        /// Previous position of the entity
        /// </summary>
        public Vector2 PreviousPosition { set; get; }

        /// <summary>
        /// Current position of the entity
        /// </summary>
        public Vector2 Position { set; get; }

        /// <summary>
        /// The sprite representing the entity on-screen
        /// </summary>
        public Sprite Sprite { set; get; }
        
        /// <summary>
        /// The spatial layer the entity is on
        /// </summary>
        public float Layer {

            set
            {
                Sprite.Depth = value;
            }

            get
            {
                return Sprite.Depth;
            }
        }

        /// <summary>
        /// The bounding box of the entity
        /// </summary>
        public Rectangle BoundingBox
        {
            get
            {
                return Sprite.BoundingBox;
            }

            set
            {
                Sprite.BoundingBox = value;
            }
        }

        /// <summary>
        /// The width of the entity
        /// </summary>
        public int Width
        {
            get
            {
                return Sprite.Width;
            }
        }

        /// <summary>
        /// The height of the entity
        /// </summary>
        public int Height
        {
            get
            {
                return Sprite.Height;
            }
        }

        #endregion

        #region Methods

        public IEntity(float xPosition, float yPosition, Sprite sprite, float layer)
        {
            this.Position = new Vector2(xPosition, yPosition);
            this.Velocity = new Vector2(0.0f, 0.0f);
            this.Sprite = sprite;
            this.Layer = layer;
        }
        
        /// <summary>
        /// What this entity will do in the event of a collision.
        /// </summary>
        /// <param name="other">The entity with which this one collideds</param>
        public virtual void OnCollide(IEntity other)
        {

        }

        /// <summary>
        /// Update the entity for the current frame.
        /// </summary>
        /// <param name="time">time handler for this frame</param>
        public virtual void UpdatePosition(GameTime time)
        {

            PreviousPosition = Position;
            Position += Velocity;

            // TODO: movement checks
        }

        /// <summary>
        /// Draw the sprite to the sprite batch
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to which the sprite will be drawn</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Sprite.Draw(spriteBatch);
        }

        #endregion
    }
}

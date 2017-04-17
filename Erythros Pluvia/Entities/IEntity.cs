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
    public abstract class IEntity
    {
        #region Properties

        protected Vector2 position;

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
        public Vector2 Position
        {
            set
            {
                position = value;
            }

            get
            {
                return position;
            }
        }

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
        /// True if the entity's feet are planted on the ground, false otherwise.
        /// </summary>
        public bool IsOnGround { set; get; }

        /// <summary>
        /// The bounding box of the entity
        /// </summary>
        public Microsoft.Xna.Framework.Rectangle BoundingBox
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
        /// <param name="other">The entity with which this one collides</param>
        public virtual void OnCollide(IEntity other)
        {

        }

        public abstract void OnUpdate(GameTime time);

        /// <summary>
        /// Update the entity for the current frame.
        /// </summary>
        /// <param name="time">time handler for this frame</param>
        public virtual void UpdatePosition(GameTime time)
        {
            PreviousPosition = Position;
            position.X += Velocity.X * (float)(time.ElapsedGameTime.TotalSeconds);
            position.Y += Velocity.Y * (float)(time.ElapsedGameTime.TotalSeconds);
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

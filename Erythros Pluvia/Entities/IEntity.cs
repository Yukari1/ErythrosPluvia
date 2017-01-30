using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Erythros_Pluvia.Util;

namespace Erythros_Pluvia.Entities
{
    public class IEntity
    {
        #region Properties

        /// <summary>
        /// Current velocity of the entity.
        /// </summary>
        public Vector2 Velocity { set; get; }

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

        #endregion

        #region Methods
        
        public virtual void OnCollide(IEntity other)
        {

        }

        public virtual void EndUpdate(GameTime time)
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

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

        public virtual void EndUpdate()
        {
            PreviousPosition = Position;
            Position += Velocity;

            // TODO: movement checks
        }

        #endregion
    }
}

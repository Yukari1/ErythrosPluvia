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
    public class PlayerEntity : IEntity
    {

        /// <summary>
        /// Construct a new instance of PlayerEntity
        /// </summary>
        /// <param name="xPosition">x coordinate of the player's position, in world coordinates</param>
        /// <param name="yPosition">y coordinate of the player's position, in world coordinates</param>
        /// <param name="sprite">the sprite for this entity</param>
        public PlayerEntity(float xPosition, float yPosition, Sprite sprite)
        {
            this.Position = new Vector2(xPosition, yPosition);
            this.Velocity = new Vector2(0.0f, 0.0f);
            this.Sprite = sprite;
        }
    }
}

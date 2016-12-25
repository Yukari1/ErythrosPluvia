using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Erythros_Pluvia.Entities
{
    public class IEntity : Util.Sprite
    {
        #region Fields

        public Vector2 Speed = Vector2.Zero;

        public Vector2 PreviousPosition = Vector2.Zero;

        #endregion

        #region Methods
        
        public virtual void OnCollide(IEntity other)
        {

        }

        public virtual void EndUpdate()
        {
            PreviousPosition = Position;
            Position += Speed;

            // TODO: movement checks
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace Erythros_Pluvia.Input
{
    #region Delegates

    public delegate void Command(GameTime time);

    #endregion

    #region Methods

    public interface InputManager
    {

        void executeCommands(GameTime time);
    }

    #endregion
}

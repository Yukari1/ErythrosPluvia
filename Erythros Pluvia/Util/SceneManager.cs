/***********************************************************
    Copyright 2016 ErythrosPluvia, All rights reserved.
***********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Erythros_Pluvia.Scenes;

namespace Erythros_Pluvia.Util
{
    public static class SceneManager
    {
        #region Fields

        static IScene _Scene;

        #endregion

        #region Properties

        public static IScene CurrentScene
        {
            get { return _Scene; }

            set
            {
                if (_Scene != null)
                {
                    _Scene.OnStop();
                }

                _Scene = value;

                if (_Scene != null)
                {
                    _Scene.OnStart();
                }
            }
        }

        #endregion
    }
}

/***********************************************************
    Copyright 2016 ErythrosPluvia, All rights reserved.
***********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Erythros_Pluvia.Input;
using Erythros_Pluvia.Util;

namespace Erythros_Pluvia.Scenes
{
    public abstract class IScene
    {

        #region Fields

        protected InputManager inputManager;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the GraphicsDevice in use
        /// </summary>
        protected GraphicsDevice GraphicsDevice { get { return Main.Graphics.GraphicsDevice; } }

        /// <summary>
        /// Gets the SpriteBatch in use
        /// </summary>
        protected SpriteBatch SpriteBatch { get { return Main.SpriteBatch; } }

        public InputManager InputManager { get { return this.inputManager; } set { this.inputManager = value; } }

        /// <summary>
        /// Gets the ContentManager in use
        /// </summary>
        protected Microsoft.Xna.Framework.Content.ContentManager Content { get { return Main.ContentManager; } }

        protected Effect Effect { get { return Main.RenderingEffect; } }

        #endregion


        #region Methods

        public virtual void OnStart() { }
        public virtual void OnStop() { }

        public virtual void OnUpdate(GameTime time) { InputManager.executeCommands(time); }
        public virtual void OnDraw(GameTime time) { }

        #endregion
    }
}
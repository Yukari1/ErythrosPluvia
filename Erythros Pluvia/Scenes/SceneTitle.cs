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
using Microsoft.Xna.Framework.Audio;

namespace Erythros_Pluvia.Scenes
{
    public class SceneTitle : IScene
    {

        #region Fields

        Texture2D Background;
        Texture2D StartButton;

        SoundEffectInstance MenuMusic;

        bool AddT = false;
        float T = 1.0f;

        #endregion

        #region Methods

        public override void OnStart()
        {
            Console.WriteLine(GraphicsDevice.Viewport.Width);
            Console.WriteLine(GraphicsDevice.Viewport.Height);

            Background = Content.Load<Texture2D>("Scenes/Title/Graphics/Background");
            StartButton = Content.Load<Texture2D>("Scenes/Title/Graphics/StartButton");

            //MenuMusic = Content.Load<SoundEffect>("Scenes/Title/Audio/MenuMusic").CreateInstance();

            //MenuMusic.IsLooped = true;
            //MenuMusic.Play();
        }

        public override void OnUpdate(GameTime time)
        {
            T += AddT ? 0.01f : -0.01f;

            if (T <= 0) { AddT = true; }
            if (T >= 1) { AddT = false; }
        }

        public override void OnDraw(GameTime time)
        {
            SpriteBatch.Begin();

            SpriteBatch.Draw(Background, new Vector2(0,0), Color.White);

            SpriteBatch.Draw(StartButton, new Vector2(0, GraphicsDevice.Viewport.Height - 128), new Color(1.0f, 1.0f, 1.0f, T));

            SpriteBatch.End();
        }

        #endregion
    }
}

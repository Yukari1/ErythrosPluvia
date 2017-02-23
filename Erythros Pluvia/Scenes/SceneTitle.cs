/***********************************************************
    Copyright 2016 ErythrosPluvia, All rights reserved.
***********************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Erythros_Pluvia.Input;
using Erythros_Pluvia.Util;

namespace Erythros_Pluvia.Scenes
{
    public class SceneTitle : IScene
    {

        #region Fields

        protected LinkedList<Texture2D> titleScreenSelections;
        protected LinkedListNode<Texture2D> currentSelection;

        protected int selectionsMargin;

        protected double scrollDelaySeconds;
        protected double secondsSinceLastScroll;
        protected bool initialScroll;

        Texture2D Background;

        Texture2D NewGameButton;
        Texture2D LoadGameButton;
        Texture2D OptionsButton;
        Texture2D QuitToDesktopButton; // TODO what about other platforms?

        SoundEffectInstance MenuMusic;

        private Vector2 _screenCenter;

        bool AddT = false;
        float T = 1.0f;

        #endregion

        #region Methods

        public override void OnStart()
        {
            selectionsMargin = 80;

            Background = Content.Load<Texture2D>("Scenes/Title/Graphics/Background");

            NewGameButton = Content.Load<Texture2D>("Scenes/Title/Graphics/new_game_button");
            LoadGameButton = Content.Load<Texture2D>("Scenes/Title/Graphics/load_game_button");
            OptionsButton = Content.Load<Texture2D>("Scenes/Title/Graphics/options_button");
            QuitToDesktopButton = Content.Load<Texture2D>("Scenes/Title/Graphics/quit_to_desktop_button");

            // put each button into the list for scrolling
            titleScreenSelections = new LinkedList<Texture2D>();
            titleScreenSelections.AddLast(NewGameButton);
            titleScreenSelections.AddLast(LoadGameButton);
            titleScreenSelections.AddLast(OptionsButton);
            titleScreenSelections.AddLast(QuitToDesktopButton);

            currentSelection = titleScreenSelections.First;
            scrollDelaySeconds = 0.3;
            secondsSinceLastScroll = 0.0;
            initialScroll = true;

            Command scrollUpCommand = delegate (GameTime time) {
                this.ScrollUp(time);
            };

            Command scrollDownCommand = delegate (GameTime time)
            {
                this.ScrollDown(time);
            };

            Command stopScrollingCommand = delegate (GameTime time)
            {
                this.secondsSinceLastScroll = 0.0;
                this.initialScroll = true;
            };

            Command makeSelection = delegate (GameTime time)
            {
                this.selectOption(time);
            };

            WindowsInputManager windowsInputManager = new WindowsInputManager();
            windowsInputManager.addKeyPressBinding(Keys.Up, scrollUpCommand);
            windowsInputManager.addKeyPressBinding(Keys.Down, scrollDownCommand);
            windowsInputManager.addKeyPressBinding(Keys.Enter, makeSelection);

            windowsInputManager.addKeyReleaseBinding(Keys.Up, stopScrollingCommand);
            windowsInputManager.addKeyReleaseBinding(Keys.Down, stopScrollingCommand);
            InputManager = windowsInputManager;

            //MenuMusic = Content.Load<SoundEffect>("Scenes/Title/Audio/MenuMusic").CreateInstance();

            //MenuMusic.IsLooped = true;
            //MenuMusic.Play();

        }

        public override void OnUpdate(GameTime time)
        {
            base.OnUpdate(time);

            T += AddT ? 0.01f : -0.01f;

            if (T <= 0) { AddT = true; }
            if (T >= 1) { AddT = false; }

        }

        public override void OnDraw(GameTime time)
        {
            // compare with each of these effects on/off and see what works best
            SpriteBatch.Begin(/*SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, Effect*/);

            SpriteBatch.Draw(Background, new Vector2((GraphicsDevice.Viewport.Width / 2) - (Background.Width / 2), (GraphicsDevice.Viewport.Height / 2) - (Background.Height / 2)), Color.White);

            int buttonVerticalPosition = 160;
            int buttonHorizontalPosition = (GraphicsDevice.Viewport.Width / 2) - (NewGameButton.Width / 2);

            // draw the New Game button
            drawButton(NewGameButton, new Vector2(buttonHorizontalPosition, buttonVerticalPosition));

            // draw the Load Game button
            buttonVerticalPosition += selectionsMargin;
            drawButton(LoadGameButton, new Vector2(buttonHorizontalPosition, buttonVerticalPosition));

            // draw the Options button
            buttonVerticalPosition += selectionsMargin;
            drawButton(OptionsButton, new Vector2(buttonHorizontalPosition, buttonVerticalPosition));

            // draw the Quit button
            buttonVerticalPosition += selectionsMargin;
            drawButton(QuitToDesktopButton, new Vector2(buttonHorizontalPosition, buttonVerticalPosition));

            SpriteBatch.End();
        }


        public void ScrollUp(GameTime time)
        {

            secondsSinceLastScroll += time.ElapsedGameTime.TotalSeconds;

            // set a delay to make sure we're not scrolling too quickly (i.e. every screen refresh)
            if (initialScroll || secondsSinceLastScroll >= scrollDelaySeconds)
            {
                if (currentSelection.Previous != null)
                {
                    currentSelection = currentSelection.Previous;
                }
                else
                {
                    currentSelection = titleScreenSelections.Last;
                }
                secondsSinceLastScroll = 0.0;
            }
            initialScroll = false;
        }

        public void ScrollDown(GameTime time)
        {
            secondsSinceLastScroll += time.ElapsedGameTime.TotalSeconds;

            // set a delay to make sure we're not scrolling too quickly (i.e. every screen refresh)
            if (initialScroll || secondsSinceLastScroll >= scrollDelaySeconds)
            {
                if (currentSelection.Next != null)
                {
                    currentSelection = currentSelection.Next;
                }
                else
                {
                    currentSelection = titleScreenSelections.First;
                }
                secondsSinceLastScroll = 0.0;
            }
            initialScroll = false;
        }

        public void selectOption(GameTime time)
        {
            if (currentSelection.Value == QuitToDesktopButton)
            {
                Util.SceneManager.CurrentScene = null;
            }
            else if (currentSelection.Value == NewGameButton)
            {
                Util.SceneManager.CurrentScene = new SceneLevel("Maps/test", 44, 52, 8, 8);
            }
        }

        // so that we're not repeating the same code over and over
        private void drawButton(Texture2D texture, Vector2 position)
        {

            // making the button purple when selected is just a placeholder. We'll do more fancy stuff later on
            Color buttonColor;
            if (currentSelection.Value == texture)
            {
                buttonColor = Color.Purple;
            }
            else
            {
                buttonColor = Color.White;
            }

            SpriteBatch.Draw(texture, position, buttonColor);
        }

        #endregion
    }
}

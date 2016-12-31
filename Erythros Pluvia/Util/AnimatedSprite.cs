using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Erythros_Pluvia.Util
{
    class AnimatedSprite : Sprite
    {
        #region Fields

        // for keeping track of the number of frames required before it's time to update the current animation frame
        protected int counter;

        // animation framerate
        protected int framerate;

        // total number of animation frames
        protected int numFrames;

        // animate in reverse?
        protected bool reverse;

        #endregion

        #region Constructors

        public AnimatedSprite(Texture2D spritesheet, int numCols, int numFrames, int framerate, bool reverse)
        {
            this.reverse = reverse;
            this.framerate = framerate;
            this.numFrames = numFrames;

            // we will extract the individual animation frames upon construction. This way, we're not continually recalculating the current frame at runtime
            int numRows = numFrames / numCols;
            int frameWidth = spritesheet.Width / numCols;
            int frameHeight = spritesheet.Height / numRows;
            for (int i = 0; i < numFrames; i++)
            {
                int currentCol = i % numCols;
                int currentRow = i / numCols;

                Texture2D cropImage = new Texture2D(spritesheet.GraphicsDevice, frameWidth, frameHeight);
                Color[] cropPixels = new Color[frameWidth * frameHeight];

                // first argument is mipmap level. Assuming for now that this will be 0 in this instance
                spritesheet.GetData(0, new Rectangle(currentCol * frameWidth, currentRow * frameHeight, frameWidth, frameHeight), cropPixels, 0, cropPixels.Length);
                cropImage.SetData(cropPixels);

                Textures[i] = cropImage;
            }

            this.TextureID = 0;
            this.counter = 0;
        }

        #endregion

        #region Properties

        public int Framerate
        {
            get
            {
                return framerate;
            }

            set
            {
                framerate = value;
            }
        }

        public int NumFrames
        {
            get
            {
                return numFrames;
            }
        }

        public bool Reverse
        {
            get
            {
                return reverse;
            }

            set
            {
                reverse = value;
            }
        }

        #endregion

        #region Methods

        public override void Update()
        {
            this.counter++;

            if (this.counter >= this.framerate)
            {
                if (this.reverse)
                {
                    this.TextureID -= 1;

                    // if the texture ID goes negative, it's time to wrap back around to the end
                    if (this.TextureID < 0)
                    {
                        this.TextureID = this.numFrames - 1;
                    }
                }
                else
                {
                    this.TextureID += 1;

                    // if the texture ID becomes greater or equal to than the number of frames, it's time to wrap back around to the beginning
                    if (this.TextureID >= this.numFrames)
                    {
                        this.TextureID = 0;
                    }
                }
                this.counter = 0;
            }
        }

        #endregion
    }
}

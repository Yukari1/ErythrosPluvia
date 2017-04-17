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

    /// <summary>
    /// The entity controlled by the player.
    /// </summary>
    public class PlayerEntity : IEntity
    {

        #region Jumping Controls

        /// <summary>
        /// The maximum amount of time to spend on a single jump.
        /// </summary>
        const float MAX_JUMP_DURATION = 700.0f;

        /// <summary>
        /// The jump speed that the player should start with at the beginning of a jump.
        /// </summary>
        const float INITIAL_JUMP_SPEED = 450.0f;

        /// <summary>
        /// A control value to adjust how the player's jump velocity decreases over time.
        /// </summary>
        const float JUMP_POWER_EXPONENT = 0.3f;

        /// <summary>
        /// True if the player is jumping in this frame (e.g. if the player has the "jump" button pressed).
        /// </summary>
        protected bool doJump;

        /// <summary>
        /// Whether or not to continue an already in-progress jump. Used within the context of a single jump event in order to determine
        /// when to reset jumpTime counter.
        /// </summary>
        protected bool jumpInProgress;

        /// <summary>
        /// True if the player was jumping in the last frame, false otherwise.
        /// </summary>
        protected bool wasJumpingLastFrame;

        /// <summary>
        /// The amount of time the player has been jumping. Resets to zero when the player completes a jump, either by reaching the apex of the
        /// jump or releasing the "jump" button.
        /// </summary>
        protected float jumpTime;

        public bool DoJump
        {
            set
            {
                doJump = value;
            }

            get
            {
                return doJump;
            }
        }

        #endregion

        /// <summary>
        /// Construct a new instance of PlayerEntity
        /// </summary>
        /// <param name="xPosition">x coordinate of the player's position, in world coordinates</param>
        /// <param name="yPosition">y coordinate of the player's position, in world coordinates</param>
        /// <param name="sprite">the sprite for this entity</param>
        public PlayerEntity(float xPosition, float yPosition, Sprite sprite, float layer) : base(xPosition, yPosition, sprite, layer)
        {
            this.doJump = false;
            this.jumpInProgress = false;
            this.wasJumpingLastFrame = false;
            this.jumpTime = 0.0f;
        }

        /// <summary>
        /// Actions to take each frame. Note that this is separate from updating the entity's position based upon current velocity--that takes place in UpdatePosition and should be invoked separately
        /// by the scene manager.
        /// </summary>
        public override void OnUpdate(GameTime time)
        {
            if (doJump)
            {
                // the player is just beginning a jump. We should only initiate a jump if the player is on the ground and has released
                // the jump button since their last jump ended
                if (!wasJumpingLastFrame && IsOnGround)
                {
                    jumpTime = (float)time.ElapsedGameTime.TotalMilliseconds;
                    jumpInProgress = true;
                }

                // the player's jump is still in progress
                if (jumpInProgress && jumpTime <= MAX_JUMP_DURATION)
                {
                    this.velocity.Y = -calculateJumpSpeed(jumpTime, INITIAL_JUMP_SPEED, MAX_JUMP_DURATION, JUMP_POWER_EXPONENT);
                    jumpTime += (float)time.ElapsedGameTime.TotalMilliseconds;
                    Console.WriteLine("Jump velocity: " + this.velocity.Y);
                }
                else
                {
                    // the player has reached the apex of the jump. The player is no longer jumping and is now falling
                    jumpTime = 0.0f;
                    jumpInProgress = false;
                }
                
                // so that the next frame can know what happened last frame
                wasJumpingLastFrame = true;
            }
            else
            {
                jumpTime = 0.0f;
                wasJumpingLastFrame = false;
            }
        }

        /// <summary>
        /// Determine the jump velocity for this frame, given the amount of time the player has been jumping.
        /// </summary>
        /// <param name="jumpTime">the amount of time the player has been jumping, measured in milliseconds</param>
        /// <param name="initialSpeed">the speed at which to start the jump, measured in pixels per second (TODO yea, confusing that this one goes by seconds but we'll fix that later in the IEntity class)</param>
        /// <param name="maxJumpDuration">the maximum duration of the jump, measured in milliseconds</param>
        /// <param name="jumpPowerExponent">the exponent to use in the power curve of the jump, unitless since it's an exponent</param>
        /// <returns>the player's jump speed for this frame</returns>
        protected float calculateJumpSpeed(float jumpTime, float initialSpeed, float maxJumpDuration, float jumpPowerExponent)
        {
            return initialSpeed * (1.0f - (float)Math.Pow((jumpTime / maxJumpDuration), jumpPowerExponent));
        }
    }
}

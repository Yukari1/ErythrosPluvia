using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erythros_Pluvia.Util
{

    /// <summary>
    /// An axis-aligned rectangle. Unlike the one built into XNA, this one uses floating-point numbers for all of its calculations.
    /// </summary>
    class Rectangle
    {

        /// <summary>
        /// Construct a new instance of Rectangle
        /// </summary>
        /// <param name="x">x position of the top-left vertex</param>
        /// <param name="y">y position of the top-left vertex</param>
        /// <param name="width">width of the rectangle</param>
        /// <param name="height">height of the rectangle</param>
        public Rectangle(float x, float y, float width, float height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Construct a new instance of Rectangle from an XNA rectangle
        /// </summary>
        /// <param name="rectangle">XNA integer-based rectangle</param>
        public Rectangle(Microsoft.Xna.Framework.Rectangle rectangle)
        {
            this.X = (float)rectangle.X;
            this.Y = (float)rectangle.Y;
            this.Width = (float)rectangle.Width;
            this.Height = (float)rectangle.Height;
        }

        /// <summary>
        /// x position of the top-left vertex
        /// </summary>
        public float X { set; get; }

        /// <summary>
        /// y position of the top-left vertex
        /// </summary>
        public float Y { set; get; }

        /// <summary>
        /// width of the rectangle
        /// </summary>
        public float Width { set; get; }

        /// <summary>
        /// height of the rectangle
        /// </summary>
        public float Height { set; get; }

        /// <summary>
        /// Determines if two rectangles intersect
        /// </summary>
        /// <param name="other">the other rectangle</param>
        /// <param name="inclusive">should the check be inclusive?</param>
        /// <returns>true if the rectangles intersect, false otherwise</returns>
        public bool Intersects(Rectangle other, bool inclusive = false)
        {
            if (inclusive)
            {
                return (((other.X + other.Width) >= this.X) && (other.X <= (this.X + this.Width)) && ((other.Y + other.Height) >= this.Y) && (other.Y <= (this.Y + this.Height)));
            }
            else
            {
                return (((other.X + other.Width) > this.X) && (other.X < (this.X + this.Width)) && ((other.Y + other.Height) > this.Y) && (other.Y < (this.Y + this.Height)));
            }
        }
    }
}

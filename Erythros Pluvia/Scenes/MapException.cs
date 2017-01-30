using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erythros_Pluvia.Util
{
    public class MapException : Exception
    {
        public MapException()
        {
        }

        public MapException(string message)
        : base(message)
        {
        }

        public MapException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}
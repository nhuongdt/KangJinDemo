using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class InvalidCypherTextException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public InvalidCypherTextException()
            : base()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public InvalidCypherTextException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public InvalidCypherTextException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

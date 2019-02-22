using System;
namespace CsvStreamReader
{
    /// <summary>
    /// Constants to process CSV stream
    /// </summary>
    internal sealed class CsvConstants
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:CsvStream.CsvConstants"/> class.
        /// Implemented as private
        /// </summary>
        private CsvConstants()
        {
        }

        /// <summary>
        /// https://tools.ietf.org/html/rfc4180#ref-4
        /// TEXTDATA #1 Range start
        /// </summary>
        internal const byte CONST_TD1_RNG_START = (byte)32;

        /// <summary>
        /// https://tools.ietf.org/html/rfc4180#ref-4
        /// TEXTDATA #1 Range end
        /// </summary>
        internal const byte CONST_TD1_RNG_END = (byte)33;


        /// <summary>
        /// https://tools.ietf.org/html/rfc4180#ref-4
        /// TEXTDATA #2 Range start
        /// </summary>
        internal const byte CONST_TD2_RNG_START = (byte)35;

        /// <summary>
        /// https://tools.ietf.org/html/rfc4180#ref-4
        /// TEXTDATA #2 Range end
        /// </summary>
        internal const byte CONST_TD2_RNG_END = (byte)43;

        /// <summary>
        /// https://tools.ietf.org/html/rfc4180#ref-4
        /// TEXTDATA #3 Range start
        /// </summary>
        internal const byte CONST_TD3_RNG_START = (byte)45;

        /// <summary>
        /// https://tools.ietf.org/html/rfc4180#ref-4
        /// TEXTDATA #3 Range end
        /// </summary>
        internal const byte CONST_TD3_RNG_END = (byte)126;

        /// <summary>
        /// https://tools.ietf.org/html/rfc4180#ref-4
        /// Double quote byte 
        /// </summary>
        internal const byte CONST_DQUOTE = (byte)34;

        /// <summary>
        /// https://tools.ietf.org/html/rfc4180#ref-4
        /// Comma byte 
        /// </summary>
        internal const byte CONST_COMMA = (byte)44;

        /// <summary>
        /// https://tools.ietf.org/html/rfc4180#ref-4
        /// LF 
        /// </summary>
        internal const byte CONST_LF = (byte)10;

        /// <summary>
        /// https://tools.ietf.org/html/rfc4180#ref-4
        /// CR 
        /// </summary>
        internal const byte CONST_CR = (byte)13;
    }
}

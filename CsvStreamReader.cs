using System;
using System.IO;

namespace CsvStream
{
    /// <summary>
    /// Reads CSV fields from System.IO.Stream
    /// Please, note it is not thread safe implementation (for performance reasons)
    /// </summary>
    public sealed class CsvStreamReader: IDisposable
    {
        /// <summary>
        /// The encoding to decode bytes from stream
        /// </summary>
        private System.Text.Encoding encoding = new System.Text.UTF8Encoding(true, false);

        /// <summary>
        /// CSV delimiter used for file
        /// </summary>
        private byte delimiter = (byte)',';

        /// <summary>
        /// Last byte read from stream
        /// </summary>
        private byte lastByteRead;

        /// <summary>
        /// The count bytes read from stream.
        /// </summary>
        private int bytesReadFromStream;

        /// <summary>
        /// Indicates whenever a new line was met during last Read operation
        /// </summary>
        /// <value><c>true</c> if is new line met; otherwise, <c>false</c>.</value>
        public bool IsFinishedByNewLine
        {
            get;
            internal set;
        }

        /// <summary>
        /// Requested length of bytes to be read
        /// </summary>
        private int bytesRequested;

        /// <summary>
        /// Bytes count read from stream
        /// </summary>
        private int bytesRead;

        /// <summary>
        /// Underlying stream
        /// </summary>
        private Stream stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CsvStream.CSVStreamReader"/> class.
        /// </summary>
        /// <param name="sourceStream">Source System.IO.Stream</param>
        public CsvStreamReader(Stream sourceStream, System.Text.Encoding streamEncoding, byte csvDelimiter) : this(sourceStream, csvDelimiter)
        {
            encoding    = streamEncoding;

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CsvStream.CSVStreamReader"/> class.
        /// </summary>
        /// <param name="sourceStream">Source System.IO.Stream</param>
        /// <param name="csvDelimiter">Delimiter used to separate fields</param>
        public CsvStreamReader(Stream sourceStream, byte csvDelimiter )
        {
            stream = sourceStream;

            // it is a new file
            // marking it as a new row 
            IsFinishedByNewLine = true;

            delimiter = csvDelimiter;
        }



        /// <summary>
        /// Gets a value indicating whether this <see cref="T:CsvStream.CSVStreamReader"/> can read.
        /// </summary>
        /// <value><c>true</c> if can read; otherwise, <c>false</c>.</value>
        public bool CanRead 
        {
            get 
            {
                return stream.CanRead; 
            }
        }

        /// <summary>
        /// Read the specified buffer that we were able to read in,
        /// throws Exception if reading wasn't over when buffer was done,
        /// so use a max buffer size
        /// </summary>
        /// <returns>The read.</returns>
        /// <param name="buffer">Buffer.</param>
        public int Read(byte[] buffer, int count) 
        {
            bytesRequested = count;
            bytesRead = 0;

            ReadStream(buffer);

            return bytesRead;
        }


        /// <summary>
        /// Reads a field from the stream
        /// </summary>
        /// <param name="buffer">Buffer.</param>
        private void ReadStream(byte[] buffer) 
        {

            // read field first
            if (!stream.CanRead) return ;

            // read while 
            // - we can read and
            // - the buffer length met
            while(ReadByteToBuffer(buffer) > 0 && bytesRead<=bytesRequested) 
            {


                // DELIMITER 
                // if a delimiter met
                if (lastByteRead == delimiter)
                {
                    IsFinishedByNewLine = false;
                    PopByte();

                    break;
                }
                else

                // NEWLINE
                // if a new line symols met
                if (lastByteRead == CsvConstants.CONST_CR || lastByteRead == CsvConstants.CONST_LF)
                {
                    IsFinishedByNewLine = true;
                    PopByte();

                    // if there's one more of these
                    if (ReadByteToBuffer(buffer) > 0)
                    {
                        if (lastByteRead == CsvConstants.CONST_CR || lastByteRead == CsvConstants.CONST_LF)
                        {
                            PopByte();
                        }
                        else
                        {
                            GoBackByte();
                        }
                    }

                    break;
                }
                else

                // FIELD
                // if no delimiter or newline met a field should be read
                { 
                    ReadField(buffer); 
                }

            }


        }

        /// <summary>
        /// Reads field
        /// </summary>
        /// <param name="buffer">Buffer.</param>
        private void ReadField(byte[] buffer)
        {

            if (ReadByteToBuffer(buffer) == 0) return ;


            // escaped string
            if (lastByteRead == CsvConstants.CONST_DQUOTE)
            {
                PopByte();
                ReadEscapedField(buffer);
            }
            else
            {
                GoBackByte();
                ReadNonEscapedField(buffer);
            }
        }

        /// <summary>
        /// Reads an escaped field to buffer
        /// </summary>
        /// <param name="buffer">Buffer.</param>
        private void ReadEscapedField(byte[] buffer) 
        {

            // consists of * of 

            // NonEscaped 
            // COMMA 
            // DELIM
            // CR
            // LF
            // DOUBLE QUOTES

            // so, we met on DQUOTE at the beginning if the string already
            // if next one met, that meand the line is over 
            // TODO: implement multiple double quotes
            if (ReadByteToBuffer(buffer) > 0)
            {
                if (lastByteRead == CsvConstants.CONST_DQUOTE)
                {
                    return;
                }

            }
            // if can't read
            else
            {
                return;
            }

            //quote_was_last_symbol = false;
            // now the field isn't empty doublequoted string
            while (ReadByteToBuffer(buffer) > 0) 
            {

                if (lastByteRead == CsvConstants.CONST_DQUOTE)
                {

                    //if this is a doubleQuote then ok, otherwise the line has ended
                    if (!ReadDQuotes(buffer))
                    {
                        GoBackByte();
                        break;
                    }
                }

                // just continue to write to buffer
            } 
        }

        private bool ReadDQuotes(byte[] buffer) 
        {
            if (ReadByteToBuffer(buffer) == 0) return false;

            if ( lastByteRead == CsvConstants.CONST_DQUOTE)
            {
                PopByte();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Reads the non escaped field content
        /// </summary>
        /// <param name="buffer">Buffer.</param>
        private void ReadNonEscapedField(byte[] buffer) 
        {
            while (ReadByteToBuffer(buffer) > 0)
            {
                // that doesn't look like a non escaped field
                if (lastByteRead == delimiter || lastByteRead == CsvConstants.CONST_CR || lastByteRead == CsvConstants.CONST_LF)
                {
                    GoBackByte();
                    break;
                }

                //just keep reading :)
            }
        }




        /// <summary>
        /// Reads byte from the stream
        /// </summary>
        /// <returns>The byte.</returns>
        /// <param name="buffer">Buffer.</param>
        private int ReadByteToBuffer(byte[] buffer) 
        {
            // read from stream and set up class params properly
            bytesReadFromStream = stream.Read(buffer, bytesRead, 1);

            lastByteRead = buffer[bytesRead];

            bytesRead += bytesReadFromStream;

            return bytesReadFromStream; 
        }

        private void PopByte() 
        {
            bytesRead--; 
        }

        private void GoBackByte() 
        {
            stream.Position--;
            PopByte(); 
        }

        /// <summary>
        /// Close the underlieing stream
        /// </summary>
        public void Close() 
        {
            //stream.
            stream.Close();
        }

        /// <summary>
        /// Releases all resource used by the <see cref="T:CsvStream.CSVStreamReader"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="T:CsvStream.CSVStreamReader"/>. The
        /// <see cref="Dispose"/> method leaves the <see cref="T:CsvStream.CSVStreamReader"/> in an unusable state.
        /// After calling <see cref="Dispose"/>, you must release all references to the
        /// <see cref="T:CsvStream.CSVStreamReader"/> so the garbage collector can reclaim the memory that the
        /// <see cref="T:CsvStream.CSVStreamReader"/> was occupying.</remarks>
        public void Dispose()
        {
            Close();
            stream.Dispose();
        }
    }
}

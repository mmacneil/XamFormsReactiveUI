
using System;


namespace WordScraper
{
    /// <summary>
    /// The exception that is thrown when PhantomJS process returns non-zero exit code
    /// </summary>
    public class PhantomJSException : Exception
    {
        /// <summary>Get WkHtmlToImage process error code</summary>
        public int ErrorCode { get; private set; }

        public PhantomJSException(int errCode, string message)
          : base($"PhantomJS exit code {(object) errCode}: {message as object}")
        {
            ErrorCode = errCode;
        }
    }
}

namespace WinTail
{
    public class Messages
    {
        /// <summary>
        /// use to signal to continue processing (the "blank input" case)
        /// </summary>
        public class ContinueProcessing
        {
        }

        /// <summary>
        /// use this to signal that the user's input was good and passed validation (the "valid input" case)
        /// </summary>
        public class InputSuccess
        {
            public string Reason { get; private set; }

            public InputSuccess(string reason)
            {
                Reason = reason;
            }
        }

        /// <summary>
        /// Base class for signalling that user input was invalid.
        /// </summary>
        public class InputError
        {
            public string Reason { get; private set; }

            public InputError(string reason)
            {
                Reason = reason;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class ValidationError : InputError
        {
            public ValidationError(string reason) : base(reason)
            { }
        }

        /// <summary>
        /// 
        /// </summary>
        public class NullInputError : InputError
        {
            public NullInputError(string reason) : base(reason)
            { }
        }
    }
}

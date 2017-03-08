namespace Overby.Extensions.Attachments
{
    public static partial class AttachmentExtensions
    {
        /// <summary>
        /// The result of getting an attached value from a host object.
        /// </summary>
        /// <typeparam name="T">The value's type.</typeparam>
        public class AttachmentResult<T>
        {
            /// <summary>
            /// Whether the attachment key was found.
            /// </summary>
            public bool Found { get; }

            /// <summary>
            /// The value, if found.
            /// </summary>
            public T Value { get; }

            internal AttachmentResult(bool found, T value)
            {
                Found = found;
                Value = value;
            }
                        
            public static implicit operator T(AttachmentResult<T> result) => result.Value;
        }
    }
}

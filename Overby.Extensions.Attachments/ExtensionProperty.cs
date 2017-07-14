namespace Overby.Extensions.Attachments
{
    /// <summary>
    /// Stores the value of an extension property.
    /// </summary>
    /// <typeparam name="T">Type of the value</typeparam>
    public class ExtensionProperty<T>
    {
        public string AttachmentKey { get; }

        /// <summary>
        /// The Value.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Converts the extension property to the value type implicitly.
        /// </summary>
        public static implicit operator T(ExtensionProperty<T> prop) => prop.Value;

        public ExtensionProperty(string attachmentKey) =>
            AttachmentKey = attachmentKey;
    }
}

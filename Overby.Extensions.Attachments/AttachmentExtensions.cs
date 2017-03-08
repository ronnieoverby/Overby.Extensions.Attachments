using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Overby.Extensions.Attachments
{
    /// <summary>
    /// Extensions for attaching values to objects.
    /// </summary>
    public static partial class AttachmentExtensions
    {
        private static readonly ConditionalWeakTable<object, ConcurrentDictionary<string, object>> _attachmentTable =
            new ConditionalWeakTable<object, ConcurrentDictionary<string, object>>();

        /// <summary>
        /// Copies attachments from one object to another.
        /// </summary>
        /// <param name="source">The source object to get attachments from.</param>
        /// <param name="target">The target object to set attachments on.</param>
        /// <param name="keyPredicate">An optional predicate of key values.</param>
        public static void CopyAttachments(this object source, object target, Func<string, bool> keyPredicate = null)
        {
            foreach (var key in source.GetAttachmentKeys())
            {
                if (keyPredicate?.Invoke(key) == false)
                    continue;

                var result = source.GetAttached(key);
                if (result.Found)
                    target.SetAttached(result.Value, key);
            }
        }

        /// <summary>
        /// Atomically gets or sets an attached value if it's found.
        /// </summary>
        public static AttachmentResult<T> GetOrSetAttached<T>(this object host, Func<T> factory, string key = null)
        {
            key = key ?? typeof(T).AssemblyQualifiedName;
            var dict = _attachmentTable.GetOrCreateValue(host);
            var found = true;
            var value = dict.GetOrAdd(key, _ => { found = false; return factory(); });
            var castValue = (T)value;
            return new AttachmentResult<T>(found, castValue);
        }

        /// <summary>
        /// Sets an attached value. Any existing value will be overwritten.
        /// If a key isn't provided, the value's assembly qualified type
        /// name will be used.
        /// </summary>
        public static void SetAttached(this object host, object value, string key = null)
        {
            if (value == null && key == null)
                throw new ArgumentNullException(key, $"{nameof(value)} or {nameof(key)} must be provided");

            key = key ?? value.GetType().AssemblyQualifiedName;
            var dict = _attachmentTable.GetOrCreateValue(host);
            dict[key] = value;
        }

        /// <summary>
        /// Gets an attached value using the type's AssemblyQualifiedName as the key.
        /// </summary>
        public static AttachmentResult<object> GetAttached(this object host, Type type) =>
            GetAttached(host, type.AssemblyQualifiedName);

        /// <summary>
        /// Gets an attached value by name.
        /// </summary>
        public static AttachmentResult<object> GetAttached(this object host, string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var dict = _attachmentTable.GetOrCreateValue(host);
            var found = dict.TryGetValue(key, out object value);
            return new AttachmentResult<object>(found, value);
        }

        /// <summary>
        /// Gets an attached value by key or by the the type parameter's
        /// AssemblyQualifiedName.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="host"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static AttachmentResult<T> GetAttached<T>(this object host, string key = null)
        {
            key = key ?? typeof(T).AssemblyQualifiedName;
            var dict = _attachmentTable.GetOrCreateValue(host);
            var found = dict.TryGetValue(key, out object value);
            var castValue = found ? (T)value : default(T);
            return new AttachmentResult<T>(found, castValue);
        }

        /// <summary>
        /// Returns all of the attachment keys found for the host object.
        /// </summary>
        public static ICollection<string> GetAttachmentKeys(this object host)
        {
            if (_attachmentTable.TryGetValue(host, out ConcurrentDictionary<string, object> dict))
                return dict.Keys;

            return new string[0];
        }

        /// <summary>
        /// Removes every attached value from the host object.
        /// </summary>
        public static void ClearAttached(this object host) => _attachmentTable.Remove(host);

        /// <summary>
        /// Removes the specified attached value.
        /// </summary>
        public static AttachmentResult<object> RemoveAttached(this object host, string key)
        {
            var dict = _attachmentTable.GetOrCreateValue(host);
            var found = dict.TryRemove(key, out object value);
            return new AttachmentResult<object>(found, value);
        }

        /// <summary>
        /// Removes the attached value. If a key isn't supplied
        /// the type parameter's assembly qualified type name will be used.
        /// </summary>
        public static AttachmentResult<T> RemoveAttached<T>(this object host, string key = null)
        {
            key = key ?? typeof(T).AssemblyQualifiedName;
            var dict = _attachmentTable.GetOrCreateValue(host);
            var found = dict.TryRemove(key, out object value);
            var castValue = found ? (T)value : default(T);
            return new AttachmentResult<T>(found, castValue);
        }

        private static string RefIdKey = Guid.NewGuid().ToString();

        /// <summary>
        /// Unique identifier for the object reference.
        /// </summary>
        public static Guid GetReferenceId(this object obj) =>
            obj.GetOrSetAttached(() => Guid.NewGuid(), RefIdKey);
    }
}

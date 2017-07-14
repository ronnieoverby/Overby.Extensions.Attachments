using NUnit.Framework;
using Overby.Extensions.Attachments;
using System;
// these aliases just make the extension property signatures shorter
using ODTO = Overby.Extensions.Attachments.Optional<System.DateTimeOffset>;
using ONI = Overby.Extensions.Attachments.Optional<int?>;
using OS = Overby.Extensions.Attachments.Optional<string>;

namespace Tests
{
    /// <summary>
    /// Examples of extension properties on System.Object.
    /// </summary>
    /// <remarks>
    /// Tests are down below.
    /// </remarks>
    public static class ObjectExtensionProperties
    {
        public static ExtensionProperty<int?> Id(this object obj, ONI value = default(ONI)) =>
            obj.GetExtensionProperty(value);

        public static ExtensionProperty<string> Name(this object obj, OS value = default(OS)) =>
            obj.GetExtensionProperty(value);

        public static ExtensionProperty<string> Description(this object obj, OS value = default(OS)) =>
            obj.GetExtensionProperty(value);

        public static ExtensionProperty<DateTimeOffset> Expiry(this object obj, ODTO value = default(ODTO)) =>
            obj.GetExtensionProperty(value);

        /// <summary>
        /// Example of a read only "property" based on other extension properties.
        /// This is actually nothing special at all!
        /// </summary>
        public static bool IsExpired(this object obj) =>
            DateTimeOffset.Now < obj.Expiry();
    }

    [TestFixture]
    public class ExtensionPropertiesTests
    {
        [Test]
        public void Set_Via_Optional_Parameter_Get_Via_Value_Property()
        {
            var instance = new object();
            var now = DateTimeOffset.Now;

            // using the optional value parameter of the extension method
            // this is an alternative to `instance.Expiry().Value = now;`
            instance.Expiry(now);

            // using the `Value` property to get the value
            // this is an alternative to `DateTimeOffset expiry = instance.Expiry();`
            var expiry = instance.Expiry().Value;

            Assert.AreEqual(now, expiry);
        }

        [Test]
        public void Set_Via_Value_Property_Get_Via_Implicit_Conversion()
        {
            var instance = new object();
            var now = DateTimeOffset.Now;

            //using the `Value` property to set the value
            // this is an alternative to `instance.Expiry(now);`
            instance.Expiry().Value = now;

            // using the implicit conversion operator to get the value
            // this is an alternative to `var expiry = instance.Expiry().Value;`
            DateTimeOffset expiry = instance.Expiry();

            Assert.AreEqual(now, expiry);
        }

        [Test]
        public void Defaults_When_Not_Set_Via_Optional_Value_Parameter()
        {
            var instance = new object();
            
            int? id = instance.Id();
            string name = instance.Name();
            DateTimeOffset expiry = instance.Expiry();

            Assert.AreEqual(default(int?), id);
            Assert.AreEqual(default(string), name);
            Assert.AreEqual(default(DateTimeOffset), expiry);
        }

        [Test]
        public void Property_Member_Name_Isolates_Multiple_Extension_Props_OfSame_Type()
        {
            var instance = new object();

            const string ExpectedName = "Ronnie";
            const string ExpectedDescription = "Some guy.";

            string name = instance.Name(ExpectedName);
            string description = instance.Description(ExpectedDescription);

            Assert.IsFalse(name == description, "these values should differ to prove this");
            Assert.AreEqual(ExpectedName, instance.Name().Value);
            Assert.AreEqual(ExpectedDescription, instance.Description().Value);
        }
    }
}
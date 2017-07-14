using NUnit.Framework;
using Overby.Extensions.Attachments;
using System;
using static Overby.Extensions.Attachments.AttachmentExtensions;

namespace Tests
{
    [TestFixture]
    public class AttachmentTests
    {
        [Test]
        public void FixedNullAttachmentResultImplicitOperator()
        {
            AttachmentResult<string> r = null;
            string s = r;
            Assert.IsNull(s);
        }

        [Test]
        public void CanRemoveAttachments()
        {
            const string Ronnie = nameof(Ronnie);
            var instance = new object();
            var key = Guid.NewGuid().ToString();
            instance.SetAttached("Ronnie", key);
            Assert.IsTrue(instance.GetAttached(key).Found);
            instance.RemoveAttached(key);
            Assert.IsFalse(instance.GetAttached(key).Found);
        }

        [Test]
        public void CanClearAttachments()
        {
            var instance = new object();
            CollectionAssert.IsEmpty(instance.GetAttachmentKeys());
            instance.Name("");
            CollectionAssert.IsNotEmpty(instance.GetAttachmentKeys());
            instance.ClearAttached();
            CollectionAssert.IsEmpty(instance.GetAttachmentKeys());
        }


        [Test]
        public void CanGetDefaultAttachmentValue()
        {
            var factoryInvoked = false;
            var instance = new object();
            var attachmentResult1 = instance.GetOrSetAttached(() =>
            {
                factoryInvoked = true;
                return new object();
            });

            Assert.IsTrue(factoryInvoked);
            Assert.IsFalse(attachmentResult1.Found);
            Assert.NotNull(attachmentResult1.Value);
            Assert.AreNotSame(instance, attachmentResult1.Value);

            factoryInvoked = false;
            var attachmentResult2 = instance.GetOrSetAttached(() =>
            {
                factoryInvoked = true;
                return new object();
            });

            Assert.IsFalse(factoryInvoked);
            Assert.IsTrue(attachmentResult2.Found);
            Assert.NotNull(attachmentResult2.Value);
            Assert.AreNotSame(instance, attachmentResult2.Value);
        }

        [Test]
        public void Attachment_Found()
        {
            var instance = new object();
            instance.SetAttached(default(string));
            Assert.IsTrue(instance.GetAttached<string>().Found);
        }

        [Test]
        public void Attachment_Not_Found()
        {
            var instance = new object();
            var attachment = instance.GetAttached<string>();
            Assert.IsFalse(attachment.Found);
        }

        [Test]
        public void GetReferenceId_Returns_Unique_Guid_Per_Reference()
        {
            var instance1 = string.Intern("hello");
            var instance2 = new string(instance1.ToCharArray());

            // ensure refs not same
            Assert.AreNotSame(instance1, instance2);

            // ensure consistency
            Assert.AreEqual(instance1.GetReferenceId(), instance1.GetReferenceId());
            Assert.AreEqual(instance2.GetReferenceId(), instance2.GetReferenceId());

            // ensure unique ids
            Assert.AreNotEqual(instance1.GetReferenceId(), instance2.GetReferenceId());
        }

        [Test]
        public void Boxing_Of_Value_Types_Prevents_Attaching_To_Them()
        {
            var n = 0;
            n.Name("Ronnie");
            string name = n.Name().Value;
            Assert.IsNull(name);
        }

        [Test]
        public void CanCopyAttachments_WithPredicate()
        {
            const string ExpectedName = "Ronnie";
            var ExpectedId = new Random().Next();
            var o1 = new object();
            o1.Name(ExpectedName);
            o1.Id(ExpectedId);
            o1.Description("Some guy");

            var o2 = new object();

            // copy attachments from o1 -> o2
            // but not the description extension property
            o1.CopyAttachments(o2, k => k != o1.Description().AttachmentKey);

            Assert.AreEqual(ExpectedName, o2.Name().Value);
            Assert.AreEqual(ExpectedId, o2.Id().Value);
            Assert.IsNull(o2.Description().Value);
        }
    }
}

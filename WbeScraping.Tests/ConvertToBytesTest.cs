using NUnit.Framework;
using System;
using WebScraping.Core.Utils;

namespace WbeScraping.Tests
{
    public class ConvertToBytesTest
    {
        
        [Test]
        public void ConvertGbToBytes()
        {
            var valueGB = 5.0;
            var result = ConvertToBytes.ConvertNumberToBytes(valueGB, "GB");

            Assert.AreEqual((valueGB * 1024 * 1024 * 1024), result);
        }

        [Test]
        public void ConvertTbToBytes()
        {
            var valueTB = 2.0;
            var result = ConvertToBytes.ConvertNumberToBytes(valueTB, "TB");

            Assert.AreEqual((valueTB * 1024 * 1024 * 1024 * 1024), result);
        }

        [Test]
        public void ConvertMbToBytes()
        {
            var valueMB = 2.0;
            var result = ConvertToBytes.ConvertNumberToBytes(valueMB, "MB");

            Assert.AreEqual((valueMB * 1024 * 1024), result);
        }

        [Test]
        public void ConvertKbToBytes()
        {
            var valueKB = 2.0;
            var result = ConvertToBytes.ConvertNumberToBytes(valueKB, "KB");

            Assert.AreEqual((valueKB * 1024), result);
        }

        [Test]
        public void ConvertBytesToBytes()
        {
            var valueB = 200.0;
            var result = ConvertToBytes.ConvertNumberToBytes(valueB, "Bytes");

            Assert.AreEqual((valueB ), result);
        }

        //This test have a negative value to convert
        [Test]
        public void IncorrectValueToBytes()
        {
            var valueMB = -1.0;
            var ex  = Assert.Throws<ArgumentException>(() => ConvertToBytes.ConvertNumberToBytes(valueMB, "MB"));
            Assert.That(ex.ParamName, Is.EqualTo("size"));
            Assert.That(ex.Message, Is.EqualTo("Size cannot be less than zero (Parameter 'size')"));
        }
    }
}
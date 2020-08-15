using WebScraping.Core.Utils;
using Xunit;

namespace WebScraping.Tests
{
    public class ConvertToBytesTest
    {        

        public ConvertToBytesTest()
        {            
        }

        [Fact]
        public void ConvertGBToBytes()
        {
            var result = ConvertToBytes.ConvertStringNumberToBytes(5, "GB");

            Assert.Equal((5.0* 1024 * 1024 * 1024), result);
        }
    }
}

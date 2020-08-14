using System;
using System.Collections.Generic;
using System.Text;

namespace WebScraping.Domain.Utils
{
    public static class ConvertToBytes
    {
        public static double ConvertStringNumberToBytes(double size, string typeSize)
        {
            typeSize = typeSize.ToLower().Replace(" ", "");
            if (typeSize == "tb")
            {
                size *= 1024;
                typeSize = "gb";
            }
            if (typeSize == "gb")
            {
                size *= 1024;
                typeSize = "mb";
            }
            if (typeSize == "mb")
            {
                size *= 1024;
                typeSize = "kb";
            }
            if (typeSize == "kb")
            {
                size *= 1024;
            }
            return size;
        }

    }
}

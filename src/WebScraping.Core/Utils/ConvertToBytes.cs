using System;

namespace WebScraping.Core.Utils
{
    public static class ConvertToBytes
    {
        /// <summary>
        /// Use typeSize = "TB" or "GB" or "MB" or "KB"
        /// </summary>
        /// <param name="size"></param>
        /// <param name="typeSize"></param>
        /// <returns></returns>
        public static double ConvertNumberToBytes(double size, string typeSize)
        {
            if (size < 0)
                throw new ArgumentException("Size cannot be less than zero", "size");

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

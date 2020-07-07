using System;
using System.Text;

namespace KeepSafe.Helpers
{
    public class RandomizerHelper
    {
        static Random Random = new Random();
        public static int GetRandomInteger(int maxNumber = 2)
        {
            return Random.Next(maxNumber);
        }

        public static bool GetRandomBoolean()
        {
            return Random.Next(1000) % 2 == 0;
        }


        /// <summary>
        /// Get Random Image URL.
        /// </summary>
        /// <returns>URL of Random Image.</returns>
        public static string GetRandomImageUrl(int width = 600, int height = 600, ImageCategory category = ImageCategory.people, ImageFilter filter = ImageFilter.none, bool isUnique = true)
        {
            string url = string.Format("https://placeimg.com/{0}/{1}/{2}", width, height, category.ToString());

            string filename = "?filename=" + Guid.NewGuid().ToString("N") + ".jpg";

            switch (filter)
            {
                case ImageFilter.sepia:
                case ImageFilter.grayscale:
                    url += "/" + filter.ToString();
                    break;
            }

            return isUnique ? url + filename : url;
        }

        public static string GetRandomQRCODE(string code = null)
        {
            string random = code ?? GetRandomString(8);
            return $"https://api.qrserver.com/v1/create-qr-code/?size=200x200&data={random}";
        }

        public static string GetRandomString(int size, bool lowerCase = false)
        {
            var builder = new StringBuilder(size);

            // Unicode/ASCII Letters are divided into two blocks
            // (Letters 65–90 / 97–122):
            // The first group containing the uppercase letters and
            // the second group containing the lowercase.  

            // char is a single Unicode character  
            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26; // A...Z or a..z: length=26  

            for (var i = 0; i < size; i++)
            {
                var @char = (char)Random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }

            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }
    }

    public enum ImageCategory
    {
        nature, people, animals, arch, tech
    }

    public enum ImageFilter
    {
        none, grayscale, sepia
    }
}

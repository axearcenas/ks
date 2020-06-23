using System;
namespace KeepSafe.Helpers
{
    public class RandomizerHelper
    {
        static Random Random = new Random();
        public static int GetRandomInteger(int maxNumber = 2)
        {
            return Random.Next(maxNumber);
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

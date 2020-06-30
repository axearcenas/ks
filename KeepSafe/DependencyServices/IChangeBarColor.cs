using System;
using FFImageLoading;

namespace KeepSafe
{
    [Preserve(AllMembers = true)]
    public interface IChangeBarColor
    {
        void ChangeColor(BarStyle barStyle);
    }

    [Preserve(AllMembers = true)]
    public enum BarStyle
    {
        Light,
        Dark
    }
}

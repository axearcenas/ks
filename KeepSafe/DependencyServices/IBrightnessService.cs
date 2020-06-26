using System;

namespace KeepSafe
{
    public interface IBrightnessService
    {
        void SetBrightness(float brightness);
        void ResetBrightness();
    }
}

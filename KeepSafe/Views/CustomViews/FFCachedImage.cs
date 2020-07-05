using System;
using FFImageLoading.Forms;
using FFImageLoading.Transformations;
using Xamarin.Forms;

namespace KeepSafe
{
    public class FFCachedImage : CachedImage
    {
        public static readonly BindableProperty CachedImageSourceProperty = BindableProperty.Create(nameof(CachedImageSource), typeof(string), typeof(FFCachedImage), string.Empty, propertyChanged: CachedImageSource_PropertyChanged);

        async static void CachedImageSource_PropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (newValue != null && newValue.ToString() != string.Empty)
            {
                App.Log("Setting source... " + newValue);
                var image = bindable as FFCachedImage;

                //if (newValue.ToString().Contains("http"))
                //{
                //    if (image.AddTransparentImage)
                //        image.Source = "Empty";
                //    image.Source = await ImageCache.GetInstance.LoadImagePathFromUrlAsync(newValue.ToString(), image.IsForceDownload);
                //}
                //else
                //{
                //    image.Source = newValue.ToString();
                //}
                image.Source = newValue.ToString();
            }
        }

        public string CachedImageSource
        {
            get { return (string)GetValue(CachedImageSourceProperty); }
            set { SetValue(CachedImageSourceProperty, value); }
        }

        public static readonly BindableProperty IsForceDownloadProperty = BindableProperty.Create(nameof(IsForceDownload), typeof(bool), typeof(FFCachedImage), false);
        public bool IsForceDownload
        {
            get { return (bool)GetValue(IsForceDownloadProperty); }
            set { SetValue(IsForceDownloadProperty, value); }
        }

        public static readonly BindableProperty AddTransparentImageProperty = BindableProperty.Create(nameof(AddTransparentImage), typeof(bool), typeof(FFCachedImage), true);

        public bool AddTransparentImage
        {
            get { return (bool)GetValue(AddTransparentImageProperty); }
            set { SetValue(AddTransparentImageProperty, value); }
        }

        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(FFCachedImage), Color.Default, BindingMode.TwoWay, propertyChanged: OnPropertyChanged);

        private static void OnPropertyChanged(BindableObject bindable, object oldColor, object newColor)
        {
            var oldcolor = (Color)oldColor;
            var color = (Color)newColor;
            var view = (FFCachedImage)bindable;

            if (color.Equals(Color.Default))
            {
                if (view.Transformations.Exists(x => x.Equals(view.roundedTransformation)))
                {
                    view.Transformations.Remove(view.roundedTransformation);
                }
            }
            else
            {
                if (!oldcolor.Equals(color))
                {
                    view.roundedTransformation = new RoundedTransformation();
                    view.roundedTransformation.BorderHexColor = string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", (int)(color.A * 255), (int)(color.R * 255), (int)(color.G * 255), (int)(color.B * 255));
                    view.roundedTransformation.Radius = 10.ScaleWidth();
                    view.roundedTransformation.BorderSize = Device.RuntimePlatform == Device.iOS ? 15.ScaleWidth() : 7.ScaleWidth();
                    view.Transformations.Add(view.roundedTransformation);

                }
            }

            view.ReloadImage();
        }

        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        RoundedTransformation roundedTransformation = new RoundedTransformation();

        public FFCachedImage()
        {
            this.CacheDuration = TimeSpan.FromDays(90);
        }
    }
}

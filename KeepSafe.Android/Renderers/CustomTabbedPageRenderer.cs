using System;
using System.ComponentModel;
using Android.Content;
using Android.Graphics;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using KeepSafe.Droid.Renderers;
using KeepSafe;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;
using Android.Views;
using Android.Support.Design.Internal;
using Android.Support.V7.Widget;
using Android.Graphics.Drawables;
using Java.Lang;
using Android.Media;
using Android.Net;
using System.Linq;

[assembly: ExportRenderer(typeof(TabbedPage), typeof(CustomTabbedPageRenderer))]
namespace KeepSafe.Droid.Renderers
{
    public class CustomTabbedPageRenderer : TabbedPageRenderer
    {

        private bool _isConfigured = false;
        BottomNavigationView bottomNavigationView;
        Android.Widget.RelativeLayout pagerView;
        Context _context;
        TabbedPage thisXFView;

        public CustomTabbedPageRenderer(Context context) : base(context)
        {
            _context = context;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            //_pager = (ViewPager)ViewGroup.GetChildAt(0);
            //_layout = (TabLayout)ViewGroup.GetChildAt(1);

            //_layout = FindViewById<Android.Support.Design.Widget.TabLayout>(Resource.Id.sliding_tabs);
            ////_pager = (ViewPager)_layout.GetChildAt(1);
            thisXFView = (TabbedPage)sender;
            //Android.Graphics.Color selectedColor = Android.Graphics.Color.Gray;
            //Android.Graphics.Color unselectedColor = Android.Graphics.Color.White;
            //if (control != null)
            //{
            //    selectedColor = control.SelectedIconColor.ToAndroid();
            //    unselectedColor = control.UnselectedIconColor.ToAndroid();
            //}
            ////else
            ////{
            ////    selectedColor = new Android.Graphics.Color(ContextCompat.GetColor(Context, Resource.Color.tabBarSelected));
            ////    unselectedColor = new Android.Graphics.Color(ContextCompat.GetColor(Context, Resource.Color.tabBarUnselected));
            ////}

            //for (int i = 0; i < _layout.TabCount; i++)
            //{
            //    var tab = _layout.GetTabAt(i);
            //    var icon = tab.Icon;
            //    if (icon != null)
            //    {
            //        var color = tab.IsSelected ? selectedColor : unselectedColor;
            //        icon = Android.Support.V4.Graphics.Drawable.DrawableCompat.Wrap(icon);
            //        icon.SetColorFilter(color, PorterDuff.Mode.SrcIn);
            //    }
            //}
        }

        protected override async void OnLayout(bool changed, int l, int t, int r, int b)
        {
            ViewGroup viewGroup = GetChildAt(0) as ViewGroup;
            if (viewGroup.GetChildAt(1) is BottomNavigationView tab)
            {
                bottomNavigationView = tab;
                //bottomNavigationView.BackgroundTintMode =
                if (tab.GetChildAt(0) is BottomNavigationMenuView menuView)
                {
                    for (int menuItemIndex = 0; menuItemIndex < menuView.ChildCount; menuItemIndex++)
                    {
                        if (menuView.GetChildAt(menuItemIndex) is BottomNavigationItemView itemView)
                        {
                            var text = itemView.GetChildAt(1);
                            var text2 = itemView.GetChildAt(2);
                            if (itemView.GetChildAt(0) is AppCompatImageView image)
                            {
                                //if (image.Selected)
                                //{
                                if (Element.Children[menuItemIndex] is Page navPage
                                    && navPage.IconImageSource is FileImageSource fileImageSource)
                                {
                                    //Drawable.CreateFromPath(fileImageSource.File);
                                    //Android.Support.V4.Graphics.Drawable.DrawableCompat.Wrap();
                                    int resourceId = Resources.GetIdentifier(fileImageSource.File.ToLower(), "drawable", _context.PackageName);
                                    image.SetImageIcon(icon: Icon.CreateWithResource(_context, resourceId));
                                    image.SetImageResource(resourceId);
                                    //image.SetColorFilter()
                                    //image.
                                }
                                //}
                            }
                        }
                    }
                }
                if (viewGroup.GetChildAt(0) is Android.Widget.RelativeLayout page)
                {
                    pagerView = page;
                }


            }
            base.OnLayout(changed, l, t, r, b);
        }


        int IdFromTitle(string title, Type type)
        {
            string name = System.IO.Path.GetFileNameWithoutExtension(title);
            int id = GetId(type, name);
            return id;
        }

        int GetId(Type type, string memberName)
        {
            object value = type.GetFields().FirstOrDefault(p => p.Name == memberName)?.GetValue(type)
                ?? type.GetProperties().FirstOrDefault(p => p.Name == memberName)?.GetValue(type);
            if (value is int)
                return (int)value;
            return 0;
        }
    }
}
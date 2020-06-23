//using System;
//using System.Linq;
//using System.Reflection;
//using Xamarin.Forms;
//using Xamarin.Forms.Xaml;

//namespace KeepSafe
//{
//    public class ThemeColorXamlExtension : IMarkupExtension
//    {
//        public ThemeColor ThemeColor { get; set; }

//        public object ProvideValue(IServiceProvider serviceProvider)
//        {
//            return typeof(Theme).GetValue(ThemeColor.ToString());
//        }
//    }

//    public static class ValueGetter
//    {
//        public static object GetValue(this Type type,string propertyName)
//        {
//            FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
//            //foreach (FieldInfo fi in fields)
//            //{
//            //    App.Log($"Searching for {propertyName}: Checking [{fi.Name}] with a value of [{fi.GetValue(null).ToString()}]");
//            //    Console.WriteLine(fi.Name);
//            //    Console.WriteLine(fi.GetValue(null).ToString());
//            //}
//            return fields.FirstOrDefault<FieldInfo>((fieldInfo) => fieldInfo.Name.Equals(propertyName)).GetValue(null);
            
//        }
//    }
//}

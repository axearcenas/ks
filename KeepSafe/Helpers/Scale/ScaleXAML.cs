using System;
using System.Data;
using System.Globalization;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KeepSafe
{
    public class ScaleXAML : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter.ToString().Contains("="))
            {
                var paramType = parameter.ToString().Remove(parameter.ToString().IndexOf("=", StringComparison.CurrentCulture));
                var param = parameter.ToString().Remove(0, paramType.Length + 1);

                if (param.Contains("|"))
                {
                    string[] paramPlatform = param.Split('|');
                    param = Device.RuntimePlatform == Device.Android ? paramPlatform[0] : paramPlatform[1];
                }

                double convertedValue;

                if (paramType.Contains("height"))
                {
                    if (double.TryParse(param, NumberStyles.Number, CultureInfo.InvariantCulture, out convertedValue))
                        return convertedValue.ScaleHeight();
                }
                else if (paramType.Contains("height-responsive")) //DO NOT USE
                {
                    if (double.TryParse(param, NumberStyles.Number, CultureInfo.InvariantCulture, out convertedValue))
                        return convertedValue.ScaleHeightResponsive();
                }
                else if (paramType.Contains("width-responsive"))
                {
                    if (double.TryParse(param, NumberStyles.Number, CultureInfo.InvariantCulture, out convertedValue))
                        return convertedValue.ScaleWidthResponsive();
                }
                else if (paramType.Contains("width"))
                {
                    if (double.TryParse(param, NumberStyles.Number, CultureInfo.InvariantCulture, out convertedValue))
                        return convertedValue.ScaleWidth();
                }
                else if (paramType.Contains("platform-thickness")) // example platform-thickness='[android]|[iOS]'
                {
                    return ConvertToThicknessProperty(param);
                }
                else if (paramType.Contains("stroke-thickness"))
                {
                    double val = double.Parse(param);
                    if (Device.Idiom == TargetIdiom.Phone)
                        return val.ScaleWidth();
                    return val.ScaleHeight();
                }
                else if (paramType.Contains("thickness"))
                {
                    return ConvertToThicknessProperty(param);
                }
                else if (paramType.Contains("fontSize"))
                {
                    if (double.TryParse(param, NumberStyles.Number, CultureInfo.InvariantCulture, out convertedValue))
                        return convertedValue.ScaleHeight() - (Device.RuntimePlatform == Device.iOS ? 0.5 : 0);
                }
                else if (paramType.Contains("absolute-WH"))
                {
                    return ConvertToAbsoluteProperty(param);
                }
                else if (paramType.Contains("absolute-NONE"))
                {
                    return ConvertToRectangleProperty(param);
                }
                throw new InvalidOperationException($"[{parameter.ToString()}] is an Invalid parameters. Supported parameters are height, width, thickness, fontSize, absolute-WH, absolute-NONE");
            }

            // default
            if (parameter.ToString().Contains(",") == true)
            {
                return ConvertToThicknessProperty(parameter.ToString());
            }

            return (double.Parse(parameter.ToString()) * (App.ScreenHeight / 568.0));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        object ConvertToThicknessProperty(string param)
        {
            double l, t, r, b;
            string[] thickness = param.Split(',');

            if (thickness.Length == 4)
            {
                if (double.TryParse(thickness[0], NumberStyles.Number, CultureInfo.InvariantCulture, out l) && double.TryParse(thickness[1], NumberStyles.Number, CultureInfo.InvariantCulture, out t) && double.TryParse(thickness[2], NumberStyles.Number, CultureInfo.InvariantCulture, out r) && double.TryParse(thickness[3], NumberStyles.Number, CultureInfo.InvariantCulture, out b))
                {
                    return new Thickness(l * (App.ScreenWidth / 320.0), t * (App.ScreenHeight / 568.0), r * (App.ScreenWidth / 320.0), b * (App.ScreenHeight / 568.0));
                }
            }
            throw new InvalidOperationException("Cannot convert thickness");
        }

        object ConvertToPlatformThicknessProperty(string param)
        {
            string[] paramPlatform = param.Split('|');
            double l, t, r, b;
            string[] thickness = Device.RuntimePlatform == Device.Android ? paramPlatform[0].Split(',') : paramPlatform[1].Split(',');


            if (thickness.Length == 4)
            {
                if (double.TryParse(thickness[0], NumberStyles.Number, CultureInfo.InvariantCulture, out l) && double.TryParse(thickness[1], NumberStyles.Number, CultureInfo.InvariantCulture, out t) && double.TryParse(thickness[2], NumberStyles.Number, CultureInfo.InvariantCulture, out r) && double.TryParse(thickness[3], NumberStyles.Number, CultureInfo.InvariantCulture, out b))
                {
                    return new Thickness(l * (App.ScreenWidth / 320.0), t * (App.ScreenHeight / 568.0), r * (App.ScreenWidth / 320.0), b * (App.ScreenHeight / 568.0));
                }
            }
            throw new InvalidOperationException("Cannot convert platform-thickness");
        }

        object ConvertToAbsoluteProperty(string param)
        {
            double l, t, r, b;
            string[] thickness = param.Split(',');

            if (thickness.Length == 4)
            {
                if (double.TryParse(thickness[0], NumberStyles.Number, CultureInfo.InvariantCulture, out l) && double.TryParse(thickness[1], NumberStyles.Number, CultureInfo.InvariantCulture, out t) && double.TryParse(thickness[2], NumberStyles.Number, CultureInfo.InvariantCulture, out r) && double.TryParse(thickness[3], NumberStyles.Number, CultureInfo.InvariantCulture, out b))
                {
                    return new Rectangle(l, t, r * (App.ScreenWidth / 320.0), b * (App.ScreenHeight / 568.0));
                }
            }
            throw new InvalidOperationException("Cannot convert rectangle");
        }

        object ConvertToRectangleProperty(string param)
        {
            double l, t, r, b;
            string[] thickness = param.Split(',');

            if (thickness.Length == 4)
            {
                if (double.TryParse(thickness[0], NumberStyles.Number, CultureInfo.InvariantCulture, out l) && double.TryParse(thickness[1], NumberStyles.Number, CultureInfo.InvariantCulture, out t) && double.TryParse(thickness[2], NumberStyles.Number, CultureInfo.InvariantCulture, out r) && double.TryParse(thickness[3], NumberStyles.Number, CultureInfo.InvariantCulture, out b))
                {
                    return new Rectangle(l * (App.ScreenWidth / 320.0), t * (App.ScreenHeight / 568.0), (r >= 0 ? r * (App.ScreenWidth / 320.0) : AbsoluteLayout.AutoSize), (b >= 0 ? b * (App.ScreenHeight / 568.0) : AbsoluteLayout.AutoSize));
                }
            }
            throw new InvalidOperationException("Cannot convert thickness");
        }
    }

    public class ScaleHeight : IMarkupExtension
    {

        public string Value { get; set; } = null;
        public string Android { get; set; } = null;
        public bool IsResponsive { get; set; } = false;

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            Type property = typeof(double);
            App.Log($"SCALING_STARTING : IsResponsive? => {IsResponsive} | Value => {Android ?? Value} {(serviceProvider.GetService<IProvideValueTarget>().TargetProperty as BindableProperty) == null}", this.GetType().ToString());
            IProvideValueTarget provideValueTarget = serviceProvider.GetService<IProvideValueTarget>();
            if (provideValueTarget != null)
            {
                if (provideValueTarget.TargetObject is Setter setter)
                    property = setter.Property.ReturnType;
                else if (provideValueTarget.TargetProperty is BindableProperty bindableProperty)
                    property = bindableProperty.ReturnType;
                if (!string.IsNullOrEmpty(Value))
                {
                    if (Value.Contains("|"))
                    {
                        string[] paramPlatform = Value.Split('|');
                        Value = Device.RuntimePlatform == Device.Android ? paramPlatform[0] : paramPlatform[1];
                    }
                    double.TryParse(Android ?? Value, out double numberValue);
                    var returnData = Convert.ChangeType(IsResponsive ? (int)numberValue.ScaleHeightResponsive() : (int)numberValue.ScaleHeight(), Type.GetType(property.ToString()));
                    App.Log($"SCALING_STARTING : IsResponsive? => {IsResponsive} | Value => {returnData} :TYPE {returnData.GetType()} | {Type.GetType(property.ToString())}", this.GetType().ToString());
                    if (returnData != null)
                        return returnData;
                    //return Convert.ChangeType( IsResponsive ? (int)numberValue.ScaleHeightResponsive() : (int)numberValue.ScaleHeight(), property.GetType());
                    //switch (property.ToString())
                    //{
                    //    case "System.Int16": case "System.Int32": case "System.Int64": App.Log($"END INT", this.GetType().ToString()); return IsResponsive ? (int)numberValue.ScaleHeightResponsive()  : (int)numberValue.ScaleHeight();
                    //    case "System.Double": App.Log($"END DOUBLE", this.GetType().ToString()); return IsResponsive ? (double)numberValue.ScaleHeightResponsive() : (double)numberValue.ScaleHeight();
                    //    case "System.Single": App.Log($"END FLOAT", this.GetType().ToString()); return IsResponsive ? (float)numberValue.ScaleHeightResponsive() : (float)numberValue.ScaleHeight();
                    //}
                }
            }
            throw new InvalidOperationException($"Cannot convert Height[{Value}]");
        }
    }

    public class ScaleWidth : IMarkupExtension
    {
        public string Value { get; set; } = null;
        public string Android { get; set; } = null;
        public bool IsResponsive { get; set; } = false;

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            Type property = typeof(double);
            App.Log($"SCALING_STARTING : IsResponsive? => {IsResponsive} | Value => {Android ?? Value} {(serviceProvider.GetService<IProvideValueTarget>().TargetProperty as BindableProperty) == null}", this.GetType().ToString());
            IProvideValueTarget provideValueTarget = serviceProvider.GetService<IProvideValueTarget>();
            if (provideValueTarget != null)
            {
                if (provideValueTarget.TargetObject is Setter setter)
                    property = setter.Property.ReturnType;
                else if (provideValueTarget.TargetProperty is BindableProperty bindableProperty)
                    property = bindableProperty.ReturnType;
                if (!string.IsNullOrEmpty(Value))
                {
                    if (Value.Contains("|"))
                    {
                        string[] paramPlatform = Value.Split('|');
                        Value = Device.RuntimePlatform == Device.Android ? paramPlatform[0] : paramPlatform[1];
                    }
                    double.TryParse(Android ?? Value, out double numberValue);
                    var returnData = Convert.ChangeType(IsResponsive ? (int)numberValue.ScaleWidthResponsive() : (int)numberValue.ScaleWidth(), Type.GetType(property.ToString()));
                    App.Log($"SCALING_ENDING : IsResponsive? => {IsResponsive} | Value => {returnData} :TYPE {returnData.GetType()} | {property.ToString()}", this.GetType().ToString());
                    if(returnData != null)
                        return returnData;
                    //return Convert.ChangeType(IsResponsive ? (int)numberValue.ScaleHeightResponsive() : (int)numberValue.ScaleHeight(), property.GetType());

                    //switch (property.ToString())
                    //{
                    //    case "System.Int16": case "System.Int32": case "System.Int64": App.Log($"END INT", this.GetType().ToString()); return IsResponsive ? (int)numberValue.ScaleWidthResponsive() : (int)numberValue.ScaleWidth();
                    //    case "System.Double": App.Log($"END DOUBLE", this.GetType().ToString()); return IsResponsive ? (double)numberValue.ScaleWidthResponsive() : (double)numberValue.ScaleWidth();
                    //    case "System.Single": App.Log($"END FLOAT", this.GetType().ToString()); return IsResponsive ? (float)numberValue.ScaleWidthResponsive() : (float)numberValue.ScaleWidth();
                    //}
                }
            }
            throw new InvalidOperationException($"Cannot convert Width[{Value}]");
        }
    }
    #region GRID
    public class ScaleRowDefinition : IMarkupExtension
    {
        public string Value { get; set; } = null;
        public string Android { get; set; } = null;
        public bool IsResponsive { get; set; } = false;

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            Type property = typeof(double);
            App.Log($"SCALING_STARTING : IsResponsive? => {IsResponsive} | Value => {Android ?? Value} {(serviceProvider.GetService<IProvideValueTarget>().TargetProperty as BindableProperty) == null}", this.GetType().ToString());
            IProvideValueTarget provideValueTarget = serviceProvider.GetService<IProvideValueTarget>();
            if (provideValueTarget != null)
            {
                if (provideValueTarget.TargetObject is Setter setter)
                    property = setter.Property.ReturnType;
                else if (provideValueTarget.TargetProperty is BindableProperty bindableProperty)
                    property = bindableProperty.ReturnType;
                if (!string.IsNullOrEmpty(Value))
                {
                    Value = Device.RuntimePlatform == Device.Android ? Android ?? Value : Value;
                    if (property == typeof(RowDefinitionCollection))
                    {
                        RowDefinitionCollection rowDefinitions = new RowDefinitionCollection();
                        foreach(string value in Value.Split(','))
                        {
                            if (value.Equals("*"))
                                rowDefinitions.Add(new RowDefinition() { Height = GridLength.Star });
                            else if (value.Contains("*"))
                            {
                                double.TryParse(value.Replace("*", ""), out double starValue);
                                rowDefinitions.Add(new RowDefinition() { Height = new GridLength(starValue, GridUnitType.Star) });
                            }
                            else if (value.ToLower().Equals("auto"))
                                rowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                            else
                            {
                                if(double.TryParse(value.Replace("*", ""), out double absoluteValue))
                                    rowDefinitions.Add(new RowDefinition() { Height = new GridLength( IsResponsive ? absoluteValue.ScaleHeightResponsive() : absoluteValue.ScaleHeight(), GridUnitType.Absolute) });
                                else
                                    throw new InvalidOperationException($"Cannot convert Width[{Value}]");
                            }
                        }
                        //App.Log($"SCALING_ENDING : IsResponsive? => {IsResponsive} | Value => {Android ?? Value}", this.GetType().ToString());
                        App.Log($"SCALING_ENDING : IsResponsive? => {IsResponsive} | Value => {Android ?? Value} ||| RowDefinationCollection: {JsonConvert.SerializeObject(rowDefinitions)}", this.GetType().ToString());
                        return rowDefinitions;
                    }
                }
            }
            throw new InvalidOperationException($"Cannot convert Width[{Value}]");
        }
    }

    public class ScaleColumnDefinition : IMarkupExtension
    {
        public string Value { get; set; } = null;
        public string Android { get; set; } = null;
        public bool IsResponsive { get; set; } = false;

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            Type property = typeof(double);
            App.Log($"SCALING_STARTING : IsResponsive? => {IsResponsive} | Value => {Android ?? Value} {(serviceProvider.GetService<IProvideValueTarget>().TargetProperty as BindableProperty) == null}", this.GetType().ToString());
            IProvideValueTarget provideValueTarget = serviceProvider.GetService<IProvideValueTarget>();
            if (provideValueTarget != null)
            {
                if (provideValueTarget.TargetObject is Setter setter)
                    property = setter.Property.ReturnType;
                else if (provideValueTarget.TargetProperty is BindableProperty bindableProperty)
                    property = bindableProperty.ReturnType;
                if (!string.IsNullOrEmpty(Value))
                {
                    Value = Device.RuntimePlatform == Device.Android ? Android ?? Value : Value;
                    if (property == typeof(ColumnDefinitionCollection))
                    {
                        ColumnDefinitionCollection columnDefinitions = new ColumnDefinitionCollection();
                        foreach (string value in Value.Split(','))
                        {
                            if (value.Equals("*"))
                                columnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });
                            else if (value.Contains("*"))
                            {
                                double.TryParse(value.Replace("*", ""), out double starValue);
                                columnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(starValue, GridUnitType.Star) });
                            }
                            else if (value.ToLower().Equals("auto"))
                                columnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                            else
                            {
                                if (double.TryParse(value.Replace("*", ""), out double absoluteValue))
                                    columnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(IsResponsive ? absoluteValue.ScaleWidthResponsive() : absoluteValue.ScaleWidth(), GridUnitType.Absolute) });
                                else
                                    throw new InvalidOperationException($"Cannot convert Width[{Value}]");
                            }
                        }
                        //App.Log($"SCALING_ENDING : IsResponsive? => {IsResponsive} | Value => {Android ?? Value}", this.GetType().ToString());
                        App.Log($"SCALING_ENDING : IsResponsive? => {IsResponsive} | Value => {Android ?? Value} ||| RowDefinationCollection: {JsonConvert.SerializeObject(columnDefinitions)}", this.GetType().ToString());
                        return columnDefinitions;
                    }
                }
            }
            throw new InvalidOperationException($"Cannot convert Width[{Value}]");
        }
    }

    public class ScaleGridHeight : IMarkupExtension
    {
        public double? Value { get; set; } = null;
        public double? Android { get; set; } = null;
        public bool IsResponsive { get; set; } = false;

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            App.Log($"SCALING_STARTING : IsResponsive? => {IsResponsive} | Value => {Android ?? Value}", this.GetType().ToString());
            return new GridLength(IsResponsive ? (double)(Android ?? Value)?.ScaleHeightResponsive(Value) : (double)(Android ?? Value)?.ScaleHeight(Value));
        }
    }

    public class ScaleGridWidth : IMarkupExtension
    {
        public double? Value { get; set; } = null;
        public double? Android { get; set; } = null;
        public bool IsResponsive { get; set; } = false;

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            App.Log($"SCALING_STARTING : IsResponsive? => {IsResponsive} | Value => {Android ?? Value}", this.GetType().ToString());
            return new GridLength(IsResponsive ? (double)(Android ?? Value)?.ScaleWidthResponsive(Value) : (double)(Android ?? Value)?.ScaleWidth(Value));
        }
    }
    #endregion
    public class ScaleHeightDouble : IMarkupExtension
    {
        public double? Value { get; set; } = null;
        public double? Android { get; set; } = null;
        public bool IsResponsive { get; set; } = false;


        public object ProvideValue(IServiceProvider serviceProvider)
        {
            App.Log($"SCALING_STARTING : IsResponsive? => {IsResponsive} | Value => {Android ?? Value}", this.GetType().ToString());
            return IsResponsive ? (double)(Android ?? Value)?.ScaleHeightResponsive(Value) : (double)(Android ?? Value)?.ScaleHeight(Value);
        }
    }

    public class ScaleHeightInt : IMarkupExtension
    {
        public int? Value { get; set; } = null;
        public int? Android { get; set; } = null;
        public bool IsResponsive { get; set; } = false;


        public object ProvideValue(IServiceProvider serviceProvider)
        {
            App.Log($"SCALING_STARTING : IsResponsive? => {IsResponsive} | Value => {Android ?? Value}", this.GetType().ToString());
            return IsResponsive ? (int)(Android ?? Value)?.ScaleHeightResponsive(Value) : (int)(Android ?? Value)?.ScaleHeight(Value);
        }
    }

    public class ScaleHeightFloat : IMarkupExtension
    {
        public float? Value { get; set; } = null;
        public float? Android { get; set; } = null;
        public bool IsResponsive { get; set; } = false;

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            App.Log($"SCALING_STARTING : IsResponsive? => {IsResponsive} | Value => {Android ?? Value}", this.GetType().ToString());
            return IsResponsive ? (float)(Android ?? Value)?.ScaleWidthResponsive(Value) : (float)(Android ?? Value)?.ScaleWidth(Value);
        }
    }

    public class ScaleWidthDouble : IMarkupExtension
    {
        public double? Value { get; set; } = null;
        public double? Android { get; set; } = null;
        public bool IsResponsive { get; set; } = false;


        public object ProvideValue(IServiceProvider serviceProvider)
        {
            App.Log($"SCALING_STARTING : IsResponsive? => {IsResponsive} | Value => {Android ?? Value}", this.GetType().ToString());
            return IsResponsive ? (double)(Android ?? Value)?.ScaleWidthResponsive(Value) : (double)(Android ?? Value)?.ScaleWidth(Value);
        }
    }

    public class ScaleWidthInt : IMarkupExtension
    {
        public int? Value { get; set; } = null;
        public int? Android { get; set; } = null;
        public bool IsResponsive { get; set; } = false;


        public object ProvideValue(IServiceProvider serviceProvider)
        {
            App.Log($"SCALING_STARTING : IsResponsive? => {IsResponsive} | Value => {Android ?? Value}", this.GetType().ToString());
            return IsResponsive ? (int)(Android ?? Value)?.ScaleWidthResponsive(Value) : (int)(Android ?? Value)?.ScaleWidth(Value);
        }
    }

    public class ScaleWidthFloat : IMarkupExtension
    {
        public float? Value { get; set; } = null;
        public float? Android { get; set; } = null;
        public bool IsResponsive { get; set; } = false;

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            App.Log($"SCALING_STARTING : IsResponsive? => {IsResponsive} | Value => {Android ?? Value}", this.GetType().ToString());
            return IsResponsive ? (float)(Android ?? Value)?.ScaleWidth(Value) : (float)(Android ?? Value)?.ScaleWidth(Value);
        }
    }

    public class ScaleThickness : IMarkupExtension
    {
        public string Value { get; set; }
        public bool IsResponsive { get; set; } = false;


        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (!string.IsNullOrEmpty(Value))
            {
                App.Log($"Original Value Scaling: {this.GetType().ToString()}: {Value}");

                if (Value.Contains("|"))
                {
                    string[] paramPlatform = Value.Split('|');
                    Value = Device.RuntimePlatform == Device.Android ? paramPlatform[0] : paramPlatform[1];
                }

                App.Log($"Value Scaling: {this.GetType().ToString()}: {Value}");

                ThicknessTypeConverter thicknessTypeConverter = new ThicknessTypeConverter();
                return IsResponsive ? ((Thickness)thicknessTypeConverter.ConvertFromInvariantString(Value)).ScaleThicknessResponsive() : ((Thickness)thicknessTypeConverter.ConvertFromInvariantString(Value)).ScaleThickness();
            }

            throw new InvalidOperationException("Cannot convert thickness");
        }
    }

    public class ScaleFontSize : IMarkupExtension
    {
        public double? Value { get; set; } = null;
        public double? Android { get; set; } = null;

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            App.Log($"SCALING_STARTING : Value => {Android ?? Value}", this.GetType().ToString());
            return (Android ?? Value)?.ScaleFont(Value);
        }
    }

    public class ScaleCornerRadius : IMarkupExtension
    {
        public string Value { get; set; }
        public bool IsResponsive { get; set; } = false;


        public object ProvideValue(IServiceProvider serviceProvider)
        {
            App.Log($"SCALING_STARTING : IsResponsive? => {IsResponsive} | Value => {Value}", this.GetType().ToString());
            if (!string.IsNullOrEmpty(Value))
            {
                if (Value.Contains("|"))
                {
                    string[] paramPlatform = Value.Split('|');
                    Value = Device.RuntimePlatform == Device.Android ? paramPlatform[0] : paramPlatform[1];
                }

                CornerRadiusTypeConverter cornerRadiusTypeConverter = new CornerRadiusTypeConverter();
                return IsResponsive ? ((Xamarin.Forms.CornerRadius)cornerRadiusTypeConverter.ConvertFromInvariantString(Value)).ScaleCornerRadiusResponsive() : ((Xamarin.Forms.CornerRadius)cornerRadiusTypeConverter.ConvertFromInvariantString(Value)).ScaleCornerRadius();
            }
            throw new InvalidOperationException("Cannot convert CornerRadius");
        }
    }

    public class ScaleThicknessWidth : IMarkupExtension
    {
        public string Value { get; set; }
        public bool IsResponsive { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            App.Log($"SCALING_STARTING : IsResponsive? => {IsResponsive} | Value => {Value}", this.GetType().ToString());
            if (!string.IsNullOrEmpty(Value))
            {
                if (Value.Contains("|"))
                {
                    string[] paramPlatform = Value.Split('|');
                    Value = Device.RuntimePlatform == Device.Android ? paramPlatform[0] : paramPlatform[1];
                }

                ThicknessTypeConverter thicknessTypeConverter = new ThicknessTypeConverter();
                return ((Thickness)thicknessTypeConverter.ConvertFromInvariantString(Value)).ScaleThicknessWidth();
            }

            throw new InvalidOperationException("Cannot convert Thickness based on Width");
        }
    }
}

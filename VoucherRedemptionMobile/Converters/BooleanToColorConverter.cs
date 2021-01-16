namespace VoucherRedemptionMobile.Converters
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using Xamarin.Forms;
    using Xamarin.Forms.Internals;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Xamarin.Forms.IValueConverter" />
    [Preserve(AllMembers = true)]
    [ExcludeFromCodeCoverage]
    public class BooleanToColorConverter : IValueConverter
    {
        #region Methods

        /// <summary>
        /// This method is used to convert the bool to color.
        /// </summary>
        /// <param name="value">Gets the value.</param>
        /// <param name="targetType">Gets the target type.</param>
        /// <param name="parameter">Gets the parameter.</param>
        /// <param name="culture">Gets the culture.</param>
        /// <returns>
        /// Returns the color.
        /// </returns>
        /// <remarks>
        /// To be added.
        /// </remarks>
        public Object Convert(Object value,
                              Type targetType,
                              Object parameter,
                              CultureInfo culture)
        {
            if (parameter == null)
            {
                return Color.Default;
            }

            switch(parameter.ToString())
            {
                case "0" when (Boolean)value:
                    return Color.FromRgba(255, 255, 255, 0.6);
                case "1" when (Boolean)value:
                    return Color.FromHex("#FF4A4A");
                case "2" when (Boolean)value:
                    return Color.FromHex("#FF4A4A");
                case "2":
                    return Color.FromHex("#ced2d9");
                case "3" when (Boolean)value:
                    return Color.FromHex("#959eac");
                case "3":
                    return Color.FromHex("#ced2d9");
                case "4" when (Boolean)value:
                    Application.Current.Resources.TryGetValue("PrimaryColor", out var retVal);
                    return (Color)retVal;
                case "4":
                    Application.Current.Resources.TryGetValue("Gray-600", out var outVal);
                    return (Color)outVal;
                case "5" when (Boolean)value:
                    Application.Current.Resources.TryGetValue("Green", out var retGreen);
                    return (Color)retGreen;
                case "5":
                    Application.Current.Resources.TryGetValue("Red", out var retRed);
                    return (Color)retRed;
                default:
                    return Color.Transparent;
            }
        }

        /// <summary>
        /// This method is used to convert the color to bool.
        /// </summary>
        /// <param name="value">Gets the value.</param>
        /// <param name="targetType">Gets the target type.</param>
        /// <param name="parameter">Gets the parameter.</param>
        /// <param name="culture">Gets the culture.</param>
        /// <returns>
        /// Returns the string.
        /// </returns>
        /// <remarks>
        /// To be added.
        /// </remarks>
        public Object ConvertBack(Object value,
                                  Type targetType,
                                  Object parameter,
                                  CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}
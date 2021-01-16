namespace VoucherRedemptionMobile.Converters
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using Controls;
    using Xamarin.Forms;
    using Xamarin.Forms.Internals;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Xamarin.Forms.IValueConverter" />
    [Preserve(AllMembers = true)]
    [ExcludeFromCodeCoverage]
    public class StringToBooleanConverter : IValueConverter
    {
        #region Methods

        /// <summary>
        /// This method is used to convert the string to boolean.
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="targetType">The target</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="culture">The culture</param>
        /// <returns>
        /// The result
        /// </returns>
        /// <remarks>
        /// To be added.
        /// </remarks>
        public Object Convert(Object value,
                              Type targetType,
                              Object parameter,
                              CultureInfo culture)
        {
            if (value == null || !(parameter is BorderlessEntry email))
            {
                return false;
            }

            var isFocused = (Boolean)value;
            var isInvalidEmail = !isFocused && !StringToBooleanConverter.CheckValidEmail(email.Text);

            return !isFocused && isInvalidEmail;
        }

        /// <summary>
        /// This method is used to convert the boolean to string.
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="targetType">The target</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="culture">The culture</param>
        /// <returns>
        /// The result
        /// </returns>
        /// <remarks>
        /// To be added.
        /// </remarks>
        public Object ConvertBack(Object value,
                                  Type targetType,
                                  Object parameter,
                                  CultureInfo culture)
        {
            return true;
        }

        /// <summary>
        /// Validates the email.
        /// </summary>
        /// <param name="email">Gets the email</param>
        /// <returns>
        /// Returns the boolean value.
        /// </returns>
        private static Boolean CheckValidEmail(String email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return true;
            }

            var regex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            return regex.IsMatch(email) && !email.EndsWith(".");
        }

        #endregion
    }
}
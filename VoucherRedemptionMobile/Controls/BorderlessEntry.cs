namespace VoucherRedemptionMobile.Controls
{
    using System.Diagnostics.CodeAnalysis;
    using Xamarin.Forms;
    using Xamarin.Forms.Internals;

    /// <summary>
    /// This class is inherited from Xamarin.Forms.Entry to remove the border for Entry control in the Android platform.
    /// </summary>
    /// <seealso cref="Xamarin.Forms.Entry" />
    [ExcludeFromCodeCoverage]
    [Preserve(AllMembers = true)]
    public class BorderlessEntry : Entry
    {
    }
}
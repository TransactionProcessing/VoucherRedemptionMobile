namespace VoucherRedemptionMobile.Controls
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;
    using SkiaSharp;
    using SkiaSharp.Views.Forms;
    using Xamarin.Forms;
    using SKSvg = SkiaSharp.Extended.Svg.SKSvg;

    /// <summary>
    /// This is a helper class to render the SVG files.
    /// </summary>
    /// <seealso cref="Xamarin.Forms.ContentView" />
    [ExcludeFromCodeCoverage]
    public class SVGImage : ContentView
    {
        #region Fields

        // Bindable property to set the SVG image path
        /// <summary>
        /// The source property
        /// </summary>
        public static readonly BindableProperty SourceProperty =
            BindableProperty.Create(nameof(SVGImage.Source), typeof(String), typeof(SVGImage), default(String), propertyChanged:SVGImage.RedrawCanvas);

        /// <summary>
        /// The canvas view
        /// </summary>
        private readonly SKCanvasView canvasView = new SKCanvasView();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SVGImage"/> class.
        /// </summary>
        public SVGImage()
        {
            this.Padding = new Thickness(0);
            this.BackgroundColor = Color.Transparent;
            this.Content = this.canvasView;
            this.canvasView.PaintSurface += this.CanvasView_PaintSurface;
        }

        #endregion

        #region Properties

        // Property to set the SVG image path
        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        public String Source
        {
            get => (String)this.GetValue(SVGImage.SourceProperty);
            set => this.SetValue(SVGImage.SourceProperty, value);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Method to invaldate the canvas to update the image
        /// </summary>
        /// <param name="bindable">The target canvas</param>
        /// <param name="oldValue">Previous state</param>
        /// <param name="newValue">Updated state</param>
        public static void RedrawCanvas(BindableObject bindable,
                                        Object oldValue,
                                        Object newValue)
        {
            SVGImage sVGImage = bindable as SVGImage;
            sVGImage?.canvasView.InvalidateSurface();
        }

        /// <summary>
        /// This method update the canvas area with teh image
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="args">The arguments</param>
        private void CanvasView_PaintSurface(Object sender,
                                             SKPaintSurfaceEventArgs args)
        {
            SKCanvas skCanvas = args.Surface.Canvas;
            skCanvas.Clear();

            if (string.IsNullOrEmpty(this.Source))
            {
                return;
            }

            // Get the assembly information to access the local image
            var assembly = typeof(SVGImage).GetTypeInfo().Assembly.GetName();

            // Update the canvas with the SVG image
            using(Stream stream = typeof(SVGImage).GetTypeInfo().Assembly.GetManifestResourceStream(assembly.Name + ".Images." + this.Source))
            {
                SKSvg skSVG = new SKSvg();
                skSVG.Load(stream);
                SKImageInfo imageInfo = args.Info;
                skCanvas.Translate(imageInfo.Width / 2f, imageInfo.Height / 2f);
                SKRect rectBounds = skSVG.ViewBox;
                Single xRatio = imageInfo.Width / rectBounds.Width;
                Single yRatio = imageInfo.Height / rectBounds.Height;
                Single minRatio = Math.Min(xRatio, yRatio);
                skCanvas.Scale(minRatio);
                skCanvas.Translate(-rectBounds.MidX, -rectBounds.MidY);
                skCanvas.DrawPicture(skSVG.Picture);
            }
        }

        #endregion
    }
}
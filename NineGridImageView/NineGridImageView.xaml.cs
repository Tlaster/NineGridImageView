using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace NineGridImageView
{

    public sealed partial class NineGridImageView : ItemsControl
    {
        public delegate void ItemClickedEventHandler(int position);
        public event ItemClickedEventHandler ItemClicked;

        #region Property
       
        public double ItemSize
        {
            get { return (double)GetValue(ItemSizeProperty); }
            set { SetValue(ItemSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemSizeProperty =
            DependencyProperty.Register(nameof(ItemSize), typeof(double), typeof(NineGridImageView), new PropertyMetadata(double.NaN));

        #endregion

        public NineGridImageView()
        {
            this.InitializeComponent();
            SizeChanged += NineGridImageView_SizeChanged;
            Items.VectorChanged += Items_VectorChanged;
            Loaded += NineGridImageView_Loaded;
        }
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            if (element as FrameworkElement == null)
                return;
            (element as FrameworkElement).SetBinding(HeightProperty, new Binding
            {
                Source = this,
                Path = new PropertyPath(nameof(ItemSize)),
                Mode = BindingMode.TwoWay,
            });
            (element as FrameworkElement).SetBinding(WidthProperty, new Binding
            {
                Source = this,
                Path = new PropertyPath(nameof(ItemSize)),
                Mode = BindingMode.TwoWay,
            });
            (element as FrameworkElement).Tapped += NineGridImageView_Tapped;
        }

        private void NineGridImageView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ItemClicked?.Invoke(IndexFromContainer(sender as ContentPresenter));
        }
        
        private void NineGridImageView_Loaded(object sender, RoutedEventArgs e)
        {
            ScrollViewer.SetVerticalScrollBarVisibility(this, ScrollBarVisibility.Disabled);
            ScrollViewer.SetHorizontalScrollBarVisibility(this, ScrollBarVisibility.Disabled);
            ScrollViewer.SetVerticalScrollMode(this, ScrollMode.Disabled);
            ScrollViewer.SetHorizontalScrollMode(this, ScrollMode.Disabled);
        }

        private int CalculateColumns()
        {
            switch (Math.Min(Items.Count, 9))
            {
                case 1: return 1;
                case 2:
                case 4:
                    return 2;
                default:
                    return 3;
            }
        }

        private double CalculateItemWidth(double containerWidth)
        {
            var columns = CalculateColumns();
            return (containerWidth / columns) - 5;
        }

        private void Items_VectorChanged(IObservableVector<object> sender, IVectorChangedEventArgs @event)
        {
            if (!double.IsNaN(ActualWidth))
                RecalculateLayout(ActualWidth);
        }

        private void NineGridImageView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize.Width != e.NewSize.Width)
                RecalculateLayout(e.NewSize.Width);
        }

        private void RecalculateLayout(double containerWidth)
        {
            if (containerWidth > 0)
                ItemSize = CalculateItemWidth(containerWidth);
        }
    }
}

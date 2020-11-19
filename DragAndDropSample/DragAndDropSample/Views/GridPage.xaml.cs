﻿using System.Threading.Tasks;
using DragAndDropSample.ViewModels;
using Sharpnado.HorizontalListView.Helpers;
using Sharpnado.HorizontalListView.RenderedViews;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DragAndDropSample.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GridPage : ContentPage
    {
        public GridPage()
        {
            InitializeComponent();

            //HorizontalListView.PreRevealAnimationAsync = async (viewCell) =>
            //    {
            //        viewCell.View.Opacity = 0;

            //        if (HorizontalListView.ListLayout == HorizontalListViewLayout.Vertical)
            //        {
            //            viewCell.View.RotationX = 90;
            //        }
            //        else
            //        {
            //            viewCell.View.RotationY = -90;
            //        }

            //        await Task.Delay(200);
            //    };

            //HorizontalListView.RevealAnimationAsync = async (viewCell) =>
            //    {
            //        await viewCell.View.FadeTo(1);

            //        if (HorizontalListView.ListLayout == HorizontalListViewLayout.Vertical)
            //        {
            //            await viewCell.View.RotateXTo(0);
            //        }
            //        else
            //        {
            //            await viewCell.View.RotateYTo(0);
            //        }
            //    };
        }

        private void ListLayoutChanging(object sender, ListLayoutChangedEventArgs e)
        {
            switch (e.ListLayout)
            {
                case HorizontalListViewLayout.Linear:
                    HorizontalListView.ItemWidth = 260;
                    HorizontalListView.ItemHeight = 260;
                    HorizontalListView.ColumnCount = 0;
                    HorizontalListView.Margin = Device.RuntimePlatform == Device.Android
                        ? new Thickness(0, 60, 0, 0)
                        : new Thickness(0, -60, 0, 0);

                    break;

                case HorizontalListViewLayout.Grid:
                    HorizontalListView.ItemWidth = 120;
                    HorizontalListView.ItemHeight = 120;
                    HorizontalListView.ColumnCount = 0;
                    HorizontalListView.Margin = new Thickness(0);
                    break;

                case HorizontalListViewLayout.Vertical:
                    HorizontalListView.ItemWidth = 0;
                    HorizontalListView.ItemHeight = 120;
                    HorizontalListView.Margin = new Thickness(0);
                    break;
            }
        }

        private void OnRadioButtonCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var r = (RadioButton)sender;
            switch (r.Text)
            {
                case "Flip":
                    HorizontalListView.RevealAnimation = RevealAnimationHelper.RevealFlipAnimation();
                    break;
                case "Fade":
                    HorizontalListView.RevealAnimation = RevealAnimationHelper.RevealFadeAnimation();
                    break;
                case "Rotate":
                    HorizontalListView.RevealAnimation = RevealAnimationHelper.RevealRotateAnimation();
                    break;
                case "Complex":
                    HorizontalListView.RevealAnimation = ((GridPageViewModel)BindingContext).MyCustomAnimation;
                    break;

            }
        }
    }
}
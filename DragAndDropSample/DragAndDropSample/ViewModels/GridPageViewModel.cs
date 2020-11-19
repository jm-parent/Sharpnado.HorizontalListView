﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using DragAndDropSample.Navigables;
using DragAndDropSample.Services;
using Sharpnado.HorizontalListView.Helpers;
using Sharpnado.HorizontalListView.Paging;
using Sharpnado.HorizontalListView.Services;
using Sharpnado.HorizontalListView.ViewModels;
using Sharpnado.Presentation.Forms;

using Xamarin.Forms;

namespace DragAndDropSample.ViewModels
{
    public enum ListMode
    {
        Grid = 0,
        Horizontal = 1,
        Vertical = 2,
    }

    public class GridPageViewModel : ANavigableViewModel
    {
        private const int PageSize = 20;
        private readonly ISillyDudeService _sillyDudeService;

        private ObservableRangeCollection<SillyDudeVmo> _sillyPeople;
        private ListMode _mode;
        private int _currentIndex = -1;

        private int? _selectedDudeId;

        public GridPageViewModel(INavigationService navigationService, ISillyDudeService sillyDudeService)
            : base(navigationService)
        {
            _sillyDudeService = sillyDudeService;

            InitCommands();

            SillyPeople = new ObservableRangeCollection<SillyDudeVmo>();
            SillyPeoplePaginator = new Paginator<SillyDude>(LoadSillyPeoplePageAsync, pageSize: PageSize);
            SillyPeopleLoaderNotifier = new TaskLoaderNotifier<IReadOnlyCollection<SillyDude>>();
        }

        public LogoLetterVmo[] Logo { get; } = new[]
        {
            new LogoLetterVmo("H", Color.FromHex("#FF0266"), "ThinAccentNeumorphism"),
            new LogoLetterVmo("L", Color.White, "ThinDarkerNeumorphism"),
            new LogoLetterVmo("V", Color.White, "ThinDarkerNeumorphism"),
        };

        public int CurrentIndex
        {
            get => _currentIndex;
            set => SetAndRaise(ref _currentIndex, value);
        }

        public ICommand TapCommand { get; private set; }

        public ICommand OnScrollBeginCommand { get; private set; }

        public ICommand OnScrollEndCommand { get; private set; }

        public TaskLoaderNotifier<IReadOnlyCollection<SillyDude>> SillyPeopleLoaderNotifier { get; }

        public ListMode Mode
        {
            get => _mode;
            set => SetAndRaise(ref _mode, value);
        }

        public Paginator<SillyDude> SillyPeoplePaginator { get; }

        public ObservableRangeCollection<SillyDudeVmo> SillyPeople
        {
            get => _sillyPeople;
            set => SetAndRaise(ref _sillyPeople, value);
        }

        public int? SelectedDudeId
        {
            get => _selectedDudeId;
            set => SetAndRaise(ref _selectedDudeId, value);
        }

        public override void Load(object parameter)
        {
            SillyPeople = new ObservableRangeCollection<SillyDudeVmo>();

            SillyPeopleLoaderNotifier.Load(async () => (await SillyPeoplePaginator.LoadPage(1)).Items);
        }

        private void InitCommands()
        {
            TapCommand = new Command<SillyDudeVmo>(
                async (vm) => await NavigationService.DisplayAlert("Dude Tapped", $"{vm.Name} was tapped !", "OK"));

            OnScrollBeginCommand = new Command(
                () => System.Diagnostics.Debug.WriteLine("SillyInfiniteGridPeopleVm: OnScrollBeginCommand"));
            OnScrollEndCommand = new Command(
                () => System.Diagnostics.Debug.WriteLine("SillyInfiniteGridPeopleVm: OnScrollEndCommand"));
        }
        public RevealAnimation MyCustomAnimation { get; set; } = new RevealAnimation()
        {
            PreRevealAnimationAsync = async (viewCell) =>
            {
                viewCell.View.Opacity = 0;
                await Task.Delay(300);
            },
            RevealAnimationAsync = async (viewCell) =>
            {
                //Fade then Shake
                await viewCell.View.FadeTo(1, 750);
                await viewCell.View.TranslateTo(-15, 0, 50);
                await viewCell.View.TranslateTo(15, 0, 50);
                await viewCell.View.TranslateTo(-10, 0, 50);
                await viewCell.View.TranslateTo(10, 0, 50);
                await viewCell.View.TranslateTo(-5, 0, 50);
                await viewCell.View.TranslateTo(5, 0, 50);
                await viewCell.View.TranslateTo(0, 0, 50);
                await Task.WhenAny<bool>(
                    viewCell.View.RotateXTo(180, 300),
                    viewCell.View.ScaleTo(1.3, 300, easing: Easing.CubicOut)
                );
                await Task.WhenAny<bool>(
                    viewCell.View.RotateXTo(0, 300),
                    viewCell.View.ScaleTo(1, 300, easing: Easing.CubicIn)
                );
                await Task.Delay(200);
            },
            PostRevealAnimationAsync = RevealAnimationHelper.NoAnim()
        };

        private async Task<PageResult<SillyDude>> LoadSillyPeoplePageAsync(int pageNumber, int pageSize, bool isRefresh)
        {
            PageResult<SillyDude> resultPage = await _sillyDudeService.GetSillyPeoplePage(pageNumber, pageSize);
            var viewModels = resultPage.Items.Select(dude => new SillyDudeVmo(dude, TapCommand)).ToList();

            if (isRefresh)
            {
                SillyPeople = new ObservableRangeCollection<SillyDudeVmo>();
            }

            SillyPeople.AddRange(viewModels);

            // Uncomment to test CurrentIndex property
            // TaskMonitor.Create(
            //    async () =>
            //    {
            //        await Task.Delay(2000);
            //        CurrentIndex = 15;
            //    });

            // Uncomment to test Reset action
            // TaskMonitor.Create(
            //   async () =>
            //   {
            //       await Task.Delay(6000);
            //       SillyPeople.Clear();
            //       await Task.Delay(3000);
            //       SillyPeople = new ObservableRangeCollection<SillyDudeVmo>(viewModels);
            //   });

            return resultPage;
        }
    }
}
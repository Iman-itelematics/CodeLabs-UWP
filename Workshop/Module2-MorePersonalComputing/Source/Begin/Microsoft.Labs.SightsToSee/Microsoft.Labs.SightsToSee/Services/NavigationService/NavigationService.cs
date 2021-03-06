﻿using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Microsoft.Labs.SightsToSee.Services.NavigationService
{
    public class NavigationService
    {
        private const string EmptyNavigation = "1,0";
        private readonly NavigationFacade _frame;

        public NavigationService(Frame frame)
        {
            _frame = new NavigationFacade(frame);
            _frame.Navigating += (s, e) => NavigateFrom(false);
            _frame.Navigated += (s, e) => NavigateTo(e.NavigationMode, e.Parameter);
        }

        private string LastNavigationParameter { get; set; /* Sights2SeeSample: persist */ }
        private string LastNavigationType { get; set; /* Sights2SeeSample: persist */ }

        public bool CanGoBack
        {
            get { return _frame.CanGoBack; }
        }

        public bool CanGoForward
        {
            get { return _frame.CanGoForward; }
        }

        public Type CurrentPageType
        {
            get { return _frame.CurrentPageType; }
        }

        public string CurrentPageParam
        {
            get { return _frame.CurrentPageParam; }
        }

        private async void NavigateFrom(bool suspending)
        {
            var page = _frame.Content as FrameworkElement;
            if (page != null)
            {
                var dataContext = page.DataContext as INavigatable;
                if (dataContext != null)
                {
                    await dataContext.OnNavigatedFromAsync(suspending);
                }
            }
        }

        private async void NavigateTo(NavigationMode mode, string parameter)
        {
            LastNavigationParameter = parameter;
            LastNavigationType = _frame.Content.GetType().FullName;

            if (mode == NavigationMode.New)
            {
                // Sights2SeeSample: clear existing state
            }

            var page = _frame.Content as FrameworkElement;
            if (page != null)
            {
                var dataContext = page.DataContext as INavigatable;
                if (dataContext != null)
                {
                    await dataContext.OnNavigatedToAsync(parameter, mode);
                }
            }
        }

        public bool Navigate(Type page, string parameter = null)
        {
            if (page == null)
                throw new ArgumentNullException(nameof(page));
            if (page.FullName.Equals(LastNavigationType)
                && parameter == LastNavigationParameter)
                return false;
            return _frame.Navigate(page, parameter);
        }

        public void RestoreSavedNavigation()
        {
            /* Sights2SeeSample */
        }

        public void GoBack()
        {
            if (_frame.CanGoBack) _frame.GoBack();
        }

        public void GoForward()
        {
            _frame.GoForward();
        }

        public void ClearHistory()
        {
            _frame.SetNavigationState(EmptyNavigation);
        }

        public void Suspending()
        {
            NavigateFrom(true);
        }

        public async void Show(SettingsFlyout flyout, string parameter = null)
        {
            if (flyout == null)
                throw new ArgumentNullException(nameof(flyout));
            var dataContext = flyout.DataContext as INavigatable;
            if (dataContext != null)
            {
                await dataContext.OnNavigatedToAsync(parameter, NavigationMode.New);
            }
            flyout.Show();
        }
    }
}
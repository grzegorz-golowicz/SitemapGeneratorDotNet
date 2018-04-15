using Google_Sitemap_Generator.Common;
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
using Windows.Storage.Pickers;
using Google_Sitemap_Generator.Service;
using Windows.Storage;
using Windows.Storage.Provider;
using Windows.UI.Popups;


// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Google_Sitemap_Generator
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private Crawler CrawlerService;

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public MainPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void CrawlGenerate_Click(object sender, RoutedEventArgs e)
        {
            if (IsAbsoluteUrl(urlTb.Text))
            { 
                CrawlGenerate.IsEnabled = false;
                ChangeFreqCB.IsEnabled = false;
                LastModDP.IsEnabled = false;
              
                CrawlerService = new Crawler(urlTb.Text);
                CrawlerService.UrlAnalysedEvent += UrlAnalysed;

                ProcessStatusTB.Visibility = Windows.UI.Xaml.Visibility.Visible;
                ProcessStatusGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;

                await CrawlerService.CrawlUris();
                SaveSitemap();
                CrawlGenerate.IsEnabled = true;
                ChangeFreqCB.IsEnabled = true;
                LastModDP.IsEnabled = true;
            }
            else
            {
                MessageDialog md = new MessageDialog("Invalid URL. URL should be in format http://example.com");
                await md.ShowAsync();
            }
        }

        private async void SaveSitemap()
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.Desktop;
            savePicker.FileTypeChoices.Add("Google Sitemap", new List<string> { ".xml" });
            savePicker.SuggestedFileName = "sitemap";
            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                CachedFileManager.DeferUpdates(file);
                await FileIO.WriteTextAsync(file, GetGoogleSitemapAsString());
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);

                if (status == FileUpdateStatus.Complete)
                {
                    MessageDialog messageDialog = new MessageDialog("Sitemap " + file.Name + " was saved.");
                    await messageDialog.ShowAsync();
                }
                else
                {
                    MessageDialog messageDialog = new MessageDialog("Sitemap " + file.Name + " couldn't be saved.");
                    await messageDialog.ShowAsync();
                }
            }
        }

        private string GetGoogleSitemapAsString()
        {
            SiteMapGenerator generator = new SiteMapGenerator(CrawlerService.Collected);
            generator.SiteChangeFreq = ChangeFreqCB.SelectedItem.ToString();
            generator.SiteLastMod = LastModDP.Date;
            return generator.GetGoogleSitemapAsString(); ;
        }

        private void UrlAnalysed(object Sender, EventArgs oEventArgs)
        {
            UrlsCollectedTb.Text = CrawlerService.CollectedCount.ToString();
            UrlsQueuedTb.Text = CrawlerService.QueleLength.ToString();
        }

        private bool IsAbsoluteUrl(string url)
        {
            Uri result;
            return Uri.TryCreate(url, UriKind.Absolute, out result);
        }
    }
}

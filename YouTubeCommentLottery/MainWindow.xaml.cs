using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using YouTubeCommentLottery.Interfaces;
using YouTubeCommentLottery.Providers;
using Comment = YouTubeCommentLottery.Models.Comment;

namespace YouTubeCommentLottery
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UserCredential credential;
        private YouTubeService youtubeService;
        private IRandomProvider randomProvider;

        public MainWindow()
        {
            InitializeComponent();
            randomProvider = new SystemRandomProvider();
        }

        private async void SignIn_OnClick(object sender, RoutedEventArgs e)
        {
            using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    // This OAuth 2.0 access scope allows for read-only access to the authenticated 
                    // user's account, but not other types of account access.
                    new[] {YouTubeService.Scope.YoutubeForceSsl},
                    "user",
                    CancellationToken.None,
                    new FileDataStore(GetType().ToString())
                    );
            }
            Debug.WriteLine("Signed in");
            youtubeService = new YouTubeService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = GetType().ToString()
            });
        }

        private async void SignOut_OnClick(object sender, RoutedEventArgs e)
        {
            await credential.RevokeTokenAsync(CancellationToken.None);
            Debug.WriteLine("Signed out");
        }

        private void GetComments_OnClick(object sender, RoutedEventArgs e)
        {
            var videoReq = youtubeService.Videos.List("statistics");
            videoReq.Id = VideoId.Text;
            var videoRes = videoReq.Execute();
            MessageBox.Show(String.Format("{0} comments", videoRes.Items[0].Statistics.CommentCount));

            var list = new List<Comment>();
            string nextPageToken = "";
            while (nextPageToken != null)
            {
                CommentThreadsResource.ListRequest req = youtubeService.CommentThreads.List("snippet");
                req.MaxResults = 100;
                req.Fields =
                    "items(snippet(topLevelComment(snippet(textDisplay,authorDisplayName,authorProfileImageUrl)))),nextPageToken";
                req.VideoId = VideoId.Text;
                req.TextFormat = CommentThreadsResource.ListRequest.TextFormatEnum.PlainText;
                req.PageToken = nextPageToken;
                CommentThreadListResponse res = req.Execute();
                if (res.Items != null)
                {
                    foreach (CommentThread re in res.Items)
                    {
                        var data = new Comment
                        {
                            Content = re.Snippet.TopLevelComment.Snippet.TextDisplay,
                            AuthorDisplayName = re.Snippet.TopLevelComment.Snippet.AuthorDisplayName,
                            AvatarUrl = re.Snippet.TopLevelComment.Snippet.AuthorProfileImageUrl
                        };
                        list.Add(data);
                    }
                }
                nextPageToken = String.IsNullOrWhiteSpace(res.NextPageToken) ? null : res.NextPageToken;
            }
            MessageBox.Show("pobrane: " + list.Count.ToString());
            list = list.Where(a => a.Content.Contains("27")).ToList();
            Comments.ItemsSource = list;
            MessageBox.Show("z liczbą: " + list.Count.ToString());
            var t = list.GroupBy(a => a.Content);
            foreach (var k in t)
            {
                if (k.Count() > 1)
                {
                    //MessageBox.Show(String.Format("{0}: {1}", k.FirstOrDefault().AuthorDisplayName, k.Key));
                }
            }
        }

        private void SelectRandomComment_OnClick(object sender, RoutedEventArgs e)
        {
            var list = (Comments.ItemsSource as List<Comment>);
            if (list != null)
            {
                if (RemoveDuplicates.IsChecked == true)
                    list = list.GroupBy(a => a.AuthorDisplayName).Select(a => a.FirstOrDefault()).ToList();

                var exclude = ExcludeAuthors.Text;
                if (!String.IsNullOrWhiteSpace(exclude))
                {
                    var excludeArray = exclude.Split(',').Select(a => a.Trim());
                    foreach (var author in excludeArray)
                    {
                        list.RemoveAll(a => a.AuthorDisplayName == author);
                    }
                }

                var final = new List<Comment>();
                var keywords = IncludeKeyword.Text.Split('|');
                foreach (var key in keywords)
                {
                    if (!String.IsNullOrWhiteSpace(key))
                    {
                        final.AddRange(list.Where(a => a.Content.ToLower().Contains(key)).ToList());
                    }
                }
                MessageBox.Show(final.Count.ToString());
                var dialog = new SimpleSelectionDialog(final, randomProvider);
                dialog.ShowDialog();
            }
        }
    }
}
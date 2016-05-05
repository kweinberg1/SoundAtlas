namespace SoundAtlas2
{
    using System;
    using System.Windows.Controls.Primitives;
    using SoundAtlas2.Model;
    using Spotify;
    using Spotify.Model;
    
    public class NewReleaseNetworkNodeViewModel : NetworkNodeViewModel
    {
        #region Properties
        public override string ID
        {
            get { return _newReleaseItem.Album.ID; }
        }

        public string ReleaseName
        {
            get
            {
                return _newReleaseItem.Album.Name;
            }
        }

        public string ImageUri
        {
            get { return _newReleaseItem.Album.Images[0].Url; }
        }

        private NewReleaseItem _newReleaseItem;
        #endregion

        #region Constructors
        public NewReleaseNetworkNodeViewModel(NewReleaseItem newReleaseItem)
            : base(newReleaseItem.Artist.Name)
        {
            _newReleaseItem = newReleaseItem;
        }
        #endregion

        #region Public Methods
        public override void AddTracks()
        {
            //Popup the Playlist Chooser.
            PlaylistChooser chooser = new PlaylistChooser();
            chooser.Placement = PlacementMode.MousePoint;
            chooser.Closed += OnPlaylistChooserClosed;
            chooser.IsOpen = true;
        }

        public void OnPlaylistChooserClosed(object sender, EventArgs e) {
            PlaylistChooser chooser = (PlaylistChooser) sender;
            if (chooser.SelectedPlaylist != null) {
                AlbumTrackList albumTrackList = SpotifyClientService.Client.GetAlbumTracks(_newReleaseItem.Album);
                SpotifyClientService.Client.AddTracksToPlaylist(chooser.SelectedPlaylist, albumTrackList.Tracks);
            }
        }

        public override void FollowArtist()
        {
            IsHighlighted = true;

            if (!SpotifyCacheService.IsArtistFollowed(_newReleaseItem.Artist))
            {
                SpotifyCacheService.FollowArtist(_newReleaseItem.Artist);
            }
        }

        public override void UnfollowArtist()
        {
            IsHighlighted = false;

            if (SpotifyCacheService.IsArtistFollowed(_newReleaseItem.Artist))
            {
                SpotifyCacheService.UnfollowArtist(_newReleaseItem.Artist);
            }
        }
        #endregion
    }
}

namespace SoundAtlas2
{
    using System.Diagnostics;
    using SoundAtlas2.Model;
    using Spotify;

    public sealed class ArtistNetworkNodeViewModel : NetworkNodeViewModel
    {
        #region Properties
        public ArtistViewModel ArtistViewModel
        {
            get;
            private set;
        }

        public override string ID
        {
            get { return ArtistViewModel.Artist.ID;  }
        }

        /// <summary>
        /// Number of tracks this node is associated with this node.
        /// </summary>
        private int _numTracks;
        public int NumTracks
        {
            get
            {
                return _numTracks;
            }
            set
            {
                if (_numTracks == value)
                {
                    return;
                }

                _numTracks = value;
                OnPropertyChanged("NumTracks");
            }
        }

        private AtlasHierarchy _hierarchy;
        private ArtistHierarchyNode _hierarchyNode;
        private IPlaylistView _playlist;
        #endregion

        #region Constructors
        public ArtistNetworkNodeViewModel(
            ArtistViewModel viewModel, 
            AtlasHierarchy hierarchy, 
            IHierarchyNode hierarchyNode,
            IPlaylistView playlist)
            : base(viewModel.Name)
        {
            ArtistViewModel = viewModel;

            Debug.Assert(hierarchyNode is ArtistHierarchyNode);
            _hierarchyNode = (ArtistHierarchyNode) hierarchyNode; 
            _hierarchy = hierarchy;
            _playlist = playlist;

            _numTracks = GetArtistTrackCount();

            IsHighlighted = SpotifyCacheService.IsArtistFollowed(ArtistViewModel.Artist);
        }
        #endregion

        #region Public Methods
        public override void OnSelected()
        {
            _hierarchy.GenerateSubTree(_hierarchyNode);
        }

        public override void OnExpand(AtlasExpandChildSetting childSetting)
        {
            int previousLimit = _hierarchy.AddChildrenLimit;
            _hierarchy.AddChildrenLimit = ((childSetting == AtlasExpandChildSetting.CreateAllChildren) ? int.MaxValue : previousLimit);
            _hierarchy.GenerateSubTree(_hierarchyNode);
            _hierarchy.AddChildrenLimit = previousLimit;
            
            IsChildrenExpanded = _hierarchyNode.IsSubTreeExpanded();
        }

        /// <summary>
        /// Adds tracks to the playlist based on the selected node.
        /// </summary>
        public override void AddTracks()
        {
            int trackCount = _playlist.AddArtistTracks(ArtistViewModel.Artist);
            NumTracks += trackCount;
        }

        public override void FollowArtist() {
            IsHighlighted = true;

            if (!SpotifyCacheService.IsArtistFollowed(ArtistViewModel.Artist))
            {
                SpotifyCacheService.FollowArtist(ArtistViewModel.Artist);
            }
        }

        public override void UnfollowArtist() {
            IsHighlighted = false;

            if (SpotifyCacheService.IsArtistFollowed(ArtistViewModel.Artist))
            {
                SpotifyCacheService.UnfollowArtist(ArtistViewModel.Artist);
            }
        }
        #endregion

        #region Private Methods
        private int GetArtistTrackCount()
        {
            return _playlist.GetArtistTrackCount(ArtistViewModel.Artist ?? null);
        }
        #endregion
    }
}

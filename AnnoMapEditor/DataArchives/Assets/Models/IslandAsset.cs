﻿using AnnoMapEditor.MapTemplates.Enums;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace AnnoMapEditor.DataArchives.Assets.Models
{
    public class IslandAsset
    {
        public string DisplayName { get; init; }

        public string FilePath { get; init; }

        public BitmapImage? Thumbnail { get; init; }

        public Region Region { get; init; }

        public IEnumerable<IslandDifficulty> IslandDifficulty { get; init; }

        public IEnumerable<IslandSize> IslandSize { get; init; }

        public IEnumerable<IslandType> IslandType { get; init; }

        public int SizeInTiles { get; init; }

        public IReadOnlyDictionary<long, Slot> Slots { get; init; }
    }
}

﻿using AnnoMapEditor.MapTemplates.Enums;
using System;
using System.Collections.Generic;

namespace AnnoMapEditor.MapTemplates
{
    public class Pool
    {
        public static readonly IEnumerable<Pool> All = new List<Pool>()
        {
            // Moderate
            new(Region.Moderate, IslandSize.Small,
                "data/sessions/islands/pool/moderate/moderate_s_{0}/moderate_s_{0}.a7m", 12
                ),
            new(Region.Moderate, IslandSize.Medium,
                "data/sessions/islands/pool/moderate/moderate_m_{0}/moderate_m_{0}.a7m", 9
                ),
            new(Region.Moderate, IslandSize.Large,
                new FilePathRange[]
                {
                    new FilePathRange("data/sessions/islands/pool/moderate/moderate_l_{0}/moderate_l_{0}.a7m", 1, 14),
                    new FilePathRange("data/sessions/islands/pool/moderate/community_island/community_island.a7m", 1, 1)
                }),
            
             // NewWorld
            new(Region.NewWorld, IslandSize.Small,
                new FilePathRange[]
                {
                    new FilePathRange("data/sessions/islands/pool/colony01/colony01_s_{0}/colony01_s_{0}.a7m", 1, 4),
                    new FilePathRange("data/dlc12/sessions/islands/pool/colony01/colony01_s_{0}/colony01_s_{0}.a7m", 5, 3)
                }),
            new(Region.NewWorld, IslandSize.Medium,
                new FilePathRange[]
                {
                    new FilePathRange("data/sessions/islands/pool/colony01/colony01_m_{0}/colony01_m_{0}.a7m", 1, 6),
                    new FilePathRange("data/dlc12/sessions/islands/pool/colony01/colony01_m_{0}/colony01_m_{0}.a7m", 7, 3)
                }),
            new(Region.NewWorld, IslandSize.Large,
                new FilePathRange[]
                {
                    new FilePathRange("data/sessions/islands/pool/colony01/colony01_l_{0}/colony01_l_{0}.a7m", 1, 5),
                    new FilePathRange("data/dlc12/sessions/islands/pool/colony01/colony01_l_{0}/colony01_l_{0}.a7m", 6, 3),

                }),

            // Arctic
            new(Region.Arctic, IslandSize.Small,  "data/dlc03/sessions/islands/pool/colony03_a01_{0}/colony03_a01_{0}.a7m", 8),
            new(Region.Arctic, IslandSize.Medium, "data/dlc03/sessions/islands/pool/colony03_a02_{0}/colony03_a02_{0}.a7m", 4),
            new(Region.Arctic, IslandSize.Large,  "data/dlc03/sessions/islands/pool/moderate/moderate_l_{0}/moderate_l_{0}.a7m", 14),

            // Enbesa
            new(Region.Enbesa, IslandSize.Small,  "data/dlc06/sessions/islands/pool/colony02_s_{0}/colony02_s_{0}.a7m", new int[] { 1, 2, 3, 5 }),
            new(Region.Enbesa, IslandSize.Medium, "data/dlc06/sessions/islands/pool/colony02_m_{0}/colony02_m_{0}.a7m", new int[] { 2, 4, 5, 9 }),
            new(Region.Enbesa, IslandSize.Large,  "data/dlc06/sessions/islands/pool/colony02_l_{0}/colony02_l_{0}.a7m", new int[] { 1, 3, 5, 6 }),
        };

        private static readonly Dictionary<(Region, IslandSize), Pool> _poolsMap;
        static Pool() {
            _poolsMap = new();
            foreach (Pool pool in All)
            {
                _poolsMap[(pool.Region, pool.IslandSize)] = pool;
            }
        }


        public readonly Region Region;

        public readonly IslandSize IslandSize;


        public static Pool GetPool(Region region, IslandSize islandSize)
        {
            return _poolsMap[(region, islandSize)];
        }

        public static string GetRandomIslandPath(Region region, IslandSize islandSize)
        {
            // use a random Small island for IslandSize.Default
            if (islandSize == IslandSize.Default)
                islandSize = IslandSize.Small;

            Pool pool = GetPool(region, islandSize);
            int index = Random.Shared.Next(1, pool.Size);

            string path = pool.GetPath(index);
            return path;
        }


        private readonly FilePathRange[] _paths;

        public int Size
        {
            get
            {
                int sum = 0;
                foreach (var path in _paths)
                {
                    sum += path.size;
                }
                return sum;
            }
        }


        public string GetPath(int i)
        {
            int rangeIdx = 0;
            FilePathRange range = _paths[rangeIdx];
            int skipped = 0;
            while(skipped + range.size <= i)
            {
                skipped += range.size;
                range = _paths[++rangeIdx];
            }

            return range.GetPath(i - skipped);
        }


        public Pool(Region region, IslandSize islandSize, FilePathRange[] paths)
        {
            Region = region;
            IslandSize = islandSize;
            _paths = paths;
        }

        public Pool(Region region, IslandSize islandSize, string filePath, int size)
            : this(region, islandSize, new FilePathRange[]
            {
                new FilePathRange(filePath, 1, size)
            })
        {
        }

        public Pool(Region region, IslandSize islandSize, string filePath, int[] ids)
            : this(region, islandSize, new FilePathRange[]
            {
                new FilePathRange(filePath, ids)
            })
        {
        }
    }
}

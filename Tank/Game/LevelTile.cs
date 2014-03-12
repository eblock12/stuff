using System;
using System.Collections.Generic;
using System.Text;

using Tank.Common;

namespace Tank.Game
{
    public struct LevelTile
    {
        public int Size;
        public TileType Type;
        public ModelTransformer[] Models;

        public LevelTile(int size, TileType type, ModelTransformer[] model)
        {
            this.Size = size;
            this.Type = type;
            this.Models = model;
        }
    }

    public enum TileType
    {
        Empty,
        Normal,
        Breakable
    }
}

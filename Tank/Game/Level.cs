using System;
using System.Collections.Generic;
using System.Text;

using Tank.Common;

namespace Tank.Game
{
    public class Level
    {
        private LevelTile[,] tiles;

        private int numTilesWidth, numTilesHeight;
        private double tileWidth, tileHeight;
        private double width, height;
        private double widthHalf, heightHalf;

        private Model floorModel, cubeBlock;
        private Texture floorTexture;

        public Level(int numTilesWidth, int numTilesHeight, double width, double height)
        {
            this.numTilesWidth = numTilesWidth;
            this.numTilesHeight = numTilesHeight;
            this.width = width;
            this.height = height;
            this.tileWidth = width / (double)numTilesWidth;
            this.tileHeight = height / (double)numTilesHeight;
            this.widthHalf = width / 2.0;
            this.heightHalf = height / 2.0;

            this.tiles = new LevelTile[numTilesWidth, numTilesHeight];

            this.floorTexture = Texture.FromFile("wood1.jpg");

            this.floorModel = Model.CreateTexturedPlane(Vector3.Zero, width, height, width / 10.0, height / 10.0);
            this.floorModel.Material.Texture = floorTexture;

            this.cubeBlock = Model.LoadModelFromX3m("cube_block.x3m");
            this.cubeBlock.Material.Texture = floorTexture;

            this.NewLevel();
        }

        public double Width
        {
            get { return width; }
        }

        public double Height
        {
            get { return height; }
        }

        public void NewLevel()
        {
            Random random = new Random();

            for (int tileX = 0; tileX < numTilesWidth; tileX++)
            {
                for (int tileY = 0; tileY < numTilesHeight; tileY++)
                {
                    tiles[tileX, tileY].Type = TileType.Empty;
                }
            }

            for (int tileX = 0; tileX < numTilesWidth; tileX++)
            {
                tiles[tileX, 0] = CreateTile(tileX, 0, TileType.Normal, random.Next(5) + 1);
                tiles[tileX, numTilesHeight - 1] = CreateTile(tileX, numTilesHeight - 1, 
                    TileType.Normal, random.Next(5) + 1);
            }

            for (int tileY = 1; tileY < numTilesHeight - 1; tileY++)
            {
                tiles[0, tileY] = CreateTile(0, tileY, TileType.Normal, random.Next(5) + 1);
                tiles[numTilesWidth - 1, tileY] = CreateTile(numTilesWidth - 1, tileY,
                    TileType.Normal, random.Next(5) + 1);
            }
        }

        public void Render()
        {
            Renderer.Instance.DrawModel(floorModel);

            foreach (LevelTile tile in tiles)
            {
                if (tile.Type != TileType.Empty)
                {
                    foreach (ModelTransformer model in tile.Models)
                    {
                        Renderer.Instance.DrawModel(model);
                    }
                }
            }
        }

        private LevelTile CreateTile(int tileX, int tileY, TileType type, int height)
        {
            LevelTile tile = new LevelTile();

            tile.Size = height;
            tile.Models = new ModelTransformer[height];

            double tileSize = (tileWidth + tileHeight) / 2.0;

            for (int i = 0; i < height; i++)
            {
                ModelTransformer model = new ModelTransformer(cubeBlock);

                Vector3 position = CalculateTileCenter3D(tileX, tileY);
                position.Y = -model.Model.BoundingVolume.Bottom;
                position.Y += tileSize * (double)i;

                model.Displacement = position;
                model.Scale = new Vector3(tileWidth, tileSize, tileHeight);
                tile.Models[i] = model;
            }

            tile.Type = TileType.Normal;

            return tile;
        }

        private Vector3 CalculateTileCenter3D(int tileX, int tileY)
        {
            double x = (double)tileX * tileWidth + tileWidth / 2.0;
            double y = (double)tileY * tileHeight + tileHeight / 2.0;

            x -= widthHalf;
            y -= heightHalf;

            return new Vector3(x, 0, y);
        }
    }
}

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;

using Erythros_Pluvia.Entities;
using Erythros_Pluvia.Util;

namespace Erythros_Pluvia.Scenes
{

    /// <summary>
    /// This class represents a physical scene, containing entities that are subject to the laws of physics.
    /// </summary>
    public class ScenePhysical : IScene
    {

        #region Fields

        /// <summary>
        /// the ID of the Tiled layer under which the foreground is stored. The foreground is where solid blocks and collidable entities can be found
        /// </summary>
        public const string FOREGROUND_LAYER_ID = "Foreground";

        /// <summary>
        /// the name of the file containing the Tiled Map data in the content manager (.tmx format)
        /// </summary>
        protected string mapAssetName;

        /// <summary>
        /// the Tiled map for this scene
        /// </summary>
        protected TiledMap map;

        /// <summary>
        /// the Tiled map renderer
        /// </summary>
        protected IMapRenderer mapRenderer;

        /// <summary>
        /// list of Entities currently being managed by the physics engine
        /// </summary>
        protected LinkedList<IEntity> managedEntities;

        /// <summary>
        /// spatial hash of game entities currently loaded into the scene, for optimizing collision checking
        /// </summary>
        protected Dictionary<int, LinkedList<IEntity>> entitySpatialHash;

        /// <summary>
        /// spatial hash of solid tiles currently loaded into the scene
        /// </summary>
        protected Dictionary<int, LinkedList<TiledTile>> tileSpatialHash;

        /// <summary>
        /// the number of hash columns in the spatial hash
        /// </summary>
        protected int numHashCols;

        /// <summary>
        /// the number of hash rows in the spatial hash
        /// </summary>
        protected int numHashRows;

        /// <summary>
        /// the width of each bucket of the spatial hash
        /// </summary>
        protected float bucketWidth;

        /// <summary>
        /// the height of each bucket of the spatial hash
        /// </summary>
        protected float bucketHeight;

        /// <summary>
        /// the scene camera view
        /// </summary>
        protected Camera2D camera;

        /// <summary>
        /// gravitational acceleration
        /// </summary>
        protected float gravityAcceleration;

        /// <summary>
        /// the maximum falling speed for an entity
        /// </summary>
        protected float maxFallSpeed;

        #endregion

        #region Public Methods

        /// <summary>
        /// Construct a new instance of ScenePhysical
        /// </summary>
        /// <param name="mapAssetName">the name of the file that the TiledMap is located in</param>
        /// <param name="numHashCols">the number of columns to use for the spatial hash</param>
        /// <param name="numHashRows">the number of rows to use for the spatial hash</param>
        public ScenePhysical(string mapAssetName, int numHashCols, int numHashRows)
        {
            this.mapAssetName = mapAssetName;
            mapRenderer = new FullMapRenderer(GraphicsDevice);
            camera = new Camera2D(GraphicsDevice);
            entitySpatialHash = new Dictionary<int, LinkedList<IEntity>>();
            tileSpatialHash = new Dictionary<int, LinkedList<TiledTile>>();
            this.numHashCols = numHashCols;
            this.numHashRows = numHashRows;
            managedEntities = new LinkedList<IEntity>();
            gravityAcceleration = 10.0f;
            maxFallSpeed = 350.0f;
        }

        /// <summary>
        /// Load all data to initialize the scene.
        /// </summary>
        public override void OnStart()
        {
            // load the map
            map = Content.Load<TiledMap>(mapAssetName);
            mapRenderer.SwapMap(map);
            bucketWidth = map.WidthInPixels / numHashCols;
            bucketHeight = map.HeightInPixels / numHashRows;
           
            // load all solid foreground tiles into the spatial hash
            TiledTileLayer layer = map.GetLayer<TiledTileLayer>(FOREGROUND_LAYER_ID);
            IReadOnlyList<TiledTile> tilesInLayer = layer.Tiles;
            foreach (TiledTile currentTile in tilesInLayer)
            {
                if (!currentTile.IsBlank)
                {
                    Vector2 tilePosition = currentTile.Position;
                    int tileWidth = map.TileWidth;
                    int tileHeight = map.TileHeight;

                    // each corner of the entity's bounding box needs to be checked, because the entity could be within multiple cells at once
                    Vector2 topLeftVertex = new Vector2(currentTile.Position.X * map.TileWidth, currentTile.Position.Y * map.TileHeight);
                    int topLeftHashId = _calculateHashId(topLeftVertex, bucketWidth, bucketHeight);
                    _addToHash<TiledTile>(topLeftHashId, currentTile, tileSpatialHash);

                    Vector2 topRightVertex = new Vector2(currentTile.Position.X * map.TileWidth + map.TileWidth, currentTile.Position.Y * map.TileHeight);
                    int topRightHashId = _calculateHashId(topRightVertex, bucketWidth, bucketHeight);
                    _addToHash<TiledTile>(topRightHashId, currentTile, tileSpatialHash);

                    Vector2 bottomRightVertex = new Vector2(currentTile.Position.X * map.TileWidth + map.TileWidth, currentTile.Position.Y * map.TileHeight + map.TileHeight);
                    int bottomRightHash = _calculateHashId(bottomRightVertex, bucketWidth, bucketHeight);
                    _addToHash<TiledTile>(bottomRightHash, currentTile, tileSpatialHash);

                    Vector2 bottomLeftVertex = new Vector2(currentTile.Position.X * map.TileWidth, currentTile.Position.Y * map.TileHeight + map.TileHeight);
                    int bottomLeftHash = _calculateHashId(bottomLeftVertex, bucketWidth, bucketHeight);
                    _addToHash<TiledTile>(bottomLeftHash, currentTile, tileSpatialHash);
                }
            }

            // sort the entities, if any, into their proper place in the spatial hash
            _updateSpatialHash();
        }

        /// <summary>
        /// Refresh the state of the scene. Checks for collisions, resolves physics calculations, etc.
        /// </summary>
        /// <param name="time">the game time object</param>
        public override void OnUpdate(GameTime time) {
            base.OnUpdate(time);
            
            updateEntityPositions(time);
            _updateSpatialHash();            
            checkCollisions();
        }

        /// <summary>
        /// Register an entity to be managed by the physics engine
        /// </summary>
        /// <param name="entity">the entity to be managed</param>
        public void RegisterEntity(IEntity entity)
        {
            managedEntities.AddLast(entity);
        }

        /// <summary>
        /// Remove an entity from the physics engine
        /// </summary>
        /// <param name="entity">the entity that will be managed no longer</param>
        public void RemoveEntity(IEntity entity)
        {
            managedEntities.Remove(entity);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Check all entities within the scene for a collision, either with tiles or with another entity.
        /// </summary>
        protected void checkCollisions()
        {
            foreach (int currentBucketId in entitySpatialHash.Keys)
            {
                foreach (IEntity currentEntity1 in entitySpatialHash[currentBucketId])
                {
                    _checkSurfaceCollision(currentEntity1, currentBucketId);

                    // TODO check entities for collisions with each other
                }
            }
        }

        /// <summary>
        /// Update the position of all managed entities
        /// </summary>
        /// <param name="time">the game time object</param>
        protected void updateEntityPositions(GameTime time)
        {
            foreach (IEntity currentEntity in managedEntities)
            {
                currentEntity.UpdatePosition(time);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Update the spatial hash. Recalculates which entities go into which bucket of the hash based on their current position.
        /// </summary>
        private void _updateSpatialHash()
        {
            LinkedListNode<IEntity> currentEntityNode = managedEntities.First;
            if (currentEntityNode == null)
            {
                return;
            }

            IEntity currentEntity = currentEntityNode.Value;

            // the hash needs to be cleared each update before we update all the entities into
            // their proper place in the hash. I'd like it if we could skip this and just update the entities
            // that have moved since the last frame, but until and unless this becomes a performance bottleneck we shall
            // continue to do it this way
            foreach (int currentBucketId in entitySpatialHash.Keys)
            {
                entitySpatialHash[currentBucketId].Clear();
            }

            while (currentEntityNode != null)
            {
                currentEntity = currentEntityNode.Value;
                
                // each corner of the entity's bounding box needs to be checked, because the entity could be within multiple cells at once
                Vector2 topLeftVertex = new Vector2(currentEntity.Position.X, currentEntity.Position.Y);
                int topLeftHashId = _calculateHashId(topLeftVertex, bucketWidth, bucketHeight);
                _addToHash<IEntity>(topLeftHashId, currentEntity, entitySpatialHash);

                Vector2 topRightVertex = new Vector2(currentEntity.Position.X + currentEntity.Sprite.Width, currentEntity.Position.Y);
                int topRightHashId = _calculateHashId(topRightVertex, bucketWidth, bucketHeight);
                _addToHash<IEntity>(topRightHashId, currentEntity, entitySpatialHash);

                Vector2 bottomRightVertex = new Vector2(currentEntity.Position.X + currentEntity.Sprite.Width, currentEntity.Position.Y + currentEntity.Sprite.Height);
                int bottomRightHash = _calculateHashId(bottomRightVertex, bucketWidth, bucketHeight);
                _addToHash<IEntity>(bottomRightHash, currentEntity, entitySpatialHash);

                Vector2 bottomLeftVertex = new Vector2(currentEntity.Position.X, currentEntity.Position.Y + currentEntity.Sprite.Height);
                int bottomLeftHash = _calculateHashId(bottomLeftVertex, bucketWidth, bucketHeight);
                _addToHash<IEntity>(bottomLeftHash, currentEntity, entitySpatialHash);

                currentEntityNode = currentEntityNode.Next;
            }
        }

        /// <summary>
        /// calculates the hash ID that an entity at a given location should be placed in
        /// </summary>
        /// <param name="entityLocation">2D vector containing the location of the entity in world space</param>
        /// <param name="bucketWidth">the width of each bucket of the hash</param>
        /// <param name="bucketHeight">the height of each bucket of the hash</param>
        /// <returns>the hash key that this point belongs to</returns>
        private int _calculateHashId(Vector2 entityLocation, float bucketWidth, float bucketHeight)
        {
            return (int)(Math.Floor(entityLocation.X / bucketWidth)) + numHashCols * (int)(Math.Floor(entityLocation.Y / bucketHeight));
        }

        /// <summary>
        /// add an object to a spatial hash
        /// </summary>
        /// <param name="hashId">the hash ID to store the entity under</param>
        /// <param name="entity">the entity in question</param>
        /// <param name="hash">the hash in which the entity will be stored</param>
        private void _addToHash<T>(int hashId, T hashObject, Dictionary<int, LinkedList<T>> hash)
        {
            if (!hash.ContainsKey(hashId))
            {
                hash[hashId] = new LinkedList<T>();
            }

            // the hash might already contain the entity, in which case there's nothing more for us to do
            if (hash[hashId].Contains(hashObject))
            {
                return;
            }

            hash[hashId].AddLast(hashObject);
        }

        /// <summary>
        /// determine if two entities collide with one another
        /// </summary>
        /// <param name="entity1">the first entity</param>
        /// <param name="entity2">the second entity</param>
        /// <returns></returns>
        private bool _checkEntityCollision(IEntity entity1, IEntity entity2)
        {
            return entity1.BoundingBox.Intersects(entity2.BoundingBox);
        }

        /// <summary>
        /// Check to see if an entity collides with any surfaces. Also applies gravity if the entity is found not to be on top of a surface.
        /// </summary>
        /// <param name="entity">the entity in question</param>
        /// <param name="hashId">the ID of the hash bucket the entity is in</param>
        private void _checkSurfaceCollision(IEntity entity, int hashId)
        {
            bool onSurface = false;
            if (tileSpatialHash.ContainsKey(hashId))
            {
                LinkedList<TiledTile> tilesInBucket = tileSpatialHash[hashId];

                foreach (TiledTile currentTile in tilesInBucket)
                {
                    int tileWidth = map.TileWidth;
                    int tileHeight = map.TileHeight;

                    // TODO this will create a metric butt ton of Rectangle objects in memory and who knows when the garbage collector will free it all up.
                    // We should profile this and if we find that it's eating up too much memory, we should use a single instance of Rectangle and swap out
                    // the values instead of constantly allocating memory for new instances
                    Erythros_Pluvia.Util.Rectangle tileBoundingBox = new Erythros_Pluvia.Util.Rectangle(currentTile.Position.X * map.TileWidth, currentTile.Position.Y * map.TileHeight, tileWidth, tileHeight);
                    Erythros_Pluvia.Util.Rectangle entityBoundingBox = new Erythros_Pluvia.Util.Rectangle(entity.Position.X, entity.Position.Y, entity.Sprite.Width, entity.Sprite.Height);

                    if (entityBoundingBox.Intersects(tileBoundingBox, false))
                    {
                        // we need the four corners of both the entity's current and previous bounding box in order to determine which side of the tile the entity collided with
                        float entityBoundingBoxLeft = entity.Position.X;
                        float entityBoundingBoxRight = entity.Position.X + entity.Width;
                        float entityBoundingBoxTop = entity.Position.Y;
                        float entityBoundingBoxBottom = entity.Position.Y + entity.Height;

                        float entityPreviousBoundingBoxLeft = entity.PreviousPosition.X;
                        float entityPreviousBoundingBoxRight = entity.PreviousPosition.X + entity.Width;
                        float entityPreviousBoundingBoxTop = entity.PreviousPosition.Y;
                        float entityPreviousBoundingBoxBottom = entity.PreviousPosition.Y + entity.Height;

                        float tileBoundingBoxLeft = (currentTile.Position.X * map.TileWidth);
                        float tileBoundingBoxRight = (currentTile.Position.X * map.TileWidth) + map.TileWidth;
                        float tileBoundingBoxTop = (currentTile.Position.Y * map.TileHeight);
                        float tileBoundingBoxBottom = (currentTile.Position.Y * map.TileHeight) + map.TileHeight;

                        // check for a collision in each direction and force the entity's position to the edge of the tile
                        if ((entityPreviousBoundingBoxBottom <= tileBoundingBoxTop) && (entityBoundingBoxBottom >= tileBoundingBoxTop))
                        {
                            Vector2 entityPosition = entity.Position;
                            Vector2 entityVelocity = entity.Velocity;
                            entityPosition.Y = tileBoundingBoxTop - entity.Width;
                            if (entityVelocity.Y > 0.0f)
                            {
                                entityVelocity.Y = 0.0f;
                            }
                            entity.Position = entityPosition;
                            entity.Velocity = entityVelocity;
                            onSurface = true;
                        }
                        if ((entityPreviousBoundingBoxTop >= tileBoundingBoxBottom) && (entityBoundingBoxTop <= tileBoundingBoxBottom))
                        {
                            Vector2 entityPosition = entity.Position;
                            Vector2 entityVelocity = entity.Velocity;
                            entityPosition.Y = tileBoundingBoxBottom;
                            if (entityVelocity.Y < 0.0f)
                            {
                                entityVelocity.Y = 0.0f;
                            }
                            entity.Position = entityPosition;
                            entity.Velocity = entityVelocity;
                        }
                        if (entityPreviousBoundingBoxLeft >= tileBoundingBoxRight && (entityBoundingBoxLeft <= tileBoundingBoxRight))
                        {
                            Vector2 entityPosition = entity.Position;
                            Vector2 entityVelocity = entity.Velocity;
                            entityPosition.X = tileBoundingBoxRight;
                            if (entityVelocity.X < 0.0f)
                            {
                                entityVelocity.X = 0.0f;
                            }
                            entity.Position = entityPosition;
                            entity.Velocity = entityVelocity;
                        }
                        if (entityPreviousBoundingBoxRight <= tileBoundingBoxLeft && (entityBoundingBoxRight >= tileBoundingBoxLeft))
                        {
                            Vector2 entityPosition = entity.Position;
                            Vector2 entityVelocity = entity.Velocity;
                            entityPosition.X = tileBoundingBoxLeft - entity.Width;
                            if (entityVelocity.X > 0.0f)
                            {
                                entityVelocity.X = 0.0f;
                            }
                            entity.Position = entityPosition;
                            entity.Velocity = entityVelocity;
                        }
                    }
                }
            }

            // if the player is not on top of a surface, resolve gravity calculations
            if (!onSurface)
            {
                Vector2 entityVelocity = entity.Velocity;
                entityVelocity.Y = this._calculateGravity(entityVelocity.Y);
                entity.Velocity = entityVelocity;
            }
        }

        /// <summary>
        /// Calculate the new velocity based on gravitation acceleration, also applying a maximum falling speed where necessary
        /// </summary>
        /// <param name="currentFallSpeed">the current Y velocity</param>
        /// <returns>the resulting Y velocity</returns>
        private float _calculateGravity(float initialVelocityY)
        {
            // TODO make this time-based
            return MathHelper.Clamp(initialVelocityY + gravityAcceleration, -maxFallSpeed, +maxFallSpeed);

        }

        #endregion
    }
}

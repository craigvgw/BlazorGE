﻿#region Namespaces

using BlazorGE.Game.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#endregion

namespace BlazorGE.Game.Entities
{
    public class GameEntityBase
    {
        #region Protected Properties

        protected List<IGameComponent> GameComponents = new();

        #endregion

        #region Public Properties

        public List<GameEntityBase> ChildEntities { get; protected set; } = new();
        public bool IsActive { get; protected set; }
        public GameEntityBase ParentEntity { get; protected set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set this entity as active
        /// </summary>
        public void Activate() => IsActive = true;

        /// <summary>
        /// Create and add a child entity to this entity
        /// </summary>
        /// <returns></returns>
        public GameEntityBase AddChildEntity()
        {
            var child = new GameEntityBase
            {
                ParentEntity = this
            };

            ChildEntities.Add(child);

            return child;
        }

        /// <summary>
        /// Attach the specified game component to this entity
        /// </summary>
        /// <param name="gameComponent"></param>
        public void AttachGameComponent(IGameComponent gameComponent)
        {
            // Add component if it doesn't already exist!
            if (!GameComponents.Any(component => component.GetType() == gameComponent.GetType()))
            {
                gameComponent.GameEntityOwner = this;
                GameComponents.Add(gameComponent);
            }
        }

        /// <summary>
        /// Clones this entity
        /// </summary>
        /// <returns></returns>
        public GameEntityBase Clone()
        {
            var entity = new GameEntityBase
            {
                ParentEntity = ParentEntity,
                GameComponents = new List<IGameComponent>(GameComponents)
            };

            entity.IsActive = false;

            return entity;
        }

        /// <summary>
        /// Set this entity and any of its children to inactive
        /// </summary>
        public void Deactivate()
        {
            IsActive = false;

            foreach (var childEntity in ChildEntities.Where(entity => entity.IsActive))
            {
                childEntity.Deactivate();
            }
        }

        /// <summary>
        /// Remove all attached game components of specified type from this entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void DetachGameComponent<T>() where T : IGameComponent
        {
            GameComponents.RemoveAll(component => component is T);
        }

        /// <summary>
        /// Draw this entity
        /// </summary>
        /// <param name="gameTime"></param>
        /// <returns></returns>
        public virtual async Task DrawAsync(GameTime gameTime)
        {
            foreach (var gameComponent in GameComponents.Where(component => component is IDrawableGameComponent))
            {
                await ((IDrawableGameComponent)gameComponent).DrawAsync(gameTime);
            }

            foreach (var childEntity in ChildEntities.Where(entity => entity.IsActive))
            {
                await childEntity.DrawAsync(gameTime);
            }
        }

        /// <summary>
        /// Get the attached game component, throws exception if none found
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetComponent<T>() where T : IGameComponent
        {
            return (T)GameComponents.Where(component => component is T).Single();
        }

        /// <summary>
        /// Returns true if this entity has an attached game component of the specified type
        /// </summary>
        /// <typeparam name="T">Type of game component</typeparam>
        /// <returns></returns>
        public bool HasComponent<T>() where T : IGameComponent
        {
            return GameComponents.Any(component => component is T);
        }

        /// <summary>
        /// Update this entity
        /// </summary>
        /// <param name="gameTime"></param>
        /// <returns></returns>
        public virtual async Task UpdateAsync(GameTime gameTime)
        {
            foreach (var gameComponent in GameComponents.Where(component => component is IUpdatableGameComponent))
            {
                await ((IUpdatableGameComponent)gameComponent).UpdateAsync(gameTime);
            }

            foreach (var childEntity in ChildEntities.Where(entity => entity.IsActive))
            {
                await childEntity.UpdateAsync(gameTime);
            }
        }

        #endregion
    }
}

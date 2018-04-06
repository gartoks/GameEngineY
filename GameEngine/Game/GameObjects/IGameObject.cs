using System;
using System.Collections.Generic;
using GameEngine.Game.GameObjects.GameObjectComponents;
using GameEngine.Game.Utility;

namespace GameEngine.Game.GameObjects {

    public delegate void GameObjectComponentModificationEventHandler(IGameObject gameObject, GOC component);

    public interface IGameObject {
        /// <summary>
        /// Gets a value indicating whether this GameObject is alive.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this GameObject is alive; otherwise, <c>false</c>.
        /// </value>
        bool IsAlive { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this GameObject is enabled. If a GameObject is not enabled its components will not update or be rendered.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
        /// </value>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the parent of this GameObject.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        IGameObject Parent { get; set; }

        /// <summary>
        /// Occurs when a new GameObjectComponent (GOC) is added to this GameObject.
        /// </summary>
        event GameObjectComponentModificationEventHandler OnComponentAdd;

        /// <summary>
        /// Occurs when a new GameObjectComponent (GOC) is removed to this GameObject.
        /// </summary>
        event GameObjectComponentModificationEventHandler OnComponentRemove;

        /// <summary>
        /// Destroys this GameObject and removes it consequentely from the scene.
        /// </summary>
        void Destroy();

        /// <summary>
        /// Adds a new GameObjectComponent (GOC) to this GameObject.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="initializationParameters">The initialization parameters.</param>
        /// <returns></returns>
        T AddComponent<T>(params object[] initializationParameters) where T : GOC;

        /// <summary>
        /// Removes the GameObjectComponent (GOC) from this GameObject and destroys it.
        /// </summary>
        /// <param name="component">The component.</param>
        void RemoveComponent(GOC component);

        /// <summary>
        /// Gets the first GameObjectComponent (GOC) from this GameObject matching the given type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="searchMode">The search mode.</param>
        /// <param name="includeDerivations">if set to <c>true</c> the search includes derivations of the searched GameObjectComponent's type.</param>
        /// <returns></returns>
        T GetComponent<T>(GOCSearchMode searchMode = GOCSearchMode.This, bool includeDerivations = true) where T : GOC;

        /// <summary>
        /// Gets all GameObjectComponents (GOC) from this GameObject  matching the given type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="searchMode">The search mode.</param>
        /// <param name="includeDerivations">if set to <c>true</c> the search includes derivations of the searched GameObjectComponent's type.</param>
        /// <returns></returns>
        IEnumerable<T> GetComponents<T>(GOCSearchMode searchMode = GOCSearchMode.This, bool includeDerivations = true) where T : GOC;

        /// <summary>
        /// Gets all GameObjectComponents (GOC) from this GameObject matching the given selector criteria.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <param name="searchMode">The search mode.</param>
        /// <returns></returns>
        IEnumerable<GOC> FindComponents(Func<GOC, bool> selector, GOCSearchMode searchMode = GOCSearchMode.This);

        /// <summary>
        /// Finds all children of this GameObject matching the given selector criteria.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <param name="includeChildrensChildren">if set to <c>true</c> the children of children are searched recursively as well.</param>
        /// <returns></returns>
        IEnumerable<IGameObject> FindChildren(Func<IGameObject, bool> selector, bool includeChildrensChildren = false);

        /// <summary>
        /// Gets the parental hierarchy.
        /// </summary>
        /// <param name="includeCurrent">if set to <c>true</c> includes the current GameObejct.</param>
        /// <returns></returns>
        IEnumerable<IGameObject> GetParentalHierarchy(bool includeCurrent = true);

        /// <summary>
        /// Gets the root game object.
        /// </summary>
        /// <returns></returns>
        IGameObject GetRootGameObject();

        /// <summary>
        /// Gets the children of this GameObject.
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        IEnumerable<IGameObject> Children { get; }

        /// <summary>
        /// Gets the GameObject's transform.
        /// </summary>
        /// <value>
        /// The transform.
        /// </value>
        Transform Transform { get; }
    }
}
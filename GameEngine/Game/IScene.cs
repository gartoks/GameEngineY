using System;
using System.Collections.Generic;
using GameEngine.Game.GameObjects;
using GameEngine.Game.GameObjects.GameObjectComponents;
using GameEngine.Math;

namespace GameEngine.Game {
    public interface IScene {

        /// <summary>
        /// Creates a new GameObject and adds it to the scene.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="position">The position.</param>
        /// <param name="rotation">The rotation.</param>
        /// <param name="scale">The scale.</param>
        /// <returns></returns>
        IGameObject CreateGameObject(string name, IGameObject parent, Vector2 position, float rotation, Vector2 scale);
        
        /// <summary>
        /// Gets all game objects in the scene.
        /// </summary>
        /// <value>
        /// The game objects.
        /// </value>
        IEnumerable<IGameObject> GameObjects { get; }

        /// <summary>
        /// Finds the game objects by their name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="activeOnly">if set to <c>true</c> [active only].</param>
        /// <returns></returns>
        IEnumerable<IGameObject> FindGameObjectsByName(string name, bool activeOnly = true);

        /// <summary>
        /// Finds the game objects in range of the given point.
        /// </summary>
        /// <param name="p">The point.</param>
        /// <param name="range">The range.</param>
        /// <returns></returns>
        IEnumerable<IGameObject> FindGameObjectsInRange(Vector2 p, float range);

        /// <summary>
        /// Finds game objects using a selector.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <returns></returns>
        IEnumerable<IGameObject> FindGameObjects(Func<IGameObject, bool> selector);

        /// <summary>
        /// Finds all game object components of a specific type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="activeOnly">if set to <c>true</c> [active only].</param>
        /// <returns></returns>
        IEnumerable<T> FindComponentsByType<T>(bool activeOnly = true) where T : GOC;

        /// <summary>
        /// Gets the name of the scene.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Gets or sets the main viewport of the scene. The main viewport is rendered to the screen.
        /// </summary>
        /// <value>
        /// The main viewport.
        /// </value>
        Viewport MainViewport { get; set; }

    }
}
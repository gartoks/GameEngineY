using System;
using System.Collections.Generic;
using GameEngine.Game.GameObjects;
using GameEngine.Game.GameObjects.GameObjectComponents;

namespace GameEngine.Game {
    public interface IScene {

        /// <summary>
        /// Gets all game objects in the scene.
        /// </summary>
        /// <value>
        /// The game objects.
        /// </value>
        IEnumerable<GameObject> GameObjects { get; }

        /// <summary>
        /// Finds the game objects by their name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="activeOnly">if set to <c>true</c> [active only].</param>
        /// <returns></returns>
        IEnumerable<GameObject> FindGameObjectsByName(string name, bool activeOnly = true);

        /// <summary>
        /// Finds game objects using a selector.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <returns></returns>
        IEnumerable<GameObject> FindGameObjects(Func<GameObject, bool> selector);

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
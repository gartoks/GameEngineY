namespace GameEngine.Resources {
    public interface IResourceManager {

        /// <summary>
        /// Tries to get the resource.
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="resourceIdentifer">The resource identifer.</param>
        /// <param name="resource">The resource.</param>
        /// <param name="waitForLoading">if set to <c>true</c> waits until the resource is loaded.</param>
        /// <returns></returns>
        bool TryGetResource<R>(string resourceIdentifer, out R resource, bool waitForLoading = true);

        /// <summary>
        /// Loads the resource.
        /// </summary>
        /// <typeparam name="R">The resource type.</typeparam>
        /// <typeparam name="RLP">The resource loading parameters type.</typeparam>
        /// <param name="resourceIdentifier">The resource identifier.</param>
        /// <param name="resourceLoadingParameters">The resource loading parameters.</param>
        /// <param name="loadingPriority">The loading priority.</param>
        /// <param name="globalResource">if set to <c>true</c> the resource is loaded as a global resource and not unloaded when the scene changes.</param>
        void LoadResource<R, RLP>(string modID, string resourceIdentifier, RLP resourceLoadingParameters, int loadingPriority, bool globalResource = false) where RLP : ResourceLoadingParameters<R>;

        /// <summary>
        /// Unloads the resource.
        /// </summary>
        /// <param name="resourceIdentifier">The resource identifier.</param>
        void UnloadResource(string resourceIdentifier);

        /// <summary>
        /// Registers the resource loader.
        /// </summary>
        /// <typeparam name="R">The resource type.</typeparam>
        /// <typeparam name="RLP">The resource loading parameters type.</typeparam>
        /// <param name="loader">The loader.</param>
        void RegisterResourceLoader<R, RLP>(ResourceLoader<R, RLP> loader) where RLP : ResourceLoadingParameters<R>;

        /// <summary>
        /// Gets a value indicating whether resources are currently loading.
        /// </summary>
        /// <value>
        ///   <c>true</c> if resources are currently loading; otherwise, <c>false</c>.
        /// </value>
        bool IsLoading { get; }
    }
}
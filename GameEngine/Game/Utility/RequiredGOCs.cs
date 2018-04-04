using System;
using GameEngine.Game.GameObjects.GameObjectComponents;

namespace GameEngine.Game.Utility {
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class RequiredGOCs : Attribute {
        public Type[] Required { get; set; }
        public bool InHierarchy { get; set; }

        public RequiredGOCs(bool inHierarchy, Type req, params Type[] requiredTypes) {
            InHierarchy = inHierarchy;
            Required = new Type[requiredTypes.Length + 1];

            if (!req.IsSubclassOf(typeof(GOC)))
                throw new ArgumentException(nameof(req));
            Required[0] = req;

            for (int i = 0; i < requiredTypes.Length; i++) {
                if (!requiredTypes[i].IsSubclassOf(typeof(GOC)))
                    throw new ArgumentException(nameof(requiredTypes));

                Required[i + 1] = requiredTypes[i];
            }
        }
    }
}
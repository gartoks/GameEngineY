using System;

namespace GameEngine.Game.Utility {
    [Flags]
    public enum GOCSearchMode {
        This = 0, ParentalHierarchy = 1, ChildHierarchy = 2
    }
}
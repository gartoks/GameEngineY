using System;

namespace GameEngine.Exceptions {
    public class RenderException : SystemException {

        public RenderException() {
        }

        public RenderException(string message) 
            :base (message) {

        }

    }
}

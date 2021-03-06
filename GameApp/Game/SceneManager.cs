﻿using System;
using GameApp.Application;
using GameApp.Graphics;
using GameApp.Resources;
using GameEngine.Game;
using GameEngine.Modding;

namespace GameApp.Game {
    internal class SceneManager : ISceneManager {

        private static SceneManager instance;
        public static SceneManager Instance {
            get => SceneManager.instance;
            private set { if (SceneManager.instance != null) throw new InvalidOperationException("Only one instance of SceneManager permitted."); else instance = value; }
        }

        private Scene scene;

        internal SceneManager() {
            Instance = this;
        }

        internal void Install() { }

        internal bool VerifyInstallation() => true;

        internal void Initialize() {
            Window.Window.Instance.GameWindow.RenderFrame += (sender, args) => Render();
        }

        internal void TryLoadDefaultScene() {
            if (this.scene == null)
                LoadScene(AppConstants.Defaults.SCENE_DEFAULT_SCENE_NAME);
        }

        internal void Update() => this.scene.Update();

        private void Render() {
            if (this.scene == null)
                return;

            GLHandler.Instance.BeginRendering();

            this.scene.Render();

            GLHandler.Instance.EndRendering();
        }

        public void LoadScene(string sceneName) {
            this.scene = new Scene(sceneName);
            // TODO load scene data

            ResourceManager.Instance.ClearSceneResources();

            Modding.ModManager.Instance.BaseMod.OnSceneLoad(sceneName);
            foreach (string modID in Modding.ModManager.Instance.InstalledMods) {
                ModBase mod = Modding.ModManager.Instance.GetMod(modID);

                mod.OnSceneLoad(sceneName);
            }
        }

        internal Scene RawActiveScene => this.scene;

        public IScene ActiveScene => this.scene;
    }
}
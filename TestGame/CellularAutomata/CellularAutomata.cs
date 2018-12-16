using System;
using System.Collections.Generic;
using System.Drawing;
using GameEngine.Application;
using GameEngine.Game.GameObjects.GameObjectComponents;
using GameEngine.Game.Utility;
using GameEngine.Graphics;
using GameEngine.Input;
using GameEngine.Logging;
using GameEngine.Modding;
using GameEngine.Window;
using Color = GameEngine.Utility.Color;

namespace TestGame.CellularAutomata {
    [RequiredGOCs(false, typeof(Sprite))]
    public abstract class CellularAutomata : GOC {

        public int Seed { get; private set; }

        public int CellCount { get; private set; }

        public bool IsTorus { get; private set; }
        public NeighbourhoodMode NeighbourhoodMode { get; private set; }

        public Key PauseKey = Key.Space;
        public Key ExitKey = Key.Escape;

        private Random random;
        private Sprite sprite;
        private Bitmap dataImage1; 
        private Bitmap dataImage2;

        private bool needGeneration;

        private readonly Dictionary<long, int> stateCounter = new Dictionary<long, int>();

        private CellularAutomataCell[,] grid;
        private bool gridSelector;

        private bool isPaused;

        public override void Initialize() {
            Initialize(new object[]{ new CellularAutomataInitializationData(0, 100, true, NeighbourhoodMode.Moore) });
        }

        public override void Initialize(object[] parameters) {

            CellularAutomataInitializationData initializationData;
            if (parameters.Length > 0 && parameters[0] is CellularAutomataInitializationData)
                initializationData = parameters[0] as CellularAutomataInitializationData;
            else
                initializationData = new CellularAutomataInitializationData(0, 100, true, NeighbourhoodMode.Moore);

            CellCount = initializationData.CellCount;
            IsTorus = initializationData.IsTorus;
            NeighbourhoodMode = initializationData.NeighbourhoodMode;
            Seed = initializationData.Seed == 0 ? new Random().Next() : initializationData.Seed;
            this.random = new Random(Seed);
            needGeneration = !initializationData.IsReadOnly();

            sprite = GameObject.GetComponent<Sprite>();
            sprite.SetTexture(GraphicsHandler.CreateDefaultTexture(CellCount, CellCount, Color.WHITE));
            this.dataImage1 = new Bitmap(CellCount, CellCount);
            this.dataImage2 = new Bitmap(CellCount, CellCount);

            isPaused = true;

            this.grid = new CellularAutomataCell[CellCount, CellCount];
            InitializeGrid(initializationData.CellData);

            InputHandler.AddKeyUpEventHandler((key, modifiers) => {
                if (key == ExitKey)
                    ModBase.Instance.Shutdown();
                else if (key == PauseKey)
                    isPaused = !isPaused;
            });
        }

        private void InitializeGrid((long state, float stateValue)[,] initCellData) {
            foreach (long state in States) {
                this.stateCounter[state] = 0;
            }

            using (Graphics g = Graphics.FromImage(DataImage)) {
                for (int y = 0; y < CellCount; y++) {
                    for (int x = 0; x < CellCount; x++) {
                        long state;
                        float stateValue;
                        if (!needGeneration) {
                            this.grid[x, y] = new CellularAutomataCell(initCellData[x, y], () => gridSelector);
                            state = this.grid[x, y].State;
                            stateValue = this.grid[x, y].StateValue;
                        } else {
                            this.grid[x, y] = new CellularAutomataCell(() => gridSelector);
                            GenerateCellState(this.random, x, y, out state, out stateValue, out object cellData);
                            this.grid[x, y].Set(state, stateValue, cellData);
                        }

                        g.FillRectangle(new SolidBrush(GetStateColor(state, stateValue).ToWinColor), x, y, 1, 1);   //TODO cache brush
                        TrackState(state, null);
                    }
                }
            }

            Bitmap drawImage = DataImage;
            sprite.Texture.UpdateData(drawImage);
        }

        protected override void Update() {
            //UpdateInput();

            //UpdateEditing();

            Window.Title = $"{Window.FramesPerSecond}fps {Time.UpdatesPerSecond}ups";

            if (isPaused)
                return;

            UpdateCells(Time.DeltaTime);
        }

        private void UpdateCells(float deltaTime) {
            for (int y = 0; y < CellCount; y++) {
                for (int x = 0; x < CellCount; x++) {
                    CurrentStateAt(x, y, out long currentState, out float currentStateValue, out object currentCellData);
                    CalculateNewState(x, y, currentState, currentStateValue, currentCellData, deltaTime, random, out long state, out float stateValue, out object cellData);
                    this.grid[x, y].SetCalculation(state, stateValue);
                    DataImage.SetPixel(x, y, GetStateColor(state, stateValue).ToWinColor);
                    TrackState(this.grid[x, y].CalculationState, this.grid[x, y].State);
                }
            }
            //this.stateHistoryIndicator++;
            //this.stateHistoryIndicator %= StateHistorySize;

            Bitmap drawImage = DataImage;
            sprite.Texture.UpdateData(drawImage);
            SwapGrids();

            StepComplete();
        }

        protected abstract void StepComplete();

        protected abstract void GenerateCellState(Random random, int x, int y, out long state, out float stateValue, out object cellData);

        protected abstract void CalculateNewState(int x, int y, long currentState, float currentStateValue, object currentCellData, float deltaTime, Random random, out long state, out float stateValue, out object cellData);

        protected void CurrentStateAt(int x, int y, out long state, out float stateValue, out object cellData) {
            if (IsTorus) {
                while (x < 0)
                    x += CellCount;
                while (y < 0)
                    y += CellCount;

                x %= CellCount;
                y %= CellCount;
            }

            if (x < 0 || x >= CellCount)
                throw new ArgumentOutOfRangeException(nameof(x));

            if (y < 0 || y >= CellCount)
                throw new ArgumentOutOfRangeException(nameof(y));

            CellularAutomataCell c = this.grid[x, y];
            state = c.State;
            stateValue = c.StateValue;
            cellData = c.CellData;
        }

        protected int Neighbours(int x, int y, long state) {
            int count = 0;
            for (int yi = -1; yi <= 1; yi++) {
                for (int xi = -1; xi <= 1; xi++) {
                    if (xi == 0 && yi == 0)
                        continue;

                    if (NeighbourhoodMode == NeighbourhoodMode.VonNeumann && Math.Abs(xi) == 1 && Math.Abs(yi) == 1)
                        continue;

                    int xx = x + xi;
                    int yy = y + yi;

                    if (IsTorus) {
                        xx = xx < 0 ? xx + CellCount : xx;
                        yy = yy < 0 ? yy + CellCount : yy;
                        xx = xx >= CellCount ? xx - CellCount : xx;
                        yy = yy >= CellCount ? yy - CellCount : yy;
                    }

                    if (xx >= 0 && xx < CellCount && yy >= 0 && yy < CellCount && (grid[xx, yy].State & state) > 0)
                        count += 1;
                }
            }

            return count;
        }

        protected HashSet<long> NeighbouringStates(int x, int y) {
            HashSet<long> neighbouringStates = new HashSet<long>();
            for (int yi = -1; yi <= 1; yi++) {
                for (int xi = -1; xi <= 1; xi++) {
                    if (xi == 0 && yi == 0)
                        continue;

                    if (NeighbourhoodMode == NeighbourhoodMode.VonNeumann && Math.Abs(xi) == 1 && Math.Abs(yi) == 1)
                        continue;

                    int xx = x + xi;
                    int yy = y + yi;

                    if (IsTorus) {
                        xx = xx < 0 ? xx + CellCount : xx;
                        yy = yy < 0 ? yy + CellCount : yy;
                        xx = xx >= CellCount ? xx - CellCount : xx;
                        yy = yy >= CellCount ? yy - CellCount : yy;
                    }

                    CellularAutomataCell c = grid[xx, yy];
                    neighbouringStates.Add(c.State);
                }
            }

            return neighbouringStates;
        }

        protected float NeighbourStateValue(int x, int y, long state, Func<float, float, float> action) {
            float value = 0;
            for (int yi = -1; yi <= 1; yi++) {
                for (int xi = -1; xi <= 1; xi++) {
                    if (xi == 0 && yi == 0)
                        continue;

                    if (NeighbourhoodMode == NeighbourhoodMode.VonNeumann && Math.Abs(xi) == 1 && Math.Abs(yi) == 1)
                        continue;

                    int xx = x + xi;
                    int yy = y + yi;

                    if (IsTorus) {
                        xx = xx < 0 ? xx + CellCount : xx;
                        yy = yy < 0 ? yy + CellCount : yy;
                        xx = xx >= CellCount ? xx - CellCount : xx;
                        yy = yy >= CellCount ? yy - CellCount : yy;
                    }

                    CellularAutomataCell c = grid[xx, yy];
                    if (xx >= 0 && xx < CellCount && yy >= 0 && yy < CellCount && (c.State & state) > 0)
                        value = action(value, c.StateValue);
                }
            }

            return value;
        }

        protected IEnumerable<object> NeighbourCellData(int x, int y) {
            for (int yi = -1; yi <= 1; yi++) {
                for (int xi = -1; xi <= 1; xi++) {
                    if (xi == 0 && yi == 0)
                        continue;

                    if (NeighbourhoodMode == NeighbourhoodMode.VonNeumann && Math.Abs(xi) == 1 && Math.Abs(yi) == 1)
                        continue;

                    int xx = x + xi;
                    int yy = y + yi;

                    if (IsTorus) {
                        xx = xx < 0 ? xx + CellCount : xx;
                        yy = yy < 0 ? yy + CellCount : yy;
                        xx = xx >= CellCount ? xx - CellCount : xx;
                        yy = yy >= CellCount ? yy - CellCount : yy;
                    }

                    CellularAutomataCell c = grid[xx, yy];
                    yield return c.CellData;
                }
            }
        }

        public int StateCount(long state) {
            lock (this.stateCounter) {
                if (!this.stateCounter.ContainsKey(state))
                    throw new ArgumentException();

                return this.stateCounter[state];
            }
        }

        private void TrackState(long state, long? oldState) {
            lock (this.stateCounter) {
                if (oldState != null)
                    this.stateCounter[(long)oldState]--;


                //if (this.stateCounter.TryGetValue(state, out int count))
                this.stateCounter[state] = this.stateCounter[state] + 1;
                //else
                //this.stateCounter[state] = 1;

                //this.stateHistory[stateHistoryIndicator][state] = this.stateCounter[state] / (float)(CellCount * CellCount);
            }
        }

        public abstract IEnumerable<long> States { get; }

        public abstract Color GetStateColor(long state, float stateValue);

        public abstract string CellStateToString(long state);

        protected abstract string SavePath { get; }

        private void SwapGrids() {
            gridSelector = !gridSelector;
        }

        private Bitmap DataImage => gridSelector ? this.dataImage1 : this.dataImage2;
    }
}
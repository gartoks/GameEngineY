using System;
using System.Linq;
using System.Threading;
using GameEngine.Game.GameObjects.GameObjectComponents;
using GameEngine.Graphics;
using GameEngine.Graphics.Utility;
using GameEngine.Logging;
using GameEngine.Math;
using GameEngine.Resources;
using GameEngine.Utility;
using TestGame.CellularAutomata.ForestFire;

namespace TestGame {
    public class PieChart : GOC {

        private float[] startAngles;
        private float[] sweepAngles;
        private CustomSprite[] pieSprites;

        public override void Initialize() {
            base.Initialize();
        }

        public override void Initialize(object[] parameters) {
            ForestFire ff = parameters[0] as ForestFire;

            this.startAngles = new float[ff.States.Count()];
            this.sweepAngles = new float[ff.States.Count()];
            this.pieSprites = new CustomSprite[ff.States.Count()];

            ResourceManager.TryGetResource("internal_circleShader", out IShader shader, true);

            int cellCount0 = ff.CellCount * ff.CellCount;

            float globalStartAngle = 0;
            int i = 0;
            foreach (long ffState in ff.States) {

                if (i == 0)
                    continue;

                float pct0 = ff.StateCount(ffState) / (float)cellCount0;
                
                // uniforms
                Color cSpriteColor = ff.GetStateColor(ffState, 0.5f);
                float innerRadius = 0.01f;
                startAngles[i] = globalStartAngle;
                sweepAngles[i] = pct0;
                globalStartAngle += pct0;

                // vertex attributes
                (VertexAttribute va, float[][] data)[] vertexAttributes = {
                    (new VertexAttribute("in_localCoords", VertexAttributeType.FloatVector2),
                    new[] {
                        new[] {-0.5f, 0.5f},
                        new[] {0.5f, 0.5f},
                        new[] {0.5f, -0.5f},
                        new[] {-0.5f, -0.5f}
                    })
                };
                int k = i;

                void UniformAssigner(IShaderUniformAssigner assigner, IUniform uniform) {

                    if (uniform.Name.Equals("u_color"))
                        assigner.SetUniform(uniform.Name, cSpriteColor.r, cSpriteColor.g, cSpriteColor.b, cSpriteColor.a);
                    else if (uniform.Name.Equals("u_innerRadius"))
                        assigner.SetUniform(uniform.Name, innerRadius);
                    else if (uniform.Name.Equals("u_startAngle"))
                        assigner.SetUniform(uniform.Name, startAngles[k]);
                    else if (uniform.Name.Equals("u_sweepAngle"))
                        assigner.SetUniform(uniform.Name, sweepAngles[k]);
                }

                CustomSprite cSprite = GameObject.AddComponent<CustomSprite>(shader, vertexAttributes, (Action<IShaderUniformAssigner, IUniform>)UniformAssigner);
                pieSprites[i] = cSprite;


                i++;
                
                Thread.Sleep(150 / 6);
            }

            ff.OnStepComplete += ca => {
                int cellCount = ff.CellCount * ff.CellCount;

                float startAngle = 0;
                int j = 0;
                foreach (long ffState in ff.States) {
                    int stateCount = ff.StateCount(ffState);
                    float pct = stateCount / (float)cellCount;

                    startAngles[j] = startAngle;
                    sweepAngles[j] = pct;

                    startAngle += pct;

                    j++;
                }
            };
        }

        public override void Death() {
            base.Death();
        }

        protected override void Update() {
            base.Update();
        }
    }
}
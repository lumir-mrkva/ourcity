using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using JigLibX.Physics;
using JigLibX.Collision;
using JigLibX.Geometry;

using OurCityEngine.Cameras;
using OurCityEngine.Debug;
using OurCityEngine.Utils;

namespace OurCityEngine.PhysicObjects
{

    /// <summary>
    /// Helps to combine the physics with the graphics.
    /// </summary>
    public abstract class PhysicObject : DrawableGameComponent
    {

        protected Body body;
        protected CollisionSkin collision;

        protected Model model;
        protected Vector3 color;
        protected Game game;
        protected Vector3 scale = Vector3.One;

        public Body PhysicsBody{get { return body; }}
        public CollisionSkin PhysicsSkin{ get { return collision; }}
        public Game GameplayScreen { get { return game; } }

        protected static Random random = new Random();

        public PhysicObject(Game game, Model model)
            : base(game)
        {
            this.model = model;
//             color = new Vector3(random.Next(255), random.Next(255), random.Next(255));
//             color /= 255.0f;
            color = RandomHelper.Instance.StandardColor.ToVector3();
            this.game = game;
        }

        public PhysicObject(Game game)
            : base(game)
        {
            this.model = null;
            color = new Vector3(random.Next(255), random.Next(255), random.Next(255));
            color /= 255.0f;
            this.game = game;
        }

        protected Vector3 SetMass(float mass)
        {
            PrimitiveProperties primitiveProperties =
                new PrimitiveProperties(PrimitiveProperties.MassDistributionEnum.Solid, PrimitiveProperties.MassTypeEnum.Density, mass);

            float junk; Vector3 com; Matrix it, itCoM;

            collision.GetMassProperties(primitiveProperties, out junk, out com, out it, out itCoM);
            body.BodyInertia = itCoM;
            body.Mass = junk;

            return com;
        }
        Matrix[] boneTransforms = null;
        int boneCount = 0;

        public abstract void ApplyEffects(BasicEffect effect);
        public override void Draw(GameTime gameTime)
        {
            

            if (model != null)
            {
                if (boneTransforms == null || boneCount != model.Bones.Count)
                {
                    boneTransforms = new Matrix[model.Bones.Count];
                    boneCount = model.Bones.Count;
                }

                model.CopyAbsoluteBoneTransformsTo(boneTransforms);




                Camera camera = CameraManager.Instance.Cameras["Default"];

                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (Effect eff in mesh.Effects)
                    {

                        BasicEffect effect = eff as BasicEffect;

                        if (effect == null) return;

                        // the body has an orientation but also the primitives in the collision skin
                        // owned by the body can be rotated!
                        if (body.CollisionSkin != null)
                            effect.World = boneTransforms[mesh.ParentBone.Index] * Matrix.CreateScale(scale) * body.CollisionSkin.GetPrimitiveLocal(0).Transform.Orientation * body.Orientation * Matrix.CreateTranslation(body.Position);
                        else
                            effect.World = boneTransforms[mesh.ParentBone.Index] * Matrix.CreateScale(scale) * body.Orientation * Matrix.CreateTranslation(body.Position);

                        effect.View = camera.View;
                        effect.Projection = camera.Projection;

                        effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;

                        ApplyEffects(effect);

                        //if (!this.PhysicsBody.IsActive)
                        //    effect.Alpha = 0.4f;
                        //else
                        //    effect.Alpha = 1.0f;

                    }
                    mesh.Draw();
                }


            }

            // DebugDrawer
            DebugDrawer drawer = DebugManager.Instance.Drawer;
            if (drawer != null && drawer.Enabled)
            {

                wf = collision.GetLocalSkinWireframe();

                // if the collision skin was also added to the body
                // we have to transform the skin wireframe to the body space
                if (body.CollisionSkin != null)
                {
                    body.TransformWireframe(wf);
                }

                drawer.DrawShape(wf);
            }

            // base.Draw(gameTime);
        }



        VertexPositionColor[] wf;

    }
}

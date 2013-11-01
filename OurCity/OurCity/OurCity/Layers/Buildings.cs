using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using OurCityEngine.PhysicObjects;
using OurCityEngine.Layers;
using Microsoft.Xna.Framework.Graphics;
using JigLibX.Geometry;
using JigLibX.Collision;
using OurCityEngine.Utils;

namespace OurCity.Layers {
    /// <summary>
    /// Layer of city builduings
    /// </summary>
    class Buildings : Layer {

        ContentManager content;
        Model boxModel, b1Model, b2Model, b3Model;
        PhysicObject island;
        List<Vector3> positions = new List<Vector3>();

        public Buildings(Game game, PhysicObject island)
            : base(game) {
            content = new ContentManager(game.Services, "Content");
            this.island = island;
        }

        protected override void LoadContent()
        {
            // load buildings models
            boxModel = content.Load<Model>("Buildings/box");
            //b1Model = content.Load<Model>("Buildings/B1");
            //b2Model = content.Load<Model>("Buildings/B2");
            //b3Model = content.Load<Model>("Buildings/B3");

            //TODO load positions from xml
            positions.Add(new Vector3(166.9487f, 1f, 434.3723f));
            positions.Add(new Vector3(168.4675f, 1, 549.2824f));
            positions.Add(new Vector3(167.6763f, 1, 677.634f));
            //positions.Add(new Vector3(160.0003f, 1, 795.5444f));
            positions.Add(new Vector3(283.9968f, 1, 797.2574f));
            positions.Add(new Vector3(272.4207f, 1, 677.0789f));
            positions.Add(new Vector3(275.1774f, 1, 547.6032f));
            positions.Add(new Vector3(273.0377f, 1, 431.0633f));
            positions.Add(new Vector3(269.0444f, 1, 303.8127f));
            positions.Add(new Vector3(-43.94331f, 1, 179.145f));
            positions.Add(new Vector3(-47.37173f, 1, 322.9816f));
            positions.Add(new Vector3(-46.40455f, 1, 441.4849f));
            positions.Add(new Vector3(-31.72021f, 1, 672.3676f));
            positions.Add(new Vector3(-42.207f, 1, 795.5719f));
            positions.Add(new Vector3(-53.9709f, 1, 936.0055f));
            positions.Add(new Vector3(60.96737f, 1, 923.8198f));
            positions.Add(new Vector3(33.68722f, 1, 814.2637f));
            positions.Add(new Vector3(27.85988f, 1, 670.6398f));
            positions.Add(new Vector3(26.70795f, 1, 555.5757f));
            positions.Add(new Vector3(33.56064f, 1, 440.2159f));
            positions.Add(new Vector3(49.79668f, 1, 318.3828f));
            positions.Add(new Vector3(-157.3272f, 1, 315.545f));
            positions.Add(new Vector3(-161.9852f, 1, 166.9004f));
            positions.Add(new Vector3(-223.9722f, 1, 89.26124f));
            positions.Add(new Vector3(-236.7276f, 1, -62.83633f));
            positions.Add(new Vector3(-234.8328f, 1, -191.267f));
            positions.Add(new Vector3(-146.1622f, 1, -181.0468f));
            positions.Add(new Vector3(-244.032f, 1, 327.1061f));
            positions.Add(new Vector3(-252.514f, 1, 427.2779f));
            positions.Add(new Vector3(-256.5259f, 1, 556.8967f));
            positions.Add(new Vector3(-259.7678f, 1, 610.7238f));
            positions.Add(new Vector3(-256.6954f, 1, 673.5879f));
            positions.Add(new Vector3(-249.2846f, 1, 794.825f));
            positions.Add(new Vector3(-149.3899f, 1, 675.6553f));
            positions.Add(new Vector3(-195.9989f, 1, -680.9547f));
            positions.Add(new Vector3(-149.2921f, 1, -795.1763f));
            positions.Add(new Vector3(56.96995f, 1, -801.707f));
            positions.Add(new Vector3(56.9794f, 1, -676.1238f));
            positions.Add(new Vector3(45.63493f, 1, -548.5434f));
            positions.Add(new Vector3(214.2144f, 1, -550.7584f));
            positions.Add(new Vector3(272.5127f, 1, -570.7652f));
            positions.Add(new Vector3(277.0208f, 1, -672.0103f));
            //positions.Add(new Vector3(201.0361f, 1, -779.8707f));
            //positions.Add(new Vector3(195.336f, 1, -845.1798f));
            positions.Add(new Vector3(268.6021f, 1, -435.1483f));
            positions.Add(new Vector3(176.1388f, 1, -302.9746f));
            positions.Add(new Vector3(170.0108f, 1, -189.8559f));
            positions.Add(new Vector3(272.4767f, 1, -165.8992f));
            positions.Add(new Vector3(276.6815f, 1, -66.49078f));
            positions.Add(new Vector3(275.2916f, 1, 22.44919f));
            positions.Add(new Vector3(275.4729f, 1, 94.04279f));
            //positions.Add(new Vector3(274.844f, 1, 171.3921f));


            // spawn box buildings
            foreach (Vector3 p in positions) {
                int rand = RandomHelper.Instance.Random.Next(1,3);
                Model model = boxModel;
                switch (rand) {
                    
                    case 1: model = b1Model; break;
                    case 2: model = b2Model; break;
                    case 3: model = b3Model; break;
                    default: model = boxModel; break;
                }
                if (model == null) model = boxModel;

                BuildingObject b = new BuildingObject(Game, model, new Vector3(50, 50, 50), Matrix.Identity, p + new Vector3(0, 23, 0));
                Objects.Add(b);
            }

            //OXO - too big, yet low detail
//             BuildingObject oxo = new BuildingObject(Game, content.Load<Model>("Buildings/Oxo"), Vector3.One, Matrix.Identity, new Vector3(158, 3, 801));
//             oxo.PhysicsSkin.AddPrimitive(new Box(new Vector3(-28.5f, -3, -28.5f), Matrix.Identity, new Vector3(57, 57, 57)), new MaterialProperties(0.8f, 0.8f, 0.7f));
//             Objects.Add(oxo);

            //HQ
            Model hq = content.Load<Model>("Buildings/HQ");
            BuildingObject hqb = new BuildingObject(Game, hq, Vector3.One, Matrix.Identity, new Vector3(0, -4, 0));
            hqb.PhysicsSkin.AddPrimitive(new Sphere(new Vector3(0, 0, 0), 22), new MaterialProperties(0.8f, 0.8f, 0.7f));
            hqb.PhysicsSkin.AddPrimitive(new Box(new Vector3(-10, 0, -10), Matrix.Identity, new Vector3(20,200,20)), new MaterialProperties(0.8f, 0.8f, 0.7f));
            Objects.Add(hqb);

            


            //block1 - too big model (40mb)
//             Model block1 = content.Load<Model>("Buildings/block2");
//             BuildingObject block1b = new BuildingObject(Game, block1, new Vector3(3,3,3), Matrix.CreateRotationX(MathHelper.ToRadians(-90)), new Vector3(230f, 1, -823));
//             block1b.PhysicsSkin.AddPrimitive(new Box(new Vector3(-55, 0, -73), Matrix.Identity, new Vector3(100, 40, 150)), new MaterialProperties(0.8f, 0.8f, 0.7f));
//             Objects.Add(block1b);

            


            // dont check collision between building and island
            foreach (PhysicObject o in Objects) {
                o.PhysicsSkin.NonCollidables.Add(island.PhysicsSkin);
            }
        }

        protected override void UnloadContent() 
        {
            content.Unload();
        }
    }
    
}

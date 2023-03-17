using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using SW=CEM_Lib_Yuchi.SW;

namespace CEM_C_sharp
{
    public class CEM_Material_Properties : GH_Component
    {

        public CEM_Material_Properties()
          : base("CEM_Material_Properties", "CEM_Material_Properties",
              "[The values and options for calculating the gravity]",
              "CEM_TEST", "CEM_C_sharp")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager Input)
        {
            bool default_bool = false;
            double default_yieldStress = 500000;
            double default_specWeight = 73.50;
            Input.AddBooleanParameter("selfWeight", "selfWeight", "[Bool]Activate the gravity porperties", GH_ParamAccess.item, default_bool);
            Input.AddNumberParameter("yieldStress", "yieldStress", "The yieldStress of the structural members[kN/m2]", GH_ParamAccess.item, default_yieldStress);
            Input.AddNumberParameter("specWeight", "specWeight", "The specific weight of the structural members[kN/m3]", GH_ParamAccess.item, default_specWeight);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager Output)
        {
            Output.AddGenericParameter("SW", "SW", "The assembled information of Self Weight and Matrial", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess data)
        {
            bool selfWeight = false;
            double yieldStress = 500000;
            double specWeight = 73.50;
            data.GetData("selfWeight", ref selfWeight);
            data.GetData("yieldStress", ref yieldStress);
            data.GetData("specWeight", ref specWeight);
            SW Material = new SW(selfWeight, yieldStress, specWeight);
            data.SetData("SW", Material);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.SW;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("7ba9d2ee-2eb9-4ff3-a515-ecc01b1ef837"); }
        }
    }
}
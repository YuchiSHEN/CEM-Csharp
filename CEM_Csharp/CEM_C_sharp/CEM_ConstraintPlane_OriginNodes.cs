using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using CO = CEM_Lib_Yuchi.CO;

namespace CEM_C_sharp
{
    public class CEM_ConstraintPlane_OriginNodes : GH_Component
    {

        public CEM_ConstraintPlane_OriginNodes()
          : base("CEM_ConstraintPlane_OriginNodes", "CEM_ConstraintPlane_OriginNodes",
              "[The Constraint Planes effects the Geometry and The OriginNodes define the start points of the generated form]",
              "CEM_TEST", "CEM_C_sharp")
        {
        }
        
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager Input)
        {
            List<int> default_int = new List<int>() {-1};
            List<Point3d> default_Pts = new List<Point3d>() { new Point3d(0.0,0.0,0.0)};
            Input.AddPlaneParameter("constraintPlane", "constraintPlane", "The constraint Plane for specified node [Each Plane input cooresponding to each node input]", GH_ParamAccess.list, Plane.WorldXY);
            Input.AddIntegerParameter("constraintPlaneID", "constraintPlaneID", "The ID number of the specified nodes", GH_ParamAccess.list, default_int);
            Input.AddPointParameter("originNode", "originNode", "The nodes defines the start point of the generated structure form", GH_ParamAccess.list, default_Pts);
            Input.AddIntegerParameter("originNodeID", "originNodeID", "The corresponding nodeID of the orgin nodes which should be identical to the ones in topology", GH_ParamAccess.list, default_int);
        }
        
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager Output)
        {
            Output.AddGenericParameter("CO", "CO", "The assembled constraint Plane information", GH_ParamAccess.item);
        }
         
        protected override void SolveInstance(IGH_DataAccess data)
        {
            List<Plane> constraintPlane = new List<Plane>();
            List<int> constraintPlaneID = new List<int>();
            List<Point3d> originNode = new List<Point3d>();
            List<int> originNodeID = new List<int>();
            data.GetDataList("constraintPlane", constraintPlane);
            data.GetDataList("constraintPlaneID", constraintPlaneID);
            data.GetDataList("originNode", originNode);
            data.GetDataList("originNodeID", originNodeID);

            CO C_O =new CO(constraintPlane, constraintPlaneID, originNode, originNodeID);
            string ERROR_1=C_O.checkPlane(constraintPlane, constraintPlaneID);
            if (ERROR_1 != null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, ERROR_1); }
            string ERROR_2 = C_O.checkNode(originNode, originNodeID);
            if (ERROR_2 != null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, ERROR_2); }
            data.SetData("CO",C_O);
        }

           protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.CPL;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("ef162f6e-52b1-440d-ad99-202e59c1acf1"); }
        }
    }
}
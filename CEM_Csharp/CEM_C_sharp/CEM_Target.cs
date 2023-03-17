using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using TAR = CEM_Lib_Yuchi.TAR;


namespace CEM_C_sharp
{
    public class CEM_Target : GH_Component
    {
        public CEM_Target()
          : base("CEM_Target", "CEM_Target",
              "The Target structural geometry status aimed to approach ",
              "CEM_TEST", "CEM_C_sharp")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager Input)
        {
            List<Point3d> default_poilist = new List<Point3d>() {new Point3d()};
            List<string> default_stringlist = new List<string>() {""};
            List<int> default_intlist = new List<int>() {0};
            double  default_magni = 0.0;
            Input.AddPointParameter("targetNode", "targetNode", "The target node positions try to approach to", GH_ParamAccess.list,default_poilist);
            Input.AddIntegerParameter("targetNodeID", "targetNodeID", "The ID corresponding to the target nodes", GH_ParamAccess.list, default_intlist);
            Input.AddNumberParameter("targetNodeCoeff", "targetNodeCoeff", "The weight of Node Target", GH_ParamAccess.item, default_magni);
            Input.AddVectorParameter("targetVector", "targetVector", "The target Vector try to approach to", GH_ParamAccess.list,new Vector3d());
            Input.AddTextParameter("targetVectorID", "targetVectorID", "The corresponding ID of the Vectors", GH_ParamAccess.list, default_stringlist);
            Input.AddNumberParameter("targetVectorCoeffMag", "targetVectorCoeffMag", "The weight of Vector Magnitude", GH_ParamAccess.item, default_magni);
            Input.AddNumberParameter("targetVectorCoeffDir", "targetVectorCoeffDir", "The weight of Vector Direction", GH_ParamAccess.item, default_magni);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager Output)
        {
            Output.AddGenericParameter("TA", "TA", "The assembled TARGET information", GH_ParamAccess.item);
        }


        protected override void SolveInstance(IGH_DataAccess data)
        {
            List<Point3d> targetNode=new List<Point3d>();
            List<int> targetNodeID=new List<int>();
            double targetNodeCoeff=0.0;
            List<Vector3d> targetVector=new List<Vector3d>();
            List<string> targetVectorID=new List<string>();
            double targetVectorCoeffMag=0.0;
            double targetVectorCoeffDir=0.0;
            data.GetDataList("targetNode", targetNode); 
            data.GetDataList("targetNodeID", targetNodeID);
            data.GetData("targetNodeCoeff",ref targetNodeCoeff);
            data.GetDataList("targetVector", targetVector);
            data.GetDataList("targetVectorID", targetVectorID);
            data.GetData("targetVectorCoeffMag", ref targetVectorCoeffMag);
            data.GetData("targetVectorCoeffDir", ref targetVectorCoeffDir);

            if (targetVectorID.Count > targetVector.Count) { for (int i = 0; i < targetVectorID.Count - 1; i++) { targetVector.Add(targetVector[0]); } };

            TAR Target = new TAR(targetNode, targetNodeID, targetNodeCoeff, targetVector, targetVectorID, targetVectorCoeffMag, targetVectorCoeffDir);
            Target.report += string.Format(
                "[The Target Setting]:\r\n" +
                "The targetNode num to approach:[{0}]\r\n" +
                "The targetNodeID num:[{1}]\r\n" +
                "The targetNode Weight:[{2}]\r\n" +
                "\r\n" +
                "The targetVector num to approach:[{3}]\r\n" +
                "The targetVectorID:[{4}]\r\n" +
                "The targetVector Magnitude Weight:[{5}]\r\n" +
                "The targetVector Direction Weight:[{6}]",
                Target.targetNode.Count,
                Target.targetNodeID.Count,
                Target.targetNodeCoeff,
                Target.targetVector.Count,
                Target.targetVectorID.Count,
                Target.targetVectorCoeffMag,
                Target.targetVectorCoeffDif);
            data.SetData("TA", Target);
        }
        
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.TAR;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("27d476ce-cad9-49a7-9884-6a0a4e0404bb"); }
        }
    }
}
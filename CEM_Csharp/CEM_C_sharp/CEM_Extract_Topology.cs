using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

using DATA_TO = CEM_Lib_Yuchi.DATA_TO;
using Extract_Topology = CEM_Lib_Yuchi.Extract_Topology;

namespace CEM_C_sharp
{
    public class CEM_Extract_Topology : GH_Component
    {
        public CEM_Extract_Topology()
          : base("CEM_Extract_Topology", "CEM_Extract_Topology",
              "[Extract information from the assembled Topology]",
              "CEM_TEST", "CEM_C_sharp")
        {
        }
         protected override void RegisterInputParams(GH_Component.GH_InputParamManager Input)
        {
            Input.AddGenericParameter("T", "T", "The topology data assembled in Build_Topology", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager Output)
        {
            Output.AddPointParameter("nodes", "nodes", "The nodes in sequence from the topology", GH_ParamAccess.list);
            Output.AddTextParameter("nodesID","nodesID","The nodesID presents the lay and the sequence of the node",GH_ParamAccess.list);
            Output.AddLineParameter("edges", "edges", "All the edges in topology model including the deviations and trails", GH_ParamAccess.list);
            Output.AddTextParameter("edgesID", "edgesID", "Two number ID which present the location Indexs in the Edge Metrix [A][B]", GH_ParamAccess.list);
            Output.AddPlaneParameter("constraintPlane", "constraintPlane", "The constraint Planes for each none_original nodes [XYPlane for default] ", GH_ParamAccess.list);
            Output.AddIntegerParameter("constraintPlaneID", "constraintPlaneID", "The corresponding NodeID for the constaint Plane [none_original nodes ID]", GH_ParamAccess.list);
            Output.AddPointParameter("originNode","originNode","The positions of the original nodes[The node starts to calculate in trails]",GH_ParamAccess.list);
            Output.AddIntegerParameter("originNodeID", "originNodeID", "The corresponding NodeID for the original nodes", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess data)
        {
            //_00_Chech the Input:
            DATA_TO TOPO = new DATA_TO();
            data.GetData("T",ref TOPO);
            
            //_01_Output nodes and nodes ID:
            data.SetDataList("nodes", TOPO.Nodes_InSeq);
            data.SetDataList("nodesID", Extract_Topology.ExtractNodesID(TOPO));

            //_02_OutPut edges and edgesID:
            List<Line> EdgesLines; List<string> EdgesID;
            Extract_Topology.ExtractEdges(TOPO, out EdgesLines, out EdgesID);
            data.SetDataList("edges", EdgesLines);
            data.SetDataList("edgesID", EdgesID);

            //_03_OutPut constraintPlane and constraintPlaneID
            List<int> allnodes_exceptstart;
            List<Plane> constraint_plane;
            Extract_Topology.ExtractConstraintPlane(TOPO, "FA", out allnodes_exceptstart, out constraint_plane);
            data.SetDataList("constraintPlane", constraint_plane);
            data.SetDataList("constraintPlaneID", allnodes_exceptstart);

            //_04_OutPut originNode and originNodeID
            List<int> originNode_ID;
            List<Point3d> originNode_pts;
            Extract_Topology.ExtractOrginNodes(TOPO, out originNode_ID, out originNode_pts);
            data.SetDataList("originNode", originNode_pts);
            data.SetDataList("originNodeID", originNode_ID);
        }
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.ETOPO;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("8a4613a8-add2-4d08-a5c5-cbf2be98a87b"); }
        }
    }
}
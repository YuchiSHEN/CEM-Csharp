using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using Node = CEM_Lib_Yuchi.Node;
using Edge = CEM_Lib_Yuchi.Edge;
using DATA_TO = CEM_Lib_Yuchi.DATA_TO;
using Topology = CEM_Lib_Yuchi.Topology;


namespace CEM_C_sharp
{
    public class CEM_Build_Topology : GH_Component
    {
  
        public CEM_Build_Topology()
          : base("CEM_Build_Topology", "CEM_Build_Topology",
              "[Initialize the geometry into Topology Form]",
              "CEM_TEST", "CEM_C_sharp")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager Input)
        {
            Line default_line = new Line(new Point3d(0,0,0),new Point3d(0,0,0));
            Point3d default_pt = new Point3d(0, 0, 0);
            Vector3d default_vec = new Vector3d(0,0,0);
            List<double> default_double = new List<double>() {0.0};
            Input.AddLineParameter("Trails", "Trails", "Input the Trail line segments", GH_ParamAccess.list, default_line);
            Input.AddNumberParameter("Trail_lengths", "Trail_lengths", "The length of the input trail(a list or one value for all list)", GH_ParamAccess.list, default_double);

            Input.AddLineParameter("Deviation_line", "Deviation_line", "Input the Deviation_line segments", GH_ParamAccess.list, default_line);
            Input.AddNumberParameter("Deviation_Magni", "Deviation_Magni", "The magnitude of the input deviation edge[+tension-compression](a list or one value for all)", GH_ParamAccess.list, default_double);

            Input.AddVectorParameter("Load_Vec", "Load_Vec", "The Load Vector3d presents the direction and magnitude of the load(a list or one value for all)", GH_ParamAccess.list, default_vec);
            Input.AddPointParameter("Load_Poi", "Load_Poi", "The action points of the loads which should meet to the points of the Topology", GH_ParamAccess.list, default_pt);

            Input.AddPointParameter("SupportPoints", "SupportPoints", "The support points of the structure which should be the end of the trail and have reaction forces", GH_ParamAccess.list,default_pt);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager Output)
        {
            Output.AddGenericParameter("T", "T", "The assembled Topology Model of the structure", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess data)
        {
            //initialize the parametirc input
            List<Line> Trails = new List<Line>();
            List<double> Trail_lengths = new List<double>();
            List<Line> Deviation_line = new List<Line>();
            List<double> Deviation_Magni = new List<double>();
            List<Vector3d> Load_Vec = new List<Vector3d>();
            List<Point3d> Load_Poi = new List<Point3d>();
            List<Point3d> SupportPoints = new List<Point3d>();

            data.GetDataList("Trails", Trails);
            data.GetDataList("Trail_lengths", Trail_lengths);
            data.GetDataList("Deviation_line", Deviation_line);
            data.GetDataList("Deviation_Magni", Deviation_Magni);
            data.GetDataList("Load_Vec", Load_Vec);
            data.GetDataList("Load_Poi", Load_Poi);
            data.GetDataList("SupportPoints", SupportPoints);

            //_00_Initially check the Input paras
            string ERROR_check = Topology.checkdata(Trails, Trail_lengths, Deviation_line, Deviation_Magni, Load_Vec, Load_Poi, SupportPoints);
            if (ERROR_check != "INPUT_ERROR:") { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, ERROR_check); }

            //_01_System configuration:
            double Sys_threshold = 0.000001;

            //_02_ANAYSIS_Trail_Lines:
            List<Point3d> Nodes_InSeq;
            List<int> Trail_numList;
            List<int> NodeID_list;
            List<int> SupportID_list;
            string ERROR_ANAYSIS_Trail_Lines;
            Topology.ANAYSIS_Trail_Lines(Trails, SupportPoints, Sys_threshold, out NodeID_list, out Trail_numList, out SupportID_list, out Nodes_InSeq, out ERROR_ANAYSIS_Trail_Lines);
            if (ERROR_ANAYSIS_Trail_Lines != null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, ERROR_ANAYSIS_Trail_Lines);}

            //_03_Each lay node ID(The sequence to calculate the topology):
            List<List<int>> EachTrail_NodeList;
            List<List<int>> EachLay_NodeList;
            string ERROR_GetHierarchical_nodeID;
            Topology.GetHierarchical_nodeID(NodeID_list, Trail_numList, SupportID_list, out EachTrail_NodeList, out EachLay_NodeList, out ERROR_GetHierarchical_nodeID);
            if (ERROR_GetHierarchical_nodeID != null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, ERROR_GetHierarchical_nodeID); }

            //_04_Initialise Node_class list:
            List<Node> Nodes_List;
            Topology.Initialize_NodeClass(Nodes_InSeq, EachTrail_NodeList, out Nodes_List);

            //_05_Import ExternalForces:
            string ERROR_ImportExternalForce;
            Topology.ImportExternalForce(Nodes_List, Load_Poi, Load_Vec, Sys_threshold,out ERROR_ImportExternalForce);
            if (ERROR_ImportExternalForce != null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, ERROR_ImportExternalForce); }

            //_06_Initialist Edge_class Metrix:
            List<List<Edge>> Edge_Metrix;
            Topology.Initialize_EdgeClass(Nodes_InSeq, out Edge_Metrix);

            //_07_Import Trail into Joint Metrix Map:
            string ERROR_ImportTrail;
            Topology.ImportTrail(Trails, Nodes_List, Edge_Metrix, Trail_lengths, Sys_threshold,out ERROR_ImportTrail);
            if (ERROR_ImportTrail != null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, ERROR_ImportTrail); }

            //_08_Import Devitaions into Joint Metrix Map
            List<List<int>> InDevMirroList = new List<List<int>>();
            string ERROR_ImportDeviation;
            Topology.ImportDeviation(Deviation_line, Deviation_Magni, Nodes_List, Edge_Metrix, EachLay_NodeList, Sys_threshold, ref InDevMirroList, out ERROR_ImportDeviation);
            if (ERROR_ImportDeviation != null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, ERROR_ImportDeviation); }
            
            //_09_Export all the DATA into class DATA_TO
            DATA_TO TO = new DATA_TO(Sys_threshold, Nodes_InSeq, Trail_numList, NodeID_list, SupportID_list, EachTrail_NodeList, EachLay_NodeList, Nodes_List, Edge_Metrix, InDevMirroList);
            //_10_Report TOPO infomation

            TO.DATA_FROM = string.Format(
                "__|THE TOPOLOGY DATA|__:\r\n" +
                "The Trail number:[{0}]\r\n" +
                 "The Direct Deviation:[{1}]\r\n" +
                "The Indirect Deviation:[{2}]\r\n" +
                "The Deviation Magnitude values:[{3}]\r\n" +
                "The Trail segments:[{4}]\r\n" +
                "The Trail length values:[{5}]\r\n" +
                "The Load Vector num:[{6}]\r\n" +
                "The Load Points num:[{7}]\r\n" +
                "The Support Points num:[{8}]\r\n", TO.EachTrail_NodeList.Count,Topology.CountEdges(TO)[0], Topology.CountEdges(TO)[1], Deviation_Magni.Count, Topology.CountEdges(TO)[2], Trail_lengths.Count, Load_Vec.Count, Load_Poi.Count, SupportPoints.Count);

            data.SetData("T", TO);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.TOPO;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("405efdd3-631c-4647-bbab-4796f35390b7"); }
        }
    }
}

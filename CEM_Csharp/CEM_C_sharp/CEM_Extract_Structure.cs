using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using Node = CEM_Lib_Yuchi.Node;
using Edge = CEM_Lib_Yuchi.Edge;
using DATA_TO = CEM_Lib_Yuchi.DATA_TO;
using Color = System.Drawing.Color;
using Extract_Structure = CEM_Lib_Yuchi.Extract_Structure;

namespace CEM_C_sharp
{
    public class CEM_Extract_Structure : GH_Component
    {

        public CEM_Extract_Structure()
          : base("CEM_Extract_Structure", "CEM_Extract_Structure",
              "[Extract the Calculated Model]",
              "CEM_TEST", "CEM_C_sharp")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager Input)
        {
            Input.AddGenericParameter("M", "M", "The Calculated Structure Model", GH_ParamAccess.item);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager Output)
        {
            Output.AddPointParameter("nodes", "nodes", "The nodes in sequence from the calculated structure", GH_ParamAccess.list);
            Output.AddTextParameter("nodesID", "nodesID", "The nodesID presents the lay and the sequence of the node", GH_ParamAccess.list);

            Output.AddLineParameter("trails", "trails", "Trail Edges Lines in structure",GH_ParamAccess.list);
            Output.AddTextParameter("trailsID", "trailsID", "The Trail ID presented within two nodeID [A][B]", GH_ParamAccess.list);
            Output.AddColourParameter("trailsCol", "trailsCol", "The display color of the Trail Edge", GH_ParamAccess.list);
            Output.AddNumberParameter("trailsMag", "trailsMag", "The magnitude of the inner force of the edge[+tension -compression]", GH_ParamAccess.list);

            Output.AddLineParameter("deviations", "deviations", "Deviation Edges Lines in structure", GH_ParamAccess.list);
            Output.AddTextParameter("deviationsID", "deviationsID", "The Deviation ID presented within two nodeID [A][B]", GH_ParamAccess.list);
            Output.AddColourParameter("deviationsCol", "deviationsCol", "The display color of the Deviation Edge", GH_ParamAccess.list);
            Output.AddNumberParameter("deviationsMag", "deviationsMag", "The magnitude of the inner force of the edge[+tension -compression]", GH_ParamAccess.list);

            Output.AddLineParameter("externalForces", "externalForces", "External force lines", GH_ParamAccess.list);
            Output.AddColourParameter("externalForcesCol", "externalForcesCol", "The display color of the external force[Green]", GH_ParamAccess.list);
            Output.AddNumberParameter("externalForcesMag", "externalForcesMag", "The magnitude of the external force", GH_ParamAccess.list);
        }


        protected override void SolveInstance(IGH_DataAccess data)
        {
            DATA_TO TOPO = new DATA_TO();
            data.GetData("M",ref TOPO);
            //System configuration:
            Color Blue = Color.FromArgb(255, 65, 80, 200);
            Color Red = Color.FromArgb(255, 200, 0, 0);
            Color Green = Color.FromArgb(255, 20, 135, 20);
            Color Black = Color.FromArgb(255, 0, 0, 0);

            //Nodes
            data.SetDataList("nodes", TOPO.Nodes_InSeq);
            data.SetDataList("nodesID", Extract_Structure.ExtractNodesID(TOPO));

            //Edges
            List<Edge> Edge_list = Extract_Structure.MakeMetrixIntoList(TOPO);

            //Trail
            List<Line> lines = new List<Line>();
            List<string> ID = new List<string>();
            List<double> magnitude = new List<double>();
            List<Color> Col = new List<Color>();

            Extract_Structure.ExtractDataFromEdges(TOPO, Edge_list, "TL", Red, Blue, Black, ref lines, ref ID, ref magnitude, ref Col);
            data.SetDataList("trails", lines);
            data.SetDataList("trailsID", ID);
            data.SetDataList("trailsMag", magnitude);
            data.SetDataList("trailsCol", Col);

            //Dev
            List<Line> dev_lines = new List<Line>();
            List<string> dev_ID = new List<string>();
            List<double> dev_magnitude = new List<double>();
            List<Color> dev_Col = new List<Color>();

            Extract_Structure.ExtractDataFromEdges(TOPO, Edge_list, "DE", Red, Blue, Black, ref dev_lines, ref dev_ID, ref dev_magnitude, ref dev_Col);
            Extract_Structure.ExtractDataFromEdges(TOPO, Edge_list, "ID", Red, Blue, Black, ref dev_lines, ref dev_ID, ref dev_magnitude, ref dev_Col);

            data.SetDataList("deviations", dev_lines);
            data.SetDataList("deviationsID", dev_ID);
            data.SetDataList("deviationsCol", dev_Col);
            data.SetDataList("deviationsMag", dev_magnitude);

            //Reaction force and external forces
            List<Line> external_lines;
            List<double> external_mags;
            List<Color> external_Col;
            Extract_Structure.ExtractExternal(TOPO, Green, out external_lines, out external_mags, out external_Col);

            data.SetDataList("externalForces", external_lines);
            data.SetDataList("externalForcesCol", external_Col);
            data.SetDataList("externalForcesMag", external_mags);

        }


        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.ES;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("d3475207-eb40-4d1f-8751-3dd098bd3333"); }
        }
    }
}
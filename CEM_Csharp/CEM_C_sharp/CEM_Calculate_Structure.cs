using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using Node = CEM_Lib_Yuchi.Node;
using Edge = CEM_Lib_Yuchi.Edge;
using DATA_TO = CEM_Lib_Yuchi.DATA_TO;
using MODEL_OPTI_B = CEM_Lib_Yuchi.MODEL_OPTI_B;
using NODE_INFO = CEM_Lib_Yuchi.NODE_INFO;
using TRAIL_INFO = CEM_Lib_Yuchi.TRAIL_INFO;
using DEVI_INFO = CEM_Lib_Yuchi.DEVI_INFO;
using AUI_FUNC = CEM_Lib_Yuchi.AUI_FUNC;
using TAR = CEM_Lib_Yuchi.TAR;
using CO = CEM_Lib_Yuchi.CO;
using SW = CEM_Lib_Yuchi.SW;
using OPT_MODEL = CEM_Lib_Yuchi.OPT_MODEL;
using Calculation = CEM_Lib_Yuchi.Calculation;
using VIA = CEM_Lib_Yuchi.Calculation.VIA;
using NLoptNet;

namespace CEM_C_sharp
{
    public class CEM_Calculate_Structure : GH_Component
    {

        public CEM_Calculate_Structure()
          : base("CEM_Calculate_Structure", "CEM_Calculate_Structure",
              "[Calculate the Structure with the input Topology]",
              "CEM_TEST", "CEM_C_sharp")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager Input)
        {
            Input.AddGenericParameter("T", "T", "The Assembled Topology from [CEM_Build_Topology]", GH_ParamAccess.item);
            Input.AddGenericParameter("CO", "CO", "The Assembled ConstraintPlane and OrginNode information from [CEM_ConstraintPlane_OriginNodes]", GH_ParamAccess.item);
            Input.AddGenericParameter("SW", "SW", "The Assembled Material and Gravity information from [CEM_Material_Properties]", GH_ParamAccess.item);
            Input.AddGenericParameter("OPT", "OPT", "The Assembled Optimization information from [CEM_Optimization_Setup]", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager Output)
        {
            Output.AddGenericParameter("M", "M", "The Calculated Structure Model", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess data)
        {
            DATA_TO TOPO_temp = new DATA_TO();
            data.GetData("T",ref TOPO_temp);
            DATA_TO TOPO= TOPO_temp.CopySelf();

            OPT_MODEL OPTI_temp = new OPT_MODEL();
            data.GetData("OPT",ref OPTI_temp);
            OPT_MODEL OPTI = OPTI_temp.CopySelf();

            CO ConstraintP_OriginN = new CO();
            data.GetData("CO", ref ConstraintP_OriginN);

            SW SelfWeight = new SW();
            data.GetData("SW", ref SelfWeight);
            
            //_00_System Configuration:
            int bracing_count = 50;
            double threshold = TOPO.Sys_threshold;

            //_01_The Calculation Sequence:
            List<int> Calculation_Seq = Calculation.FlatternTheList(Calculation.FlipMetrix(TOPO.EachTrail_NodeList));

            //_02_Import ConstraintPlanes Data into Nodes Class:
            foreach (Node node in TOPO.Nodes_List) { node.constraintPlane = new Plane(); node.cons_Plane_bool = false; }
            Calculation.ImportConstraintPlanes(TOPO, ConstraintP_OriginN.constraintPlane, ConstraintP_OriginN.constraintPlaneID);

            //_03_Check the boundary setting information(OrginID)
            Calculation.GetOrigin_Nodes_info(TOPO, OPTI);
            
            //_04_Put the starting points poistion(form diagram) to Node class(Draw the diagram in other places)
            for (int i = 0; i < ConstraintP_OriginN.originNode.Count; i++) { TOPO.Nodes_List[ConstraintP_OriginN.originNodeID[i]].Position = ConstraintP_OriginN.originNode[i]; }

            //_05_Initialize the data in the OPTI with Initial setting of TOPO
            OPTI.Boundary.exte.GetExteF_coordinates_List(TOPO);
            OPTI.Boundary.node.GetNodes_coordinates_List(TOPO);
            OPTI.Boundary.trail.GetTrail_length_datalist(TOPO);
            OPTI.Boundary.devi.GetDevi_magnitude_datalist(TOPO);
            OPTI.Boundary.AmendTheBound();
            
            //_06_get initial variables;
            List<double> initial_viables = Calculation.CopyDoubleList(OPTI.Boundary.Get_vlist()); double[] via = initial_viables.ToArray();
            List<double> initial_max = Calculation.CopyDoubleList(OPTI.Boundary.Get_maxlist()); double[] max = initial_max.ToArray();
            List<double> initial_min = Calculation.CopyDoubleList(OPTI.Boundary.Get_minlist()); double[] min = initial_min.ToArray();

            TOPO.DATA_FROM += string.Format("\r\n__|Variable_length|__\r\n1. variables[{0}]  max:[{1}] min:[{2}]\r\n", via.Length, max.Length,min.Length);

            //_07_Simplify the variables
            VIA VAR = new VIA(via, max, min);
            double[] via_simp = Calculation.ArrayCopy(VAR.sim_via);

            TOPO.DATA_FROM += string.Format("\r\n__|Sim_Variable_length|__\r\n1. variables[{0}]  max:[{1}] min:[{2}]  ReturnL[{3}]\r\n", VAR.sim_via.Length, VAR.sim_max.Length, VAR.sim_min.Length, VAR.Refill(VAR.sim_via).Length);
            
            if (via_simp.Length>0)
            {
                TOPO.DATA_FROM += string.Format("\r\n__|USING ALGORITHM|:{0}\r\n", OPTI.optimAlgorithm);
                //[Optimization_Gra]:Gadient calculation
                double[] opti_list = Calculation.Optimization_Gra(
                via_simp,
                VAR,
                VAR.sim_min,
                VAR.sim_max,
                OPTI.gradientDelta,
                OPTI.relativeTolerance,
                OPTI.maxIterations,
                OPTI.optimAlgorithm,
                OPTI,
                TOPO,
                Calculation_Seq,
                SelfWeight,
                bracing_count,
                threshold
              );           
            Calculation.FinalCalculation(opti_list, VAR, OPTI, TOPO, Calculation_Seq, SelfWeight, bracing_count, threshold,false);
            }
            else { Calculation.FinalCalculation(via_simp, VAR, OPTI, TOPO, Calculation_Seq, SelfWeight, bracing_count, threshold,true);}     
            TOPO.UpdateNodeInSequence();
            data.SetData("M",TOPO);
            
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.CAL;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("8824df0e-2ca5-4461-93b4-a40833e8490f"); }
        }
    }
}
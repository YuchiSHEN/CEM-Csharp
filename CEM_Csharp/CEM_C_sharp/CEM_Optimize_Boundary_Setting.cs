using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using MODEL_OPTI_B = CEM_Lib_Yuchi.MODEL_OPTI_B;
using NODE_INFO = CEM_Lib_Yuchi.NODE_INFO;
using TRAIL_INFO = CEM_Lib_Yuchi.TRAIL_INFO;
using DEVI_INFO = CEM_Lib_Yuchi.DEVI_INFO;
using EXTE_INFO = CEM_Lib_Yuchi.EXTE_INFO;

namespace CEM_C_sharp
{
    public class CEM_Optimize_Boundary_Setting : GH_Component
    {

        public CEM_Optimize_Boundary_Setting()
          : base("CEM_Optimize_Boundary_Setting", "CEM_Optimize_Boundary_Setting",
              "[Setting the boundary values for the variables for optimization]",
              "CEM_TEST", "CEM_C_sharp")    
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager Input)
        {
            List<int> default_intlist = new List<int>() {-1};
            List<string> default_stringlist = new List<string>() { "NONE" };
            List<string> default_domainlist = new List<string>() { "0.0 to 0.0" };  

            Input.AddIntegerParameter("Action_Nodes_ID", "Action_Nodes_ID", "The ID of the action point of the external forces", GH_ParamAccess.list, default_intlist);
            Input.AddTextParameter("Domain_Vector_X", "Domain_Vector_X", "The Domain of X coordinate of the external force vector:[STRING A to B]", GH_ParamAccess.list, default_domainlist);
            Input.AddTextParameter("Domain_Vector_Y", "Domain_Vector_Y", "The Domain of Y coordinate of the external force vector:[STRING A to B]", GH_ParamAccess.list, default_domainlist);
            Input.AddTextParameter("Domain_Vector_Z", "Domain_Vector_Z", "The Domain of Z coordinate of the external force vector:[STRING A to B]", GH_ParamAccess.list, default_domainlist);

            Input.AddIntegerParameter("Origin_Nodes_ID", "Origin_Nodes_ID", "The ID of the movible original nodes", GH_ParamAccess.list, default_intlist);
            Input.AddTextParameter("Domain_Nodes_X", "Domain_Nodes_X", "The Domain of X coordinate of the origin points:[STRING A to B]", GH_ParamAccess.list, default_domainlist);
            Input.AddTextParameter("Domain_Nodes_Y", "Domain_Nodes_Y", "The Domain of Y coordinate of the origin points:[STRING A to B]", GH_ParamAccess.list, default_domainlist);
            Input.AddTextParameter("Domain_Nodes_Z", "Domain_Nodes_Z", "The Domain of Z coordinate of the origin points:[STRING A to B]", GH_ParamAccess.list, default_domainlist);
            Input.AddTextParameter("Trail_ID", "Trail_ID", "The changable trail ID with STRING list :[format:[A][B]]", GH_ParamAccess.list, default_stringlist);
            Input.AddTextParameter("Domain_Trail", "Domain_Trail","The length changing range of the selected trails:[STRING A to B]",GH_ParamAccess.list, default_domainlist);
            Input.AddTextParameter("Devi_ID", "Devi_ID", "The changable Devi ID with STRING list :[format:[A][B]]", GH_ParamAccess.list, default_stringlist);
            Input.AddTextParameter("Domain_Devi", "Domain_Devi", "The force magnitude changing range of the selected Devis:[STRING A to B]", GH_ParamAccess.list, default_domainlist);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager Output)
        {
            Output.AddGenericParameter("BD", "BD", "The assembled boundary information", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess data)
        {
            List<int> Action_Nodes_ID = new List<int>();
            List<string> Domain_Vector_X = new List<string>();
            List<string> Domain_Vector_Y = new List<string>();
            List<string> Domain_Vector_Z = new List<string>();

            List<int> Origin_Nodes_ID = new List<int>();
            List<string> Domain_Nodes_X = new List<string>();
            List<string> Domain_Nodes_Y = new List<string>();
            List<string> Domain_Nodes_Z = new List<string>();

            List<string> Trail_ID = new List<string>();
            List<string> Domain_Trail = new List<string>();
            List<string> Devi_ID = new List<string>();
            List<string> Domain_Devi = new List<string>();

            data.GetDataList("Action_Nodes_ID", Action_Nodes_ID);
            data.GetDataList("Domain_Vector_X", Domain_Vector_X);
            data.GetDataList("Domain_Vector_Y", Domain_Vector_Y);
            data.GetDataList("Domain_Vector_Z", Domain_Vector_Z);

            data.GetDataList("Origin_Nodes_ID", Origin_Nodes_ID);
            data.GetDataList("Domain_Nodes_X", Domain_Nodes_X);
            data.GetDataList("Domain_Nodes_Y", Domain_Nodes_Y);
            data.GetDataList("Domain_Nodes_Z", Domain_Nodes_Z);

            data.GetDataList("Trail_ID", Trail_ID);
            data.GetDataList("Domain_Trail", Domain_Trail);
            data.GetDataList("Devi_ID", Devi_ID);
            data.GetDataList("Domain_Devi", Domain_Devi);
            
            NODE_INFO Node_infor = new NODE_INFO(Domain_Nodes_X, Domain_Nodes_Y, Domain_Nodes_Z, Origin_Nodes_ID);
            TRAIL_INFO Trail_infor = new TRAIL_INFO(Trail_ID, Domain_Trail);
            DEVI_INFO Devi_infor = new DEVI_INFO(Devi_ID, Domain_Devi);
            EXTE_INFO Exte_infor = new EXTE_INFO(Domain_Vector_X, Domain_Vector_Y, Domain_Vector_Z, Action_Nodes_ID);
            MODEL_OPTI_B OPM = new MODEL_OPTI_B(Node_infor, Trail_infor, Devi_infor, Exte_infor);

            data.SetData("BD", OPM);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.BD;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("4793cae3-3edb-4408-8b2f-3f2e519a8719"); }
        }
    }
}
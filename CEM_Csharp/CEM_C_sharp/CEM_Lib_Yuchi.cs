using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino;
using Rhino.Geometry;
using Grasshopper.Kernel;
using NLoptNet;
using Color = System.Drawing.Color;

namespace CEM_Lib_Yuchi
{
    //[MODEL TOPO]__________________________________________________________________________________________

    //_Node_
    public class Node
    {
        public int node_ID;
        public Point3d Position;
        public string Node_Type;

        public List<int> TrailEdgeIn_ID;
        public List<int> TrailEdgeOut_ID;
        public List<List<int>> DeviEdge_ID;
        public List<List<int>> InDeviEdge_ID;

        public List<Vector3d> ExternalForce_Vec;
        public Plane constraintPlane;
        public bool cons_Plane_bool;

        public Vector3d ReactionForce;
        public Node(int ID, Point3d POI)
        {
            node_ID = ID;
            Position = POI;
            cons_Plane_bool = false;
        }

        public Node CopySelf()
        {
            Node temp = new Node(this.node_ID, this.Position);
            temp.Node_Type = this.Node_Type;
            temp.TrailEdgeIn_ID = CopyIntList(this.TrailEdgeIn_ID);
            temp.TrailEdgeOut_ID = CopyIntList(this.TrailEdgeOut_ID);
            temp.DeviEdge_ID = CopyIntListList(this.DeviEdge_ID);
            temp.InDeviEdge_ID = CopyIntListList(this.InDeviEdge_ID);

            temp.ExternalForce_Vec = CopyVector3dList(this.ExternalForce_Vec);
            temp.constraintPlane = this.constraintPlane;
            temp.cons_Plane_bool = this.cons_Plane_bool;

            temp.ReactionForce = this.ReactionForce;

            return temp;
        }

        public List<int> CopyIntList(List<int> list)
        {
            List<int> temp = new List<int>();
            foreach (int i in list)
            {
                temp.Add(i);
            }
            return temp;
        }

        public List<List<int>> CopyIntListList(List<List<int>> listlist)
        {
            List<List<int>> templist = new List<List<int>>();
            foreach (List<int> list in listlist)
            {
                templist.Add(CopyIntList(list));
            }
            return templist;
        }

        public List<Vector3d> CopyVector3dList(List<Vector3d> list)
        {
            List<Vector3d> temp = new List<Vector3d>();
            foreach (Vector3d i in list)
            {
                temp.Add(i);
            }
            return temp;
        }
    }

    //_Edge_
    public class Edge
    {
        public List<int> Edge_ID;
        public double length;
        public double magnitude;

        public Line line;

        public string ForceType()
        {
            if (magnitude > 0) { return "TE"; }
            else if (magnitude < 0) { return "CO"; }
            else { return "--"; }
        }

        public string EdgeType;//"DE"\"ID"\"--"\"TL"

        public Vector3d Calculated_Vec;

        public List<int> CopyIntList(List<int> list)
        {
            List<int> temp = new List<int>();
            foreach (int i in list)
            {
                temp.Add(i);
            }
            return temp;
        }

        public Edge CopySelf()
        {
            Edge temp = new Edge();
            temp.Edge_ID = CopyIntList(this.Edge_ID);
            temp.length = this.length;
            temp.magnitude = this.magnitude;
            temp.line = this.line;
            temp.EdgeType = this.EdgeType;
            temp.Calculated_Vec = this.Calculated_Vec;
            return temp;
        }
    }

    //DATA_TO
    public class DATA_TO
    {
        public string DATA_FROM = "[CEM CALCULATION SUMMARY]:";
        public double Sys_threshold;
        public List<Point3d> Nodes_InSeq;
        public List<int> Trail_numList;
        public List<int> NodeID_list;
        public List<int> SupportID_list;
        public List<List<int>> EachTrail_NodeList;
        public List<List<int>> EachLay_NodeList;
        public List<Node> Nodes_List;
        public List<List<Edge>> Edge_Metrix;
        public List<List<int>> InDevMirroList;
        public DATA_TO(double Sys_threshold_In, List<Point3d> Nodes_InSeq_In, List<int> Trail_numList_In, List<int> NodeID_list_In, List<int> SupportID_list_In, List<List<int>> EachTrail_NodeList_In, List<List<int>> EachLay_NodeList_In, List<Node> Nodes_List_In, List<List<Edge>> Edge_Metrix_In, List<List<int>> InDevMirroList_In)
        {
            DATA_FROM = "[CEM CALCULATION SUMMARY]:";
            Sys_threshold = Sys_threshold_In;
            Nodes_InSeq = Nodes_InSeq_In;
            Trail_numList = Trail_numList_In;
            NodeID_list = NodeID_list_In;
            SupportID_list = SupportID_list_In;
            EachTrail_NodeList = EachTrail_NodeList_In;
            EachLay_NodeList = EachLay_NodeList_In;
            Nodes_List = Nodes_List_In;
            Edge_Metrix = Edge_Metrix_In;
            InDevMirroList = InDevMirroList_In;
        }

        public DATA_TO() { }

        public override string ToString()
        {
            return this.DATA_FROM;
        }

        public void renew_info()
        { this.DATA_FROM = "[CEM CALCULATION SUMMARY]:"; }

        public DATA_TO CopySelf()
        {
            DATA_TO temp = new DATA_TO(
                this.Sys_threshold,
                CopyPoint3dList(this.Nodes_InSeq),
                CopyIntList(this.Trail_numList),
                CopyIntList(this.NodeID_list),
                CopyIntList(this.SupportID_list),
                CopyIntListList(EachTrail_NodeList),
                CopyIntListList(EachLay_NodeList),
                CopyNodeList(Nodes_List),
                CopyEdgeListList(Edge_Metrix),
                CopyIntListList(InDevMirroList)
                );
            return temp;
        }

        public void UpdateNodeInSequence()
        {
            List<Point3d> new_NodesList = new List<Point3d>();
            foreach (Node node in this.Nodes_List) { new_NodesList.Add(node.Position); }
            this.Nodes_InSeq = new_NodesList;
        }
        public List<int> CopyIntList(List<int> list)
        {
            List<int> temp = new List<int>();
            foreach (int i in list)
            {
                temp.Add(i);
            }
            return temp;
        }

        public List<List<int>> CopyIntListList(List<List<int>> listlist)
        {
            List<List<int>> templist = new List<List<int>>();
            foreach (List<int> list in listlist)
            {
                templist.Add(CopyIntList(list));
            }
            return templist;
        }

        public List<Point3d> CopyPoint3dList(List<Point3d> list)
        {
            List<Point3d> temp = new List<Point3d>();
            foreach (Point3d i in list)
            {
                temp.Add(i);
            }
            return temp;
        }

        public List<Node> CopyNodeList(List<Node> list)
        {
            List<Node> temp = new List<Node>();
            foreach (Node i in list)
            {
                temp.Add(i.CopySelf());
            }
            return temp;
        }

        public List<Edge> CopyEdgeList(List<Edge> list)
        {
            List<Edge> temp = new List<Edge>();
            foreach (Edge i in list)
            {
                temp.Add(i.CopySelf());
            }
            return temp;
        }

        public List<List<Edge>> CopyEdgeListList(List<List<Edge>> listlist)
        {
            List<List<Edge>> templist = new List<List<Edge>>();
            foreach (List<Edge> list in listlist)
            {
                templist.Add(CopyEdgeList(list));
            }
            return templist;
        }
    }
    //[MODEL TOPO]__________________________________________________________________________________________

    //[BOUNDARY_SETTING]__________________________________________________________________________________
    public class MODEL_OPTI_B : AUI_FUNC
    {
        public NODE_INFO node;
        public TRAIL_INFO trail;
        public DEVI_INFO devi;
        public EXTE_INFO exte;

        public MODEL_OPTI_B(NODE_INFO NODE, TRAIL_INFO TRAIL, DEVI_INFO DEVI, EXTE_INFO EXTE)
        {
            this.node = NODE;
            this.trail = TRAIL;
            this.devi = DEVI;
            this.exte = EXTE;
        }
        public MODEL_OPTI_B() { }

        public MODEL_OPTI_B CopySelf()
        {
            MODEL_OPTI_B temp = new MODEL_OPTI_B(this.node.CopySelf(), this.trail.CopySelf(), this.devi.CopySelf(), this.exte.CopySelf());
            return temp;
        }

        //this Amend should be done after the origin point have been set
        public void AmendTheBound()
        {
            this.node.AmendTheBound();
            this.trail.AmendTheBound();
            this.devi.AmendTheBound();
            this.exte.AmendTheBound();
        }

        public List<double> Get_vlist()
        {
            List<double> templist = new List<double>();
            templist.AddRange(this.node.Nodes_coordinates_List);
            templist.AddRange(this.trail.Trail_length_datalist);
            templist.AddRange(this.devi.Devi_magnitude_datalist);
            templist.AddRange(this.exte.Vector_coordinates_List);
            return CopyDoubleList(templist);
        }

        public List<double> Get_maxlist()
        {
            List<double> templist = new List<double>();
            templist.AddRange(this.node.Nodes_coordinates_Max);
            templist.AddRange(this.trail.Trail_length_Bound_Max);
            templist.AddRange(this.devi.Devi_magnitude_Bound_Max);
            templist.AddRange(this.exte.Vector_coordinates_Max);
            return CopyDoubleList(templist);
        }

        public List<double> Get_minlist()
        {
            List<double> templist = new List<double>();
            templist.AddRange(this.node.Nodes_coordinates_Min);
            templist.AddRange(this.trail.Trail_length_Bound_Min);
            templist.AddRange(this.devi.Devi_magnitude_Bound_Min);
            templist.AddRange(this.exte.Vector_coordinates_Min);
            return CopyDoubleList(templist);
        }

        public void InPut_list(List<double> optimized_list)
        {
            int num1 = this.node.Nodes_coordinates_List.Count;
            int num2 = this.trail.Trail_length_datalist.Count;
            int num3 = this.devi.Devi_magnitude_datalist.Count;
            int num4 = this.exte.Vector_coordinates_List.Count;

            List<double> for_node = new List<double>();
            List<double> for_trail = new List<double>();
            List<double> for_devi = new List<double>();
            List<double> for_exte = new List<double>();

            if (optimized_list.Count == (num1 + num2 + num3 + num4))
            {
                for (int i = 0; i < num1; i++)
                { for_node.Add(optimized_list[i]); }
                for (int i = num1; i < num1 + num2; i++)
                { for_trail.Add(optimized_list[i]); }
                for (int i = num1 + num2; i < num1 + num2 + num3; i++)
                { for_devi.Add(optimized_list[i]); }
                for (int i = num1 + num2 + num3; i < num1 + num2 + num3 + num4; i++)
                { for_exte.Add(optimized_list[i]); }

                this.node.Nodes_coordinates_List = for_node;
                this.trail.Trail_length_datalist = for_trail;
                this.devi.Devi_magnitude_datalist = for_devi;
                this.exte.Vector_coordinates_List = for_exte;
            }
        }

        public void feedback_DATA(DATA_TO TOPO)
        {
            this.node.feedback_DATA(TOPO);
            this.trail.feedback_DATA(TOPO);
            this.devi.feedback_DATA(TOPO);
            this.exte.feedback_DATA(TOPO);
        }
    }

    public class NODE_INFO : AUI_FUNC
    {
        public List<int> originNode_ID;
        public List<double> Nodes_coordinates_List;
        public List<double> Nodes_coordinates_Max;
        public List<double> Nodes_coordinates_Min;

        //construct function
        public NODE_INFO(List<string> Domain_Nodes_X, List<string> Domain_Nodes_Y, List<string> Domain_Nodes_Z, List<int> ori_Node_ID)
        {
            this.Nodes_coordinates_List = new List<double>();
            if (ori_Node_ID.Count > 0)
            { if (ori_Node_ID[0] != -1) { this.originNode_ID = ori_Node_ID; } else { this.originNode_ID = new List<int>(); } }
            else
            { this.originNode_ID = new List<int>(); }

            this.Node_Bound_List(Domain_Nodes_X, Domain_Nodes_Y, Domain_Nodes_Z, this.originNode_ID, out Nodes_coordinates_Max, out Nodes_coordinates_Min);
        }
        //The bound input to the Alg should be within and based on the initial viable
        public void AmendTheBound()
        {
            for (int i = 0; i < this.Nodes_coordinates_List.Count; i++)
            {
                this.Nodes_coordinates_Max[i] += this.Nodes_coordinates_List[i];
                this.Nodes_coordinates_Min[i] += this.Nodes_coordinates_List[i];
            }
        }
        //construct for copy
        public NODE_INFO(List<int> para_0, List<double> para_1, List<double> para_2, List<double> para_3)
        {
            this.originNode_ID = para_0;
            this.Nodes_coordinates_List = para_1;
            this.Nodes_coordinates_Max = para_2;
            this.Nodes_coordinates_Min = para_3;
        }

        //CopySelf
        public NODE_INFO CopySelf()
        {
            NODE_INFO temp = new NODE_INFO(
              CopyIntList(this.originNode_ID),
              CopyDoubleList(this.Nodes_coordinates_List),
              CopyDoubleList(this.Nodes_coordinates_Max),
              CopyDoubleList(this.Nodes_coordinates_Min));
            return temp;
        }

        //initialize Nodes_coordinates_List;
        public void GetNodes_coordinates_List(DATA_TO TOPO)
        {
            foreach (int i in this.originNode_ID)
            {
                Nodes_coordinates_List.Add(TOPO.Nodes_List[i].Position.X);
                Nodes_coordinates_List.Add(TOPO.Nodes_List[i].Position.Y);
                Nodes_coordinates_List.Add(TOPO.Nodes_List[i].Position.Z);
            }
        }

        //feed back the data to DATA_TO TOPO
        public void feedback_DATA(DATA_TO TOPO)
        {
            for (int i = 0; i < originNode_ID.Count; i++)
            {
                TOPO.Nodes_InSeq[this.originNode_ID[i]] = new Point3d(this.Nodes_coordinates_List[i * 3], this.Nodes_coordinates_List[i * 3 + 1], this.Nodes_coordinates_List[i * 3 + 2]);
                TOPO.Nodes_List[this.originNode_ID[i]].Position = new Point3d(this.Nodes_coordinates_List[i * 3], this.Nodes_coordinates_List[i * 3 + 1], this.Nodes_coordinates_List[i * 3 + 2]);
            }
        }
    }

    public class TRAIL_INFO : AUI_FUNC
    {
        public List<List<int>> Trail_ID;
        public List<double> Trail_length_datalist;
        public List<double> Trail_length_Bound_Max;
        public List<double> Trail_length_Bound_Min;

        //Construct Func
        public TRAIL_INFO(List<string> Trail_ID_string, List<string> Domain_Trail)
        {
            if (Trail_ID_string.Count > 0)
            { if (Trail_ID_string[0] != "NONE") { this.Trail_ID = Read_Edge_ID(Trail_ID_string); } else { this.Trail_ID = new List<List<int>>(); } }
            else
            { this.Trail_ID = new List<List<int>>(); }

            Edge_Bound_List(Domain_Trail, this.Trail_ID.Count, out Trail_length_Bound_Max, out Trail_length_Bound_Min);
            Trail_length_datalist = new List<double>();
        }

        //Construct for Copy
        public TRAIL_INFO(List<List<int>> para_0, List<double> para_1, List<double> para_2, List<double> para_3)
        {
            this.Trail_ID = para_0;
            this.Trail_length_datalist = para_1;
            this.Trail_length_Bound_Max = para_2;
            this.Trail_length_Bound_Min = para_3;
        }

        public void AmendTheBound()
        {
            for (int i = 0; i < this.Trail_length_datalist.Count; i++)
            {
                this.Trail_length_Bound_Max[i] += this.Trail_length_datalist[i];
                this.Trail_length_Bound_Min[i] += this.Trail_length_datalist[i];
            }
        }
        //CopySelf
        public TRAIL_INFO CopySelf()
        {
            TRAIL_INFO temp = new TRAIL_INFO(CopyIntListList(this.Trail_ID), CopyDoubleList(this.Trail_length_datalist), CopyDoubleList(this.Trail_length_Bound_Max), CopyDoubleList(this.Trail_length_Bound_Min));
            return temp;
        }

        public void GetTrail_length_datalist(DATA_TO TOPO)
        {
            foreach (List<int> ID in this.Trail_ID)
            {
                Trail_length_datalist.Add(TOPO.Edge_Metrix[ID[0]][ID[1]].length);
            }
        }

        //feed back the data to DATA_TO TOPO
        public void feedback_DATA(DATA_TO TOPO)
        {
            for (int i = 0; i < Trail_ID.Count; i++)
            {
                TOPO.Edge_Metrix[Trail_ID[i][0]][Trail_ID[i][1]].length = this.Trail_length_datalist[i];
                TOPO.Edge_Metrix[Trail_ID[i][1]][Trail_ID[i][0]].length = this.Trail_length_datalist[i];
            }
        }
    }

    public class DEVI_INFO : AUI_FUNC
    {
        public List<List<int>> Devi_ID;
        public List<double> Devi_magnitude_datalist;
        public List<double> Devi_magnitude_Bound_Max;
        public List<double> Devi_magnitude_Bound_Min;

        public DEVI_INFO(List<string> Devi_ID_string, List<string> Domain_Devi)
        {
            if (Devi_ID_string.Count > 0)
            { if (Devi_ID_string[0] != "NONE") { this.Devi_ID = Read_Edge_ID(Devi_ID_string); } else { this.Devi_ID = new List<List<int>>(); } }
            else { this.Devi_ID = new List<List<int>>(); }

            Edge_Bound_List(Domain_Devi, this.Devi_ID.Count, out Devi_magnitude_Bound_Max, out Devi_magnitude_Bound_Min);
            Devi_magnitude_datalist = new List<double>();
        }

        public DEVI_INFO(List<List<int>> para_0, List<double> para_1, List<double> para_2, List<double> para_3)
        {
            this.Devi_ID = para_0;
            this.Devi_magnitude_datalist = para_1;
            this.Devi_magnitude_Bound_Max = para_2;
            this.Devi_magnitude_Bound_Min = para_3;
        }

        public void AmendTheBound()
        {
            for (int i = 0; i < this.Devi_magnitude_datalist.Count; i++)
            {
                this.Devi_magnitude_Bound_Max[i] += this.Devi_magnitude_datalist[i];
                this.Devi_magnitude_Bound_Min[i] += this.Devi_magnitude_datalist[i];
            }
        }
        //CopySelf
        public DEVI_INFO CopySelf()
        {
            DEVI_INFO temp = new DEVI_INFO(
              CopyIntListList(this.Devi_ID),
              CopyDoubleList(this.Devi_magnitude_datalist),
              CopyDoubleList(this.Devi_magnitude_Bound_Max),
              CopyDoubleList(this.Devi_magnitude_Bound_Min));
            return temp;
        }

        public void GetDevi_magnitude_datalist(DATA_TO TOPO)
        {
            foreach (List<int> ID in this.Devi_ID)
            {
                Devi_magnitude_datalist.Add(TOPO.Edge_Metrix[ID[0]][ID[1]].magnitude);
            }
        }

        //feed back the data to DATA_TO TOPO
        public void feedback_DATA(DATA_TO TOPO)
        {
            for (int i = 0; i < Devi_ID.Count; i++)
            {
                TOPO.Edge_Metrix[Devi_ID[i][0]][Devi_ID[i][1]].magnitude = this.Devi_magnitude_datalist[i];
                TOPO.Edge_Metrix[Devi_ID[i][1]][Devi_ID[i][0]].magnitude = this.Devi_magnitude_datalist[i];
            }
        }
    }

    public class EXTE_INFO : AUI_FUNC
    {
        public List<int> ActionPointID;
        public List<double> Vector_coordinates_List;
        public List<double> Vector_coordinates_Max;
        public List<double> Vector_coordinates_Min;


        public EXTE_INFO(List<string> Vector_Domain_X, List<string> Vector_Domain_Y, List<string> Vector_Domain_Z,List<int> ActionPointID )
        {
            this.Vector_coordinates_List = new List<double>();
            if (ActionPointID.Count > 0)
            { if (ActionPointID[0] != -1) { this.ActionPointID = ActionPointID; } else { this.ActionPointID = new List<int>(); } }
            else { this.ActionPointID = new List<int>(); }

            this.Node_Bound_List(Vector_Domain_X, Vector_Domain_Y, Vector_Domain_Z, this.ActionPointID, out Vector_coordinates_Max, out Vector_coordinates_Min);
        }

        public void AmendTheBound()
        {
            for (int i = 0; i < this.Vector_coordinates_List.Count; i++)
            {
                this.Vector_coordinates_Max[i] += this.Vector_coordinates_List[i];
                this.Vector_coordinates_Min[i] += this.Vector_coordinates_List[i];
            }
        }

        public EXTE_INFO(List<int> ActionPointID, List<double> Vector_coordinates_List, List<double> Vector_coordinates_Max, List<double> Vector_coordinates_Min)
        {
            this.ActionPointID = ActionPointID;
            this.Vector_coordinates_List = Vector_coordinates_List;
            this.Vector_coordinates_Max = Vector_coordinates_Max;
            this.Vector_coordinates_Min = Vector_coordinates_Min;
        }

        public EXTE_INFO CopySelf()
        {
            EXTE_INFO temp = new EXTE_INFO(
                CopyIntList(this.ActionPointID),
                CopyDoubleList(this.Vector_coordinates_List),
                CopyDoubleList(this.Vector_coordinates_Max),
                CopyDoubleList(this.Vector_coordinates_Min));
            return temp;
        }

        public void GetExteF_coordinates_List(DATA_TO TOPO)
        {
            foreach (int i in this.ActionPointID)
            {
                Vector_coordinates_List.Add(Sum_Vec(TOPO.Nodes_List[i].ExternalForce_Vec).X);
                Vector_coordinates_List.Add(Sum_Vec(TOPO.Nodes_List[i].ExternalForce_Vec).Y);
                Vector_coordinates_List.Add(Sum_Vec(TOPO.Nodes_List[i].ExternalForce_Vec).Z);
            }
        }
        public void feedback_DATA(DATA_TO TOPO)
        {
            for (int i = 0; i < this.ActionPointID.Count; i++)
            {
                TOPO.Nodes_List[ActionPointID[i]].ExternalForce_Vec = new List<Vector3d>() { new Vector3d(this.Vector_coordinates_List[i * 3], this.Vector_coordinates_List[i * 3 + 1], this.Vector_coordinates_List[i * 3 + 2]) };
            }
        }
    }

    //[BOUNDARY_SETTING]__________________________________________________________________________________

    //[OPTMIZATION MODEL]__________________________________________________________________________________
    public class OPT_MODEL : AUI_FUNC
    {
        public TAR Target;
        public MODEL_OPTI_B Boundary;
        public double gradientDelta;
        public double relativeTolerance;
        public int maxIterations;
        public string optimAlgorithm;

        public OPT_MODEL(TAR Target, MODEL_OPTI_B Boundary, double gradientDelta, double relativeTolerance, int maxIterations, string optimAlgorithm)
        {
            this.Target = Target;
            this.Boundary = Boundary;
            this.gradientDelta = gradientDelta;
            this.relativeTolerance = relativeTolerance;
            this.maxIterations = maxIterations;
            this.optimAlgorithm = optimAlgorithm;
        }

        public OPT_MODEL() { }
        public OPT_MODEL CopySelf()
        {
            OPT_MODEL temp = new OPT_MODEL(
              this.Target.CopySelf(),
              this.Boundary.CopySelf(),
              this.gradientDelta,
              this.relativeTolerance,
              this.maxIterations,
              this.optimAlgorithm
              );
            return temp;
        }
    }
    //[TARGET_SETTING]__________________________________________________________________________________
    public class TAR : AUI_FUNC
    {
        public List<Point3d> targetNode;
        public List<int> targetNodeID;
        public double targetNodeCoeff;

        public List<Vector3d> targetVector;
        public List<string> targetVectorID;
        public double targetVectorCoeffMag;
        public double targetVectorCoeffDif;

        public string report = "";

        public TAR(List<Point3d> targetNode, List<int> targetNodeID, double targetNodeCoeff, List<Vector3d> targetVector, List<string> targetVectorID, double targetVectorCoeffMag, double targetVectorCoeffDif)
        {
            if (targetNodeCoeff == 0.0)
            { this.targetNode = new List<Point3d>(); this.targetNodeID = new List<int>(); }
            else { this.targetNode = targetNode; this.targetNodeID = targetNodeID; }
            this.targetNodeCoeff = targetNodeCoeff;

            if (targetVectorCoeffMag == 0.0 && targetVectorCoeffDif == 0.0)
            { this.targetVector = new List<Vector3d>(); this.targetVectorID = new List<string>(); }
            else { this.targetVector = targetVector; this.targetVectorID = targetVectorID; }
            this.targetVectorCoeffMag = targetVectorCoeffMag;
            this.targetVectorCoeffDif = targetVectorCoeffDif;
        }

        public TAR() { }

        public override string ToString()
        {
            return report;
        }
        public TAR CopySelf()
        {
            TAR temp = new TAR(
              CopyPoint3dList(this.targetNode),
              CopyIntList(this.targetNodeID),
              this.targetNodeCoeff,
              CopyVector3dList(this.targetVector),
              CopystringList(this.targetVectorID),
              this.targetVectorCoeffMag,
              this.targetVectorCoeffDif);
            return temp;
        }

        public double GetScore(DATA_TO TOPO)
        {
            double score_nodedistance_sum = 0.0;
            double score_vectorMag_sum = 0.0;
            double score_vectorDir_sum = 0.0;

            for (int i = 0; i < targetNodeID.Count; i++)
            {
                score_nodedistance_sum += System.Math.Pow((TOPO.Nodes_List[targetNodeID[i]].Position.DistanceTo(targetNode[i])), 2);
            }

            List<List<int>> targetVecID = Read_Edge_ID(targetVectorID);

            for (int i = 0; i < targetVecID.Count; i++)
            {
                score_vectorMag_sum += System.Math.Pow((System.Math.Abs(TOPO.Edge_Metrix[targetVecID[i][0]][targetVecID[i][1]].magnitude) - targetVector[i].Length), 2);

                Vector3d VecInTrail = new Vector3d(TOPO.Nodes_List[targetVecID[i][1]].Position - TOPO.Nodes_List[targetVecID[i][0]].Position);
                //score_vectorDir_sum += System.Math.Pow((1 - System.Math.Abs(Rhino.Geometry.Vector3d.Multiply(VecInTrail, targetVector[i]))), 2);//Need test 
                score_vectorDir_sum += System.Math.Pow(DirectionDiff_Vec(VecInTrail, targetVector[i]) + 1, 4) - 1;
            }

            double finall_score = score_nodedistance_sum * targetNodeCoeff + score_vectorMag_sum * targetVectorCoeffMag + score_vectorDir_sum * targetVectorCoeffDif;
            return finall_score;
        }
    }
    public class CO
    {
        public List<Plane> constraintPlane;
        public List<int> constraintPlaneID;
        public List<Point3d> originNode;
        public List<int> originNodeID;
        public CO() { }
        public CO(List<Plane> constraintPlane, List<int> constraintPlaneID, List<Point3d> originNode, List<int> originNodeID)
        {
            if (constraintPlaneID.Count > 0) { if (constraintPlaneID[0] == -1) { this.constraintPlaneID = new List<int>(); } else { this.constraintPlaneID = constraintPlaneID; } }
            else { this.constraintPlaneID = new List<int>(); }
            if (this.constraintPlaneID.Count == constraintPlane.Count) { this.constraintPlane = constraintPlane; }
            else { this.constraintPlane = new List<Plane>(); }

            if (originNodeID.Count > 0) { if (originNodeID[0] == -1) { this.originNodeID = new List<int>(); } else { this.originNodeID = originNodeID; } }
            else { this.originNodeID = new List<int>(); }
            if (this.originNodeID.Count == originNode.Count) { this.originNode = originNode; }
            else { this.originNode = new List<Point3d>(); }
        }
        public string checkPlane(List<Plane> constraintPlane, List<int> constraintPlaneID)
        {
            if (constraintPlaneID.Count != constraintPlane.Count)
            { return "INPUT_ERROR:Each Constraint Plane should cooresponds to a nodeID please check"; }
            else { return null; }
        }
        public string checkNode(List<Point3d> originNode, List<int> originNodeID)
        {
            if (originNode.Count != originNodeID.Count)
            { return "INPUT_ERROR:Each Origin Node should cooresponds to a nodeID please check"; }
            else { return null; }
        }
        public override string ToString()
        {
            string report_1 = string.Format("The Constraint Plane Configuration:\r\nConstraintPlanes:{0}\r\nConstraintPlaneID:{1}", constraintPlane.Count, constraintPlaneID.Count);
            string report_2 = string.Format("The Origin Node Configuration:\r\nOriginNode:{0}\r\nOriginNodeID:{1}", originNode.Count, originNodeID.Count);
            string report = report_1 + "\r\n" + report_2;
            return report;
        }
    }
    public class SW
    {
        public bool selfWeight;
        public double yieldStress;
        public double specWeight;
        public SW() { }
        public SW(bool selfWeight, double yieldStress, double specWeight)
        {
            this.selfWeight = selfWeight;
            this.yieldStress = yieldStress;
            this.specWeight = specWeight;
        }

        public override string ToString()
        {
            return string.Format("The Self Weight Configuration:\r\nselfWeight:{0}\r\nyieldStress:{1}[KN/M2]\r\nspecWeight:{2}[KN/M3]", selfWeight, yieldStress, specWeight);
        }
    }
    //[AUI]__________________________________________________________________________________
    public class AUI_FUNC
    {
        public double com_threshold = 0.00001;
        //The difference of two Vector3d
        public double DirectionDiff_Vec(Vector3d Vector_1, Vector3d Vector_2)
        {
            Vector3d vec1 = Vector_1; vec1.Unitize();
            Vector3d vec2 = Vector_2; vec2.Unitize();
            if (vec1.Length == 0.0 || vec2.Length == 0.0) { return 0.0; }
            else
            {
                double num = System.Math.Acos((vec1 * vec2) / (vec1.Length * vec2.Length));
                if (double.IsNaN(num) && (vec1 + vec2) == new Vector3d(0.0, 0.0, 0.0)) { num = 3.1415926; }
                else if (double.IsNaN(num) && (vec1 + vec2) != new Vector3d(0.0, 0.0, 0.0)) { num = 0.0; }
                return num;
            }
        }
        //Six transform functions
        public List<int> CopyIntList(List<int> list)
        {
            List<int> temp = new List<int>();
            foreach (int i in list)
            {
                temp.Add(i);
            }
            return temp;
        }
        public List<double> CopyDoubleList(List<Double> list)
        {
            List<Double> temp = new List<Double>();
            foreach (Double i in list)
            {
                temp.Add(i);
            }
            return temp;
        }
        public List<List<int>> CopyIntListList(List<List<int>> listlist)
        {
            List<List<int>> templist = new List<List<int>>();
            foreach (List<int> list in listlist)
            {
                templist.Add(CopyIntList(list));
            }
            return templist;
        }
        public List<Vector3d> CopyVector3dList(List<Vector3d> list)
        {
            List<Vector3d> temp = new List<Vector3d>();
            foreach (Vector3d i in list)
            {
                temp.Add(i);
            }
            return temp;
        }
        public List<Point3d> CopyPoint3dList(List<Point3d> list)
        {
            List<Point3d> temp = new List<Point3d>();
            foreach (Point3d i in list)
            {
                temp.Add(i);
            }
            return temp;
        }
        public List<string> CopystringList(List<string> list)
        {
            List<string> temp = new List<string>();
            foreach (string i in list)
            {
                temp.Add(i);
            }
            return temp;
        }

        //Put the trail/edge bound into list
        public void Edge_Bound_List(List<string> Domain_Edge, int IDnum, out List<double> Edges_Max, out List<double> Edges_Min)
        {
            Edges_Max = new List<double>();
            Edges_Min = new List<double>();

            for (int i = 0; i < IDnum; i++)
            {
                if (Domain_Edge.Count == 1) // && this.GetDomain(Domain_Edge[0])[0] < this.GetDomain(Domain_Edge[0])[1] need to purely the data later
                {
                    Edges_Min.Add(this.GetDomain(Domain_Edge[0])[0]);
                    Edges_Max.Add(this.GetDomain(Domain_Edge[0])[1]);
                }
                else if (Domain_Edge.Count == IDnum)
                {
                    Edges_Min.Add(this.GetDomain(Domain_Edge[i])[0]);
                    Edges_Max.Add(this.GetDomain(Domain_Edge[i])[1]);
                }
            }
        }
        //Read the num from input string list ID
        public List<List<int>> Read_Edge_ID(List<string> ID_string)
        {
            List<List<int>> main_list = new List<List<int>>();
            foreach (string ID in ID_string)
            {
                List<char> number = new List<char>() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                char[] ID_char = ID.ToCharArray();
                List<char> ID_c1 = new List<char>();
                List<char> ID_c2 = new List<char>();
                int num = 0;
                for (int i = 0; i < ID_char.Length; i++)
                {
                    if (number.Contains(ID_char[i]))
                    { ID_c1.Add(ID_char[i]); }
                    else if (ID_char[i] == ']' && ID_char[i + 1] == '[')
                    { num = i + 1; break; }
                }
                for (int i = num; i < ID_char.Length; i++)
                {
                    if (number.Contains(ID_char[i]))
                    { ID_c2.Add(ID_char[i]); }
                }
                int ID_1 = int.Parse(new string(ID_c1.ToArray()));
                int ID_2 = int.Parse(new string(ID_c2.ToArray()));
                List<int> sublist = new List<int>() { ID_1, ID_2 };
                main_list.Add(sublist);
            }
            return main_list;
        }
        //Put the Node Bound info into list
        public void Node_Bound_List(List<string> Domain_Nodes_X, List<string> Domain_Nodes_Y, List<string> Domain_Nodes_Z, List<int> Nodes_ID, out List<double> Nodes_coordinates_Max, out List<double> Nodes_coordinates_Min)
        {
            Nodes_coordinates_Max = new List<double>();
            Nodes_coordinates_Min = new List<double>();

            for (int i = 0; i < Nodes_ID.Count; i++)
            {
                if (Domain_Nodes_X.Count == 1)
                {
                    Nodes_coordinates_Min.Add(this.GetDomain(Domain_Nodes_X[0])[0]);
                    Nodes_coordinates_Max.Add(this.GetDomain(Domain_Nodes_X[0])[1]);
                }
                else if (Domain_Nodes_X.Count == Nodes_ID.Count)
                {
                    Nodes_coordinates_Min.Add(this.GetDomain(Domain_Nodes_X[i])[0]);
                    Nodes_coordinates_Max.Add(this.GetDomain(Domain_Nodes_X[i])[1]);
                }

                if (Domain_Nodes_Y.Count == 1)
                {
                    Nodes_coordinates_Min.Add(this.GetDomain(Domain_Nodes_Y[0])[0]);
                    Nodes_coordinates_Max.Add(this.GetDomain(Domain_Nodes_Y[0])[1]);
                }
                else if (Domain_Nodes_Y.Count == Nodes_ID.Count)
                {
                    Nodes_coordinates_Min.Add(this.GetDomain(Domain_Nodes_Y[i])[0]);
                    Nodes_coordinates_Max.Add(this.GetDomain(Domain_Nodes_Y[i])[1]);
                }

                if (Domain_Nodes_Z.Count == 1)
                {
                    Nodes_coordinates_Min.Add(this.GetDomain(Domain_Nodes_Z[0])[0]);
                    Nodes_coordinates_Max.Add(this.GetDomain(Domain_Nodes_Z[0])[1]);
                }
                else if (Domain_Nodes_Z.Count == Nodes_ID.Count)
                {
                    Nodes_coordinates_Min.Add(this.GetDomain(Domain_Nodes_Z[i])[0]);
                    Nodes_coordinates_Max.Add(this.GetDomain(Domain_Nodes_Z[i])[1]);
                }

            }
        }
        //Get the Bound DATA From either String or Domain_2-1
        public void GetNum(string domain, out double Max, out double Min)
        {
            char[] everychar = domain.ToCharArray();
            int num_1 = 0;

            List<char> min_c = new List<char>();
            List<char> max_c = new List<char>();

            for (int i = 0; i < everychar.Length; i++)
            {
                if (everychar[i] == ' ' || everychar[i] == 'T' || everychar[i] == 't')
                {
                    num_1 = i; break;
                }
                min_c.Add(everychar[i]);
            }

            for (int i = num_1 + 1; i < everychar.Length; i++)
            {
                if (everychar[i] == 'o' || everychar[i] == 'O' || everychar[i] == ' ' || everychar[i] == 'T' || everychar[i] == 't') { continue; }
                else { max_c.Add(everychar[i]); }
            }

            Max = Double.Parse(new string(max_c.ToArray()));
            Min = Double.Parse(new string(min_c.ToArray()));
        }
        //Get the Bound DATA From either String or Domain_2-2 Main
        public List<double> GetDomain(string Domain)
        {
            double max;
            double min;
            this.GetNum(Domain, out max, out min);
            return new List<double>() { min, max };
        }
        //Get the sum of external forces
        public Vector3d Sum_Vec(List<Vector3d> Vectors)
        {
            Vector3d sum_Vec = new Vector3d(0.0, 0.0, 0.0);
            foreach (Vector3d vec in Vectors)
            { sum_Vec += vec; }
            return sum_Vec;
        }
    }
    //[AUI]__________________________________________________________________________________


    public class Topology : Auxiliary
    {
        //_00_Check the input data:
        static public string checkdata(List<Line> Trails, List<double> Trail_lengths, List<Line> Deviation_line, List<double> Deviation_Magni, List<Vector3d> Load_Vec, List<Point3d> Load_Poi, List<Point3d> SupportPoints)
        {
            string ERROR = "INPUT_ERROR:";

            if (Trail_lengths.Count > 0 && Trails.Count > 0)
            {
                if (Trail_lengths.Count == 1 && Trails.Count >= 1) { for (int i = 0; i < Trails.Count - 1; i++) { Trail_lengths.Add(Trail_lengths[0]); } }
                if (Trail_lengths.Count != Trails.Count) { ERROR += "\r\nThe num of trail lines and lengh are not equal please check"; }
            }
            else { ERROR += "\r\n None trail or length"; }

            if (Deviation_Magni.Count >= 1 && Deviation_line.Count >= 1)
            {
                if (Deviation_Magni.Count == 1 && Deviation_line.Count >= 1) { for (int i = 0; i < Deviation_line.Count - 1; i++) { Deviation_Magni.Add(Deviation_Magni[0]); } }
                if (Deviation_Magni.Count == 1 && Deviation_line.Count == 1 )
                {
                    if ( Deviation_line[0].Length == 0 && Deviation_Magni[0]==0) {Deviation_line=new List<Line>(); Deviation_Magni = new List<double>(); }
                }
                if (Deviation_Magni.Count != Deviation_line.Count) { ERROR += "\r\nThe num of Deviation lines and magnitudes are not equal please check"; }
            }
            else { ERROR += "\r\n None Deviation or magnitude"; }

            if (Load_Vec.Count != Load_Poi.Count && Load_Vec.Count == 1 && Load_Poi.Count > 0) { for (int i = 0; i < Load_Poi.Count - 1; i++) { Load_Vec.Add(Load_Vec[0]); } }
            else if (Load_Vec.Count != Load_Poi.Count) { ERROR += "\r\nThe num of Vectors and action Points are not equal please check"; }
            else if (Load_Vec.Count == 0 || Load_Poi.Count == 0) { ERROR += "\r\n None Vector or Action Points"; }

            if (SupportPoints.Count == 0) { ERROR += "\r\n None Support Points"; }

            return ERROR;
        }
        
        //_00_ANAYSIS_Trail_Lines:
        static public void ANAYSIS_Trail_Lines(List<Line> Trails, List<Point3d> SupportPoints, double Threshold, out List<int> Nodes_ID, out List<int> Trail_numList, out List<int> Supports_ID, out List<Point3d> Nodes_Inseq, out string ERROR)
        {
            //step1:ReOrder_Trail_Lines
            List<List<Point3d>> MainList = new List<List<Point3d>>();
            List<Line> Trails_copy = Copy_LineList(Trails);
            while (Trails_copy.Count > 0)
            {
                List<Point3d> SubList = new List<Point3d>();
                SubList.Add(Trails_copy[0].From); SubList.Add(Trails_copy[0].To);
                Trails_copy.RemoveAt(0);
                int count = 0;

                while (true)
                {
                    for (int i = 0; i < Trails_copy.Count; i++)
                    {
                        if (ComparePts(SubList[0], Trails_copy[i].From, Threshold))
                        {
                            SubList.Insert(0, Trails_copy[i].To); Trails_copy.Remove(Trails_copy[i]);
                            i += -1; count += 1;
                        }
                        else if (ComparePts(SubList[0], Trails_copy[i].To, Threshold))
                        {
                            SubList.Insert(0, Trails_copy[i].From); Trails_copy.Remove(Trails_copy[i]);
                            i += -1; count += 1;
                        }
                    }
                    if (count == 0) { break; }
                    else { count = 0; }
                }

                while (true)
                {
                    for (int i = 0; i < Trails_copy.Count; i++)
                    {
                        if (ComparePts(SubList[SubList.Count - 1], Trails_copy[i].From, Threshold))
                        {
                            SubList.Add(Trails_copy[i].To); Trails_copy.Remove(Trails_copy[i]);
                            i += -1; count += 1;
                        }
                        else if (ComparePts(SubList[SubList.Count - 1], Trails_copy[i].To, Threshold))
                        {
                            SubList.Add(Trails_copy[i].From); Trails_copy.Remove(Trails_copy[i]);
                            i += -1; count += 1;
                        }
                    }
                    if (count == 0) { break; }
                    else { count = 0; }
                }

                MainList.Add(SubList);
            }

            //step2:ReOrder_Points
            List<Point3d> SupportPts = Copy_Point3dList(SupportPoints);

            List<Point3d> NodesInOrder = new List<Point3d>();

            foreach (List<Point3d> sublist in MainList)
            {
                for (int i = 0; i < SupportPts.Count; i++)
                {
                    if (ComparePts(sublist[0], SupportPts[i], Threshold))
                    {
                        sublist.Reverse(); SupportPts.RemoveAt(i); break;
                    }
                    else if (ComparePts(sublist[sublist.Count - 1], SupportPts[i], Threshold))
                    {
                        SupportPts.RemoveAt(i); break;
                    }
                }
                NodesInOrder.AddRange(sublist);
            }
            if (SupportPts.Count > 0) { ERROR = "InPut_ERROR:Some supports are not in the extreme of trails"; }
            else { ERROR = null; }
            //step3:Export DATAS:
            Nodes_ID = new List<int>(); for (int i = 0; i < NodesInOrder.Count; i++) { Nodes_ID.Add(i); }
            Trail_numList = new List<int>(); foreach (List<Point3d> sublist in MainList) { Trail_numList.Add(sublist.Count - 1); }
            Supports_ID = new List<int>(); int sum_temp = Trail_numList[0];
            for (int i = 0; i < Trail_numList.Count; i++)
            {
                Supports_ID.Add(Nodes_ID[sum_temp]);
                if (i < Trail_numList.Count - 1) { sum_temp += Trail_numList[i + 1] + 1; }
            }

            Nodes_Inseq = NodesInOrder;
        }

        //_01_GetHierarchical_nodeID
        static public List<List<int>> GetHierarchical_nodeID(List<int> ID_list, List<int> Trail_numList, List<int> SupportID_list, out List<List<int>> EachTrail_NodeList, out List<List<int>> EachLay_NodeList, out string ERROR)
        {
            ERROR = null;
            List<int> Trail_numList_add = new List<int>();
            foreach (int num in Trail_numList) { Trail_numList_add.Add(num + 1); }
            //step1:get Hierarchical ID without considering support point
            EachTrail_NodeList = new List<List<int>>();
            int sum = 0;
            for (int i = 0; i < Trail_numList_add.Count; i++)
            {
                List<int> sub_list = new List<int>();
                for (int j = sum; j < sum + Trail_numList_add[i]; j++)
                {
                    sub_list.Add(ID_list[j]);
                }
                sum += Trail_numList_add[i];
                EachTrail_NodeList.Add(sub_list);
            }
            //step2:consider the support point and reverse the related ones
            for (int i = 0; i < SupportID_list.Count; i++)
            {
                if (SupportID_list[i] != EachTrail_NodeList[i][0] && SupportID_list[i] == EachTrail_NodeList[i][EachTrail_NodeList[i].Count - 1])
                {
                    EachTrail_NodeList[i].Reverse();
                }
                else if (SupportID_list[i] != EachTrail_NodeList[i][0] && SupportID_list[i] != EachTrail_NodeList[i][EachTrail_NodeList[i].Count - 1])
                {
                    ERROR = "ERROR in _GetHierarchical_nodeID_:the support point is not on the End or Start point of the trail";
                }
            }
            //step3:Flip the matrix EachTrail_NodeList to EachLay_NodeList
            EachLay_NodeList = new List<List<int>>();

            foreach (List<int> TrailList in EachTrail_NodeList)
            {
                while (TrailList.Count > EachLay_NodeList.Count)
                {
                    EachLay_NodeList.Add(new List<int>());
                }
                for (int i = 0; i < TrailList.Count; i++)
                {
                    EachLay_NodeList[i].Add(TrailList[i]);
                }
            }

            foreach (List<int> TrailList in EachTrail_NodeList) { TrailList.Reverse(); }//Reverse the order because the calculation should start from the opposite side of the trail(not support)
            EachLay_NodeList.Reverse();//Same reason with above;
            return EachLay_NodeList;

        }

        //_02_Initialize Node class:
        static public void Initialize_NodeClass(List<Point3d> Nodes_InSeq, List<List<int>> EachTrail_NodeList, out List<Node> Nodes_List)
        {
            Nodes_List = new List<Node>();
            //Creat the list of Node class
            for (int i = 0; i < Nodes_InSeq.Count; i++)
            {
                Nodes_List.Add(new Node(i, Nodes_InSeq[i]));

                Nodes_List[i].DeviEdge_ID = new List<List<int>>();
                Nodes_List[i].InDeviEdge_ID = new List<List<int>>();

                Nodes_List[i].ExternalForce_Vec = new List<Vector3d>();

            }
            //Import the trailinID and trailoutID into Node(attention:when adjusting the trail attributes, definitely change all the [i][j] and [j][i])
            foreach (List<int> SubID_list in EachTrail_NodeList)
            {
                Nodes_List[SubID_list[0]].Node_Type = "TAIL";
                Nodes_List[SubID_list[0]].TrailEdgeOut_ID = new List<int>() { SubID_list[0], SubID_list[1] };
                Nodes_List[SubID_list[0]].TrailEdgeIn_ID = new List<int>() { -1, -1 };

                Nodes_List[SubID_list[SubID_list.Count - 1]].Node_Type = "SUPP";
                Nodes_List[SubID_list[SubID_list.Count - 1]].TrailEdgeOut_ID = new List<int>() { -1, -1 };
                Nodes_List[SubID_list[SubID_list.Count - 1]].TrailEdgeIn_ID = new List<int>() { SubID_list[SubID_list.Count - 1], SubID_list[SubID_list.Count - 2] };

                for (int i = 1; i < SubID_list.Count - 1; i++)
                {
                    Nodes_List[SubID_list[i]].Node_Type = "NORM";
                    Nodes_List[SubID_list[i]].TrailEdgeOut_ID = new List<int>() { SubID_list[i], SubID_list[i + 1] };
                    Nodes_List[SubID_list[i]].TrailEdgeIn_ID = new List<int>() { SubID_list[i], SubID_list[i - 1] };
                }
            }
        }

        //_03_Import external forces into Nodes:
        static public void ImportExternalForce(List<Node> Nodes_List, List<Point3d> Positions, List<Vector3d> Vectors, double Threshold, out string ERROR)
        {
            if (Positions.Count != Vectors.Count) { ERROR = "ERROR in _ImportExternalForce_:Not each Load has both vector and position,Please check"; return; }
            else { ERROR = null; }
            for (int i = 0; i < Positions.Count; i++)
            {
                foreach (Node node in Nodes_List)
                {
                    if (ComparePts(node.Position, Positions[i], Threshold)) { node.ExternalForce_Vec.Add(Vectors[i]); }//here should not be a "break" cause when two point in the same position, they should both obtian an external force;
                }
            }
        }

        //_04_Initialize Edge class:
        static public void Initialize_EdgeClass(List<Point3d> Nodes_InSeq, out List<List<Edge>> Edge_Metrix)
        {
            Edge_Metrix = new List<List<Edge>>();

            for (int i = 0; i < Nodes_InSeq.Count; i++)
            {
                List<Edge> Edge_List = new List<Edge>();
                for (int j = 0; j < Nodes_InSeq.Count; j++)
                {
                    Edge_List.Add(new Edge());
                    Edge_List[j].Edge_ID = new List<int>() { i, j };
                    Edge_List[j].EdgeType = "--";
                }
                Edge_Metrix.Add(Edge_List);
            }
        }

        //_05_Import Traillines into the joint Metrix:
        static public void ImportTrail(List<Line> Trails, List<Node> Nodes, List<List<Edge>> EdgeMetrix, List<double> Trail_lengths, double Threshold, out string ERROR)
        {
            ERROR = null;
            //trail into Trails[n]
            for (int n = 0; n < Trails.Count; n++)
            {
                //First get the two NodeID of the nodes corresponding to the end and st point of trail
                int end_ID = -1; int st_ID = -1;
                for (int i = 0; i < Nodes.Count; i++)
                {
                    if (ComparePts(Trails[n].To, Nodes[i].Position, Threshold))
                    {
                        end_ID = Nodes[i].node_ID;
                        break;
                    }
                }

                for (int i = 0; i < Nodes.Count; i++)
                {
                    if (ComparePts(Trails[n].From, Nodes[i].Position, Threshold))
                    {
                        st_ID = Nodes[i].node_ID;
                        break;
                    }
                }

                if (st_ID != -1 && end_ID != -1 && st_ID != end_ID)//import attributes into EdgeMetrix
                {
                    EdgeMetrix[st_ID][end_ID].line = Trails[n];
                    EdgeMetrix[st_ID][end_ID].EdgeType = "TL";
                    EdgeMetrix[st_ID][end_ID].length = Trail_lengths[n];

                    EdgeMetrix[end_ID][st_ID].line = Trails[n];
                    EdgeMetrix[end_ID][st_ID].EdgeType = "TL";
                    EdgeMetrix[end_ID][st_ID].length = Trail_lengths[n];
                }
                else
                {
                    ERROR = "ERROR in _ImportTrail_:One trail cannot be matched in the Nodes";
                }
            }
        }

        //_06_Import DeviationLines into the joint Metrix:
        static public void ImportDeviation(List<Line> Dev_lines, List<double> Dev_magni, List<Node> Nodes, List<List<Edge>> EdgeMetrix, List<List<int>> EachLayMetrix, double Threshold, ref List<List<int>> InDevMirroList, out string ERROR)
        {
            ERROR = null;
            for (int num = 0; num < Dev_lines.Count; num++)
            {
                Line devi_line = Dev_lines[num];
                //First get the two NodeID of the nodes corresponding to the end and st point of devi_line
                int end_ID = -1; int st_ID = -1;
                for (int i = 0; i < Nodes.Count; i++)
                {
                    if (ComparePts(devi_line.To, Nodes[i].Position, Threshold))
                    {
                        end_ID = Nodes[i].node_ID;
                        break;
                    }
                }

                for (int i = 0; i < Nodes.Count; i++)
                {
                    if (ComparePts(devi_line.From, Nodes[i].Position, Threshold))
                    {
                        st_ID = Nodes[i].node_ID;
                        break;
                    }
                }

                if (st_ID != -1 && end_ID != -1)//import attributes into EdgeMetrix
                {
                    if (SameLayCheck(st_ID, end_ID, EachLayMetrix))//distinguish the deviation is direct or indirect
                    {
                        EdgeMetrix[st_ID][end_ID].EdgeType = "DE"; EdgeMetrix[end_ID][st_ID].EdgeType = "DE";
                        Nodes[st_ID].DeviEdge_ID.Add(new List<int>() { st_ID, end_ID });
                        Nodes[end_ID].DeviEdge_ID.Add(new List<int>() { end_ID, st_ID });
                    }
                    else
                    {
                        EdgeMetrix[st_ID][end_ID].EdgeType = "ID"; EdgeMetrix[end_ID][st_ID].EdgeType = "ID";
                        Nodes[st_ID].InDeviEdge_ID.Add(new List<int>() { st_ID, end_ID }); InDevMirroList.Add(new List<int>() { st_ID, end_ID });
                        Nodes[end_ID].InDeviEdge_ID.Add(new List<int>() { end_ID, st_ID });
                    }

                    EdgeMetrix[st_ID][end_ID].line = devi_line;
                    EdgeMetrix[st_ID][end_ID].magnitude = Dev_magni[num];

                    EdgeMetrix[end_ID][st_ID].line = devi_line;
                    EdgeMetrix[end_ID][st_ID].magnitude = Dev_magni[num];
                }
                else if (st_ID == -1 && end_ID == -1) { continue; }
                else
                {
                    ERROR = "ERROR in _ImportDeviation_:One devi_line cannot be matched in the Nodes";
                }
            }
        }
        //_07_Count the deviations
        static public List<int> CountEdges(DATA_TO TOPO)
        {
            List<int> num = new List<int>();
            int dev = 0;
            int indev = 0;
            int trail = 0;
            for (int i = 0; i < TOPO.Edge_Metrix.Count - 1; i++)
            {
                for (int j = i + 1; j < TOPO.Edge_Metrix.Count; j++)
                {
                    if (TOPO.Edge_Metrix[i][j].EdgeType == "DE")
                    { dev += 1; }
                    else if (TOPO.Edge_Metrix[i][j].EdgeType == "ID")
                    { indev += 1; }
                    else if (TOPO.Edge_Metrix[i][j].EdgeType == "TL")
                    { trail += 1; }
                }
            }
            num.Add(dev); num.Add(indev); num.Add(trail);
            return num;
        }

    }

    public class Extract_Topology: Auxiliary
    {
        //ExtractNodesID：
        static public List<string> ExtractNodesID(DATA_TO TOPO)
        {
            List<string> nodeID_str_list = new List<string>();
            foreach (int i in TOPO.NodeID_list)
            {
                for (int j = 0; j < TOPO.EachLay_NodeList.Count; j++)
                {
                    bool check = false;
                    foreach (int num in TOPO.EachLay_NodeList[j])
                    {
                        if (i == num) { string ID = i.ToString() + "(" + j.ToString() + ")"; nodeID_str_list.Add(ID); check = true; break; }
                    }
                    if (check) { break; }
                }
            }
            return nodeID_str_list;
        }

        //ExtractEdges：
        static public void ExtractEdges(DATA_TO TOPO, out List<Line> EdgesLines, out List<string> EdgesID)
        {
            EdgesLines = new List<Line>();
            EdgesID = new List<string>();

            for (int i = 0; i < TOPO.Edge_Metrix.Count - 1; i++)
            {
                for (int j = i + 1; j < TOPO.Edge_Metrix.Count; j++)
                {
                    if (TOPO.Edge_Metrix[i][j].EdgeType != "--")
                    {
                        EdgesLines.Add(TOPO.Edge_Metrix[i][j].line);
                        EdgesID.Add("[" + TOPO.Edge_Metrix[i][j].Edge_ID[0] + "]" + "[" + TOPO.Edge_Metrix[i][j].Edge_ID[1] + "]" + TOPO.Edge_Metrix[i][j].EdgeType);
                    }
                }
            }
        }

        //ExtractconstraintPlane：
        static public void ExtractConstraintPlane(DATA_TO TOPO, string Plane, out List<int> allnodes_exceptstart, out List<Plane> constraint_plane)
        {
            allnodes_exceptstart = new List<int>();
            constraint_plane = new List<Plane>();
            for (int i = 0; i < TOPO.EachTrail_NodeList.Count; i++)
            {
                for (int j = 1; j < TOPO.EachTrail_NodeList[i].Count; j++)
                {
                    allnodes_exceptstart.Add(TOPO.EachTrail_NodeList[i][j]);
                }
            }
            
            if (Plane == "XY")
            { foreach (int i in allnodes_exceptstart) { constraint_plane.Add(new Plane(TOPO.Nodes_InSeq[i], new Vector3d(0, 0, -1))); } }
            else if (Plane == "XZ")
            { foreach (int i in allnodes_exceptstart) { constraint_plane.Add(new Plane(TOPO.Nodes_InSeq[i], new Vector3d(0, 1, 0))); } }
            else if (Plane == "FA")
            {
                foreach (int i in allnodes_exceptstart)
                {
                    int pt_0=TOPO.Nodes_List[i].TrailEdgeIn_ID[0];
                    int pt_1 = TOPO.Nodes_List[i].TrailEdgeIn_ID[1];
                    Vector3d vec=new Vector3d(TOPO.Nodes_InSeq[pt_0] - TOPO.Nodes_InSeq[pt_1]);
                    //Vector3d vec=GetForceInEdge(TOPO.Nodes_List[i].TrailEdgeIn_ID, TOPO);
                    vec.Unitize();
                    constraint_plane.Add(new Plane(TOPO.Nodes_InSeq[i], vec));
                }
            }
            else
            { foreach (int i in allnodes_exceptstart) { constraint_plane.Add(new Plane(TOPO.Nodes_InSeq[i], new Vector3d(1, 0, 0))); } }
        }

        //ExtractOrginNodes
        static public void ExtractOrginNodes(DATA_TO TOPO, out List<int> originNode_ID, out List<Point3d> originNode_pts)
        {
            originNode_ID = new List<int>();
            originNode_pts = new List<Point3d>();
            foreach (List<int> numlist in TOPO.EachTrail_NodeList) { originNode_ID.Add(numlist[0]); originNode_pts.Add(TOPO.Nodes_InSeq[numlist[0]]); }
        }
    }

    public class Optimization_set
    {
        static public string NLoptAlgorithm_selection(int num)
        {
            if (num == 0) { return "LD_AUGLAG"; }
            else if (num == 1) { return "LD_LBFGS"; }
            else if (num == 2) { return "LD_SLSQP"; }
            else if (num == 3) { return "LD_TNEWTON"; }
            else if (num == 4) { return "LN_BOBYQA"; }
            else if (num == 5) { return "LN_COBYLA"; }
            else if (num == 6) { return "LN_SBPLX"; }
            else if (num == 7) { return "GD_MLSL"; }
            else if (num == 8) { return "GN_MLSL"; }
            else if (num == 9) { return "GN_ISRES"; }
            else { return "LD_LBFGS"; }
        }
    }

    public class Calculation : Auxiliary
    {

        public class VIA
        {
            public double[] sim_via;
            public double[] ori_via;
            public double[] sim_min;
            public double[] sim_max;
            public List<int> record;

            public VIA(double[] via, double[] max, double[] min)
            {
                ori_via = ArrayCopy(via);
                Simplify(via, max, min, out this.sim_via, out this.sim_max, out this.sim_min, out this.record);
            }

            public double[] Refill(double[] via)
            {
                return ReturnBack(via, this.ori_via, this.record);
            }
        }
        //_00_ImportConstraintPlanes
        static public void ImportConstraintPlanes(DATA_TO TOPO, List<Plane> constraintPlane, List<int> constraintPlaneID)
        {
            if (constraintPlane.Count == constraintPlaneID.Count)
            {
                for (int i = 0; i < constraintPlane.Count; i++)
                {
                    TOPO.Nodes_List[constraintPlaneID[i]].constraintPlane = constraintPlane[i];
                    TOPO.Nodes_List[constraintPlaneID[i]].cons_Plane_bool = true;
                }
            }
            //else { Print("INPUT_ERROR:The num of constraintPlane and constraintPlaneID are not equal"); }
        }
        //_01_Flip the int metrix
        static public List<List<int>> FlipMetrix(List<List<int>> TheMetrix)
        {
            List<List<int>> templist = new List<List<int>>();
            foreach (List<int> sublist in TheMetrix)
            {
                while (templist.Count < sublist.Count) { templist.Add(new List<int>()); }
                for (int i = 0; i < sublist.Count; i++)
                {
                    templist[i].Add(sublist[i]);
                }
            }
            return templist;
        }
        //_02_Select the NLoptAlgorithm
        static public NLoptNet.NLoptAlgorithm selected(string algorithmName)
        {
            if (algorithmName == "LD_AUGLAG")
            { return NLoptNet.NLoptAlgorithm.LD_AUGLAG; }
            else if (algorithmName == "LD_LBFGS")
            { return NLoptNet.NLoptAlgorithm.LD_LBFGS; }
            else if (algorithmName == "LD_SLSQP")
            { return NLoptNet.NLoptAlgorithm.LD_SLSQP; }
            else if (algorithmName == "LD_TNEWTON")
            { return NLoptNet.NLoptAlgorithm.LD_TNEWTON; }
            else if (algorithmName == "LN_BOBYQA")
            { return NLoptNet.NLoptAlgorithm.LN_BOBYQA; }
            else if (algorithmName == "LN_COBYLA")
            { return NLoptNet.NLoptAlgorithm.LN_COBYLA; }
            else if (algorithmName == "LN_SBPLX")
            { return NLoptNet.NLoptAlgorithm.LN_SBPLX; }
            else if (algorithmName == "GD_MLSL")
            { return NLoptNet.NLoptAlgorithm.GD_MLSL; }
            else if (algorithmName == "GN_MLSL")
            { return NLoptNet.NLoptAlgorithm.GN_MLSL; }
            else if (algorithmName == "GN_ISRES")
            { return NLoptNet.NLoptAlgorithm.GN_ISRES; }
            else
            { return NLoptNet.NLoptAlgorithm.LD_LBFGS; }
        }

        //_03_Check whether the Input OrginNodeID setting from boundary component is in the range of OriginNodeID
        static public string GetOrigin_Nodes_info(DATA_TO TOPO, OPT_MODEL OPTI)
        {
            string ERROR = null;
            List<int> originNode_ID = new List<int>();
            List<int> TestID = OPTI.Boundary.node.originNode_ID; List<int> temp_list = CopyIntList(TestID);
            foreach (List<int> numlist in TOPO.EachTrail_NodeList) { originNode_ID.Add(numlist[0]); }

            List<int> originNodeID = new List<int>();

            foreach (int ID in TestID)
            {
                if (originNode_ID.Contains(ID) == false)
                {
                    ERROR = string.Format("INPUT_ERROR:The input nodes are not all original points!", ID);
                    int remove_index = temp_list.IndexOf(ID) * 3; temp_list.RemoveAt(remove_index / 3);
                    for (int i = 0; i < 3; i++) { OPTI.Boundary.node.Nodes_coordinates_Max.RemoveAt(remove_index); OPTI.Boundary.node.Nodes_coordinates_Min.RemoveAt(remove_index); }
                }
                else
                {
                    originNodeID.Add(ID);
                }
            }

            OPTI.Boundary.node.originNode_ID = originNodeID;

            return ERROR;
        }

        //_00_MAIN_[NodeNodeCalculation]
        static public void NodeNodeCal(Node T_node, DATA_TO TOPO, SW SelfWeight, int iter_Count)
        {
            bool selfWeight = SelfWeight.selfWeight;
            double yieldStress = SelfWeight.yieldStress;
            double specWeight = SelfWeight.specWeight;
            Vector3d trail_forceIn;
            Vector3d dev_force_sum = new Vector3d(0.0, 0.0, 0.0);
            Vector3d Indev_force_sum = new Vector3d(0.0, 0.0, 0.0);
            Vector3d external_force_sum = new Vector3d(0.0, 0.0, 0.0);
            Vector3d weight_force_sum = new Vector3d(0.0, 0.0, 0.0);

            //trail force
            if (T_node.TrailEdgeIn_ID[0] == -1 && T_node.TrailEdgeIn_ID[1] == -1) { trail_forceIn = new Vector3d(0.0, 0.0, 0.0); }
            else { trail_forceIn = GetForceInEdge(T_node.TrailEdgeIn_ID, TOPO); }

            //dev force
            foreach (List<int> devID in T_node.DeviEdge_ID) { dev_force_sum += GetForceInEdge(devID, TOPO); }

            //Indev force
            foreach (List<int> IndevID in T_node.InDeviEdge_ID)
            {
                Vector3d Indev_Vec = GetForceInEdge(IndevID, TOPO);
                Vector3d Indev_Cal_Vec = new Vector3d(Indev_Vec.X, Indev_Vec.Y, Indev_Vec.Z); Indev_Cal_Vec.Unitize();
                Indev_force_sum += Indev_Vec;
                TOPO.Edge_Metrix[IndevID[0]][IndevID[1]].Calculated_Vec = Indev_Cal_Vec;
            }

            //external force
            foreach (Vector3d external_force in T_node.ExternalForce_Vec) { external_force_sum += external_force; }

            //weight force
            if (selfWeight)
            {
                if (iter_Count == 0) { weight_force_sum = new Vector3d(0.0, 0.0, 0.0); }
                else if (T_node.Node_Type == "TAIL")
                {
                    foreach (List<int> devID in T_node.DeviEdge_ID)
                    {
                        if (TOPO.Nodes_List[devID[1]].Node_Type == "TAIL") { weight_force_sum += 0.5 * GetWeight_Vec(devID, TOPO, yieldStress, specWeight); }
                    }
                }
                else if (TOPO.Nodes_List[T_node.node_ID - 1].Node_Type == "TAIL")
                {
                    weight_force_sum += GetWeight_Vec(T_node.TrailEdgeIn_ID, TOPO, yieldStress, specWeight);

                    foreach (List<int> IndevID in T_node.InDeviEdge_ID)
                    {
                        if (TOPO.Nodes_List[IndevID[1]].Node_Type == "TAIL") { weight_force_sum += GetWeight_Vec(IndevID, TOPO, yieldStress, specWeight); }
                        else { weight_force_sum += 0.5 * GetWeight_Vec(IndevID, TOPO, yieldStress, specWeight); }
                    }

                    foreach (List<int> devID in T_node.DeviEdge_ID)
                    {
                        if (TOPO.Nodes_List[devID[1]].Node_Type == "TAIL") { weight_force_sum += GetWeight_Vec(devID, TOPO, yieldStress, specWeight); }
                        else { weight_force_sum += 0.5 * GetWeight_Vec(devID, TOPO, yieldStress, specWeight); }
                    }
                }
                else
                {
                    weight_force_sum += 0.5 * GetWeight_Vec(T_node.TrailEdgeIn_ID, TOPO, yieldStress, specWeight);
                    foreach (List<int> IndevID in T_node.InDeviEdge_ID) { weight_force_sum += 0.5 * GetWeight_Vec(IndevID, TOPO, yieldStress, specWeight); }
                    foreach (List<int> devID in T_node.DeviEdge_ID) { weight_force_sum += 0.5 * GetWeight_Vec(devID, TOPO, yieldStress, specWeight); }
                }
            }

            //Calculate Next Trail Information:
            Vector3d SUM_VEC = new Vector3d(0.0, 0.0, 0.0);
            if (iter_Count == 0)
            { SUM_VEC = trail_forceIn + dev_force_sum  + external_force_sum + weight_force_sum; }
            else
            { SUM_VEC = trail_forceIn + dev_force_sum + Indev_force_sum + external_force_sum + weight_force_sum; }

            double magni = SUM_VEC.Length;
            Point3d next_point;

            //Update the attributes in next trail and node
            if (T_node.Node_Type != "SUPP")
            {
                Edge updateEdge_0 = TOPO.Edge_Metrix[T_node.TrailEdgeOut_ID[0]][T_node.TrailEdgeOut_ID[1]];
                Edge updateEdge_1 = TOPO.Edge_Metrix[T_node.TrailEdgeOut_ID[1]][T_node.TrailEdgeOut_ID[0]];
                Node updateNode = TOPO.Nodes_List[T_node.TrailEdgeOut_ID[1]];

                Vector3d next_vec_Uni = -SUM_VEC; next_vec_Uni.Unitize();
                if (updateEdge_0.length < 0&&!updateNode.cons_Plane_bool)
                {
                    next_vec_Uni *= -1;
                    updateEdge_0.magnitude = -magni;
                    updateEdge_1.magnitude = -magni;
                }
                else
                {
                    updateEdge_0.magnitude = magni;
                    updateEdge_1.magnitude = magni;
                }
                next_point = T_node.Position + next_vec_Uni * System.Math.Abs(updateEdge_0.length);//Calculate the next point position;

                Vector3d vec_1 = new Vector3d(next_point - T_node.Position); vec_1.Unitize();//detect whether the constraintPlane change the direction of force(make the default compression to tension)

                //Add constraintPlane here(change the next_point)
                if (updateNode.cons_Plane_bool)
                {
                    Line line = new Line(T_node.Position, next_point);
                    Point3d inter_point = new Point3d(0.0, 0.0, 0.0);
                    if (intersect_LinePlane(updateNode.constraintPlane, line, ref inter_point))
                    { next_point = inter_point;}

                    Vector3d vec_2 = new Vector3d(next_point - T_node.Position); vec_2.Unitize();//here the next_point was updated by the constraintPlane
                    
                    if (System.Math.Abs(vec_1.X - vec_2.X) < TOPO.Sys_threshold && System.Math.Abs(vec_1.Y - vec_2.Y) < TOPO.Sys_threshold && System.Math.Abs(vec_1.Z - vec_2.Z) < TOPO.Sys_threshold)//update attributes in next node and trail
                    {
                        updateEdge_0.magnitude = magni;
                        updateEdge_1.magnitude = magni;
                    }
                    else
                    {
                        updateEdge_0.magnitude = -magni;
                        updateEdge_1.magnitude = -magni;
                    }
                }                      
        
                updateNode.Position = next_point;
            }
            else if (T_node.Node_Type == "SUPP")
            {
                Vector3d reaction_Vec = -SUM_VEC;
                T_node.ReactionForce = reaction_Vec;//Only the "SUPP" type node have the reaction force
            }
        }

        //_01_MAIN_[EQUILIBRIUM]
        static public void EQUILIBRIUM(List<int> Calculation_Seq, DATA_TO TOPO, SW SelfWeight, int iter_Count, ref double divergence)
        {

            //Each Node calculate(each step)
            foreach (int i in Calculation_Seq)
            {
                NodeNodeCal(TOPO.Nodes_List[i], TOPO, SelfWeight, iter_Count);
            }

            //Divergen calculation
            double dive_sum;
            if (iter_Count == 0) { dive_sum = double.PositiveInfinity; }//find the big num
            else { dive_sum = 0; }
            if (iter_Count != 0)
            {
                foreach (List<int> ID in TOPO.InDevMirroList)
                {
                    dive_sum += GetCalculated_Vec(ID, TOPO);
                }
            }

            divergence = dive_sum;
        }
        //_02_MAIN_[SCORE]
        static public double Score(double[] via, VIA VAR, OPT_MODEL OPTI, DATA_TO TOPO, List<int> Calculation_Seq, SW SelfWeight, int bracing_count, double threshold, bool boolean)
        {
            List<double> initial_viables = new List<double>(VAR.Refill(via));
            OPT_MODEL OPTI_temp = OPTI.CopySelf();//The process of getting socre will not change the initial class
            DATA_TO TOPO_temp = TOPO.CopySelf();//The process of getting socre will not change the initial class
            if (initial_viables.Count > 0)
            {
                OPTI_temp.Boundary.InPut_list(initial_viables);
                OPTI_temp.Boundary.feedback_DATA(TOPO_temp);
            }
            int iter_Count = 0; double divergence = double.PositiveInfinity;

            while (iter_Count < bracing_count && divergence > threshold)
            {
                EQUILIBRIUM(Calculation_Seq, TOPO_temp, SelfWeight, iter_Count, ref divergence);
                iter_Count += 1;
            }
            if (boolean) { TOPO.DATA_FROM += string.Format("\r\n     |sub_iteration[{0}](divergence：{1})\r\n", iter_Count, divergence); }
            double socre = OPTI_temp.Target.GetScore(TOPO_temp);
            //need a disconstruct function
            return socre;
        }
        //_03_MAIN_[Calculate Gradient List]
        static public double[] Gradient(double gra, double[] via, VIA VAR, OPT_MODEL OPTI, DATA_TO TOPO, List<int> Calculation_Seq, SW SelfWeight, int bracing_count, double threshold)
        {
            List<double> gradients = new List<double>();
            double[] via_temp = ArrayCopy(via);

            double Score_Norm = Score(via, VAR, OPTI, TOPO, Calculation_Seq, SelfWeight, bracing_count, threshold, false);

            for (int i = 0; i < via_temp.Length; i++)
            {
                via_temp[i] += gra;
                double Score_Plus = Score(via_temp, VAR, OPTI, TOPO, Calculation_Seq, SelfWeight, bracing_count, threshold, false);
                via_temp[i] -= gra;

                gradients.Add((Score_Plus - Score_Norm) / gra);
            }

            return gradients.ToArray();
        }
        //_04_[Optimization_Gra]
        static public double[] Optimization_Gra(double[] via, VIA VAR, double[] min, double[] max, double gradientDelta, double relativeTolerance, int maxIterations, string optimAlgorithm, OPT_MODEL OPTI, DATA_TO TOPO, List<int> Calculation_Seq, SW SelfWeight, int bracing_count, double threshold)
        {
            var Algorithm = selected(optimAlgorithm);
            int count = 0;
            System.UInt32 icount = System.UInt32.Parse(via.Length.ToString());
            double[] new_via;
            using (var solver = new NLoptSolver(Algorithm, icount, relativeTolerance, maxIterations))
            {
                solver.SetLowerBounds(min);
                solver.SetUpperBounds(max);
                solver.SetMinObjective((variables, gradient) =>
                {
                    count += 1;
                    if (gradient != null)
                    {
                        double[] temp = Gradient(gradientDelta, variables, VAR, OPTI, TOPO, Calculation_Seq, SelfWeight, bracing_count, threshold);
                        for (int i = 0; i < icount; i++)
                        {
                            gradient[i] = temp[i];
                        }
                    }
                    double score = Score(variables, VAR, OPTI, TOPO, Calculation_Seq, SelfWeight, bracing_count, threshold, true);
                    TOPO.DATA_FROM += string.Format("__[{0}]|OPT_ITERATION|__divergence:[{1}]\r\n", count, score);
                    return score;
                });
                double? finalScore;
                var initialValue = via;
                var result = solver.Optimize(initialValue, out finalScore);
                new_via = initialValue;

            }
            return new_via;
        }
        //_05_MAIN_[FinalCalculation]
        static public void FinalCalculation(double[] via, VIA VAR, OPT_MODEL OPTI, DATA_TO TOPO, List<int> Calculation_Seq, SW SelfWeight, int bracing_count, double threshold, bool boolean)
        {
            List<double> initial_viables = new List<double>(VAR.Refill(via));
            if (initial_viables.Count > 0)
            {
                OPTI.Boundary.InPut_list(initial_viables);
                OPTI.Boundary.feedback_DATA(TOPO);
            }

            int iter_Count = 0; double divergence = double.PositiveInfinity;

            while (iter_Count < bracing_count && divergence > threshold)
            {
                EQUILIBRIUM(Calculation_Seq, TOPO, SelfWeight, iter_Count, ref divergence);
                iter_Count += 1;
            }
            if (boolean) { TOPO.DATA_FROM += string.Format("\r\n     |sub_iteration[{0}](divergence：{1})\r\n", iter_Count, divergence); }
            EQUILIBRIUM(Calculation_Seq, TOPO, SelfWeight, iter_Count, ref divergence);//DEBUG Some cases need a final iteration to ensure the equilibrium[210829]
        }

    }

    public class Extract_Structure : Auxiliary
    {
        //_00_ExtractExternal
        static public void ExtractExternal(DATA_TO TOPO, Color Green, out List<Line> external_lines, out List<double> external_mags, out List<Color> external_Col)
        {
            external_lines = new List<Line>();
            external_mags = new List<double>();
            external_Col = new List<Color>();
            foreach (Node node in TOPO.Nodes_List)
            {
                foreach (Vector3d vec in node.ExternalForce_Vec)
                {
                    external_lines.Add(new Line((node.Position + vec), node.Position));
                    external_mags.Add(vec.Length);
                    external_Col.Add(Green);
                }
                if (node.ReactionForce != new Vector3d(0, 0, 0))
                {
                    external_lines.Add(new Line((node.Position + node.ReactionForce), node.Position));
                    external_mags.Add(node.ReactionForce.Length);
                    external_Col.Add(Green);
                }
            }
        }

        //_01_ExtractDataFromEdges
        static public void ExtractDataFromEdges(DATA_TO TOPO, List<Edge> Edge_list, string type, Color Red, Color Blue, Color Black, ref List<Line> lines, ref List<string> ID, ref List<double> magnitude, ref List<Color> Col)
        {
            foreach (Edge edge in Edge_list)
            {
                int ID1 = edge.Edge_ID[0]; int ID2 = edge.Edge_ID[1];
                if (edge.EdgeType == type)
                {
                    lines.Add(new Line(TOPO.Nodes_InSeq[ID1], TOPO.Nodes_InSeq[ID2]));
                    ID.Add("[" + ID1.ToString() + "]" + "[" + ID2.ToString() + "]");
                    magnitude.Add(edge.magnitude);
                    if (edge.ForceType() == "TE") { Col.Add(Red); }
                    else if (edge.ForceType() == "CO") { Col.Add(Blue); }
                    else { Col.Add(Black); }
                }
            }
        }

        //_02_ExtractNodesID
        static public List<string> ExtractNodesID(DATA_TO TOPO)
        {
            List<string> nodeID_str_list = new List<string>();
            foreach (int i in TOPO.NodeID_list)
            {
                for (int j = 0; j < TOPO.EachLay_NodeList.Count; j++)
                {
                    bool check = false;
                    foreach (int num in TOPO.EachLay_NodeList[j])
                    {
                        if (i == num) { string ID = i.ToString() + "(" + j.ToString() + ")"; nodeID_str_list.Add(ID); check = true; break; }
                    }
                    if (check) { break; }
                }
            }
            return nodeID_str_list;
        }

    }

    public class Auxiliary
    {
        //[Auxiliary Functions]:_______________________________________________________________________________________________________________________________________
        //_00_Copy Point3d<List>
        static public List<Point3d> Copy_Point3dList(List<Point3d> list) { List<Point3d> newlist = new List<Point3d>(); foreach (Point3d i in list) { newlist.Add(i); } return newlist; }
        //_00_Copy Line<List>
        static public List<Line> Copy_LineList(List<Line> list) { List<Line> newlist = new List<Line>(); foreach (Line i in list) { newlist.Add(i); } return newlist; }
        //_00_Copy Double<List>
        static public List<double> CopyDoubleList(List<Double> list)
        {
            List<Double> temp = new List<Double>();
            foreach (Double i in list)
            {
                temp.Add(i);
            }
            return temp;
        }
        //_00_Copy Double<List>
        static public List<int> CopyIntList(List<int> list)
        {
            List<int> temp = new List<int>();
            foreach (int i in list)
            {
                temp.Add(i);
            }
            return temp;
        }
        //_00_Copy Double<Array>
        static public double[] ArrayCopy(double[] array)
        {
            double[] new_array = new double[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                new_array[i] = array[i];
            }
            return new_array;
        }

        //_01_CompareTwoPoints:return ture if two points are in same Position
        static public bool ComparePts(Point3d Pt_1, Point3d Pt_2, double Threshold)
        {
            if (System.Math.Abs(Pt_1.X - Pt_2.X) < Threshold && System.Math.Abs(Pt_1.Y - Pt_2.Y) < Threshold && System.Math.Abs(Pt_1.Z - Pt_2.Z) < Threshold)
            { return true; }
            else
            { return false; }
        }

        //_02_check two nodeID are in same lay:
        static public bool SameLayCheck(int ID_1, int ID_2, List<List<int>> EachLayMetrix)
        {
            int ID_1_laynum = -1; int ID_2_laynum = -1;
            for (int i = 0; i < EachLayMetrix.Count; i++)
            {
                if (ID_1_laynum == -1)
                {
                    foreach (int layID in EachLayMetrix[i])
                    {
                        if (layID == ID_1) { ID_1_laynum = i; break; }
                    }
                }

                if (ID_2_laynum == -1)
                {
                    foreach (int layID in EachLayMetrix[i])
                    {
                        if (layID == ID_2) { ID_2_laynum = i; break; }
                    }
                }

                if (ID_1_laynum != -1 && ID_2_laynum != -1) { break; }
            }

            if (ID_1_laynum != -1 && ID_2_laynum != -1 && ID_1_laynum == ID_2_laynum) { return true; }
            else if (ID_1_laynum != -1 && ID_2_laynum != -1 && ID_1_laynum != ID_2_laynum) { return false; }
            else { return false; }
        }

        //_03_From List<List<int>>To List<int>
        static public List<int> FlatternTheList(List<List<int>> intMetrix)
        {
            List<int> FlatternList = new List<int>();
            foreach (List<int> sublist in intMetrix)
            {
                FlatternList.AddRange(sublist);
            }
            return FlatternList;
        }

        //_04_Intersect_LinePlane
        static public bool intersect_LinePlane(Plane plane, Line line, ref Point3d inter_point)
        {
            double para;
            if (Rhino.Geometry.Intersect.Intersection.LinePlane(line, plane, out para))
            {
                if (para < 0 || para > 1)
                {
                    double pa;
                    line.Extend(System.Math.Abs(para) * line.Length, System.Math.Abs(para) * line.Length);
                    Rhino.Geometry.Intersect.Intersection.LinePlane(line, plane, out pa);
                    inter_point = line.PointAt(pa);
                }
                else
                {
                    inter_point = line.PointAt(para);
                }
                return true;
            }
            else
            { return false; }
        }

        //_05_Get the forceVector from the DATA_TO class with ID input;
        static public Vector3d GetForceInEdge(List<int> ID, DATA_TO TOPO)
        {
            Edge OB_Edge = TOPO.Edge_Metrix[ID[0]][ID[1]];
            Point3d Vec_From = TOPO.Nodes_List[ID[0]].Position;
            Point3d Vec_To = TOPO.Nodes_List[ID[1]].Position;

            Vector3d Unit_Vec = new Vector3d(Vec_To - Vec_From); Unit_Vec.Unitize();
            Vector3d Force_Vec = Unit_Vec * OB_Edge.magnitude;

            return Force_Vec;
        }

        //_06_GetWeight_Vec_:
        static public Vector3d GetWeight_Vec(List<int> ID, DATA_TO TOPO, double yieldStress, double specWeight)
        {
            Edge OB_Edge = TOPO.Edge_Metrix[ID[0]][ID[1]];
            Vector3d Vec = ((System.Math.Abs(OB_Edge.magnitude) * (TOPO.Nodes_List[ID[0]].Position.DistanceTo(TOPO.Nodes_List[ID[1]].Position)) * specWeight / yieldStress)) * (new Vector3d(0.0, 0.0, -1.0));
            return Vec;
        }

        //_07_GetCalculated_Divergence of Two Vec:
        static public double GetCalculated_Vec(List<int> ID, DATA_TO TOPO)
        {
            Edge OB_Edge_1 = TOPO.Edge_Metrix[ID[0]][ID[1]];
            Edge OB_Edge_2 = TOPO.Edge_Metrix[ID[1]][ID[0]];

            Vector3d vec1 = OB_Edge_1.Calculated_Vec;
            Vector3d vec2 = -OB_Edge_2.Calculated_Vec;

            double num = System.Math.Acos((vec1 * vec2) / (vec1.Length * vec2.Length));
            if (double.IsNaN(num) && (vec1 + vec2) == new Vector3d(0.0, 0.0, 0.0)) { num = 3.1415926; }
            else if (double.IsNaN(num) && (vec1 + vec2) != new Vector3d(0.0, 0.0, 0.0)) { num = 0; }
            return num;
        }

        //_08_Simplify the variables:
        static public void Simplify(double[] variable, double[] max, double[] min, out double[] var_simplified, out double[] max_simplified, out double[] min_simplified, out List<int> record)
        {
            List<double> variable_temp = new List<double>(ArrayCopy(variable));
            List<double> max_temp = new List<double>(ArrayCopy(max));
            List<double> min_temp = new List<double>(ArrayCopy(min));
            List<double> variable_sim = new List<double>();
            List<double> max_sim = new List<double>();
            List<double> min_sim = new List<double>();
            record = new List<int>();

            for (int i = 0; i < variable_temp.Count; i++)
            {
                if (max_temp[i] <= min_temp[i]) { record.Add(i); }
                else
                {
                    variable_sim.Add(variable_temp[i]);
                    max_sim.Add(max_temp[i]);
                    min_sim.Add(min_temp[i]);
                }
            }

            var_simplified = variable_sim.ToArray();
            max_simplified = max_sim.ToArray();
            min_simplified = min_sim.ToArray();
        }
        //_09_Recovery the the variables:
        static public double[] ReturnBack(double[] var_simplified, double[] var_original, List<int> record)
        {
            List<double> var_simplified_temp = new List<double>(ArrayCopy(var_simplified));

            foreach (int ID in record)
            {
                var_simplified_temp.Insert(ID, var_original[ID]);
            }
            return var_simplified_temp.ToArray();
        }

        //_10_Make Metrix Into List:
        static public List<Edge> MakeMetrixIntoList(DATA_TO TOPO)
        {
            List<Edge> Edge_list = new List<Edge>();
            for (int i = 0; i < TOPO.Edge_Metrix.Count; i++)
            {
                for (int j = i + 1; j < TOPO.Edge_Metrix.Count; j++)
                {
                    Edge_list.Add(TOPO.Edge_Metrix[i][j]);
                }
            }
            return Edge_list;
        }




        //[Auxiliary Functions];_______________________________________________________________________________________________________________________________________
    }
}

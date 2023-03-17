using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using MODEL_OPTI_B = CEM_Lib_Yuchi.MODEL_OPTI_B;
using TAR = CEM_Lib_Yuchi.TAR;
using OPT_MODEL = CEM_Lib_Yuchi.OPT_MODEL;
using Optimization_set = CEM_Lib_Yuchi.Optimization_set;

namespace CEM_C_sharp
{
    public class CEM_Optimization_Setup : GH_Component
    {
        public CEM_Optimization_Setup()
          : base("CEM_Optimization_Setup", "CEM_Optimization_Setup",
              "[Assemble the OPTIMIZATION Settings]",
              "CEM_TEST", "CEM_C_sharp")
        {
        }


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager Input)
        {
            double default_double_gra = 0.0001;
            double default_double_tol = 0.000001;
            int default_int = 1;
            int default_al =1;

            Input.AddGenericParameter("TA", "TA", "The assembled Target information from the [CEM_Target] component", GH_ParamAccess.item);
            Input.AddGenericParameter("BD", "BD", "The assembled Boundary information from the [CEM_Optimize_Boundary_Setting] component", GH_ParamAccess.item);
            Input.AddNumberParameter("gradientDelta", "gradientDelta", "The gradient step of the objection function [A parameter for optimization]", GH_ParamAccess.item, default_double_gra);
            Input.AddNumberParameter("relativeTolerance", "relativeTolerance", "The tolerance ends the iterations when the optimization score reach the tolerance", GH_ParamAccess.item, default_double_tol);
            Input.AddIntegerParameter("maxIterations", "maxIterations", "The max iteration time for the optimization algorithm", GH_ParamAccess.item, default_int);
            Input.AddIntegerParameter("optimAlgorithm", "optimAlgorithm", "[Right cleck]Select the algorithm name from NLoptNet", GH_ParamAccess.item, default_al);
            Grasshopper.Kernel.Parameters.Param_Integer param = Input[5] as Grasshopper.Kernel.Parameters.Param_Integer;
            param.AddNamedValue("LD_AUGLAG", 0);
            param.AddNamedValue("LD_LBFGS", 1);
            param.AddNamedValue("LD_SLSQP", 2);
            param.AddNamedValue("LD_TNEWTON", 3);
            param.AddNamedValue("LN_BOBYQA", 4);
            param.AddNamedValue("LN_COBYLA", 5);
            param.AddNamedValue("LN_SBPLX", 6);
            param.AddNamedValue("GD_MLSL", 7);
            param.AddNamedValue("GN_MLSL", 8);
            param.AddNamedValue("GN_ISRES", 9);

        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager Output)
        {
            Output.AddGenericParameter("OPT", "OPT", "The assembled OPTIMIZATION SETTING", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess data)
        {
            TAR temp_TAR = new TAR();
            data.GetData("TA",ref temp_TAR);
            TAR TARGET = temp_TAR.CopySelf();

            MODEL_OPTI_B temp_BD = new MODEL_OPTI_B();
            data.GetData("BD",ref temp_BD);
            MODEL_OPTI_B BOUND = temp_BD.CopySelf();

            double gradientDelta = 0.0;
            double relativeTolerance = 0.0;
            int maxIterations = 0;
            int optimAlgorithm_int = 0;
            data.GetData("gradientDelta",ref gradientDelta);
            data.GetData("relativeTolerance", ref relativeTolerance);
            data.GetData("maxIterations", ref maxIterations);
            data.GetData("optimAlgorithm", ref optimAlgorithm_int);
            string optimAlgorithm = Optimization_set.NLoptAlgorithm_selection(optimAlgorithm_int);
            OPT_MODEL OPTIMIZATION = new OPT_MODEL(TARGET, BOUND, gradientDelta, relativeTolerance, maxIterations, optimAlgorithm);
            data.SetData("OPT", OPTIMIZATION);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.OPT;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("b085c260-04fb-45e3-b893-f649711bf9c2"); }
        }
    }
}
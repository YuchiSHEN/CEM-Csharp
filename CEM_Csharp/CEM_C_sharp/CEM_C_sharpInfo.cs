using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace CEM_C_sharp
{
    public class CEM_C_sharpInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "CEMinC";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("bc7fa5cb-63ba-4091-9b47-333833f255f3");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}

using System;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

namespace gh
{
    public class ghComponent : GH_Component
    {
        public ghComponent()
          : base("Watch", "watch",
              "Displays the geometry in the component itself. Useful for debugging.",
              "Display", "Preview")
        {
        }

        public override void CreateAttributes()
        {
            Attributes = new WatchAttributes(this);
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Geometry", "G", "Geometry to display", GH_ParamAccess.tree);
            pManager[0].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // No outputs — geometry is shown in the component viewer
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_Structure<IGH_Goo> data = null;
            DA.GetDataTree(0, out data);

            var allData = data.AllData(true);
            ((WatchAttributes)m_attributes).UpdateWebView(allData);
        }

        public override void RemovedFromDocument(GH_Document document)
        {
            ((WatchAttributes)m_attributes).DestroyPanel();
            base.RemovedFromDocument(document);
        }



        protected override System.Drawing.Bitmap Icon => null;

        public override Guid ComponentGuid => new Guid("f498d738-bafa-49c0-b056-52ab788fb095");
    }
}

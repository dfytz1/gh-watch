using System;
using Grasshopper.Kernel;
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
            m_attributes = new WatchAttributes(this);
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Geometry", "G", "Geometry to display", GH_ParamAccess.item);
            pManager[0].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // No outputs — geometry is shown in the component viewer
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IGH_Goo data = null;
            DA.GetData(0, ref data);
            ReceivedData = data;
        }

        public override void RemovedFromDocument(GH_Document document)
        {
            ((WatchAttributes)m_attributes).DestroyPanel();
            base.RemovedFromDocument(document);
        }

        /// <summary>Data received from the input — will be passed to the WebView later.</summary>
        public IGH_Goo ReceivedData { get; private set; }

        protected override System.Drawing.Bitmap Icon => null;

        public override Guid ComponentGuid => new Guid("f498d738-bafa-49c0-b056-52ab788fb095");
    }
}

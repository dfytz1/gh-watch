using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace Gh.Watch
{
    public class GhInfo : GH_AssemblyInfo
    {
        public override string Name => "Gh.Watch";
        public override Bitmap Icon => null;
        public override string Description => "A Grasshopper plugin for viewing geometry inside Grasshopper itself.";
        public override Guid Id => new Guid("fe8a82cc-02b8-4eea-80b7-c5133ac36e88");
        public override string AuthorName => "Omkar Bhagwat";
        public override string AuthorContact => "ombhagwat29@gmail.com";
        public override string AssemblyVersion => GetType().Assembly.GetName().Version.ToString();
    }
}

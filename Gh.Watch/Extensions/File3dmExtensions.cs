using Rhino.FileIO;
using Rhino.Geometry;

namespace Gh.Watch.Extensions
{
    public static class File3dmExtensions
    {
        public static void AddBrepEdgesToFile(this File3dm file, Brep brep)
        {

            foreach (BrepEdge edge in brep.Edges)
            {
                file.Objects.AddCurve(edge);
            }
        }

        public static void AddMeshEdgesToFile(this File3dm file, Mesh mesh)
        {
           var nakedEdges = mesh.GetNakedEdges();

            if (nakedEdges != null)
            {
                foreach (var edge in nakedEdges)
                {
                    file.Objects.AddPolyline(edge);
                }
            }

            //foreach (var edge in mesh.TopologyEdges)
            //{
            //    var curve = mesh.TopologyEdgeLine(edge);
            //    file.Objects.AddCurve(curve);
            //}
        }
    }
}

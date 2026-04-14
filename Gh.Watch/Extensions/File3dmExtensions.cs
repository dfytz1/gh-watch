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
            var faces = mesh.Faces;

                foreach (var face in faces)
                {
                //get polyline for each face
                var pl = new Polyline();

                pl.Add(mesh.Vertices[face.A]);
                pl.Add(mesh.Vertices[face.B]);
                pl.Add(mesh.Vertices[face.C]);
                pl.Add(mesh.Vertices[face.D]);

                file.Objects.AddPolyline(pl);
            }
       

            //foreach (var edge in mesh.TopologyEdges)
            //{
            //    var curve = mesh.TopologyEdgeLine(edge);
            //    file.Objects.AddCurve(curve);
            //}
        }
    }
}

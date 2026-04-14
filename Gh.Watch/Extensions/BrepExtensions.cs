using Rhino.Geometry;

namespace Gh.Watch.Extensions
{
    public static class BrepExtensions
    {
        public static bool SetFaceMesh(this BrepFace face)
        {
            var faceMesh = Mesh.CreateFromBrep(face.ToBrep(), MeshingParameters.Default)[0];
            return face.SetMesh(MeshType.Render, faceMesh);
        }

        public static void SetMeshToBrep(this Brep brep, MeshType type = MeshType.Render)
        {
            if (!System.Enum.IsDefined(type))
            {
                throw new System.ComponentModel.InvalidEnumArgumentException(nameof(type), (int)type, typeof(MeshType));
            }

            for (int i = 0; i < brep.Faces.Count; i++)
            {
                var face = brep.Faces[i];

                face.SetFaceMesh();
            }
        }



    }
}
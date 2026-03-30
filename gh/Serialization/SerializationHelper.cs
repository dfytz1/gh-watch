using gh.Constants;
using gh.Dtos;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System.Collections.Generic;

namespace gh.Serialization
{

    public static class SerializationHelper
    {
        // FastRenderMesh is Rhino's built-in low-quality preset — fastest meshing,
        // good enough for a live preview. Swap for MeshingParameters.Default if
        // you need smoother curved surfaces.
        private static readonly MeshingParameters _mp = MeshingParameters.FastRenderMesh;

        public static SendDataDto SerializeGeometry(this IGH_StructureEnumerator _goos)
        {
            var sendData = new SendDataDto();

            var payload = new List<object>();
            foreach (var goo in _goos)
            {
                var data = SerializeGeometry(goo);
                if (data != null)
                {
                    payload.Add(data);
                }
            }

            if (payload.Count == 0) return null;

            sendData.EventType = SendToWebvViewCommand.Send_Geometry;
            sendData.Payload = payload;
            return sendData;
        }


        public static object SerializeGeometry(this IGH_Goo goo)
        {
            switch (goo)
            {
                // Brep: solid, polysurface, single face — mesh and send
                case GH_Brep ghBrep when ghBrep.Value != null:
                    return MeshPayload(MeshFromBrep(ghBrep.Value));

                // Surface: GH 8 stores this as a single-face Brep already
                case GH_Surface ghSurface when ghSurface.Value != null:
                    return MeshPayload(MeshFromBrep(ghSurface.Value));

                // Box: convert to Brep first, then mesh
                case GH_Box ghBox:
                    return MeshPayload(MeshFromBrep(ghBox.Value.ToBrep()));

                // Mesh: already tessellated, just triangulate quads and send
                case GH_Mesh ghMesh when ghMesh.Value != null:
                    return MeshPayload(ghMesh.Value);

                // Curve: sample into a polyline and send as flat point array.
                // Covers LineCurve, ArcCurve, NurbsCurve, PolylineCurve, etc.
                case GH_Curve ghCurve when ghCurve.Value != null:
                    return CurvePayload(ghCurve.Value);

                // Line: struct — never null, just send the two endpoints
                case GH_Line ghLine:
                    return LinePayload(ghLine.Value);

                // Point: struct — just send XYZ
                case GH_Point ghPoint:
                    var p = ghPoint.Value;
                    return new PointPayloadDto
                    {
                        X = (float)p.X,
                        Y = (float)p.Y,
                        Z = (float)p.Z
                    };

                default:
                    return null;
            }
        }

        // ── Mesh construction ─────────────────────────────────────────────────

        // Meshes a Brep using the fast render-mesh parameters. Multiple disjoint
        // pieces (e.g. a polysurface) are joined into one mesh so the payload
        // stays a single object.
        static Mesh MeshFromBrep(Brep brep)
        {
            if (brep == null) return null;

            Mesh[] pieces = Mesh.CreateFromBrep(brep, _mp);
            if (pieces == null || pieces.Length == 0) return null;
            if (pieces.Length == 1) return pieces[0];

            var joined = new Mesh();
            foreach (var piece in pieces)
                joined.Append(piece);
            return joined;
        }

        // ── Payload builders ──────────────────────────────────────────────────

        // Converts a mesh to the wire format Three.js expects for BufferGeometry:
        //   vertices — flat float[vCount*3]: x0,y0,z0, x1,y1,z1, …
        //   normals  — flat float[vCount*3]: nx0,ny0,nz0, …   (for lighting)
        //   faces    — flat int[fCount*3]:   i0,i1,i2, …      (triangle indices)
        //
        // Pre-allocated arrays (not List<>) and float32 (not double) keep the
        // JSON payload small and avoid unnecessary allocations on every solve.
        static MeshPayloadDto MeshPayload(Mesh mesh)
        {
            if (mesh == null) return null;

            // Ensure all faces are triangles — Three.js does not understand quads
            mesh.Faces.ConvertQuadsToTriangles();
            mesh.Normals.ComputeNormals();

            int vc = mesh.Vertices.Count;
            int fc = mesh.Faces.Count;

            var vertices = new float[vc * 3];
            var normals = new float[vc * 3];
            var faces = new int[fc * 3];

            for (int i = 0; i < vc; i++)
            {
                Point3f v = mesh.Vertices[i];
                Vector3f n = mesh.Normals[i];
                int idx = i * 3;
                vertices[idx] = v.X; vertices[idx + 1] = v.Y; vertices[idx + 2] = v.Z;
                normals[idx] = n.X; normals[idx + 1] = n.Y; normals[idx + 2] = n.Z;
            }

            for (int i = 0; i < fc; i++)
            {
                MeshFace f = mesh.Faces[i];
                int idx = i * 3;
                faces[idx] = f.A; faces[idx + 1] = f.B; faces[idx + 2] = f.C;
            }

            return new MeshPayloadDto
            {
                Vertices = vertices,
                Normals = normals,
                Faces = faces
            };
        }

        // Samples a curve into a flat float array: x0,y0,z0, x1,y1,z1, …
        // 64 segments covers tight curves well without producing too many points.
        static CurvePayloadDto CurvePayload(Curve curve)
        {
            double[] ts = curve.DivideByCount(64, includeEnds: true);
            if (ts == null || ts.Length == 0) return null;

            var points = new float[ts.Length * 3];
            for (int i = 0; i < ts.Length; i++)
            {
                Point3d pt = curve.PointAt(ts[i]);
                int idx = i * 3;
                points[idx] = (float)pt.X; points[idx + 1] = (float)pt.Y; points[idx + 2] = (float)pt.Z;
            }

            return new CurvePayloadDto { Buffer = points };
        }

        static LinePayloadDto LinePayload(Line line)
        {
            return new LinePayloadDto
            {
                Start = new float[] { (float)line.From.X, (float)line.From.Y, (float)line.From.Z },
                End = new float[] { (float)line.To.X, (float)line.To.Y, (float)line.To.Z }
            };
        }
    }
}

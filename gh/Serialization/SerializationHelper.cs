using gh.Constants;
using gh.Dtos;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino;
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
                foreach (var item in SerializeGeometry(goo))
                    payload.Add(item);

            if (payload.Count == 0) return null;

            sendData.EventType = SendToWebvViewCommand.Send_Geometry;
            sendData.Payload = payload;
            return sendData;
        }


        public static IEnumerable<object> SerializeGeometry(this IGH_Goo goo)
        {
            switch (goo)
            {
                // Brep: solid, polysurface, single face — mesh + real edges
                case GH_Brep ghBrep when ghBrep.Value != null:
                {
                    var mesh = MeshPayload(MeshFromBrep(ghBrep.Value));
                    if (mesh != null) yield return mesh;
                    var edges = BrepEdgesPayload(ghBrep.Value);
                    if (edges != null) yield return edges;
                    yield break;
                }

                // Surface: GH 8 stores this as a single-face Brep already
                case GH_Surface ghSurface when ghSurface.Value != null:
                {
                    var mesh = MeshPayload(MeshFromBrep(ghSurface.Value));
                    if (mesh != null) yield return mesh;
                    var edges = BrepEdgesPayload(ghSurface.Value);
                    if (edges != null) yield return edges;
                    yield break;
                }

                // Box: convert to Brep first, then mesh + edges
                case GH_Box ghBox:
                {
                    var brep = ghBox.Value.ToBrep();
                    var mesh = MeshPayload(MeshFromBrep(brep));
                    if (mesh != null) yield return mesh;
                    var edges = BrepEdgesPayload(brep);
                    if (edges != null) yield return edges;
                    yield break;
                }

                // Mesh: send topology edges first (before ConvertQuadsToTriangles mutates the mesh),
                // then send the triangulated mesh payload
                case GH_Mesh ghMesh when ghMesh.Value != null:
                {
                    var meshEdges = MeshEdgesPayload(ghMesh.Value);
                    if (meshEdges != null) yield return meshEdges;
                    yield return MeshPayload(ghMesh.Value);
                    yield break;
                }

                // Curve: sample into a polyline and send as flat point array.
                // Covers LineCurve, ArcCurve, NurbsCurve, PolylineCurve, etc.
                case GH_Curve ghCurve when ghCurve.Value != null:
                    yield return CurvePayload(ghCurve.Value);
                    yield break;

                // Line: struct — never null, just send the two endpoints
                case GH_Line ghLine:
                    yield return LinePayload(ghLine.Value);
                    yield break;

                // Point: struct — just send XYZ
                case GH_Point ghPoint:
                    var p = ghPoint.Value;
                    yield return new PointPayloadDto
                    {
                        X = (float)p.X,
                        Y = (float)p.Y,
                        Z = (float)p.Z
                    };
                    yield break;

                default:
                    yield break;
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

        // Extracts all unique topology edges from a Mesh as a flat segment-pair buffer.
        // Called before MeshPayload so quads are captured before ConvertQuadsToTriangles runs.
        static MeshEdgesPayloadDto MeshEdgesPayload(Mesh mesh)
        {
            if (mesh == null || mesh.TopologyEdges.Count == 0) return null;

            int edgeCount = mesh.TopologyEdges.Count;
            var buffer = new float[edgeCount * 6];

            for (int i = 0; i < edgeCount; i++)
            {
                Line edge = mesh.TopologyEdges.EdgeLine(i);
                int idx = i * 6;
                buffer[idx]     = (float)edge.From.X; buffer[idx + 1] = (float)edge.From.Y; buffer[idx + 2] = (float)edge.From.Z;
                buffer[idx + 3] = (float)edge.To.X;   buffer[idx + 4] = (float)edge.To.Y;   buffer[idx + 5] = (float)edge.To.Z;
            }

            return new MeshEdgesPayloadDto { Buffer = buffer };
        }

        // Extracts topological edges from a Brep as a flat segment-pair buffer.
        // Straight edges contribute 1 segment (6 floats); curved edges are sampled
        // into 32 segments (192 floats each). Returns null if the Brep has no edges.
        static BrepEdgesPayloadDto BrepEdgesPayload(Brep brep)
        {
            if (brep == null || brep.Edges.Count == 0) return null;

            var segments = new List<float>();

            foreach (BrepEdge edge in brep.Edges)
            {
                if (edge.IsLinear(RhinoMath.ZeroTolerance))
                {
                    // Straight edge — just two endpoints
                    Point3d from = edge.PointAtStart;
                    Point3d to = edge.PointAtEnd;
                    segments.Add((float)from.X); segments.Add((float)from.Y); segments.Add((float)from.Z);
                    segments.Add((float)to.X);   segments.Add((float)to.Y);   segments.Add((float)to.Z);
                }
                else
                {
                    // Curved edge — sample into 32 segments
                    double[] ts = edge.DivideByCount(32, includeEnds: true);
                    if (ts == null || ts.Length < 2) continue;

                    for (int i = 0; i < ts.Length - 1; i++)
                    {
                        Point3d a = edge.PointAt(ts[i]);
                        Point3d b = edge.PointAt(ts[i + 1]);
                        segments.Add((float)a.X); segments.Add((float)a.Y); segments.Add((float)a.Z);
                        segments.Add((float)b.X); segments.Add((float)b.Y); segments.Add((float)b.Z);
                    }
                }
            }

            if (segments.Count == 0) return null;

            return new BrepEdgesPayloadDto { Buffer = segments.ToArray() };
        }
    }
}

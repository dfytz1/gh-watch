using Gh.Watch.Constants;
using Gh.Watch.Dtos;
using Gh.Watch.Extensions;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.FileIO;
using Rhino.Geometry;
using System.Collections.Generic;

namespace Gh.Watch.Serialization
{
    public static class Rhino3dmSerialization
    {
        public static List<SendDataDto> SerializeObjects(this IGH_StructureEnumerator _goos)
        {
            List<SendDataDto> dataDtos = [];

            var file = new File3dm();
            var brep_payload = new List<object>();
            var mesh_payload = new List<object>();

            var s_opt = new SerializationOptions();
            foreach (var goo in _goos)
            {
                switch (goo)
                {
                    case GH_Brep ghBrep:
                        {
                            var brp = ghBrep.Value;
                            brp.SetMeshToBrep();

                            brep_payload.Add(new GenericPayloadDto
                            {
                                Data = brp.ToJSON(s_opt),
                            });

                            file.AddBrepEdgesToFile(brp);
                            //edge_payload.AddRange(brp.GetEdgePayload(s_opt));

                        }
                        break;
                    case GH_Surface ghSurface:
                        {
                            var brp = ghSurface.Value;
                            brp.SetMeshToBrep();
                            brep_payload.Add(new GenericPayloadDto
                            {
                                Data = brp.ToJSON(s_opt),
                            });

                            file.AddBrepEdgesToFile(brp);
                        }
                        break;
                    case GH_Box ghBox:
                        {
                            var box = ghBox.Value;
                            var brp = box.ToBrep();
                            brp.SetMeshToBrep();
                            brep_payload.Add(new GenericPayloadDto
                            {
                                Data = brp.ToJSON(s_opt),
                            });

                            file.AddBrepEdgesToFile(brp);
                        }
                        break;
                    case GH_Curve gh_Cv:
                        {
                                file.Objects.AddCurve(gh_Cv.Value);
                       
                        }
                        break;
                    case GH_Line gh_Line:
                        {
                           
                           file.Objects.AddLine(gh_Line.Value);
                        }
                        break;
                    case GH_Point gh_Pt:
                        {
                            file.Objects.AddPoint(gh_Pt.Value);
                        }
                        break;
                    case GH_PointCloud gh_PtCloud:
                        {
                            file.Objects.AddPointCloud(gh_PtCloud.Value);
                        }
                        break;
                    default:
                        break;
                }
            }

            if (brep_payload.Count > 0)
            {
                dataDtos.Add(new SendDataDto
                {
                    EventType = GeometryType.Brep,
                    Payload = brep_payload
                });
            }

            if (mesh_payload.Count > 0)
            {
                dataDtos.Add(new SendDataDto
                {
                    EventType = GeometryType.Mesh,
                    Payload = mesh_payload
                });
            }

            if (file.Objects.Count > 0)
            {
                dataDtos.Add(new SendDataDto
                {
                    EventType = SendToWebvViewCommand.Send_File_Geometry,
                    Payload = file.ToByteArray()
                });
            }

            return dataDtos;

        }
        static IEnumerable<GenericPayloadDto> GetEdgePayload(this Brep brp, SerializationOptions s_opt)
        {

            foreach (var edge in brp.Edges)
            {
                yield return new GenericPayloadDto
                {
                    Data = edge.ToJSON(s_opt),
                };
            }

        }
    }


}
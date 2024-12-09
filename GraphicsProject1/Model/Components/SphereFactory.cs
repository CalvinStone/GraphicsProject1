using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GraphicsProject1.Model.Renderables;
using OpenTK.Mathematics;

namespace GraphicsProject1.Model.Components
{
	// Creates Icosahedrons for spheres that will probably be used for the head
    internal class SphereFactory
    {
		private List<Vector3> points;
		private int index;
		private Dictionary<long, int> middlePointIndexCache;

		private struct TriIndices
		{
			public Vector3 V1;
			public Vector3 V2;
			public Vector3 V3;

			public TriIndices(Vector3 v1, Vector3 v2, Vector3 v3)
			{
				V1 = v1;
				V2 = v2;
				V3 = v3;
			}
		}

		public List<TexturedVertex> Create(int recursionLevel)
		{
			middlePointIndexCache = new Dictionary<long, int>();
			points = new List<Vector3>();
			index = 0;
			var t = (float)((1.0 + Math.Sqrt(5.0)) / 2.0);
			var s = 1;

			AddVertex(new Vector3(-s, t, 0));
			AddVertex(new Vector3(s, t, 0));
			AddVertex(new Vector3(-s, -t, 0));
			AddVertex(new Vector3(s, -t, 0));

			AddVertex(new Vector3(0, -s, t));
			AddVertex(new Vector3(0, s, t));
			AddVertex(new Vector3(0, -s, -t));
			AddVertex(new Vector3(0, s, -t));

			AddVertex(new Vector3(t, 0, -s));
			AddVertex(new Vector3(t, 0, s));
			AddVertex(new Vector3(-t, 0, -s));
			AddVertex(new Vector3(-t, 0, s));

			var TriIndicess = new List<TriIndices>();

			// 5 TriIndicess around point 0
			TriIndicess.Add(new TriIndices(points[0], points[11], points[5]));
			TriIndicess.Add(new TriIndices(points[0], points[5], points[1]));
			TriIndicess.Add(new TriIndices(points[0], points[1], points[7]));
			TriIndicess.Add(new TriIndices(points[0], points[7], points[10]));
			TriIndicess.Add(new TriIndices(points[0], points[10], points[11]));

			// 5 adjacent TriIndicess 
			TriIndicess.Add(new TriIndices(points[1], points[5], points[9]));
			TriIndicess.Add(new TriIndices(points[5], points[11], points[4]));
			TriIndicess.Add(new TriIndices(points[11], points[10], points[2]));
			TriIndicess.Add(new TriIndices(points[10], points[7], points[6]));
			TriIndicess.Add(new TriIndices(points[7], points[1], points[8]));

			// 5 TriIndicess around point 3
			TriIndicess.Add(new TriIndices(points[3], points[9], points[4]));
			TriIndicess.Add(new TriIndices(points[3], points[4], points[2]));
			TriIndicess.Add(new TriIndices(points[3], points[2], points[6]));
			TriIndicess.Add(new TriIndices(points[3], points[6], points[8]));
			TriIndicess.Add(new TriIndices(points[3], points[8], points[9]));

			// 5 adjacent TriIndicess 
			TriIndicess.Add(new TriIndices(points[4], points[9], points[5]));
			TriIndicess.Add(new TriIndices(points[2], points[4], points[11]));
			TriIndicess.Add(new TriIndices(points[6], points[2], points[10]));
			TriIndicess.Add(new TriIndices(points[8], points[6], points[7]));
			TriIndicess.Add(new TriIndices(points[9], points[8], points[1]));



			// refine triangles
			for (int i = 0; i < recursionLevel; i++)
			{
				var TriIndicess2 = new List<TriIndices>();
				foreach (var tri in TriIndicess)
				{
					// replace triangle by 4 triangles
					int a = GetMiddlePoint(tri.V1, tri.V2);
					int b = GetMiddlePoint(tri.V2, tri.V3);
					int c = GetMiddlePoint(tri.V3, tri.V1);

					TriIndicess2.Add(new TriIndices(tri.V1, points[a], points[c]));
					TriIndicess2.Add(new TriIndices(tri.V2, points[b], points[a]));
					TriIndicess2.Add(new TriIndices(tri.V3, points[c], points[b]));
					TriIndicess2.Add(new TriIndices(points[a], points[b], points[c]));
				}
				TriIndicess = TriIndicess2;
			}


			// done, now add triangles to mesh
			var vertices = new List<TexturedVertex>();

			foreach (var tri in TriIndicess)
			{
				var uv1 = GetSphereCoord(tri.V1);
				var uv2 = GetSphereCoord(tri.V2);
				var uv3 = GetSphereCoord(tri.V3);
				// Commented out code is to make non-janky pictures on the sphere
				
				vertices.Add(new TexturedVertex(new Vector4(tri.V1, 1), uv1));
				vertices.Add(new TexturedVertex(new Vector4(tri.V2, 1), uv2));
				vertices.Add(new TexturedVertex(new Vector4(tri.V3, 1), uv3));
				
			}

			return vertices;
		}

		private int AddVertex(Vector3 p)
		{
			points.Add(p.Normalized());
			return index++;
		}

		// Returns index of point between p1 and p2
		private int GetMiddlePoint(Vector3 p1, Vector3 p2)
		{
			long index1 = points.IndexOf(p1);
			long index2 = points.IndexOf(p2);

			var firstIsSmaller = index1 < index2;
			long smallerIndex = firstIsSmaller ? index1 : index2;
			long greaterIndex = firstIsSmaller ? index2 : index1;
			long key = (smallerIndex << 32) + greaterIndex;

			int ret;

			if (middlePointIndexCache.TryGetValue(key, out ret))
			{
				return ret;
			}

			var middle = new Vector3(
				(p1.X + p2.X) / 2.0f,
				(p1.Y + p2.Y) / 2.0f,
				(p1.Z + p2.Z) / 2.0f);

			// Adds vertex to make sure point is on the unit sphere
			int i = AddVertex(middle);

			middlePointIndexCache.Add(key, i);
			return i;
		}

		public static Vector2 GetSphereCoord(Vector3 i)
		{
			var len = i.Length;
			Vector2 uv;
			uv.Y = (float)(Math.Acos(i.Y / len) / Math.PI);
			uv.X = -(float)((Math.Atan2(i.Z, i.X) / Math.PI + 1.0f) * 0.5f);
			return uv;
		}

	}
}

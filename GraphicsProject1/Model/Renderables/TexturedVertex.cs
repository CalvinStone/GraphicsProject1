using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace GraphicsProject1.Model.Renderables
{
	public struct TexturedVertex
	{
		public const int Size = (4 + 2) * 4; // size of struct in bytes

		public readonly Vector4 pos;
		public readonly Vector2 textureCoordinate;

		public TexturedVertex(Vector4 position, Vector2 textureCoordinate)
		{
			pos = position;
			this.textureCoordinate = textureCoordinate;
		}
	}
}

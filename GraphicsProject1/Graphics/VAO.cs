using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace GraphicsProject1.Graphics
{
	internal class VAO
	{
		public int ID;
		public VAO()
		{
			ID = GL.GenVertexArray();
			GL.BindVertexArray(ID);
		}

		public void LinkToVAO(int location, int size, VBO vbo)
		{
			Bind();
			vbo.Bind();
			GL.VertexAttribPointer(location, size, VertexAttribPointerType.Float, false, 0, 0);
			GL.EnableVertexAttribArray(location);
			Unbind();
		}

		// Can be used for both vertex and texture offsets
		public void LinktoVAOInstanced(int location, int size, VBO vbo)
		{
			Bind();
			vbo.Bind();
			GL.VertexAttribPointer(location, size, VertexAttribPointerType.Float, false, 0, 0);
			GL.EnableVertexAttribArray(location);

			GL.VertexBindingDivisor(location, 1);

			Unbind();
		}

		public void Bind() { GL.BindVertexArray(ID); }
		public void Unbind() { GL.BindVertexArray(0); }
		public void Delete() { GL.DeleteVertexArray(ID); }
	}
}

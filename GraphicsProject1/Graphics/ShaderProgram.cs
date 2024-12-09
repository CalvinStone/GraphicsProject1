using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace GraphicsProject1.Graphics
{
	internal class ShaderProgram
	{
		public int ID;
		public ShaderProgram(string vertexShaderFilepath, string fragmentShaderFilepath)
		{
			// Create shader program
			ID = GL.CreateProgram();

			// Create vert shader
			int vertexShader = GL.CreateShader(ShaderType.VertexShader);

			// Use code from Default.vert
			GL.ShaderSource(vertexShader, LoadShaderSource(vertexShaderFilepath));

			// Compile shader
			GL.CompileShader(vertexShader);

			// Do same with Default.frag
			int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
			GL.ShaderSource(fragmentShader, LoadShaderSource(fragmentShaderFilepath));
			GL.CompileShader(fragmentShader);

			// Attach shaders to shader program
			GL.AttachShader(ID, vertexShader);
			GL.AttachShader(ID, fragmentShader);

			// Link program to OpenGL
			GL.LinkProgram(ID);

			// Delete shaders
			GL.DeleteShader(vertexShader);
			GL.DeleteShader(fragmentShader);
		}

		public void Bind() { GL.UseProgram(ID); }
		public void Unbind() { GL.UseProgram(0); }
		public void Delete() { GL.DeleteShader(ID); }

		public static string LoadShaderSource(string filePath)
		{
			string shaderSource = "";

			try
			{
				using (StreamReader reader = new StreamReader("../../../Shaders/" + filePath))
				{
					shaderSource = reader.ReadToEnd();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Failed to load shader source file: " + e.Message);
			}

			return shaderSource;
		}
	}
}

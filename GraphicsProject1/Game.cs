using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using GraphicsProject1.Graphics;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;


namespace GraphicsProject1
{
	internal class Game : GameWindow
	{
		// CONSTANTS
		private static int screenWidth;
		private static int screenHeight;


		Camera camera;

		ShaderProgram shaderProgram;
		RenderScene renderScene;



		

		public Game(int width, int height) : base(GameWindowSettings.Default, CreateWindowSettings(width, height))
		{
			this.CenterWindow(new Vector2i(width, height));
			screenWidth = width;
			screenHeight = height;

			CenterWindow(new Vector2i(screenWidth, screenHeight));
		}

		// Needed for antialiasing
		private static NativeWindowSettings CreateWindowSettings(int width, int height)
		{
			// Configure NativeWindowSettings to enable MSAA
			return new NativeWindowSettings
			{
				Size = new Vector2i(width, height),
				Title = "Stretchy Temoc",
				NumberOfSamples = 4,
				API = ContextAPI.OpenGL,
				Profile = ContextProfile.Core,
				APIVersion = new Version(4, 1)
			};
		}

		protected override void OnResize(ResizeEventArgs e)
		{
			base.OnResize(e);
			GL.Viewport(0, 0, e.Width, e.Height);
			screenWidth = e.Width;
			screenHeight = e.Height;
		}

		protected override void OnLoad()
		{
			base.OnLoad();
			Console.WriteLine("OnLoad");

			// Enable Multisampling
			GL.Enable(EnableCap.Multisample);

			// Removes framerate cap
			VSync = VSyncMode.Off;
			// Needed to favor objects that are closer
			GL.Enable(EnableCap.DepthTest);

			// Auto Loads up textures and models
			renderScene = new RenderScene(screenWidth, screenHeight);


			shaderProgram = new ShaderProgram("Default.vert", "Default.frag");
			camera = new Camera(screenWidth, screenHeight, new Vector3(0f, 17.5f, 6.0f));
			//CursorState = CursorState.Grabbed;	
		}

		/*
		protected override void OnRenderFrame(FrameEventArgs args)
		{
			base.OnRenderFrame(args);
			// Sets screen color
			GL.ClearColor(0.3f, 0.3f, 1.0f, 1.0f);
			// Fill screen with the color
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			shaderProgram.Bind();


			// TF matrices
			Matrix4 model = Matrix4.Identity;
			Matrix4 view = camera.GetViewMatrix();
			Matrix4 projection = camera.GetProjectionMatrix();

			int modelLocation = GL.GetUniformLocation(shaderProgram.ID, "model");
			int viewLocation = GL.GetUniformLocation(shaderProgram.ID, "view");
			int projectionLocation = GL.GetUniformLocation(shaderProgram.ID, "projection");
			int boneTransformsLocation = GL.GetUniformLocation(shaderProgram.ID, "boneTransforms");
			int useBonesLocation = GL.GetUniformLocation(shaderProgram.ID, "useBones");

			GL.UniformMatrix4(modelLocation, true, ref model);
			GL.UniformMatrix4(viewLocation, true, ref view);
			GL.UniformMatrix4(projectionLocation, true, ref projection);
			GL.Uniform1(useBonesLocation, 1);

			Matrix4[] boneTransforms = renderScene.factory.boneTransforms;
			//GL.UniformMatrix4(boneTransformsLocation, boneTransforms.Length, true, ref boneTransforms[]);
			GL.UniformMatrix4(boneTransformsLocation, true, ref boneTransforms[0]);

			renderScene.RenderHead(shaderProgram);


			Context.SwapBuffers();

			base.OnRenderFrame(args);
		}
		*/
		
		protected override void OnRenderFrame(FrameEventArgs args)
		{
			base.OnRenderFrame(args);
			// Sets screen color
			GL.ClearColor(0.3f, 0.3f, 1.0f, 1.0f);
			// Fill screen with the color
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			shaderProgram.Bind();


			// TF matrices
			Matrix4 model = Matrix4.Identity;
			Matrix4 view = camera.GetViewMatrix();
			Matrix4 projection = camera.GetProjectionMatrix();

			int modelLocation = GL.GetUniformLocation(shaderProgram.ID, "model");
			int viewLocation = GL.GetUniformLocation(shaderProgram.ID, "view");
			int projectionLocation = GL.GetUniformLocation(shaderProgram.ID, "projection");

			GL.UniformMatrix4(modelLocation, true, ref model);
			GL.UniformMatrix4(viewLocation, true, ref view);
			GL.UniformMatrix4(projectionLocation, true, ref projection);

			renderScene.RenderHead(shaderProgram);
			renderScene.RenderEyes(shaderProgram);


			Context.SwapBuffers();

			base.OnRenderFrame(args);
		}
		



		protected override void OnUpdateFrame(FrameEventArgs args)
		{
			MouseState mouse = MouseState;
			KeyboardState keyboard = KeyboardState;

			camera.Update(keyboard, mouse, args);

			if (mouse.IsButtonDown(MouseButton.Left) )
			{
				Matrix4 tempView = camera.GetViewMatrix();
				Matrix4 tempProjection = camera.GetProjectionMatrix();

				renderScene.Update(mouse, tempView, tempProjection); // handles raycasting and updates model
			}

			base.OnUpdateFrame(args);
		}


		protected override void OnUnload()
		{
			base.OnUnload();
			shaderProgram.Delete();
			renderScene.Delete();
			//vao.Delete();
			//ibo.Delete();
		}
	}
}

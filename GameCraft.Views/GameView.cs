using System;
using System.Drawing;
using Foundation;
using AppKit;
using CoreGraphics;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenGL;

namespace GameCraft.Views
{
	public class GameView : OpenTK.Platform.MacOS.MonoMacGameView
	{
		[Export("initWithFrame:")]
		public GameView(CGRect frame) : this(frame, null)
		{
		}

		public GameView(CGRect frame, NSOpenGLContext context) : base(frame, context)
        {
			Console.WriteLine("GameView INIT");
			Resize += delegate {
				Console.WriteLine(Bounds);
				ResizeGL(Bounds);
			};
			Load += delegate(object src, EventArgs args)
			{
				InitGL();
				GL.Viewport(0, 0, (int)Bounds.Size.Width, (int)Bounds.Size.Height);
				UpdateView();
			};

			RenderFrame += delegate (object src, FrameEventArgs args)
			{
				if (this.OpenGLContext.View == null)
                {
					this.OpenGLContext.View = this;
                }

				// render scene
				GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			};
        }

		private void InitGL()
		{
			// Enables Smooth Shading  
			GL.ShadeModel(ShadingModel.Smooth);
			// Set background color to black     
			GL.ClearColor(Color.Black);

			// Setup Depth Testing

			// Depth Buffer setup
			GL.ClearDepth(1.0);
			// Enables Depth testing
			GL.Enable(EnableCap.DepthTest);
			// The type of depth testing to do
			GL.DepthFunc(DepthFunction.Lequal);

			// Really Nice Perspective Calculations
			GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
		}

		private void ResizeGL(CGRect bounds)
		{
			// Reset The Current Viewport
			GL.Viewport(0, 0, (int)bounds.Size.Width, (int)bounds.Size.Height);
			// Select The Projection Matrix
			GL.MatrixMode(MatrixMode.Projection);
			// Reset The Projection Matrix
			GL.LoadIdentity();

			// Set perspective here - Calculate The Aspect Ratio Of The Window
			Perspective(45, bounds.Size.Width / bounds.Size.Height, 0.1, 100);

			// Select The Modelview Matrix
			GL.MatrixMode(MatrixMode.Modelview);
			// Reset The Modelview Matrix
			GL.LoadIdentity();
		}

		// This creates a symmetric frustum.
		// It converts to 6 params (l, r, b, t, n, f) for glFrustum()
		// from given 4 params (fovy, aspect, near, far)
		public static void Perspective(double fovY, double aspectRatio, double front, double back)
		{
			const
			double DEG2RAD = Math.PI / 180;

			// tangent of half fovY
			double tangent = Math.Tan(fovY / 2 * DEG2RAD);

			// half height of near plane
			double height = front * tangent;

			// half width of near plane
			double width = height * aspectRatio;

			// params: left, right, bottom, top, near, far
			GL.Frustum(-width, width, -height, height, front, back);
		}

	}
}


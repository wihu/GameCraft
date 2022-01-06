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
		float rtri; // Angle For The Triangle 
		float rquad; // Angle For The Quad

		[Export("initWithFrame:")]
		public GameView(CGRect frame) : this(frame, null)
		{
		}

		public GameView(CGRect frame, NSOpenGLContext context) : base(frame, context)
        {
			Console.WriteLine("GameView INIT");
			Resize += delegate {
				ResizeGL(Bounds);
            };
			Load += delegate(object src, EventArgs args)
			{
				InitGL();
				UpdateView();
			};

			RenderFrame += delegate (object src, FrameEventArgs args)
			{
				if (this.OpenGLContext.View == null)
                {
					Console.WriteLine("Render");
					this.OpenGLContext.View = this;
                }

				// render scene
				DrawScene();
			};
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
			Console.WriteLine("Resize");
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
		private static void Perspective(double fovY, double aspectRatio, double front, double back)
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

		private bool DrawScene()
		{
			// Clear The Screen And The Depth Buffer
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			// Reset The Current Modelview Matrix
			GL.LoadIdentity();

			GL.Translate(-1.5f, 0.0f, -6.0f);
			// Move Left 1.5 Units And Into The Screen 6.0
			GL.Rotate(rtri, 0.0f, 1.0f, 0.0f);
			// Rotate The Triangle On The Y axis
			GL.Begin(BeginMode.Triangles);      // Start drawing the Pyramid

			GL.Color3(1.0f, 0.0f, 0.0f);            // Red
			GL.Vertex3(0.0f, 1.0f, 0.0f);           // Top Of Triangle (Front)
			GL.Color3(0.0f, 1.0f, 0.0f);            // Green
			GL.Vertex3(-1.0f, -1.0f, 1.0f);         // Left Of Triangle (Front)
			GL.Color3(0.0f, 0.0f, 1.0f);            // Blue
			GL.Vertex3(1.0f, -1.0f, 1.0f);          // Right Of Triangle (Front)

			GL.Color3(1.0f, 0.0f, 0.0f);            // Red
			GL.Vertex3(0.0f, 1.0f, 0.0f);           // Top Of Triangle (Right)
			GL.Color3(0.0f, 0.0f, 1.0f);            // Blue
			GL.Vertex3(1.0f, -1.0f, 1.0f);          // Left Of Triangle (Right)
			GL.Color3(0.0f, 1.0f, 0.0f);            // Green
			GL.Vertex3(1.0f, -1.0f, -1.0f);         // Right Of Triangle (Right)

			GL.Color3(1.0f, 0.0f, 0.0f);            // Red
			GL.Vertex3(0.0f, 1.0f, 0.0f);           // Top Of Triangle (Back)
			GL.Color3(0.0f, 1.0f, 0.0f);            // Green
			GL.Vertex3(1.0f, -1.0f, -1.0f);         // Left Of Triangle (Back)
			GL.Color3(0.0f, 0.0f, 1.0f);            // Blue
			GL.Vertex3(-1.0f, -1.0f, -1.0f);            // Right Of Triangle (Back)			

			GL.Color3(1.0f, 0.0f, 0.0f);            // Red
			GL.Vertex3(0.0f, 1.0f, 0.0f);           // Top Of Triangle (Left)
			GL.Color3(0.0f, 0.0f, 1.0f);            // Blue
			GL.Vertex3(-1.0f, -1.0f, -1.0f);            // Left Of Triangle (Left)
			GL.Color3(0.0f, 1.0f, 0.0f);            // Green
			GL.Vertex3(-1.0f, -1.0f, 1.0f);         // Right Of Triangle (Left)

			GL.End();                       // Finished Drawing The Pyramid

			// Reset The Current Modelview Matrix
			GL.LoadIdentity();

			GL.Translate(1.5f, 0.0f, -7.0f);            // Move Right 1.5 Units And Into The Screen 7.0
			GL.Rotate(rquad, 1.0f, 0.0f, 0.0f);         // Rotate The Quad On The X axis ( NEW )   
			GL.Begin(BeginMode.Quads);              // Start Drawing Cube
			GL.Color3(0.0f, 1.0f, 0.0f);            // Set The Color To Green
			GL.Vertex3(1.0f, 1.0f, -1.0f);          // Top Right Of The Quad (Top)
			GL.Vertex3(-1.0f, 1.0f, -1.0f);     // Top Left Of The Quad (Top)
			GL.Vertex3(-1.0f, 1.0f, 1.0f);          // Bottom Left Of The Quad (Top)
			GL.Vertex3(1.0f, 1.0f, 1.0f);           // Bottom Right Of The Quad (Top)                        

			GL.Color3(1.0f, 0.5f, 0.0f);            // Set The Color To Orange
			GL.Vertex3(1.0f, -1.0f, 1.0f);          // Top Right Of The Quad (Bottom)
			GL.Vertex3(-1.0f, -1.0f, 1.0f);     // Top Left Of The Quad (Bottom)
			GL.Vertex3(-1.0f, -1.0f, -1.0f);        // Bottom Left Of The Quad (Bottom)
			GL.Vertex3(1.0f, -1.0f, -1.0f);         // Bottom Right Of The Quad (Bottom)

			GL.Color3(1.0f, 0.0f, 0.0f);            // Set The Color To Red
			GL.Vertex3(1.0f, 1.0f, 1.0f);           // Top Right Of The Quad (Front)
			GL.Vertex3(-1.0f, 1.0f, 1.0f);          // Top Left Of The Quad (Front)
			GL.Vertex3(-1.0f, -1.0f, 1.0f);         // Bottom Left Of The Quad (Front)
			GL.Vertex3(1.0f, -1.0f, 1.0f);          // Bottom Right Of The Quad (Front)	

			GL.Color3(1.0f, 1.0f, 0.0f);            // Set The Color To Yellow
			GL.Vertex3(1.0f, -1.0f, -1.0f);     // Bottom Left Of The Quad (Back)
			GL.Vertex3(-1.0f, -1.0f, -1.0f);        // Bottom Right Of The Quad (Back)
			GL.Vertex3(-1.0f, 1.0f, -1.0f);     // Top Right Of The Quad (Back)
			GL.Vertex3(1.0f, 1.0f, -1.0f);      // Top Left Of The Quad (Back)

			GL.Color3(0.0f, 0.0f, 1.0f);            // Set The Color To Blue
			GL.Vertex3(-1.0f, 1.0f, 1.0f);          // Top Right Of The Quad (Left)
			GL.Vertex3(-1.0f, 1.0f, -1.0f);         // Top Left Of The Quad (Left)
			GL.Vertex3(-1.0f, -1.0f, -1.0f);            // Bottom Left Of The Quad (Left)
			GL.Vertex3(-1.0f, -1.0f, 1.0f);         // Bottom Right Of The Quad (Left)

			GL.Color3(1.0f, 0.0f, 1.0f);            // Set The Color To Violet
			GL.Vertex3(1.0f, 1.0f, -1.0f);          // Top Right Of The Quad (Right)
			GL.Vertex3(1.0f, 1.0f, 1.0f);           // Top Left Of The Quad (Right)
			GL.Vertex3(1.0f, -1.0f, 1.0f);          // Bottom Left Of The Quad (Right)
			GL.Vertex3(1.0f, -1.0f, -1.0f);         // Bottom Right Of The Quad (Right)			

			GL.End();               // Done Drawing the Cube

			rtri += 2f;             // Increase The Rotation Variable For The Triangle
			rquad -= 0.15f;             // Decrease The Rotation Variable For The Quad 
			return true;
		}
	}
}

using System;
using Foundation;
using AppKit;
using CoreGraphics;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform.MacOS;
using OpenGL;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GameCraft.Platform.Mac
{
	public class GameView : MonoMacGameView, IDisposable, IGraphicsDeviceService
	{
		public GraphicsDevice GraphicsDevice => _graphicsDevice;

		public event EventHandler<EventArgs> DeviceCreated;
		public event EventHandler<EventArgs> DeviceDisposing;
		public event EventHandler<EventArgs> DeviceReset;
		public event EventHandler<EventArgs> DeviceResetting;

		private bool _isPaused;
		private Viewport _viewport;
		private GraphicsDevice _graphicsDevice;
		private GameServiceContainer _services;
		private ContentManager _content;
		private SpriteBatch _spriteBatch;
		private Texture2D _background;

		[Export("initWithFrame:")]
		public GameView(CGRect frame) : this(frame, null)
		{
		}

		public GameView(CGRect frame, NSOpenGLContext context) : base(frame, context)
        {
			Initialize();
        }

		~GameView()
		{
			Dispose(false);
		}

        public void Show()
        {
            Run();
			// must be after starting CVDisplayLink?
			InitializeGraphicsDevice();
        }

        public override void ViewDidMoveToWindow()
        {
			base.ViewDidMoveToWindow();

			//Initialize();
			NSNotificationCenter.DefaultCenter.AddObserver(NSWindow.WillStartLiveResizeNotification, (n) =>
			{
				Console.WriteLine("Start resize");
				Stop();
				_isPaused = true;
			});
			NSNotificationCenter.DefaultCenter.AddObserver(NSWindow.DidEndLiveResizeNotification, (n) =>
			{
				Console.WriteLine("End resize");
				if (_isPaused)
                {
					Run();
					_isPaused = false;
				}
			});
		}

        protected override void Dispose(bool disposing)
        {
			Console.WriteLine("Disposing GameView = " + disposing);
			NSNotificationCenter.DefaultCenter.RemoveObserver(NSWindow.WillStartLiveResizeNotification);
			NSNotificationCenter.DefaultCenter.RemoveObserver(NSWindow.DidEndLiveResizeNotification);
		}

        void HandleResize(object src, EventArgs args)
        {
			if (_graphicsDevice == null)
            {
				return;
            }
			Console.WriteLine("Resize" + this.Size);
			if (this.Size.Width != _viewport.Width || this.Size.Height != _viewport.Height)
            {
				_graphicsDevice.PresentationParameters.BackBufferWidth = _viewport.Width = this.Size.Width;
				_graphicsDevice.PresentationParameters.BackBufferHeight = _viewport.Height = this.Size.Height;
				_graphicsDevice.Viewport = _viewport;
			}
        }

		void Initialize()
        {
			Console.WriteLine("GameView INIT");

			Resize += HandleResize;
			Load += delegate (object src, EventArgs args)
			{
				UpdateView();
			};

			RenderFrame += delegate (object src, FrameEventArgs args)
			{
				if (this.OpenGLContext.View == null)
				{
					Console.WriteLine("Render");
					this.OpenGLContext.View = this;
				}

				RenderScene();
			};
		}

		void InitializeGraphicsDevice()
		{
			// init graphics device
			_viewport = new Viewport(0, 0, this.Size.Width, this.Size.Height);

			var presParams = new PresentationParameters();
			presParams.BackBufferWidth = _viewport.Width;
			presParams.BackBufferHeight = _viewport.Height;

			_graphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, GraphicsProfile.Reach, presParams);

			_services = new GameServiceContainer();
			_services.AddService<IGraphicsDeviceService>(this);
			_content = new ContentManager(_services, "Content");
			_spriteBatch = new SpriteBatch(_graphicsDevice);
			_background = _content.Load<Texture2D>("Layer1_0");
		}

		void RenderScene()
        {
			if (_graphicsDevice == null)
			{
				return;
			}
			_graphicsDevice.Clear(Color.CornflowerBlue);
			_spriteBatch.Begin();
			var rect = new Rectangle(0, 0, _viewport.Width, _viewport.Height);
			_spriteBatch.Draw(_background, rect, Color.White);
			_spriteBatch.End();
		}
	}
}

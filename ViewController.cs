using System;

using AppKit;
using Foundation;
using GameCraft.Views;

namespace GameCraftMac
{
	public partial class ViewController : NSViewController
	{
		private GameView _gameView;

        public ViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Do any additional setup after loading the view.
			_gameView = new GameCraft.Views.GameView(View.Bounds);
			_gameView.Frame = View.Frame;
			View.AddSubview(_gameView);

			_gameView.Run();
		}

        public override void ViewDidLayout()
        {
			base.ViewDidLayout();

			if (_gameView != null)
			{
				_gameView.Frame = View.Frame;
			}
		}

        public override void ViewDidDisappear()
        {
			base.ViewDidDisappear();

			if (_gameView != null)
			{
				_gameView.Dispose();
				_gameView = null;
			}
        }

        public override NSObject RepresentedObject {
			get {
				return base.RepresentedObject;
			}
			set {
				base.RepresentedObject = value;
				// Update the view, if already loaded.
			}
		}
	}
}

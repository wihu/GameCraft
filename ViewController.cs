using System;

using AppKit;
using Foundation;

namespace GameCraftMac
{
	public partial class ViewController : NSViewController
	{
		public ViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Do any additional setup after loading the view.
			var gameView = new GameCraft.Views.GameView(View.Bounds);
			gameView.Frame = View.Frame;
			View.AddSubview(gameView);

			gameView.Run();
		}

        public override void ViewDidLayout()
        {
			Console.WriteLine(View.Frame);
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

using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace GraphicsProject1
{

	class Program
	{
		static void Main(string[] args)
		{
			using (Game game = new Game(1280, 720))
			{
				game.Run();
			}
		}
	}
}

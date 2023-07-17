using System.Globalization;

namespace FPcheck
{
	internal class Program
	{
		static List<string> files_paths = new();
		static int max_y = 0;
		static int a = 0;
		static void Main(string[] args)
		{
			Console.WriteLine("Getting paths...");
			files_paths.AddRange(File.ReadAllLines("paths.txt"));
			Console.Clear();
			string folderpath = " ";
			if (args.Length == 0)
			{
				Console.WriteLine("Enter path to folder:");
				folderpath = Console.ReadLine();
			}
			else
			{
				folderpath = args[0];
			}
			a = folderpath.Length;
			Console.WriteLine(folderpath);
			checkallfiles(folderpath, 0);
			Console.ResetColor();
			while (Console.ReadLine() != "q") { }
			//findallfiles(@"C:\Program Files (x86)\S.T.A.L.K.E.R. Shadow of Chernobyl\gamedataUE", 0);
			//File.WriteAllLines("paths.txt", files_paths);
		}
		static void checkallfiles(string path, int tabs)
		{
			if (max_y < Console.CursorTop) max_y = Console.CursorTop;
			Console.ForegroundColor = ConsoleColor.White;
			
			string predir = "";
			if(tabs > 0)
			{
				for (int i = 0; i < tabs-1; i++)
				{
					predir += "|   ";
				}
				predir += '└' + new string('-', 3);
			}
			string prefile = "";
			for (int i = 0; i < tabs; i++)
			{
				prefile += "|   ";
			}
			prefile += '├' + new string('-', tabs * 4 + 3);
			Console.WriteLine(predir + path.Split('\\').Last());

			foreach (string folder in Directory.GetDirectories(path))
			{

				checkallfiles(folder, tabs + 1);
			}
			foreach (string file in Directory.GetFiles(path))
			{

				Console.ForegroundColor = ConsoleColor.White;
				if (file == Directory.GetFiles(path).Last())
				{
					prefile = "";
					for (int i = 0; i < tabs; i++)
					{
						prefile += "|   ";
					}
					prefile += '└' + new string('-', tabs * 4 + 3);
				}
				Console.Write(prefile);

				Console.ForegroundColor = (checkfile(file)) ? ConsoleColor.Green : ConsoleColor.Red ;
				Console.WriteLine(Path.GetFileName(file));
				files_paths.Add(file);
			}

		}
		static bool checkfile(string filepath)
		{
			foreach(string path in files_paths)
			{
				string relfile = filepath.Substring(a);
				if (filepath.Substring(a+1) == path) return true;
			}
			return false;
		}
		static void findallfiles(string path, int tabs)
		{
			if (max_y < tabs) max_y = tabs;
			Console.SetCursorPosition(4 * tabs, tabs);
			Console.Write(path.Split('\\').Last());
			foreach (string folder in Directory.GetDirectories(path))
			{

				findallfiles(folder, tabs + 1);
			}
			foreach (string file in Directory.GetFiles(path))
			{
				files_paths.Add(file);
			}

			//Console.SetCursorPosition(0, 0);
			//Console.Write(files_paths.Count);
		}
	}
}
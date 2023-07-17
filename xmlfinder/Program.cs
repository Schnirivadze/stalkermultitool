using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace xmlfinder
{
	internal class Program
	{

		static bool found = false;
		static string foundstr = "";
		static List<string> files_paths = new();
		static void Main(string[] args)
		{
			#region preparation
			Console.OutputEncoding = Encoding.UTF8;

			var writer = new StringWriter();
			Console.SetOut(writer);
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			string addpath = "";
			string[] search = File.ReadAllLines("res\\search.txt");
			bool write_line_from_file = true;
			File.WriteAllText("res\\result.txt", "");
			#endregion
			#region read settings
			foreach (string setting in File.ReadAllLines("res\\settings.txt"))
			{
				string[] parts = setting.Split(' ');
				string setting_name = parts[0];
				if (setting_name == "config-path") addpath = setting.Remove(0, setting_name.Length + 1); 
				if (setting_name == "write-line-from-file") write_line_from_file = (setting.Remove(0, setting_name.Length + 1)=="true")? true:false; 
			}
			#endregion
			#region check 
			if (addpath.Length == 0)
			{
				Console.WriteLine("ERROR: config-path IS NOT SET, CHECK SETTINGS.TXT");
				return;
			}
			if (search.Length == 0)
			{
				Console.WriteLine("ERROR: search.txt IS EMPTY");
				return;
			}
			#endregion
			#region search
			#region pathes
			if (File.Exists("res\\paths.txt") && File.ReadAllText("res\\paths.txt").Length > 0)
			{
				files_paths.AddRange(File.ReadAllLines("res\\paths.txt"));
			}
			else
			{
				Console.WriteLine("Making file tree");
				findallfiles(addpath);
				File.WriteAllLines("res\\paths.txt", files_paths);
			}
			#endregion
			foreach (string query in search)
			{
				Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>" + query);
				foreach (string path in files_paths)
				{
					findinfile(path, query, write_line_from_file);
				}

				if (foundstr.Length > 0)
				{

					File.AppendAllText("res\\result.txt", "\n------------------------------------|" + query + "|------------------------------------" + foundstr);
					foundstr = "";
				}
				else
				{
					foundstr = "";
					File.AppendAllText("res\\result.txt", "\n------------------------------------|" + query + " HAS NOT BEEN FOUND|------------------------------------");

				}

			}
			#endregion
			#region End

			File.WriteAllText("res\\log.txt", writer.ToString());
			Process.Start("notepad.exe", Path.GetFullPath("res\\result.txt"));
			#endregion

		}
		static void findallfiles(string path)
		{
			foreach (string folder in Directory.GetDirectories(path))
			{

				findallfiles(folder);
			}
			foreach (string file in Directory.GetFiles(path))
			{
				files_paths.Add(file);
			}
		}
		static void findinfile(string path, string search, bool write_line_from_file)
		{
			string[] filetext = File.ReadAllLines(path, Encoding.GetEncoding("windows-1251"));
			bool added = false;
			for (int i = 0; i < filetext.Length; i++)
			{
				if (filetext[i].Length > 0 && filetext[i].Contains(search))
				{
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine("found in file "+path);
					Console.ResetColor();
					if (!added)foundstr += ($"\n{path}");
					if (write_line_from_file) foundstr += $"\n\t|->{filetext[i]}<------line {i + 1}";
					else foundstr += $"\n\t|->line {i + 1}";
				}
			}

		}
	}
}
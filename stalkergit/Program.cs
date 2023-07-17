using System.Diagnostics;
using System.Text.RegularExpressions;

public class Program
{
	public static void Main(string[] args)
	{
		File.WriteAllText("logs.txt", "");
		string[] oldoggFiles = Directory.GetFiles("old", "*.ogg");
		string[] newoggFiles = Directory.GetFiles("new", "*.ogg");
		string[] oggFiles = (oldoggFiles.Length > newoggFiles.Length) ? oldoggFiles : newoggFiles;
		StreamWriter sw = File.AppendText("logs.txt");
		sw.WriteLine($"Files [{oggFiles.Length}]");
		foreach (string oggFile in oggFiles)
		{

			if (oggFile == oggFiles.Last()) sw.WriteLine($"└-------{oggFile.Remove(0, 4)}");
			else sw.WriteLine($"├-------{oggFile.Remove(0, 4)}");
		}
		sw.Write("\n\n\n");

		foreach (string oggFile in oggFiles)
		{
			sw.WriteLine("\nFile>---- " + oggFile.Remove(0, 4));
			string oldFilePath = Path.Combine("old", Path.GetFileName(oggFile));
			if (File.Exists(oldFilePath)) sw.WriteLine("├----> File exists in folder old");
			else { sw.WriteLine("├----> File DOES NOT EXIST in folder old"); goto err; }
			string newFilePath = Path.Combine("new", Path.GetFileName(oggFile));
			if (File.Exists(newFilePath)) sw.WriteLine("├----> File exists in folder new");
			else { sw.WriteLine("├----> File DOES NOT EXIST in folder new"); goto err; }
		

			// Run OggComment for the old file and capture its output
			string output = "";
			try
			{
				output = RunOggComment(oldFilePath);
				sw.WriteLine("├----> Output of old file received");
			}
			catch
			{
				sw.WriteLine("├----> Output of old file WAS NOT received");
				goto err;
			}
			// Extract the desired values from the output
			string values = "";
			try
			{

				 values = ExtractValues(output);
				sw.WriteLine("├----> Values of old file extracted");
			}
			catch
			{

				sw.WriteLine("├----> Values of old file WERE NOT extracted");
			}

			// Run OggComment for the new file, passing the extracted values as input
			try
			{
				RunOggComment(newFilePath, values);
				sw.WriteLine("└----> Values of old file writen in new file");
			}
			catch
			{
				sw.WriteLine("├----> Values of old file WERE NOT writen in new file");
				throw;
			}
			
			continue;
		err:
			
			sw.WriteLine("└----------------FOUND EXCEPTION, FILE IS SKIPPED-----------------");
		}
		sw.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>Program finished<<<<<<<<<<<<<<<<<<<<<<<<<<");
		sw.Close();
	}
	
	private static string RunOggComment(string filePath, string input = "")
	{
		// Create a new process instance
		Process process = new Process();

		// Set the filename and arguments of the OggComment executable
		process.StartInfo.FileName = "OggComment.exe";
		process.StartInfo.Arguments = filePath;

		// Redirect the standard output and input
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.RedirectStandardInput = true;

		// Set the CreateNoWindow and UseShellExecute properties to false for redirection to work
		process.StartInfo.CreateNoWindow = true;
		process.StartInfo.UseShellExecute = false;

		// Start the process
		process.Start();

		// Pass input to the process
		process.StandardInput.WriteLine(input);
		process.StandardInput.Flush();
		process.StandardInput.Close();

		// Read the output synchronously
		string output = process.StandardOutput.ReadToEnd();

		// Wait for the process to exit
		process.WaitForExit();

		return output;
	}

	private static string ExtractValues(string output)
	{
		// Use regular expressions to extract the desired values
		string minDistancePattern = @"Min distance\s+: (\d+,\d+)";
		string maxDistancePattern = @"Max distance\s+: (\d+,\d+)";
		string baseVolumePattern = @"Base volume\s+: (\d+,\d+)";
		string maxAIDistancePattern = @"Max AI distance\s+: (\d+,\d+)";
		string soundTypePattern = @"Sound type\s+: .*\[(\d+)\]";

		string minDistance = Regex.Match(output, minDistancePattern).Groups[1].Value;
		string maxDistance = Regex.Match(output, maxDistancePattern).Groups[1].Value;
		string baseVolume = Regex.Match(output, baseVolumePattern).Groups[1].Value;
		string maxAIDistance = Regex.Match(output, maxAIDistancePattern).Groups[1].Value;
		string soundType = Regex.Match(output, soundTypePattern).Groups[1].Value;


		return $"{minDistance}.{maxDistance}.{baseVolume}.{maxAIDistance}.{soundType}";
	}
}

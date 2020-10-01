using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;
using krnl_bootstrapper_v3.Properties;

namespace krnl_bootstrapper_v3
{
	// Token: 0x02000002 RID: 2
	internal class Program
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public static Dictionary<string, string> getHashes()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (string text in new WebClient().DownloadStringTaskAsync("http://cdn.krnl.rocks:8080/hashes").GetAwaiter().GetResult().Split(new char[]
			{
				','
			}))
			{
				dictionary.Add(text.Split(new char[]
				{
					'='
				})[0], text.Split(new char[]
				{
					'='
				})[1]);
			}
			return dictionary;
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000020D2 File Offset: 0x000002D2
		public static Task<byte[]> downloadFile(string name)
		{
			return new WebClient().DownloadDataTaskAsync("http://cdn.krnl.rocks:8080/files/" + name);
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000020EC File Offset: 0x000002EC
		public static void extractZip(string path)
		{
			if (path.EndsWith("Monaco.zip"))
			{
				try
				{
					Directory.Delete("bin/Monaco", true);
				}
				catch
				{
				}
				ZipFile.ExtractToDirectory("bin/Monaco.zip", "bin");
				return;
			}
			if (path.EndsWith("src.7z"))
			{
				try
				{
					Directory.Delete("bin/src", true);
				}
				catch
				{
				}
				Process.Start(new ProcessStartInfo
				{
					WindowStyle = ProcessWindowStyle.Hidden,
					WorkingDirectory = Directory.GetCurrentDirectory(),
					FileName = "7za.exe",
					Arguments = string.Concat(new string[]
					{
						"x \"",
						Path.Combine(Directory.GetCurrentDirectory(), "bin", "src.7z"),
						"\" -y -o\"",
						Path.Combine(Directory.GetCurrentDirectory(), "bin"),
						"\""
					})
				}).WaitForExit();
			}
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000021E4 File Offset: 0x000003E4
		private static void Main(string[] args)
		{
			Program.<>c__DisplayClass3_0 CS$<>8__locals1 = new Program.<>c__DisplayClass3_0();
			AppDomain.CurrentDomain.UnhandledException += Program.CurrentDomain_UnhandledException;
			ServicePointManager.Expect100Continue = true;
			ServicePointManager.SecurityProtocol = (SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12);
			Console.Title = "Krnl Bootstrapper (v3)";
			Process[] processesByName = Process.GetProcessesByName("RobloxPlayerBeta");
			Process[] processesByName2 = Process.GetProcessesByName("krnlss");
			Console.WriteLine("Checking if bootstrapper is up-to-date...");
			byte[] result = new WebClient().DownloadDataTaskAsync("http://cdn.krnl.rocks:8080/bootstrapper").GetAwaiter().GetResult();
			Console.WriteLine("-----------------------------------------");
			try
			{
				File.Delete("krnl_bootstrapper_v3.bin");
			}
			catch
			{
			}
			try
			{
				string[] files = Directory.GetFiles(Environment.CurrentDirectory);
				string sourceFileName = "";
				for (int m = 0; m < files.Length; m++)
				{
					if (Process.GetCurrentProcess().MainModule.FileName == files[m])
					{
						sourceFileName = files[m];
					}
					else if (files[m].IndexOf("\\krnl_bootstrapper_v3") != -1)
					{
						File.Delete(files[m]);
					}
				}
				File.Move(sourceFileName, "krnl_bootstrapper_v3.exe");
				Task.Delay(1000).GetAwaiter().GetResult();
			}
			catch
			{
			}
			FileStream fileStream = File.OpenRead("krnl_bootstrapper_v3.exe");
			if (BitConverter.ToString(MD5.Create().ComputeHash(fileStream)).ToLowerInvariant() != BitConverter.ToString(MD5.Create().ComputeHash(result)).ToLowerInvariant())
			{
				Console.WriteLine("Restarting bootstrapper to the newest version.");
				Task.Delay(1000).GetAwaiter().GetResult();
				fileStream.Close();
				File.Move("krnl_bootstrapper_v3.exe", "krnl_bootstrapper_v3.bin");
				File.WriteAllBytes("krnl_bootstrapper_v3.exe", result);
				Task.Delay(500).GetAwaiter().GetResult();
				Process.Start("krnl_bootstrapper_v3.exe");
				Environment.Exit(0);
			}
			fileStream.Close();
			if (processesByName.Length != 0)
			{
				MessageBox.Show("In order to prevent any common errors, ROBLOX will be forcefully closed.", "Closing ROBLOX", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				for (int j = 0; j < processesByName.Length; j++)
				{
					try
					{
						if (!processesByName[j].HasExited)
						{
							processesByName[j].Kill();
						}
					}
					catch
					{
					}
				}
			}
			for (int k = 0; k < processesByName2.Length; k++)
			{
				try
				{
					if (!processesByName2[k].HasExited)
					{
						processesByName2[k].Kill();
					}
				}
				catch
				{
				}
			}
			Process.Start(new ProcessStartInfo
			{
				FileName = "ipconfig",
				UseShellExecute = true,
				Arguments = "/flushdns",
				WindowStyle = ProcessWindowStyle.Hidden
			});
			try
			{
				string text = File.ReadAllText("C:\\Windows\\System32\\drivers\\etc\\hosts");
				if (text.IndexOf("kanker.rocks") != -1 || text.IndexOf("krnl.rocks") != -1)
				{
					if (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
					{
						MessageBox.Show("Doing this will allow the bootstrapper to remove a file that tampered with the connection between the `client` and `krnl.rocks`.", "Requesting for Administrative Rights.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						Process.Start(new ProcessStartInfo
						{
							FileName = "krnl_bootstrapper_v3.exe",
							Verb = "runas"
						});
						Environment.Exit(0);
					}
					string[] array = text.Split(new char[]
					{
						'\n'
					});
					string text2 = "";
					for (int l = 0; l < array.Length; l++)
					{
						if (array[l].IndexOf("kanker.rocks") == -1 && array[l].IndexOf("krnl.rocks") == -1)
						{
							if (text2 != "")
							{
								text2 += "\n";
							}
							text2 += array[l];
						}
					}
					File.WriteAllText("C:\\Windows\\System32\\drivers\\etc\\hosts", text2);
					Process.Start("krnl_bootstrapper_v3.exe");
					Environment.Exit(0);
				}
			}
			catch
			{
			}
			if (Environment.CurrentDirectory.Split(new char[]
			{
				'\\'
			}).Last<string>() != "krnl")
			{
				if (!Directory.Exists("krnl"))
				{
					Directory.CreateDirectory("krnl");
				}
				Environment.CurrentDirectory += "\\krnl";
			}
			File.WriteAllBytes("7za.exe", Resources._7za);
			File.WriteAllBytes("7za.dll", Resources._7za1);
			File.WriteAllBytes("7zxa.dll", Resources._7zxa);
			CS$<>8__locals1.files = new WebClient().DownloadString("http://cdn.krnl.rocks:8080/files").Split(new char[]
			{
				','
			});
			CS$<>8__locals1.hashes = Program.getHashes();
			CS$<>8__locals1.done = false;
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			int millisecond = DateTime.Now.Millisecond;
			CS$<>8__locals1.idx = 0;
			Parallel.For(0, CS$<>8__locals1.files.Length, delegate(int i, ParallelLoopState state)
			{
				Program.<>c__DisplayClass3_0.<<Main>b__0>d <<Main>b__0>d;
				<<Main>b__0>d.<>4__this = CS$<>8__locals1;
				<<Main>b__0>d.i = i;
				<<Main>b__0>d.<>t__builder = AsyncVoidMethodBuilder.Create();
				<<Main>b__0>d.<>1__state = -1;
				<<Main>b__0>d.<>t__builder.Start<Program.<>c__DisplayClass3_0.<<Main>b__0>d>(ref <<Main>b__0>d);
			});
			while (!CS$<>8__locals1.done)
			{
				Task.Delay(1000).GetAwaiter().GetResult();
			}
			stopwatch.Stop();
			Console.ResetColor();
			File.Delete("7za.exe");
			File.Delete("7za.dll");
			File.Delete("7zxa.dll");
			Console.WriteLine("Press any key to start krnlss.exe");
			Console.ReadKey();
			if (!File.Exists("krnlss.exe"))
			{
				MessageBox.Show("Unable to open krnlss.exe due to the anti-virus protection", "Krnl Bootstrapper (v3)", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				Environment.Exit(0);
			}
			Process.Start("krnlss.exe");
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002774 File Offset: 0x00000974
		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			File.WriteAllText("error.txt", string.Format("KRNL BOOTSTRAPPER V3\n\n{0}", e.ExceptionObject.ToString()));
			Process.Start("error.txt");
		}
	}
}

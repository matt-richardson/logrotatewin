using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;

namespace logrotate
{
    class ConfigParser
	{
		public ConfigParser ()
		{
		}

		public logrotateconf Parse (List<string> configFilePaths, bool isDebug)
		{
			var GlobalConfig = new logrotateconf ();

			// now process the config files
			foreach (string s in configFilePaths)
			{
				ProcessConfigPath(s, GlobalConfig, isDebug);
			}

			// if there was an include directive in the global settings, then we need to process that 
			if (GlobalConfig.Include != "")
			{
				ProcessIncludeDirective(GlobalConfig, isDebug);
			}

			return GlobalConfig;
		}

		private static void ProcessConfigFile(string m_path_to_file, logrotateconf GlobalConfig, bool isDebug)
		{
			Logging.Log(Strings.ParseConfigFile+" " + m_path_to_file,Logging.LogType.Verbose);

			StreamReader sr = new StreamReader(m_path_to_file);
			bool bSawASection = false;
			// read in lines until done
			while (true)
			{
				string line = sr.ReadLine();
				if (line == null)
					break;

				line = line.Trim();
				Logging.Log(Strings.ReadLine+" " + line,Logging.LogType.Debug);

				// skip blank lines
				if (line == "")
					continue;

				// if line begins with #, then it is a comment and can be ignored
				if (line[0] == '#')
				{
					Logging.Log(Strings.Skipping+" "+Strings.Comment,Logging.LogType.Debug);
					continue;
				}

				// see if there is a { in the line.  If so, this is the beginning of a section 
				// otherwise it may be a global setting

				if (line.Contains("{"))
				{
					bSawASection = true;
					Logging.Log(Strings.Processing+" "+Strings.NewSection,Logging.LogType.Verbose);

					// create a new config object taking defaults from Global Config
					logrotateconf lrc = new logrotateconf(GlobalConfig);

					ProcessConfileFileSection(line, sr, lrc, GlobalConfig, isDebug);
				}
				else
				{
					if (bSawASection == false)
						GlobalConfig.Parse(line, isDebug);
					else
					{
						Logging.Log(Strings.GlobalOptionsAboveSections, Logging.LogType.Error);
					}
				}
			}
		}

		private static void ProcessConfileFileSection(string starting_line, StreamReader sr,logrotateconf lrc, logrotateconf GlobalConfig, bool isDebug)
		{
			// the first part of the line contains the file(s) or folder(s) that will be associated with this section
			// we need to break this line apart by spaces
			string split = "";
			bool bQuotedPath = false;
			for (int i = 0; i < starting_line.Length; i++)
			{
				switch (starting_line[i])
				{
				// if we see the open brace, we are done
				case '{':
					i = starting_line.Length;
					break;
					// if we see a ", then this is either starting or ending a file path with spaces
				case '\"':
					if (bQuotedPath == false)
						bQuotedPath = true;
					else
						bQuotedPath = false;
					split += starting_line[i];
					break;
				case ' ':
					// we see a space and we are not processing a quote path, so this is a delimeter and treat it as such
					if (bQuotedPath == false)
					{
						string newsplit = "";
						// remove any invalid characters before adding
						char[] invalidPathChars = Path.GetInvalidPathChars();
						foreach (char ipc in invalidPathChars)
						{
							for (int ii = 0; ii < split.Length; ii++)
							{
								if (split[ii] != ipc)
								{
									newsplit += split[ii];
								}
							}
							split = newsplit;
							newsplit = "";
						}

						lrc.Increment_ProcessCount();
						GlobalConfig.FilePathConfigSection.Add(split, lrc);
						split = "";
					}
					else
						split += starting_line[i];
					break;
				default:
					split += starting_line[i];
					break;
				}


			}

			/*
            string[] split = starting_line.Split(new char[] { ' ' });
            for (int i = 0; i < split.Length-1; i++)
            {
                FilePathConfigSection.Add(split[i], lrc);
            }
             */


			// read until we hit a } and process
			while (true)
			{
				string line = sr.ReadLine();
				if (line == null)
					break;

				line = line.Trim();
				Logging.Log(Strings.ReadLine+" " + line,Logging.LogType.Debug);

				// skip blank lines
				if (line == "")
					continue;

				// if line begins with #, then it is a comment and can be ignored
				if (line[0] == '#')
				{
					Logging.Log(Strings.Skipping+" "+Strings.Comment,Logging.LogType.Debug);
					continue;
				}

				if (line.Contains("}"))
				{
					break;
				}

				lrc.Parse(line, isDebug);
			}

		}

		private static void ProcessConfigPath(string m_path, logrotateconf GlobalConfig, bool isDebug)
		{
			// if this is pointed to a folder (most lilely), then process each file in it
			if ((File.GetAttributes(m_path) & FileAttributes.Directory) == FileAttributes.Directory)
			{
				DirectoryInfo di = new DirectoryInfo(m_path);
				FileInfo[] fis = di.GetFiles("*.*", SearchOption.TopDirectoryOnly);
				foreach (FileInfo fi in fis)
				{
					ProcessConfigFile(fi.FullName, GlobalConfig, isDebug);
				}
			}
			else
			{
				ProcessConfigFile(m_path, GlobalConfig, isDebug);
			}

		}

		/// <summary>
		/// Process the include directive if specfied
		/// </summary>
		private static void ProcessIncludeDirective(logrotateconf GlobalConfig, bool isDebug)
		{
			if (Directory.Exists(GlobalConfig.Include))
			{
				// this is a folder, so get all files in the folder and process them
				DirectoryInfo di = new DirectoryInfo(GlobalConfig.Include);
				FileInfo[] m_fis = di.GetFiles();
				// sort alphabetically
				Array.Sort(m_fis, delegate(FileSystemInfo a, FileSystemInfo b)
					{
						return ((new CaseInsensitiveComparer()).Compare(b.Name, a.Name));
					});
				// make sure ext of file is not in the tabooext list
				foreach (FileInfo m_fi in m_fis)
				{
					bool bFound = false;
					for (int i = 0; i < GlobalConfig.TabooList.Length; i++)
					{
						if (m_fi.Extension == GlobalConfig.TabooList[i])
						{
							Logging.Log(Strings.Skipping+" " + m_fi.FullName + " - " + Strings.ExtInTaboo, Logging.LogType.Verbose);
							bFound = true;
							break;
						}
					}
					if (!bFound)
					{
						Logging.Log(Strings.ProcessInclude+" " + m_fi.FullName, Logging.LogType.Verbose);
						ProcessConfigFile(m_fi.FullName, GlobalConfig, isDebug);
					}
				}
			}
			else
			{
				// this (might be) a file, so attempt to process it
				if (File.Exists(GlobalConfig.Include))
				{
					Logging.Log(Strings.ProcessInclude+" "+ GlobalConfig.Include, Logging.LogType.Verbose);
					ProcessConfigPath(GlobalConfig.Include, GlobalConfig, isDebug);
				}
				else
				{
					// this could be a directory, so let's check for that also
					if ((File.GetAttributes(GlobalConfig.Include) & FileAttributes.Directory) == FileAttributes.Directory)
					{
						DirectoryInfo di = new DirectoryInfo(GlobalConfig.Include);
						FileInfo[] fis = di.GetFiles("*");
						foreach (FileInfo fi in fis)
						{
							Logging.Log(Strings.ProcessInclude + " " + GlobalConfig.Include, Logging.LogType.Verbose);
							ProcessConfigPath(fi.FullName, GlobalConfig, isDebug);
						}
					}
					else
					{
						Logging.Log(GlobalConfig.Include + " "+Strings.CouldNotBeFound, Logging.LogType.Error);
						Environment.Exit(1);
					}
				}
			}
		}

	}
}


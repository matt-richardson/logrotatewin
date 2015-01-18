using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;

namespace logrotate
{
	public interface IFileSystem
	{
		StreamReader OpenFileAsStreamReader (string m_path_to_file);

		FileAttributes GetAttributes (string m_path);

		bool DirectoryExists (string include);

		bool FileExists (string include);

		string[] GetFiles (string m_path);
	}

	public class DefaultFileSystem : IFileSystem
	{
		public StreamReader OpenFileAsStreamReader (string m_path_to_file)
		{
			return new StreamReader (m_path_to_file);
		}

		public FileAttributes GetAttributes (string m_path)
		{
			return File.GetAttributes (m_path);
		}

		public bool DirectoryExists (string include)
		{
			return Directory.Exists (include);
		}

		public bool FileExists (string include)
		{
			return File.Exists (include);
		}

		public string[] GetFiles (string m_path)
		{
			DirectoryInfo di = new DirectoryInfo(m_path);
			FileInfo[] fis = di.GetFiles("*.*", SearchOption.TopDirectoryOnly);
			var files = new string[fis.Length];
			for (var i = 0; i < fis.Length; i++)
				files [i] = fis [i].FullName;
			return files;
		}
	}
}


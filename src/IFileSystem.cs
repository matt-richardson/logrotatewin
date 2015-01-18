using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;

/*
    LogRotate - rotates, compresses, and mails system logs
    Copyright (C) 2015 Matt Richardson

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

namespace logrotate
{
	public interface IFileSystem
	{
		StreamReader OpenFileAsStreamReader (string m_path_to_file);

		FileAttributes GetAttributes (string m_path);

		bool DirectoryExists (string include);

		bool FileExists (string include);

		string[] GetFiles (string m_path);

		long GetFileSize (string logfilepath);
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

		public long GetFileSize (string logfilepath)
		{
			FileInfo fi = new FileInfo(logfilepath);
			return fi.Length;
		}
	}
}


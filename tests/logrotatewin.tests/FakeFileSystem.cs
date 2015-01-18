using NUnit.Framework;
using System;
using logrotate;
using System.IO;
using System.Text;

namespace logrotatewin.tests
{
	public class FakeFileSystem : IFileSystem
	{
		string confFileContent;

		public FakeFileSystem (string confFileContent)
		{
			this.confFileContent = confFileContent;
		}

		public System.IO.StreamReader OpenFileAsStreamReader (string m_path_to_file)
		{
			var stream = new MemoryStream(Encoding.UTF8.GetBytes(confFileContent ?? ""));
			return new StreamReader(stream);
		}

		public System.IO.FileAttributes GetAttributes (string m_path)
		{
			return FileAttributes.Normal;
		}

		public bool DirectoryExists (string include)
		{
			throw new NotImplementedException ();
		}

		public bool FileExists (string include)
		{
			return true;
		}

		public string[] GetFiles (string m_path)
		{
			throw new NotImplementedException ();
		}

		public long GetFileSize (string logfilepath)
		{
			return 300000;
		}
	}

}


using NUnit.Framework;
using System;
using logrotate;

namespace logrotatewin.tests
{
	[TestFixture]
	public class Test
	{
		[Test]
		public void TestCase ()
		{
			var fileSystem = new FakeFileSystem (@"
C:\chef\log\client.log {
    weekly
    copytruncate
    rotate 4
    missingok
    nocompress
}");

			var configFileParser = new ConfigParser ();
			var result = configFileParser.Parse(new System.Collections.Generic.List<string> { "fake path" }, isDebug: true, fileSystem: fileSystem);

			var configSection = result.FilePathConfigSection [@"C:\chef\log\client.log"];
			Assert.That (configSection.Weekly, Is.True);
			Assert.That (configSection.CopyTruncate, Is.True);
			Assert.That (configSection.Rotate, Is.EqualTo (4));
			Assert.That (configSection.MissingOK, Is.EqualTo(true));
			Assert.That (configSection.Compress, Is.EqualTo (false));
		}
	}
}


using System;
using NUnit.Framework;
using logrotate;

namespace logrotatewin.tests
{
	[TestFixture]
	public class RotationValidatorTests
	{
		[Test]
		public void Will_rotate_if_weekly_and_last_rotate_was_more_than_seven_days_ago()
		{
			var fake = new FakeStatusRepository ();
			var fileSystem = new FakeFileSystem (@"
C:\chef\log\client.log {
    weekly
    copytruncate
    rotate 4
    missingok
    nocompress
}");

			var configFileParser = new ConfigParser ();
			var config = configFileParser.Parse(new System.Collections.Generic.List<string> { "fake path" }, isDebug: true, fileSystem: fileSystem);
			var configSection = config.FilePathConfigSection[@"C:\chef\log\client.log"];
			var rotationValidator = new RotationValidator ("fake-log-file-path", configSection, new CmdLineArgs (new string[] {}), fake, fileSystem);
			var result = rotationValidator.IsRotationRequired ();

			Assert.That (result, Is.True);
		}
	}
}


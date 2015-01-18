using System;
using NUnit.Framework;
using logrotate;

namespace logrotatewin.tests
{
	public class FakeStatusRepository : IStatusRepository
	{
		public DateTime GetRotationDate (string logfilepath)
		{
			return DateTime.MinValue;
		}
	}

}


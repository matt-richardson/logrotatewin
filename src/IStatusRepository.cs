using System;
using System.IO;

namespace logrotate
{
	public interface IStatusRepository
	{
		DateTime GetRotationDate (string logfilepath);
	}

}


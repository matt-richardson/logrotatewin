using System;
using System.IO;

namespace logrotate
{
	public class RotationValidator
	{
		logrotateconf lrc;
		CmdLineArgs cla;
		IStatusRepository Status;
		string logfilepath;
		IFileSystem fileSystem;

		public RotationValidator (string logfilepath, logrotateconf lrc, CmdLineArgs cla, IStatusRepository Status, IFileSystem fileSystem)
		{
			this.fileSystem = fileSystem;
			this.logfilepath = logfilepath;
			this.Status = Status;
			this.cla = cla;
			this.lrc = lrc;
		}

		/// <summary>
		/// Check to see if the logfile specified is eligible for rotation
		/// </summary>
		/// <param name="logfilepath">Full path to the log file to check</param>
		/// <param name="lrc">logrotationconf object</param>
		/// <returns>True if need to rotate, False if not</returns>
		public bool IsRotationRequired()
		{
			if (cla.Force)
			{
				Logging.Log(Strings.ForceOptionRotate, Logging.LogType.Verbose);
				return true;
			}

			bool bDoRotate = false;
			// first check if file exists.  if it doesn't error out unless we are set not to
			if (fileSystem.FileExists(logfilepath) == false)
			{
				if (lrc.MissingOK==false)
				{
					Logging.Log(logfilepath + " "+Strings.CouldNotBeFound,Logging.LogType.Error);
					return false;
				}
			}

			var fileSize = fileSystem.GetFileSize (logfilepath);
			if (fileSize == 0)
			{
				if (lrc.IfEmpty == false)
				{
					Logging.Log(Strings.LogFileEmpty+" - " + Strings.Skipping,Logging.LogType.Verbose);
					return false;
				}
			}

			// determine if we need to rotate the file.  this can be based on a number of criteria, including size, date, etc.
			if (lrc.MinSize != 0)
			{
				if (fileSize < lrc.MinSize)
				{
					Logging.Log(Strings.NoRotateNotGTEMinimumFileSize,Logging.LogType.Verbose);

					return false;
				}
			}

			if (lrc.Size != 0)
			{
				if (fileSize >= lrc.Size)
				{
					Logging.Log(Strings.RotateBasedonFileSize,Logging.LogType.Verbose);

					bDoRotate = true;

				}
			}
			else
			{
				if ((lrc.Daily == false) && (lrc.Monthly == false) && (lrc.Yearly == false))
				{
					// this is a misconfiguration is we get here
					Logging.Log(Strings.NoTimestampDirectives, Logging.LogType.Verbose);
				}
				else
				{
					// check last date of rotation
					DateTime lastRotate = Status.GetRotationDate(logfilepath);
					TimeSpan ts = DateTime.Now - lastRotate;
					if (lrc.Daily)
					{
						// check to see if lastRotate is more than a day old
						if (ts.TotalDays > 1)
						{
							bDoRotate = true;
						}
					}
					if (lrc.Weekly)
					{
						// check if total # of days is greater than a week or if the current weekday is less than the weekday of the last rotation
						if (ts.TotalDays > 7)
						{
							bDoRotate = true;
						}
						else if (DateTime.Now.DayOfWeek < lastRotate.DayOfWeek)
						{
							bDoRotate = true;
						}
					}
					if (lrc.Monthly)
					{
						// check if the month is different
						if ((lastRotate.Year != DateTime.Now.Year) || ((lastRotate.Year == DateTime.Now.Year) && (lastRotate.Month != DateTime.Now.Month)))
						{
							bDoRotate = true;
						}
					}
					if (lrc.Yearly)
					{
						// check if the year is different
						if (lastRotate.Year != DateTime.Now.Year)
						{
							bDoRotate = true;
						}
					}
				}
			}

			return bDoRotate;

		}

	}
}


using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Net.Mail;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Collections;

/*
    LogRotate - rotates, compresses, and mails system logs
    Copyright (C) 2012  Ken Salter

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
    class Program
    {
        // this object will parse the command line args
        private static CmdLineArgs cla = null;
        
        static void Main(string[] args)
        {
            // parse the command line
            try
            {
                if (args.Length == 0)
                {
                    PrintVersion();
                    PrintUsage();
                    Environment.Exit(0);
                }

                cla = new CmdLineArgs(args);
                if (cla.Usage)
                {
                    PrintUsage();
                    Environment.Exit(0);
                }
					              
				var configParser = new ConfigParser();
				var GlobalConfig = configParser.Parse(cla.ConfigFilePaths, cla.Debug, new DefaultFileSystem());

				var processor = new Processor(cla);
				processor.Process(GlobalConfig);
            }
            catch (Exception e)
            {
                Logging.LogException(e);
                Environment.Exit(1);
            }
        }

        private static string GetRotateLogDirectory(string logfilepath, logrotateconf lrc)
        {
            if (lrc.OldDir != "")
                return lrc.OldDir;
            else
                return Path.GetDirectoryName(logfilepath);
        }

        private static void PrintUsage()
        {
            Console.WriteLine(Strings.Usage1);
            Console.WriteLine(Strings.Usage2);
            Console.WriteLine(Strings.Usage3);
        }

        private static void PrintVersion()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            
            Console.WriteLine(Strings.ProgramName+" "+asm.GetName().Version.ToString()+" - " + Strings.CopyRight);
            Console.WriteLine(Strings.GNURights);
            Console.WriteLine();
        }
    }
}

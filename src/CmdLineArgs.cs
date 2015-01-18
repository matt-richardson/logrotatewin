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

    public class CmdLineArgs
    {
        private bool bForce = false;

        public bool Force
        {
            get { return bForce; }
        }

        private bool bVerbose = false;

        public bool Verbose
        {
            get { return bVerbose; }
        }

        private bool bUsage = false;

        public bool Usage
        {
            get { return bUsage; }
        }

        private bool bDebug = false;

        public bool Debug
        {
            get { return bDebug; }
        }

        private string sAlternateStateFile = "";
        public string AlternateStateFile
        {
            get { return sAlternateStateFile; }
        }

        private List<string> sConfigFilePaths = new List<string>();

        public List<string> ConfigFilePaths
        {
            get { return sConfigFilePaths; }
        }

        public CmdLineArgs(string[] args)
        {
            bool bWatchForState = false;
            // iterate through the args array
            foreach (string a in args)
            {
                // if the string starts with a '-' then it is a switch
                if (a[0] == '-')
                {
                    switch (a)
                    {
                        case "-d":
                            bDebug = true;
                            bVerbose = true;
                            Logging.SetDebug(true);
                            Logging.SetVerbose(true);
                            Logging.Log(Strings.DebugOptionSet);
                            Logging.Log(Strings.VerboseOptionSet);
                            break;
                        case "-f":
                        case "--force":
                            bForce = true;
                            Logging.Log(Strings.ForceOptionSet,Logging.LogType.Required);
                            break;
                        case "-?":
                        case "--usage":
                            bUsage = true;
                            break;
                        case "-v":
                        case "--verbose":
                            bVerbose = true;
                            Logging.SetVerbose(true);
                            Logging.Log(Strings.VerboseOptionSet);
                            break;
                        case "-m":
                        case "--mail":
                            Logging.Log(Strings.MailOptionSet,Logging.LogType.Error);
                            break;
                        case "-s":
                        case "--state":
                            bWatchForState = true;
                            break;
                        default:
                            // no match, so print an error
                            Logging.Log(Strings.UnknownCmdLineArg + ": "+ a , Logging.LogType.Error);
                            Environment.Exit(1);
                            break;
                    }
                }
                else
                {
                    if (bWatchForState)
                    {
                        sAlternateStateFile = a;
                        Logging.Log(Strings.AlternateStateFile+" " + a,Logging.LogType.Verbose);
                        bWatchForState = false;
                    }
                    else
                    {
                        // otherwise, it is the path to a config file or folder containing config files
                        Logging.Log(a + " " + Strings.AddingConfigFile, Logging.LogType.Verbose);
                        sConfigFilePaths.Add(a);
                    }
                }
            }
        }

    }
}

﻿using System;
using System.Collections.Generic;
using System.Text;


/*
    LogRotate - rotates, compresses, and mails system logs
    Copyright (C) 2012  Ken Salter
    Additional changes Copyright (c) 2015 Matt Richardson

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
    /// <summary>
    /// This class exposes static functions to handle different logging functions, including logging exceptions
    /// </summary>
    class Logging
    {
        public enum LogType
        {
            Debug,
            Verbose,
            Required,
            Error
        }

        private static bool bDebug = false;
        private static bool bVerbose = false;

        /// <summary>
        /// Set the debug flag
        /// </summary>
        /// <param name="m_flag">Boolean indicating if debug is enabled</param>
        public static void SetDebug(bool m_flag)
        {
            bDebug = m_flag;
        }

        /// <summary>
        /// Set the Verbose flag
        /// </summary>
        /// <param name="m_flag">Boolean indicating if verbose is enabled</param>
        public static void SetVerbose(bool m_flag)
        {
            bVerbose = m_flag;
        }

        /// <summary>
        /// Logs an exception, also logging any innerexception
        /// </summary>
        /// <param name="e">the Exception object to log</param>
        public static void LogException(Exception e)
        {
            DoErrorLog("Exception: " + e.Message);
            DoErrorLog("StackTrace: " + e.StackTrace);
            if (e.InnerException != null)
            {
                DoErrorLog("InnerException: " + e.Message);
                DoErrorLog("StackTrace: " + e.StackTrace);
            }
        }

        /// <summary>
        /// Logs as required
        /// </summary>
        /// <param name="m_text">Text to log</param>
        public static void Log(string m_text)
        {
            Log(m_text, LogType.Required);
        }

        /// <summary>
        /// Logs depending on type
        /// </summary>
        /// <param name="m_text">Text to Log</param>
        /// <param name="m_type">Type of Log (Error,Required,Debug,Verbose)</param>
        public static void Log(string m_text, Logging.LogType m_type)
        {
            switch (m_type)
            {
                case LogType.Error:
                    DoErrorLog(m_text);
                    return;
                case LogType.Required:
                    DoLog(m_text);
                    return;
                case LogType.Debug:
                    if (bDebug)
                    {
                        DoLog(m_text);
                    }
                    return;
                case LogType.Verbose:
                    if (bVerbose)
                    {
                        DoLog(m_text);
                    }
                    return;
            }

        }

        private static void DoLog(string m_text)
        {
            Console.WriteLine(Strings.ProgramName + ": " + m_text);
        }

        private static void DoErrorLog(string m_text)
        {
            Console.Error.WriteLine(Strings.ProgramName + ": " + m_text);
        }
    }
}

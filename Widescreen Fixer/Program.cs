#region File Information
/*
 * Copyright (C) 2007-2014 David Rudie
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02111, USA.
 */
#endregion

namespace WidescreenFixer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;

    public static class Program
    {
        [STAThread]
        private static void Main()
        {
            bool shouldContinue = true;

            if (!Directory.Exists(Environment.CurrentDirectory + @"\Plugins"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + @"\Plugins");
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(Environment.CurrentDirectory + @"\Plugins");
            FileInfo[] files = directoryInfo.GetFiles("Plugin.*.dll");

            // It is important to update this offset if the main application version increases while the plugins stay lower.
            int versionMajorOffset = 1;

            Assembly applicationAssembly = Assembly.GetExecutingAssembly();
            AssemblyName applicationName = applicationAssembly.GetName();
            Version applicationVersion = applicationName.Version;
            Version applicationAPIVersion = new Version(applicationVersion.Major - versionMajorOffset, applicationVersion.Minor);

            StringBuilder outdatedLibraries = new StringBuilder();

            if (null != files)
            {
                foreach (FileInfo file in files)
                {
                    Assembly libraryAssembly = Assembly.ReflectionOnlyLoadFrom(file.FullName);
                    AssemblyName libraryName = libraryAssembly.GetName();
                    Version libraryVersion = libraryName.Version;
                    Version libraryAPIVersion = new Version(libraryVersion.Major, libraryVersion.Minor);

                    if (libraryAPIVersion < applicationAPIVersion)
                    {
                        outdatedLibraries.Append(string.Format(CultureInfo.CurrentCulture, "{0}.dll, API Version: {1}\n", libraryName.Name, libraryAPIVersion));
                        shouldContinue = false;
                    }
                }
            }

            if (!shouldContinue)
            {
                outdatedLibraries.Append(string.Format(CultureInfo.CurrentCulture, "\nRequired API Version: {0}\n\nThe application will now exit.", applicationAPIVersion));
                MessageBox.Show(outdatedLibraries.ToString());
            }

            if (shouldContinue)
            {
                Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                bool isNewProcess = false;
                using (Mutex mutex = new Mutex(true, Application.ProductName, out isNewProcess))
                {
                    if (isNewProcess)
                    {
                        Application.Run(new WidescreenFixerApp());
                        mutex.ReleaseMutex();
                    }
                    else
                    {
                        MessageBox.Show("Another instance of " + Application.ProductName + " is already running.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // Handle the UI exceptions by showing a dialog box, and asking the user whether
        // or not they wish to abort execution.
        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            DialogResult dialogResult = DialogResult.Cancel;

            try
            {
                dialogResult = ShowThreadExceptionDialog("Windows Forms Error", e.Exception);
            }
            catch
            {
                try
                {
                    MessageBox.Show("Fatal Windows Forms Error", "Fatal Windows Forms Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Stop);
                }
                finally
                {
                    Application.Exit();
                }

                throw;
            }

            // Exits the program when the user clicks Abort.
            if (dialogResult == DialogResult.Abort)
            {
                Application.Exit();
            }
        }

        // Handle the UI exceptions by showing a dialog box, and asking the user whether
        // or not they wish to abort execution.
        // NOTE: This exception cannot be kept from terminating the application - it can only 
        // log the event, and inform the user about it. 
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception exception = (Exception)e.ExceptionObject;
                string errorMessage = "An application error occurred.  Please contact the author of Widescreen Fixer with the following information:\n\n";

                string exceptionSource = "Widescreen Fixer";
                string exceptionLog = "Application";
                string exceptionEvent = errorMessage + exception.Message + "\n\nStack Trace:\n" + exception.StackTrace;

                // Since we can't prevent the app from terminating, log this to the event log.
                if (!EventLog.SourceExists(exceptionSource))
                {
                    EventLog.CreateEventSource(exceptionSource, exceptionLog);
                }

                EventLog.WriteEntry(exceptionSource, exceptionEvent, EventLogEntryType.Error);
            }
            catch (Exception exception)
            {
                try
                {
                    MessageBox.Show("Fatal Non-UI Error. The error could not be written to the event log. Reason: " + exception.Message, "Fatal Non-UI Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                finally
                {
                    Application.Exit();
                }

                throw;
            }
        }

        // Creates the error message and displays it.
        private static DialogResult ShowThreadExceptionDialog(string title, Exception e)
        {
            string errorMessage = "An application error occurred. Please provide the following information:\n\n";
            errorMessage = errorMessage + e.Message + "\n\nStack Trace:\n" + e.StackTrace;
            return MessageBox.Show(errorMessage, title, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Stop);
        }
    }
}

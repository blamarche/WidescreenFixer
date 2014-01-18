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

namespace Library.Update
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Net;
    using System.Reflection;
    using System.Text;
    using System.Windows.Forms;

    public static class UpdateCheck
    {
        public static void CheckForUpdate()
        {
            Uri uri = new Uri("https://www.widescreenfixer.org/rev");

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.Method = "GET";
            httpWebRequest.KeepAlive = false;
            httpWebRequest.UserAgent = "Widescreen Fixer";
            httpWebRequest.Timeout = 5000;

            try
            {
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                if (httpWebResponse != null)
                {
                    using (WebClient webClient = new WebClient())
                    {
                        Version version = new Version(webClient.DownloadString(uri));

                        Version versionCurrent = new Version(
                            Assembly.GetCallingAssembly().GetName().Version.Major,
                            Assembly.GetCallingAssembly().GetName().Version.Minor,
                            Assembly.GetCallingAssembly().GetName().Version.Build);

                        if (versionCurrent.CompareTo(version) < 0)
                        {
                            string updateMessage = string.Format(
                                CultureInfo.CurrentCulture,
                                "An updated version of Widescreen Fixer is available.\n\nYour version: {0}\nAvailable version: {1}\n\nWould you like to go to the Widescreen Fixer website\nto download the new version?",
                                versionCurrent,
                                version);

                            if (DialogResult.Yes == MessageBox.Show(updateMessage, "Update available", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
                            {
                                Process.Start("https://www.widescreenfixer.org/");
                            }
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }
    }
}

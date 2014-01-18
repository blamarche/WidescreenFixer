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

namespace Library.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Windows.Forms;
    using Library.Plugin;

    // This class provides the plugin host to the main application.
    public class FixerHostProvider
    {
        // A list of the fixes that are loaded.
        private Collection<FixerHost> fixers;

        // Gets a list containing the fixes that are loaded.
        public Collection<FixerHost> Fixers
        {
            get
            {
                if (null == this.fixers)
                {
                    this.Reload();
                }

                return this.fixers;
            }
        }

        // Compares the fix names and sorts them in a natural order.
        public static int CompareNatural(string textOne, string textTwo)
        {
            if (!string.IsNullOrEmpty(textOne) && !string.IsNullOrEmpty(textTwo))
            {
                int indexOne = 0;
                int indexTwo = 0;
                int softResult = 0;
                while (indexOne < textOne.Length && indexTwo < textTwo.Length)
                {
                    bool isDigitA = IsDigit(textOne[indexOne]);
                    bool isDigitB = IsDigit(textTwo[indexTwo]);
                    if (isDigitA != isDigitB)
                    {
                        if (indexOne == 0 && indexTwo == 0)
                        {
                            return string.Compare(textOne, textTwo, StringComparison.CurrentCultureIgnoreCase);
                        }
                        else
                        {
                            return isDigitA ? -1 : 1;
                        }
                    }
                    else if (!isDigitA && !isDigitB)
                    {
                        int index2A = indexOne + 1;
                        int index2B = indexTwo + 1;

                        while (index2A < textOne.Length && !IsDigit(textOne[index2A]))
                        {
                            index2A++;
                        }

                        while (index2B < textTwo.Length && !IsDigit(textTwo[index2B]))
                        {
                            index2B++;
                        }

                        int cmpResult = string.Compare(
                            textOne.Substring(indexOne, index2A - indexOne),
                            textTwo.Substring(indexTwo, index2B - indexTwo),
                            StringComparison.CurrentCultureIgnoreCase);

                        if (cmpResult != 0)
                        {
                            return cmpResult;
                        }

                        indexOne = index2A;
                        indexTwo = index2B;
                    }
                    else
                    {
                        bool foundNonZeroA = false;
                        bool foundNonZeroB = false;

                        do
                        {
                            if (textOne[indexOne] != '0')
                            {
                                foundNonZeroA = true;
                            }
                            else
                            {
                                indexOne++;
                            }
                        }
                        while (!foundNonZeroA && indexOne < textOne.Length && IsDigit(textOne[indexOne]));

                        do
                        {
                            if (textTwo[indexTwo] != '0')
                            {
                                foundNonZeroB = true;
                            }
                            else
                            {
                                indexTwo++;
                            }
                        }
                        while (!foundNonZeroB && indexTwo < textTwo.Length && IsDigit(textTwo[indexTwo]));

                        if (foundNonZeroA != foundNonZeroB)
                        {
                            return foundNonZeroA ? 1 : -1;
                        }
                        else if (foundNonZeroA && foundNonZeroB)
                        {
                            int sameLenResult = 0;
                            while ((isDigitA = indexOne < textOne.Length && IsDigit(textOne[indexOne])) &
                                    (isDigitB = indexTwo < textTwo.Length && IsDigit(textTwo[indexTwo])))
                            {
                                if (textOne[indexOne] != textTwo[indexTwo] && sameLenResult == 0)
                                {
                                    sameLenResult = textOne[indexOne] < textTwo[indexTwo] ? -1 : 1;
                                }

                                indexOne++;
                                indexTwo++;
                            }

                            if (isDigitA != isDigitB)
                            {
                                return isDigitA ? 1 : -1;
                            }
                            else if (sameLenResult != 0)
                            {
                                return sameLenResult;
                            }
                        }

                        if (indexOne != indexTwo && softResult == 0)
                        {
                            softResult = indexOne > indexTwo ? -1 : 1;
                        }
                    }
                }

                if (indexOne < textOne.Length || indexTwo < textTwo.Length)
                {
                    return indexOne < textOne.Length ? 1 : -1;
                }

                if (softResult != 0)
                {
                    return softResult;
                }
            }

            return 0;
        }

        // Reloads the plugins.
        public void Reload()
        {
            if (null == this.fixers)
            {
                this.fixers = new Collection<FixerHost>();
            }
            else
            {
                this.fixers.Clear();
            }

            Collection<Assembly> pluginAssemblies = LoadPluginAssemblies();
            List<IFixer> plugins = GetPlugins(pluginAssemblies);

            foreach (IFixer fix in plugins)
            {
                this.fixers.Add(new FixerHost(fix));
            }
        }

        // Gets a list of plugins loaded.
        public static List<IFixer> GetPlugins(Collection<Assembly> assemblies)
        {
            if (assemblies != null)
            {
                List<Type> availableTypes = new List<Type>();

                foreach (Assembly currentAssembly in assemblies)
                {
                    availableTypes.AddRange(currentAssembly.GetTypes());
                }

                List<Type> fixerList = availableTypes.FindAll(delegate(Type type)
                {
                    List<Type> interfaceTypes = new List<Type>(type.GetInterfaces());
                    object[] array = type.GetCustomAttributes(typeof(FixerPluginAttribute), true);
                    return !(array == null || array.Length == 0) && interfaceTypes.Contains(typeof(IFixer));
                });

                return fixerList.ConvertAll<IFixer>(delegate(Type type) { return Activator.CreateInstance(type) as IFixer; });
            }

            return null;
        }

        // Checks if a character is a digit or not.
        private static bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        // Loads the plugins, sorts them naturally, and then returns a list of the valid plugins.
        private static Collection<Assembly> LoadPluginAssemblies()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(Environment.CurrentDirectory + @"\Plugins");
            FileInfo[] files = directoryInfo.GetFiles("Plugin.*.dll");

            Array.Sort(files, (a, b) => CompareNatural(Path.GetFileNameWithoutExtension(a.Name), Path.GetFileNameWithoutExtension(b.Name)));

            Collection<Assembly> pluginAssemblyList = new Collection<Assembly>();

            Assembly libraryAssembly = Assembly.GetExecutingAssembly();
            AssemblyName libraryName = libraryAssembly.GetName();
            Version libraryVersion = libraryName.Version;
            Version libraryAPIVersion = new Version(libraryVersion.Major, libraryVersion.Minor);

            StringBuilder outdatedPlugins = new StringBuilder();

            if (files != null)
            {
                foreach (FileInfo file in files)
                {
                    Assembly pluginAssembly = Assembly.ReflectionOnlyLoadFrom(file.FullName);
                    AssemblyName pluginName = pluginAssembly.GetName();
                    Version pluginVersion = pluginName.Version;
                    Version pluginAPIVersion = new Version(pluginVersion.Major, pluginVersion.Minor);

                    if (pluginAPIVersion >= libraryAPIVersion)
                    {
                        pluginAssemblyList.Add(Assembly.UnsafeLoadFrom(file.FullName));
                    }
                    else
                    {
                        outdatedPlugins.Append(string.Format(CultureInfo.CurrentCulture, "{0}.dll, API Version: {1}\n", pluginName.Name, pluginAPIVersion));
                    }
                }
            }

            if (outdatedPlugins.Length > 0)
            {
                outdatedPlugins.Append(string.Format(CultureInfo.CurrentCulture, "\nRequired API Version: {0}\n\nThe plugins listed above will not be loaded.", libraryAPIVersion));
                MessageBox.Show(outdatedPlugins.ToString());
            }

            return pluginAssemblyList;
        }
    }
}

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

    // This sets up the attributes for the plugin.
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class FixerPluginAttribute : Attribute
    {
        // The description of the plugin.
        private string description;

        // True if it's the default version, false otherwise.
        private bool defaultVersion;

        // Initializes a new instance of the FixerPluginAttribute class.
        public FixerPluginAttribute(string description)
        {
            this.description = description;
        }

        // Initializes a new instance of the FixerPluginAttribute class.
        public FixerPluginAttribute(string description, bool defaultVersion)
        {
            this.description = description;
            this.defaultVersion = defaultVersion;
        }

        // Gets or sets a value indicating the plugin description.
        public string Description
        {
            get { return this.description; }
        }

        // Gets or sets a value indicating whether this plugin is the default version or not.
        public bool DefaultVersion
        {
            get { return this.defaultVersion; }
        }
    }
}

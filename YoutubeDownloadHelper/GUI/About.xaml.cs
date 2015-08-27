using System;
using System.Diagnostics;
using System.Globalization;
using System.Security.Permissions;
using System.Windows;
using System.Linq;
using YoutubeDownloadHelper.Code;

namespace YoutubeDownloadHelper.Gui
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        private VersionInfo VersionInfo { get; set; }

        private readonly string disclaimer = string.Format(CultureInfo.CurrentCulture, "{0}\n\n{1}\n\n{2}\n\n{3}\n\n{4}", "I am not liable for any damage done to you, your property, or otherwise with regard to the running, compiling, and/or overall use of this program, it's associated libraries (if any), and/or any additional data contained within the source. I am likewise not financially, morally or legally obligated to pay for the cost of the aforementioned property, any lost wages, any emotional distress, and/or the repairs thereof.", "This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.", "I do not extend additional warranties and will not fulfil any warranty given to you by any third party or otherwise. By downloading this program's source, compiling said source from either github, a third party website, or otherwise, and/or using (a) pre-compiled version(s) of this program from a third party website or otherwise, you agree to not only the license contained within this project, regardless of whether your version contained said license, but the information within this disclaimer and agree to all said information from the point you downloaded forward. If using a previous version you agree to any new additions or removals of/to said information.", "This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.", "Full details available on the project page (https://github.com/AnonymousUser200102010/Youtube-Download-Helper)");

        /// <summary>
        /// About window.
        /// </summary>
        /// <param name="programInfo">
        /// This program's information (main project info).
        /// </param>
        /// <param name="assemblies">
        /// Assemblies to use.
        /// </param>
        public About (FileVersionInfo programInfo, ProjectAssemblies assemblies)
        {
            this.VersionInfo = new VersionInfo (programInfo, assemblies, disclaimer);
            this.DataContext = this.VersionInfo;
            InitializeComponent();
        }
    }

    [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
    public class VersionInfo
    {
        private FileVersionInfo ProgramInformation { get; set; }

        private readonly ProjectAssemblies embeddedLibraries;

        /// <summary>
        /// The disclaimer for the program.
        /// </summary>
        public string Disclaimer { get; set; }

        /// <summary>
        /// The name of the program.
        /// </summary>
        public string ProjectName
        {
            get
            {
                return ProgramInformation.ProductName;
            }
        }

        /// <summary>
        /// The program's copyright.
        /// </summary>
        public string Copyright
        {
            get
            {
                return ProgramInformation.LegalCopyright;
            }
        }

        /// <summary>
        /// The program's main objective.
        /// </summary>
        public string Goal
        {
            get
            {
                return ProgramInformation.Comments;
            }
        }

        /// <summary>
        /// Version information for the program.
        /// </summary>
        public string Version
        {
            get
            {
                return string.Format(CultureInfo.CurrentCulture, "[Third Party Libraries/Software:\n{0}]\n[First Party Libraries/Software:\n{1}: {2}\n{3}]", embeddedLibraries.ToString(Party.Third), this.ProjectName, this.ProgramInformation.FileVersion, embeddedLibraries.ToString(Party.First));
            }
        }

        /// <summary>
        /// Container for the UI values for the About window of this program.
        /// </summary>
        /// <param name="fileInfo">
        /// This program's information (main project info).
        /// </param>
        /// <param name="assemblies">
        /// Assemblies to use.
        /// </param>
        /// <param name="disclaimer">
        /// Disclaimer for this program.
        /// </param>
        public VersionInfo (FileVersionInfo fileInfo, ProjectAssemblies assemblies, string disclaimer)
        {
            this.ProgramInformation = fileInfo;
            this.embeddedLibraries = assemblies;
            this.Disclaimer = disclaimer;
        }
    }
}
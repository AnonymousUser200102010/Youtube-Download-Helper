
using System;
using System.Diagnostics;
using System.Globalization;
using System.Security.Permissions;
using System.Windows;

namespace YoutubeDownloadHelper.Gui
{
	/// <summary>
	/// Interaction logic for About.xaml
	/// </summary>
	public partial class About : Window
	{
		
		public FileVersionInfo ProgramInformation { get; set; }
		private VersionInfo versionInfo;
		
		public VersionInfo VersionInfo
		{
			get
			{
				return this.versionInfo;
			}
			set
			{
				this.versionInfo = value;
			}
		}
		
		public About(FileVersionInfo programInfo, string disclaimer)
		{
			this.ProgramInformation = programInfo;
			this.VersionInfo = new VersionInfo(programInfo, disclaimer);
			this.DataContext = this.VersionInfo;
			InitializeComponent();
		}
	}
	
	[PermissionSetAttribute(SecurityAction.Demand, Name="FullTrust")]
	public class VersionInfo
	{
		private FileVersionInfo ProgramInformation { get; set; }
		public string Disclaimer { get; set; }
		public string ProjectName
		{
			get
			{
				return ProgramInformation.ProductName;
			}
		}
		public string Copyright
		{
			get
			{
				return ProgramInformation.LegalCopyright;
			}
		}
		public string Goal
		{
			get
			{
				return ProgramInformation.Comments;
			}
		}
		public string Version
		{
			get
			{
				return string.Format(CultureInfo.CurrentCulture, "{0}.{1}.{2}.{3}", this.ProgramInformation.FileMajorPart, this.ProgramInformation.FileMinorPart, this.ProgramInformation.FileBuildPart, this.ProgramInformation.FilePrivatePart);
			}
		}
		
		public VersionInfo(FileVersionInfo fileInfo, string disclaimer)
		{
			this.ProgramInformation = fileInfo;
			this.Disclaimer = disclaimer;
		}
	}
}
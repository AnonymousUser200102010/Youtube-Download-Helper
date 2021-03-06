﻿
namespace YoutubeDownloadHelper
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.Label schedulerToLabel;
		private System.Windows.Forms.DateTimePicker schedulerTimePicker1;
		private System.Windows.Forms.DateTimePicker schedulerTimePicker2;
		private System.Windows.Forms.CheckBox schedulingEnabled;
		private System.Windows.Forms.ListView queuedBox;
		private System.Windows.Forms.ProgressBar videoDownloadProgressBar;
		private System.Windows.Forms.TextBox newURL;
		private System.Windows.Forms.TabControl queueDownloadsTab;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.Button addURLButton;
		private System.Windows.Forms.TabPage tabPage4;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label downloadingLabel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown newUrlRes;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown resolutionToModify;
		private System.Windows.Forms.Button submitModificationButton;
		private System.Windows.Forms.TextBox urlToModify;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button deleteUrlFromQueue;
		private System.Windows.Forms.StatusStrip statusStrip;
		private System.Windows.Forms.ToolStripStatusLabel statusLabel;
		private System.Windows.Forms.FolderBrowserDialog saveLocationDialog;
		private System.Windows.Forms.Button changeSaveLocation;
		private System.Windows.Forms.TextBox saveLocation;
		private System.Windows.Forms.TabControl tabControl2;
		private System.Windows.Forms.TabPage tabPage5;
		private System.Windows.Forms.TabPage tabPage6;
		private System.Windows.Forms.TextBox temporaryLocation;
		private System.Windows.Forms.Button changeTemporaryLocation;
		private System.Windows.Forms.FolderBrowserDialog tempLocationDialog;
		private System.Windows.Forms.Button downloadButton;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox newUrlFormat;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.ComboBox formatToModify;
		private System.Windows.Forms.ColumnHeader numInQueueColumn;
		private System.Windows.Forms.ColumnHeader urlInfoColumn;
		private System.Windows.Forms.ColumnHeader resInfoColumn;
		private System.Windows.Forms.ColumnHeader formatInfoColumn;
		private System.Windows.Forms.Button moveQueuedItemDown;
		private System.Windows.Forms.Button moveQueuedItemUp;
		private System.Windows.Forms.ComboBox schedualUOM;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.videoDownloadProgressBar = new System.Windows.Forms.ProgressBar();
			this.queueDownloadsTab = new System.Windows.Forms.TabControl();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.moveQueuedItemDown = new System.Windows.Forms.Button();
			this.moveQueuedItemUp = new System.Windows.Forms.Button();
			this.queuedBox = new System.Windows.Forms.ListView();
			this.numInQueueColumn = new System.Windows.Forms.ColumnHeader();
			this.urlInfoColumn = new System.Windows.Forms.ColumnHeader();
			this.resInfoColumn = new System.Windows.Forms.ColumnHeader();
			this.formatInfoColumn = new System.Windows.Forms.ColumnHeader();
			this.deleteUrlFromQueue = new System.Windows.Forms.Button();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.label5 = new System.Windows.Forms.Label();
			this.newUrlFormat = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.newUrlRes = new System.Windows.Forms.NumericUpDown();
			this.addURLButton = new System.Windows.Forms.Button();
			this.newURL = new System.Windows.Forms.TextBox();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.label6 = new System.Windows.Forms.Label();
			this.formatToModify = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.resolutionToModify = new System.Windows.Forms.NumericUpDown();
			this.submitModificationButton = new System.Windows.Forms.Button();
			this.urlToModify = new System.Windows.Forms.TextBox();
			this.tabPage4 = new System.Windows.Forms.TabPage();
			this.tabControl2 = new System.Windows.Forms.TabControl();
			this.tabPage5 = new System.Windows.Forms.TabPage();
			this.saveLocation = new System.Windows.Forms.TextBox();
			this.changeSaveLocation = new System.Windows.Forms.Button();
			this.tabPage6 = new System.Windows.Forms.TabPage();
			this.temporaryLocation = new System.Windows.Forms.TextBox();
			this.changeTemporaryLocation = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.schedualUOM = new System.Windows.Forms.ComboBox();
			this.schedulingEnabled = new System.Windows.Forms.CheckBox();
			this.schedulerTimePicker2 = new System.Windows.Forms.DateTimePicker();
			this.schedulerToLabel = new System.Windows.Forms.Label();
			this.schedulerTimePicker1 = new System.Windows.Forms.DateTimePicker();
			this.downloadButton = new System.Windows.Forms.Button();
			this.downloadingLabel = new System.Windows.Forms.Label();
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.saveLocationDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.tempLocationDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.queueDownloadsTab.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.newUrlRes)).BeginInit();
			this.tabPage2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.resolutionToModify)).BeginInit();
			this.tabPage4.SuspendLayout();
			this.tabControl2.SuspendLayout();
			this.tabPage5.SuspendLayout();
			this.tabPage6.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.statusStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// videoDownloadProgressBar
			// 
			this.videoDownloadProgressBar.Location = new System.Drawing.Point(0, 495);
			this.videoDownloadProgressBar.Name = "videoDownloadProgressBar";
			this.videoDownloadProgressBar.Size = new System.Drawing.Size(670, 32);
			this.videoDownloadProgressBar.Step = 1;
			this.videoDownloadProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.videoDownloadProgressBar.TabIndex = 3;
			// 
			// queueDownloadsTab
			// 
			this.queueDownloadsTab.Controls.Add(this.tabPage3);
			this.queueDownloadsTab.Controls.Add(this.tabPage4);
			this.queueDownloadsTab.Location = new System.Drawing.Point(0, 8);
			this.queueDownloadsTab.Name = "queueDownloadsTab";
			this.queueDownloadsTab.SelectedIndex = 0;
			this.queueDownloadsTab.Size = new System.Drawing.Size(670, 416);
			this.queueDownloadsTab.TabIndex = 4;
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.moveQueuedItemDown);
			this.tabPage3.Controls.Add(this.moveQueuedItemUp);
			this.tabPage3.Controls.Add(this.queuedBox);
			this.tabPage3.Controls.Add(this.deleteUrlFromQueue);
			this.tabPage3.Controls.Add(this.tabControl1);
			this.tabPage3.Location = new System.Drawing.Point(4, 25);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage3.Size = new System.Drawing.Size(662, 387);
			this.tabPage3.TabIndex = 0;
			this.tabPage3.Text = "Queue";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// moveQueuedItemDown
			// 
			this.moveQueuedItemDown.BackColor = System.Drawing.Color.Transparent;
			this.moveQueuedItemDown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.moveQueuedItemDown.Enabled = false;
			this.moveQueuedItemDown.Location = new System.Drawing.Point(4, 227);
			this.moveQueuedItemDown.Name = "moveQueuedItemDown";
			this.moveQueuedItemDown.Size = new System.Drawing.Size(24, 120);
			this.moveQueuedItemDown.TabIndex = 10;
			this.moveQueuedItemDown.UseVisualStyleBackColor = false;
			this.moveQueuedItemDown.Click += new System.EventHandler(this.MoveQueuedItem);
			// 
			// moveQueuedItemUp
			// 
			this.moveQueuedItemUp.BackColor = System.Drawing.Color.Transparent;
			this.moveQueuedItemUp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.moveQueuedItemUp.Enabled = false;
			this.moveQueuedItemUp.Location = new System.Drawing.Point(4, 106);
			this.moveQueuedItemUp.Name = "moveQueuedItemUp";
			this.moveQueuedItemUp.Size = new System.Drawing.Size(24, 120);
			this.moveQueuedItemUp.TabIndex = 9;
			this.moveQueuedItemUp.UseVisualStyleBackColor = false;
			this.moveQueuedItemUp.Click += new System.EventHandler(this.MoveQueuedItem);
			// 
			// queuedBox
			// 
			this.queuedBox.Activation = System.Windows.Forms.ItemActivation.OneClick;
			this.queuedBox.AllowColumnReorder = true;
			this.queuedBox.AutoArrange = false;
			this.queuedBox.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
			this.numInQueueColumn,
			this.urlInfoColumn,
			this.resInfoColumn,
			this.formatInfoColumn});
			this.queuedBox.FullRowSelect = true;
			this.queuedBox.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.queuedBox.HideSelection = false;
			this.queuedBox.Location = new System.Drawing.Point(32, 104);
			this.queuedBox.MultiSelect = false;
			this.queuedBox.Name = "queuedBox";
			this.queuedBox.Size = new System.Drawing.Size(630, 244);
			this.queuedBox.TabIndex = 0;
			this.queuedBox.UseCompatibleStateImageBehavior = false;
			this.queuedBox.View = System.Windows.Forms.View.Details;
			this.queuedBox.SelectedIndexChanged += new System.EventHandler(this.QueuedBoxSelectedIndexChanged);
			// 
			// numInQueueColumn
			// 
			this.numInQueueColumn.Text = "# in Queue";
			this.numInQueueColumn.Width = 24;
			// 
			// urlInfoColumn
			// 
			this.urlInfoColumn.Text = "URL";
			this.urlInfoColumn.Width = 464;
			// 
			// resInfoColumn
			// 
			this.resInfoColumn.Text = "Resolution";
			this.resInfoColumn.Width = 79;
			// 
			// formatInfoColumn
			// 
			this.formatInfoColumn.Text = "Format";
			this.formatInfoColumn.Width = 56;
			// 
			// deleteUrlFromQueue
			// 
			this.deleteUrlFromQueue.Location = new System.Drawing.Point(0, 352);
			this.deleteUrlFromQueue.Name = "deleteUrlFromQueue";
			this.deleteUrlFromQueue.Size = new System.Drawing.Size(662, 32);
			this.deleteUrlFromQueue.TabIndex = 8;
			this.deleteUrlFromQueue.Text = "Delete Selected Entry";
			this.deleteUrlFromQueue.UseVisualStyleBackColor = true;
			this.deleteUrlFromQueue.Click += new System.EventHandler(this.DeleteButtonClick);
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Location = new System.Drawing.Point(3, 8);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(661, 96);
			this.tabControl1.TabIndex = 7;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.label5);
			this.tabPage1.Controls.Add(this.newUrlFormat);
			this.tabPage1.Controls.Add(this.label4);
			this.tabPage1.Controls.Add(this.label1);
			this.tabPage1.Controls.Add(this.newUrlRes);
			this.tabPage1.Controls.Add(this.addURLButton);
			this.tabPage1.Controls.Add(this.newURL);
			this.tabPage1.Location = new System.Drawing.Point(4, 25);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(653, 67);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Add";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(0, 36);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(64, 24);
			this.label5.TabIndex = 19;
			this.label5.Text = "Format:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// newUrlFormat
			// 
			this.newUrlFormat.FormattingEnabled = true;
			this.newUrlFormat.Items.AddRange(new object[] {
			"Mp4",
			"Flash",
			"Mobile",
			"WebM"});
			this.newUrlFormat.Location = new System.Drawing.Point(64, 36);
			this.newUrlFormat.Name = "newUrlFormat";
			this.newUrlFormat.Size = new System.Drawing.Size(96, 24);
			this.newUrlFormat.TabIndex = 18;
			this.newUrlFormat.Text = "Mp4";
			this.newUrlFormat.SelectedIndexChanged += new System.EventHandler(this.NewUrlFormatSelectedIndexChanged);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(0, 8);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(40, 24);
			this.label4.TabIndex = 16;
			this.label4.Text = "URL:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(168, 40);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 16);
			this.label1.TabIndex = 14;
			this.label1.Text = "Resolution:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// newUrlRes
			// 
			this.newUrlRes.Enabled = false;
			this.newUrlRes.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.newUrlRes.Increment = new decimal(new int[] {
			120,
			0,
			0,
			0});
			this.newUrlRes.Location = new System.Drawing.Point(252, 38);
			this.newUrlRes.Maximum = new decimal(new int[] {
			600,
			0,
			0,
			0});
			this.newUrlRes.Minimum = new decimal(new int[] {
			144,
			0,
			0,
			0});
			this.newUrlRes.Name = "newUrlRes";
			this.newUrlRes.Size = new System.Drawing.Size(72, 22);
			this.newUrlRes.TabIndex = 13;
			this.newUrlRes.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.newUrlRes.UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left;
			this.newUrlRes.Value = new decimal(new int[] {
			360,
			0,
			0,
			0});
			// 
			// addURLButton
			// 
			this.addURLButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F);
			this.addURLButton.Location = new System.Drawing.Point(336, 35);
			this.addURLButton.Name = "addURLButton";
			this.addURLButton.Size = new System.Drawing.Size(304, 27);
			this.addURLButton.TabIndex = 12;
			this.addURLButton.Text = "Add URL";
			this.addURLButton.UseVisualStyleBackColor = true;
			this.addURLButton.Click += new System.EventHandler(this.AddURLButtonClick);
			// 
			// newURL
			// 
			this.newURL.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.newURL.Location = new System.Drawing.Point(40, 8);
			this.newURL.Name = "newURL";
			this.newURL.Size = new System.Drawing.Size(608, 22);
			this.newURL.TabIndex = 11;
			this.newURL.Text = "Example: https://www.youtube.com/watch?v=ft6rWcJIlpU";
			this.newURL.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.newURL.Enter += new System.EventHandler(this.NewURLEnter);
			this.newURL.Leave += new System.EventHandler(this.NewURLLeave);
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.label6);
			this.tabPage2.Controls.Add(this.formatToModify);
			this.tabPage2.Controls.Add(this.label3);
			this.tabPage2.Controls.Add(this.label2);
			this.tabPage2.Controls.Add(this.resolutionToModify);
			this.tabPage2.Controls.Add(this.submitModificationButton);
			this.tabPage2.Controls.Add(this.urlToModify);
			this.tabPage2.Location = new System.Drawing.Point(4, 25);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(653, 67);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Modify";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(0, 36);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(64, 24);
			this.label6.TabIndex = 21;
			this.label6.Text = "Format:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// formatToModify
			// 
			this.formatToModify.FormattingEnabled = true;
			this.formatToModify.Items.AddRange(new object[] {
			"Mp4",
			"Flash",
			"Mobile",
			"WebM"});
			this.formatToModify.Location = new System.Drawing.Point(64, 36);
			this.formatToModify.Name = "formatToModify";
			this.formatToModify.Size = new System.Drawing.Size(96, 24);
			this.formatToModify.TabIndex = 20;
			this.formatToModify.Text = "Mp4";
			this.formatToModify.SelectedIndexChanged += new System.EventHandler(this.FormatToModifySelectedIndexChanged);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(0, 8);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(40, 24);
			this.label3.TabIndex = 15;
			this.label3.Text = "URL:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(168, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(80, 16);
			this.label2.TabIndex = 14;
			this.label2.Text = "Resolution:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// resolutionToModify
			// 
			this.resolutionToModify.Increment = new decimal(new int[] {
			120,
			0,
			0,
			0});
			this.resolutionToModify.Location = new System.Drawing.Point(252, 38);
			this.resolutionToModify.Maximum = new decimal(new int[] {
			600,
			0,
			0,
			0});
			this.resolutionToModify.Minimum = new decimal(new int[] {
			144,
			0,
			0,
			0});
			this.resolutionToModify.Name = "resolutionToModify";
			this.resolutionToModify.Size = new System.Drawing.Size(72, 22);
			this.resolutionToModify.TabIndex = 13;
			this.resolutionToModify.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.resolutionToModify.UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left;
			this.resolutionToModify.Value = new decimal(new int[] {
			360,
			0,
			0,
			0});
			// 
			// submitModificationButton
			// 
			this.submitModificationButton.AutoSize = true;
			this.submitModificationButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F);
			this.submitModificationButton.Location = new System.Drawing.Point(336, 35);
			this.submitModificationButton.Name = "submitModificationButton";
			this.submitModificationButton.Size = new System.Drawing.Size(304, 27);
			this.submitModificationButton.TabIndex = 12;
			this.submitModificationButton.Text = "Modify Entry";
			this.submitModificationButton.UseVisualStyleBackColor = true;
			this.submitModificationButton.Click += new System.EventHandler(this.SubmitModificationButtonClick);
			// 
			// urlToModify
			// 
			this.urlToModify.Location = new System.Drawing.Point(40, 8);
			this.urlToModify.Name = "urlToModify";
			this.urlToModify.Size = new System.Drawing.Size(608, 22);
			this.urlToModify.TabIndex = 11;
			// 
			// tabPage4
			// 
			this.tabPage4.Controls.Add(this.tabControl2);
			this.tabPage4.Controls.Add(this.groupBox1);
			this.tabPage4.Location = new System.Drawing.Point(4, 25);
			this.tabPage4.Name = "tabPage4";
			this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage4.Size = new System.Drawing.Size(662, 387);
			this.tabPage4.TabIndex = 1;
			this.tabPage4.Text = "Options";
			this.tabPage4.UseVisualStyleBackColor = true;
			// 
			// tabControl2
			// 
			this.tabControl2.Controls.Add(this.tabPage5);
			this.tabControl2.Controls.Add(this.tabPage6);
			this.tabControl2.Location = new System.Drawing.Point(8, 72);
			this.tabControl2.Name = "tabControl2";
			this.tabControl2.SelectedIndex = 0;
			this.tabControl2.Size = new System.Drawing.Size(640, 88);
			this.tabControl2.TabIndex = 1;
			// 
			// tabPage5
			// 
			this.tabPage5.Controls.Add(this.saveLocation);
			this.tabPage5.Controls.Add(this.changeSaveLocation);
			this.tabPage5.Location = new System.Drawing.Point(4, 25);
			this.tabPage5.Name = "tabPage5";
			this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage5.Size = new System.Drawing.Size(632, 59);
			this.tabPage5.TabIndex = 0;
			this.tabPage5.Text = "Primary Save Location";
			this.tabPage5.UseVisualStyleBackColor = true;
			// 
			// saveLocation
			// 
			this.saveLocation.Location = new System.Drawing.Point(56, 16);
			this.saveLocation.Name = "saveLocation";
			this.saveLocation.Size = new System.Drawing.Size(568, 22);
			this.saveLocation.TabIndex = 1;
			this.saveLocation.Text = "C:\\";
			this.saveLocation.TextChanged += new System.EventHandler(this.OptionChanged);
			// 
			// changeSaveLocation
			// 
			this.changeSaveLocation.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.changeSaveLocation.Location = new System.Drawing.Point(8, 8);
			this.changeSaveLocation.Name = "changeSaveLocation";
			this.changeSaveLocation.Size = new System.Drawing.Size(40, 40);
			this.changeSaveLocation.TabIndex = 0;
			this.changeSaveLocation.UseVisualStyleBackColor = true;
			this.changeSaveLocation.Click += new System.EventHandler(this.ChangeSaveLocationClick);
			// 
			// tabPage6
			// 
			this.tabPage6.Controls.Add(this.temporaryLocation);
			this.tabPage6.Controls.Add(this.changeTemporaryLocation);
			this.tabPage6.Location = new System.Drawing.Point(4, 25);
			this.tabPage6.Name = "tabPage6";
			this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage6.Size = new System.Drawing.Size(632, 59);
			this.tabPage6.TabIndex = 1;
			this.tabPage6.Text = "Temporary Save Location";
			this.tabPage6.UseVisualStyleBackColor = true;
			// 
			// temporaryLocation
			// 
			this.temporaryLocation.Location = new System.Drawing.Point(56, 16);
			this.temporaryLocation.Name = "temporaryLocation";
			this.temporaryLocation.Size = new System.Drawing.Size(568, 22);
			this.temporaryLocation.TabIndex = 3;
			this.temporaryLocation.Text = "C:\\";
			this.temporaryLocation.TextChanged += new System.EventHandler(this.OptionChanged);
			// 
			// changeTemporaryLocation
			// 
			this.changeTemporaryLocation.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.changeTemporaryLocation.Location = new System.Drawing.Point(8, 8);
			this.changeTemporaryLocation.Name = "changeTemporaryLocation";
			this.changeTemporaryLocation.Size = new System.Drawing.Size(40, 40);
			this.changeTemporaryLocation.TabIndex = 2;
			this.changeTemporaryLocation.UseVisualStyleBackColor = true;
			this.changeTemporaryLocation.Click += new System.EventHandler(this.ChangeSaveLocationClick);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.schedualUOM);
			this.groupBox1.Controls.Add(this.schedulingEnabled);
			this.groupBox1.Controls.Add(this.schedulerTimePicker2);
			this.groupBox1.Controls.Add(this.schedulerToLabel);
			this.groupBox1.Controls.Add(this.schedulerTimePicker1);
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(400, 56);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Scheduler";
			// 
			// schedualUOM
			// 
			this.schedualUOM.Enabled = false;
			this.schedualUOM.FormattingEnabled = true;
			this.schedualUOM.Items.AddRange(new object[] {
			"Daily",
			"Weekly",
			"Monthly"});
			this.schedualUOM.Location = new System.Drawing.Point(320, 22);
			this.schedualUOM.Name = "schedualUOM";
			this.schedualUOM.Size = new System.Drawing.Size(64, 24);
			this.schedualUOM.TabIndex = 8;
			this.schedualUOM.Text = "Daily";
			this.schedualUOM.Visible = false;
			// 
			// schedulingEnabled
			// 
			this.schedulingEnabled.Enabled = false;
			this.schedulingEnabled.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.schedulingEnabled.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.schedulingEnabled.Location = new System.Drawing.Point(8, 24);
			this.schedulingEnabled.Name = "schedulingEnabled";
			this.schedulingEnabled.Size = new System.Drawing.Size(80, 24);
			this.schedulingEnabled.TabIndex = 7;
			this.schedulingEnabled.Text = "Enabled";
			this.schedulingEnabled.UseVisualStyleBackColor = true;
			this.schedulingEnabled.CheckedChanged += new System.EventHandler(this.SchedulingEnabledCheckedChanged);
			// 
			// schedulerTimePicker2
			// 
			this.schedulerTimePicker2.CustomFormat = "";
			this.schedulerTimePicker2.Format = System.Windows.Forms.DateTimePickerFormat.Time;
			this.schedulerTimePicker2.Location = new System.Drawing.Point(216, 24);
			this.schedulerTimePicker2.Name = "schedulerTimePicker2";
			this.schedulerTimePicker2.ShowUpDown = true;
			this.schedulerTimePicker2.Size = new System.Drawing.Size(96, 22);
			this.schedulerTimePicker2.TabIndex = 6;
			this.schedulerTimePicker2.ValueChanged += new System.EventHandler(this.OptionChanged);
			// 
			// schedulerToLabel
			// 
			this.schedulerToLabel.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.schedulerToLabel.Location = new System.Drawing.Point(184, 24);
			this.schedulerToLabel.Name = "schedulerToLabel";
			this.schedulerToLabel.Size = new System.Drawing.Size(32, 24);
			this.schedulerToLabel.TabIndex = 5;
			this.schedulerToLabel.Text = "to";
			this.schedulerToLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// schedulerTimePicker1
			// 
			this.schedulerTimePicker1.CustomFormat = "";
			this.schedulerTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Time;
			this.schedulerTimePicker1.Location = new System.Drawing.Point(88, 24);
			this.schedulerTimePicker1.Name = "schedulerTimePicker1";
			this.schedulerTimePicker1.ShowUpDown = true;
			this.schedulerTimePicker1.Size = new System.Drawing.Size(96, 22);
			this.schedulerTimePicker1.TabIndex = 4;
			this.schedulerTimePicker1.ValueChanged += new System.EventHandler(this.OptionChanged);
			// 
			// downloadButton
			// 
			this.downloadButton.Location = new System.Drawing.Point(0, 424);
			this.downloadButton.Name = "downloadButton";
			this.downloadButton.Size = new System.Drawing.Size(670, 30);
			this.downloadButton.TabIndex = 9;
			this.downloadButton.Text = "Start Downloading";
			this.downloadButton.UseVisualStyleBackColor = true;
			this.downloadButton.Click += new System.EventHandler(this.StartDownloadButtonClick);
			// 
			// downloadingLabel
			// 
			this.downloadingLabel.AutoEllipsis = true;
			this.downloadingLabel.BackColor = System.Drawing.SystemColors.Control;
			this.downloadingLabel.Enabled = false;
			this.downloadingLabel.Location = new System.Drawing.Point(3, 463);
			this.downloadingLabel.Name = "downloadingLabel";
			this.downloadingLabel.Size = new System.Drawing.Size(665, 24);
			this.downloadingLabel.TabIndex = 10;
			this.downloadingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// statusStrip
			// 
			this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.statusLabel});
			this.statusStrip.Location = new System.Drawing.Point(0, 533);
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.Size = new System.Drawing.Size(670, 22);
			this.statusStrip.SizingGrip = false;
			this.statusStrip.TabIndex = 11;
			// 
			// statusLabel
			// 
			this.statusLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.statusLabel.Font = new System.Drawing.Font("Segoe UI Light", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.statusLabel.ForeColor = System.Drawing.Color.Black;
			this.statusLabel.Name = "statusLabel";
			this.statusLabel.Size = new System.Drawing.Size(655, 17);
			this.statusLabel.Spring = true;
			// 
			// saveLocationDialog
			// 
			this.saveLocationDialog.Description = "Where to save the url files to.";
			// 
			// tempLocationDialog
			// 
			this.tempLocationDialog.Description = "Where to save the videos until they are finished.";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(670, 555);
			this.Controls.Add(this.statusStrip);
			this.Controls.Add(this.downloadingLabel);
			this.Controls.Add(this.downloadButton);
			this.Controls.Add(this.queueDownloadsTab);
			this.Controls.Add(this.videoDownloadProgressBar);
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Youtube Download Helper";
			this.queueDownloadsTab.ResumeLayout(false);
			this.tabPage3.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.newUrlRes)).EndInit();
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.resolutionToModify)).EndInit();
			this.tabPage4.ResumeLayout(false);
			this.tabControl2.ResumeLayout(false);
			this.tabPage5.ResumeLayout(false);
			this.tabPage5.PerformLayout();
			this.tabPage6.ResumeLayout(false);
			this.tabPage6.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
	}
}

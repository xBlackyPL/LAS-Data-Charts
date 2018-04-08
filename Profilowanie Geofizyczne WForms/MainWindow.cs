using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.AccessControl;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Profilowanie_Geofizyczne_WForms
{
    public partial class MainWindow : Form
    {
        private List<LasData> recentActiveFileList { get; }
        private LasData currentActiveFile;
        private List<double> currentDrawnChart;
        private string currentDrawnChartName;
        private bool loadedFile;
        private bool logScaleAvailabe;
        private bool useLogScale;
        Point? prevPosition = null;
        ToolTip tooltip = new ToolTip();

        public MainWindow()
        {
            InitializeComponent();
            loadedFile = false;
            Charts.Legends.Clear();

            Log.Enabled = false;
            useLogScale = false;
            logScaleAvailabe = false;
            recentActiveFileList = new List<LasData>();
            currentDrawnChart = new List<double>();
            CurrentActiveChartsComboBox.DropDownClosed += CurrentActiveChartsComboBoxOnDropDownClosed;
            ChartFileName.Text = "Load new LAS file...";
        }
        
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recentlyOpenedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.CurrentActiveChartsComboBox = new System.Windows.Forms.ComboBox();
            this.Log = new System.Windows.Forms.CheckBox();
            this.Charts = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.ChartFileName = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Charts)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.AutoSize = false;
            this.menuStrip1.BackColor = System.Drawing.SystemColors.MenuBar;
            this.menuStrip1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(1, 1, 0, 1);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.closeToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.menuStrip1.Size = new System.Drawing.Size(89, 24);
            this.menuStrip1.Stretch = false;
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "MainWindowMenu";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.recentlyOpenedToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(37, 24);
            this.toolStripMenuItem1.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // recentlyOpenedToolStripMenuItem
            // 
            this.recentlyOpenedToolStripMenuItem.Enabled = false;
            this.recentlyOpenedToolStripMenuItem.Name = "recentlyOpenedToolStripMenuItem";
            this.recentlyOpenedToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.recentlyOpenedToolStripMenuItem.Text = "&Recently Opened";
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(48, 24);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.flowLayoutPanel1.Controls.Add(this.flowLayoutPanel2);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 24);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(786, 570);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.Controls.Add(this.ChartFileName);
            this.flowLayoutPanel2.Controls.Add(this.CurrentActiveChartsComboBox);
            this.flowLayoutPanel2.Controls.Add(this.Log);
            this.flowLayoutPanel2.Controls.Add(this.Charts);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(776, 560);
            this.flowLayoutPanel2.TabIndex = 5;
            // 
            // CurrentActiveChartsComboBox
            // 
            this.CurrentActiveChartsComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CurrentActiveChartsComboBox.FormattingEnabled = true;
            this.CurrentActiveChartsComboBox.Location = new System.Drawing.Point(68, 3);
            this.CurrentActiveChartsComboBox.Name = "CurrentActiveChartsComboBox";
            this.CurrentActiveChartsComboBox.Size = new System.Drawing.Size(179, 21);
            this.CurrentActiveChartsComboBox.TabIndex = 3;
            // 
            // Log
            // 
            this.Log.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.Log.AutoSize = true;
            this.Log.Location = new System.Drawing.Point(253, 5);
            this.Log.Name = "Log";
            this.Log.Size = new System.Drawing.Size(118, 17);
            this.Log.TabIndex = 5;
            this.Log.Text = "Logarythmics Scale";
            this.Log.UseVisualStyleBackColor = true;
            this.Log.CheckedChanged += new System.EventHandler(this.Log_CheckedChanged);
            // 
            // Charts
            // 
            this.Charts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Charts.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.LeftRight;
            this.Charts.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea1.Name = "ChartArea1";
            this.Charts.ChartAreas.Add(chartArea1);
            this.Charts.Cursor = System.Windows.Forms.Cursors.Default;
            legend1.Name = "Legend1";
            this.Charts.Legends.Add(legend1);
            this.Charts.Location = new System.Drawing.Point(3, 30);
            this.Charts.Name = "Charts";
            this.Charts.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.SeaGreen;
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.Charts.Series.Add(series1);
            this.Charts.Size = new System.Drawing.Size(770, 527);
            this.Charts.TabIndex = 0;
            this.Charts.Text = "Charts";
            this.Charts.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Charts_MouseMove);
            // 
            // ChartFileName
            // 
            this.ChartFileName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ChartFileName.AutoSize = true;
            this.ChartFileName.Location = new System.Drawing.Point(3, 7);
            this.ChartFileName.Name = "ChartFileName";
            this.ChartFileName.Size = new System.Drawing.Size(59, 13);
            this.ChartFileName.TabIndex = 6;
            this.ChartFileName.Text = "Loaded file";
            // 
            // MainWindow
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(784, 584);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.menuStrip1);
            this.MinimumSize = new System.Drawing.Size(800, 623);
            this.Name = "MainWindow";
            this.ShowIcon = false;
            this.Text = "Profilowanie Geofizyczne";
            this.SizeChanged += new System.EventHandler(this.MainWindow_SizeChanged);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Charts)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void CurrentActiveChartsComboBoxOnDropDownClosed(object sender, EventArgs eventArgs)
        {
            if (loadedFile)
            {
                Log.Checked = false;
                currentDrawnChartName = CurrentActiveChartsComboBox.SelectedItem.ToString();
                currentDrawnChart = currentActiveFile.charts[currentDrawnChartName];
                drawChart();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "LAS files (*.las, *.LAS)|*.LAS;*.las|All files (*.*)|*.*";
            openFileDialog.Multiselect = false;
            openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            
            var wasOpenedRecently = false;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                foreach (var filename in openFileDialog.FileNames)
                {
                    var name = filename.Split('\\').Last();

                    foreach (var file in recentActiveFileList)
                        if (file.filename == name)
                        {
                            currentActiveFile = file;
                            wasOpenedRecently = true;
                            break;
                        }

                    if (!wasOpenedRecently)
                    {
                        if (recentActiveFileList.Count > 5) recentActiveFileList.Remove(recentActiveFileList.First());
                        var lasDataObj = new LasData(filename);
                        currentActiveFile = lasDataObj;
                        recentActiveFileList.Add(lasDataObj);

                        recentlyOpenedToolStripMenuItem.Enabled = true;

                        var lastOpenedFileList = recentlyOpenedToolStripMenuItem;
                        var lastOpenedFile = new ToolStripMenuItem{Text = lasDataObj.filename};

                        lastOpenedFile.Click += LastOpenedFileOnClick;
                        lastOpenedFile.Enabled = true;

                        lastOpenedFileList.DropDownItems.Add(lastOpenedFile);
                        loadedFile = true;
                    }
                    
                    adjustSelectionMenuForActiveChart();
                }
        }

        private void LastOpenedFileOnClick(object sender, EventArgs eventArgs)
        {
            CurrentActiveChartsComboBox.Items.Clear();

            var menuItem = (ToolStripMenuItem)sender;
            foreach (var file in recentActiveFileList)
                if (file.filename == menuItem.Text)
                    currentActiveFile = file;

            adjustSelectionMenuForActiveChart();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void adjustSelectionMenuForActiveChart()
        {
            Log.Checked = false;
            CurrentActiveChartsComboBox.Items.Clear();
            ChartFileName.Text = "Loaded file: " + currentActiveFile.filename;
             
            foreach (var chartType in currentActiveFile.charts)
            {
                if (chartType.Key == "DEPT" || chartType.Key == "DEPTH") continue;
                CurrentActiveChartsComboBox.Items.Add(chartType.Key);
                CurrentActiveChartsComboBox.ResetText();
            }
        }

        private void drawChart()
        {
            Charts.Size = new Size(this.Width-30, this.Height-105);
            Charts.ChartAreas.Clear();
            Charts.Titles.Clear();
            Charts.Legends.Clear();
            Charts.Series.Clear();
            Charts.Annotations.Clear();
            logScaleAvailabe = true;
            Charts.Invalidate();

            if (loadedFile)
            {
                var chartArea = new ChartArea();

                chartArea.AxisY.MinorGrid.LineColor = Color.LightGray;
                chartArea.AxisX.MinorGrid.LineColor = Color.LightGray;
                chartArea.AxisY.MinorGrid.Enabled = true;
                chartArea.AxisX.MinorGrid.Enabled = true;
                chartArea.AxisX.Title = currentDrawnChartName;
                chartArea.AxisX.TitleFont = new Font("Consolas", 14, FontStyle.Bold);
                chartArea.AxisY.Title = "Depth";
                chartArea.AxisY.TitleFont = new Font("Consolas", 14, FontStyle.Bold);
                chartArea.AxisY.IsReversed = true;
                chartArea.AxisY.Minimum = currentActiveFile.startValue;
                chartArea.AxisY.ScrollBar.Enabled = true;
                chartArea.AxisY.ScaleView.Zoomable = true;
                chartArea.CursorX.IsUserSelectionEnabled = true;
                chartArea.CursorY.IsUserSelectionEnabled = true;


                var series = new Series();
                series.ChartType = SeriesChartType.Line;
                series.BorderWidth = 1;

                var y = currentActiveFile.charts.First().Value.ToList();
                var x = currentDrawnChart.ToArray().ToList();


                for (int i = 0; i < x.Count; i++)
                {
                    if (Math.Abs(currentActiveFile.nullValue - x[i]) < 0.001)
                    {
                        y.RemoveAt(i);
                        x.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        series.Points.AddXY(x[i], y[i]);
                        if (x[i] < 0.0001)
                        {
                            logScaleAvailabe = false;
                        }
                    }
                }

                Log.Enabled = logScaleAvailabe;

                if (useLogScale)
                {
                    chartArea.AxisX.IsLogarithmic = true;
                }

                Charts.ChartAreas.Add(chartArea);
                Charts.Series.Add(series);
            }
        }
        
        private void Log_CheckedChanged(object sender, EventArgs e)
        {
            if (Log.Checked)
                useLogScale = true;
            else
                useLogScale = false;
            
            drawChart();
        }

        private void MainWindow_SizeChanged(object sender, EventArgs e)
        {
            drawChart();
        }

        private void Charts_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.Location;
            if (prevPosition.HasValue && pos == prevPosition.Value)
                return;
            tooltip.RemoveAll();
            prevPosition = pos;
            var results = Charts.HitTest(pos.X, pos.Y, false,
                ChartElementType.DataPoint);
            foreach (var result in results)
            {
                if (result.ChartElementType == ChartElementType.DataPoint)
                {
                    var prop = result.Object as DataPoint;
                    if (prop != null)
                    {
                        var pointXPixel = result.ChartArea.AxisX.ValueToPixelPosition(prop.XValue);
                        var pointYPixel = result.ChartArea.AxisY.ValueToPixelPosition(prop.YValues[0]);
                        
                        if (Math.Abs(pos.X - pointXPixel) < 4 && Math.Abs(pos.Y - pointYPixel) < 4)
                        {
                            tooltip.Show("X = " + prop.XValue.ToString("F3",CultureInfo.InvariantCulture) + "    Y = " + prop.YValues[0].ToString("F3", CultureInfo.InvariantCulture), this.Charts, pos.X, pos.Y - 15);
                        }
                    }
                }
            }
        }
    }
}
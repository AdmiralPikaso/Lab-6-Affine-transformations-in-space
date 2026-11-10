namespace Lab6
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.controlsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.polyhedronGroup = new System.Windows.Forms.GroupBox();
            this.polyhedronLabel = new System.Windows.Forms.Label();
            this.polyhedronCombo = new System.Windows.Forms.ComboBox();
            this.resetButton = new System.Windows.Forms.Button();
            this.axesCheckBox = new System.Windows.Forms.CheckBox();
            this.hiddenEdgesCheckBox = new System.Windows.Forms.CheckBox();
            this.infoLabel = new System.Windows.Forms.Label();
            this.canvas = new System.Windows.Forms.PictureBox();
            this.controlsPanel.SuspendLayout();
            this.polyhedronGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).BeginInit();
            this.SuspendLayout();
            //
            // controlsPanel
            //
            this.controlsPanel.AutoScroll = true;
            this.controlsPanel.Controls.Add(this.polyhedronGroup);
            this.controlsPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.controlsPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.controlsPanel.Location = new System.Drawing.Point(0, 0);
            this.controlsPanel.Name = "controlsPanel";
            this.controlsPanel.Padding = new System.Windows.Forms.Padding(6);
            this.controlsPanel.Size = new System.Drawing.Size(310, 700);
            this.controlsPanel.TabIndex = 0;
            this.controlsPanel.WrapContents = false;
            //
            // polyhedronGroup
            //
            this.polyhedronGroup.Controls.Add(this.polyhedronLabel);
            this.polyhedronGroup.Controls.Add(this.polyhedronCombo);
            this.polyhedronGroup.Controls.Add(this.resetButton);
            this.polyhedronGroup.Controls.Add(this.axesCheckBox);
            this.polyhedronGroup.Controls.Add(this.hiddenEdgesCheckBox);
            this.polyhedronGroup.Controls.Add(this.infoLabel);
            this.polyhedronGroup.Location = new System.Drawing.Point(9, 9);
            this.polyhedronGroup.Name = "polyhedronGroup";
            this.polyhedronGroup.Size = new System.Drawing.Size(270, 172);
            this.polyhedronGroup.TabIndex = 0;
            this.polyhedronGroup.TabStop = false;
            this.polyhedronGroup.Text = "Многогранник";
            //
            // polyhedronLabel
            //
            this.polyhedronLabel.AutoSize = true;
            this.polyhedronLabel.Location = new System.Drawing.Point(10, 24);
            this.polyhedronLabel.Name = "polyhedronLabel";
            this.polyhedronLabel.Size = new System.Drawing.Size(32, 15);
            this.polyhedronLabel.TabIndex = 0;
            this.polyhedronLabel.Text = "Вид:";
            //
            // polyhedronCombo
            //
            this.polyhedronCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.polyhedronCombo.FormattingEnabled = true;
            this.polyhedronCombo.Location = new System.Drawing.Point(10, 44);
            this.polyhedronCombo.Name = "polyhedronCombo";
            this.polyhedronCombo.Size = new System.Drawing.Size(250, 23);
            this.polyhedronCombo.TabIndex = 1;
            this.polyhedronCombo.SelectedIndexChanged += new System.EventHandler(this.polyhedronCombo_SelectedIndexChanged);
            //
            // resetButton
            //
            this.resetButton.Location = new System.Drawing.Point(10, 78);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(120, 27);
            this.resetButton.TabIndex = 2;
            this.resetButton.Text = "Сброс";
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
            //
            // axesCheckBox
            //
            this.axesCheckBox.AutoSize = true;
            this.axesCheckBox.Checked = true;
            this.axesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.axesCheckBox.Location = new System.Drawing.Point(145, 82);
            this.axesCheckBox.Name = "axesCheckBox";
            this.axesCheckBox.Size = new System.Drawing.Size(110, 19);
            this.axesCheckBox.TabIndex = 3;
            this.axesCheckBox.Text = "Показывать оси";
            this.axesCheckBox.UseVisualStyleBackColor = true;
            this.axesCheckBox.CheckedChanged += new System.EventHandler(this.axesCheckBox_CheckedChanged);
            //
            // hiddenEdgesCheckBox
            //
            this.hiddenEdgesCheckBox.AutoSize = true;
            this.hiddenEdgesCheckBox.Checked = true;
            this.hiddenEdgesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.hiddenEdgesCheckBox.Location = new System.Drawing.Point(10, 114);
            this.hiddenEdgesCheckBox.Name = "hiddenEdgesCheckBox";
            this.hiddenEdgesCheckBox.Size = new System.Drawing.Size(190, 19);
            this.hiddenEdgesCheckBox.TabIndex = 4;
            this.hiddenEdgesCheckBox.Text = "Невидимые рёбра пунктиром";
            this.hiddenEdgesCheckBox.UseVisualStyleBackColor = true;
            this.hiddenEdgesCheckBox.CheckedChanged += new System.EventHandler(this.hiddenEdgesCheckBox_CheckedChanged);
            //
            // infoLabel
            //
            this.infoLabel.AutoSize = true;
            this.infoLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.infoLabel.Location = new System.Drawing.Point(10, 144);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(180, 15);
            this.infoLabel.TabIndex = 5;
            this.infoLabel.Text = "Вершин: 0, рёбер: 0, граней: 0";
            //
            // canvas
            //
            this.canvas.BackColor = System.Drawing.Color.White;
            this.canvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.canvas.Location = new System.Drawing.Point(310, 0);
            this.canvas.Name = "canvas";
            this.canvas.Size = new System.Drawing.Size(790, 700);
            this.canvas.TabIndex = 1;
            this.canvas.TabStop = false;
            this.canvas.Paint += new System.Windows.Forms.PaintEventHandler(this.canvas_Paint);
            this.canvas.Resize += new System.EventHandler(this.canvas_Resize);
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 700);
            this.Controls.Add(this.canvas);
            this.Controls.Add(this.controlsPanel);
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Лаб. 6 — Аффинные преобразования в пространстве";
            this.controlsPanel.ResumeLayout(false);
            this.polyhedronGroup.ResumeLayout(false);
            this.polyhedronGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel controlsPanel;
        private System.Windows.Forms.GroupBox polyhedronGroup;
        private System.Windows.Forms.Label polyhedronLabel;
        private System.Windows.Forms.ComboBox polyhedronCombo;
        private System.Windows.Forms.Button resetButton;
        private System.Windows.Forms.CheckBox axesCheckBox;
        private System.Windows.Forms.CheckBox hiddenEdgesCheckBox;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.PictureBox canvas;
    }
}

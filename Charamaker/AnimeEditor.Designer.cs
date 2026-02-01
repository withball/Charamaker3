namespace Charamaker
{
    partial class AnimeEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            loadBox = new TextBox();
            timeLabel = new Label();
            SpeedUd = new NumericUpDown();
            PlayB = new Button();
            TimeBar = new TrackBar();
            MessageBox = new TextBox();
            CheckB = new Button();
            PlayTimeLabel = new Label();
            StartUD = new NumericUpDown();
            EndUD = new NumericUpDown();
            ResetDataB = new Button();
            blockB = new Button();
            ResetTectB = new Button();
            ((System.ComponentModel.ISupportInitialize)SpeedUd).BeginInit();
            ((System.ComponentModel.ISupportInitialize)TimeBar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)StartUD).BeginInit();
            ((System.ComponentModel.ISupportInitialize)EndUD).BeginInit();
            SuspendLayout();
            // 
            // loadBox
            // 
            loadBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            loadBox.Font = new Font("Yu Gothic UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            loadBox.Location = new Point(12, 12);
            loadBox.Name = "loadBox";
            loadBox.Size = new Size(776, 32);
            loadBox.TabIndex = 0;
            loadBox.TextChanged += loadBox_TextChanged;
            // 
            // timeLabel
            // 
            timeLabel.AutoSize = true;
            timeLabel.Font = new Font("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            timeLabel.Location = new Point(12, 56);
            timeLabel.Name = "timeLabel";
            timeLabel.Size = new Size(42, 21);
            timeLabel.TabIndex = 1;
            timeLabel.Text = "0 / 0";
            // 
            // SpeedUd
            // 
            SpeedUd.DecimalPlaces = 1;
            SpeedUd.Font = new Font("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            SpeedUd.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            SpeedUd.Location = new Point(569, 56);
            SpeedUd.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            SpeedUd.Minimum = new decimal(new int[] { 10, 0, 0, int.MinValue });
            SpeedUd.Name = "SpeedUd";
            SpeedUd.Size = new Size(120, 29);
            SpeedUd.TabIndex = 2;
            SpeedUd.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // PlayB
            // 
            PlayB.Font = new Font("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            PlayB.Location = new Point(695, 56);
            PlayB.Name = "PlayB";
            PlayB.Size = new Size(75, 35);
            PlayB.TabIndex = 3;
            PlayB.Text = "Play";
            PlayB.UseVisualStyleBackColor = true;
            PlayB.Click += PlayB_Click;
            // 
            // TimeBar
            // 
            TimeBar.Location = new Point(12, 111);
            TimeBar.Name = "TimeBar";
            TimeBar.Size = new Size(776, 45);
            TimeBar.TabIndex = 4;
            TimeBar.Scroll += TimeBar_Scroll;
            // 
            // MessageBox
            // 
            MessageBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            MessageBox.Font = new Font("Yu Gothic UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            MessageBox.Location = new Point(12, 162);
            MessageBox.Multiline = true;
            MessageBox.Name = "MessageBox";
            MessageBox.ScrollBars = ScrollBars.Both;
            MessageBox.Size = new Size(776, 276);
            MessageBox.TabIndex = 5;
            // 
            // CheckB
            // 
            CheckB.Font = new Font("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            CheckB.Location = new Point(356, 50);
            CheckB.Name = "CheckB";
            CheckB.Size = new Size(75, 35);
            CheckB.TabIndex = 6;
            CheckB.Text = "Check";
            CheckB.UseVisualStyleBackColor = true;
            CheckB.Click += CheckB_Click;
            // 
            // PlayTimeLabel
            // 
            PlayTimeLabel.AutoSize = true;
            PlayTimeLabel.Font = new Font("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            PlayTimeLabel.Location = new Point(12, 80);
            PlayTimeLabel.Name = "PlayTimeLabel";
            PlayTimeLabel.Size = new Size(42, 21);
            PlayTimeLabel.TabIndex = 7;
            PlayTimeLabel.Text = "0 / 0";
            PlayTimeLabel.Click += PlayTimeLabel_Click;
            // 
            // StartUD
            // 
            StartUD.DecimalPlaces = 1;
            StartUD.Font = new Font("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            StartUD.Location = new Point(230, 50);
            StartUD.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            StartUD.Name = "StartUD";
            StartUD.Size = new Size(120, 29);
            StartUD.TabIndex = 8;
            StartUD.ValueChanged += StartUD_ValueChanged;
            // 
            // EndUD
            // 
            EndUD.DecimalPlaces = 1;
            EndUD.Font = new Font("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            EndUD.Location = new Point(230, 85);
            EndUD.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            EndUD.Name = "EndUD";
            EndUD.Size = new Size(120, 29);
            EndUD.TabIndex = 9;
            EndUD.Value = new decimal(new int[] { 10000, 0, 0, 0 });
            EndUD.ValueChanged += EndUD_ValueChanged;
            // 
            // ResetDataB
            // 
            ResetDataB.Font = new Font("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            ResetDataB.Location = new Point(472, 50);
            ResetDataB.Name = "ResetDataB";
            ResetDataB.Size = new Size(71, 27);
            ResetDataB.TabIndex = 10;
            ResetDataB.Text = "Reset";
            ResetDataB.UseVisualStyleBackColor = true;
            ResetDataB.Click += ResetDataB_Click;
            // 
            // blockB
            // 
            blockB.Font = new Font("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            blockB.Location = new Point(371, 91);
            blockB.Name = "blockB";
            blockB.Size = new Size(75, 29);
            blockB.TabIndex = 11;
            blockB.Text = "ShowBlock";
            blockB.UseVisualStyleBackColor = true;
            blockB.Click += blockB_Click;
            // 
            // ResetTectB
            // 
            ResetTectB.Font = new Font("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            ResetTectB.Location = new Point(489, 87);
            ResetTectB.Name = "ResetTectB";
            ResetTectB.Size = new Size(71, 27);
            ResetTectB.TabIndex = 12;
            ResetTectB.Text = "TReset";
            ResetTectB.UseVisualStyleBackColor = true;
            ResetTectB.Click += ResetTectB_Click;
            // 
            // AnimeEditor
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(ResetTectB);
            Controls.Add(blockB);
            Controls.Add(ResetDataB);
            Controls.Add(EndUD);
            Controls.Add(StartUD);
            Controls.Add(PlayTimeLabel);
            Controls.Add(CheckB);
            Controls.Add(MessageBox);
            Controls.Add(TimeBar);
            Controls.Add(PlayB);
            Controls.Add(SpeedUd);
            Controls.Add(timeLabel);
            Controls.Add(loadBox);
            Name = "AnimeEditor";
            Text = "AnimeEditor";
            FormClosed += close;
            Load += AnimeEditor_Load;
            Shown += shown;
            ((System.ComponentModel.ISupportInitialize)SpeedUd).EndInit();
            ((System.ComponentModel.ISupportInitialize)TimeBar).EndInit();
            ((System.ComponentModel.ISupportInitialize)StartUD).EndInit();
            ((System.ComponentModel.ISupportInitialize)EndUD).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox loadBox;
        private Label timeLabel;
        private NumericUpDown SpeedUd;
        private Button PlayB;
        private TrackBar TimeBar;
        private TextBox MessageBox;
        private Button CheckB;
        private Label PlayTimeLabel;
        private NumericUpDown StartUD;
        private NumericUpDown EndUD;
        private Button ResetDataB;
        private Button blockB;
        private Button ResetTectB;
    }
}
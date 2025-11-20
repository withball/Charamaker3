namespace Charamaker
{
    partial class Charamaker
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
            components = new System.ComponentModel.Container();
            timer1 = new System.Windows.Forms.Timer(components);
            loadB = new Button();
            textB = new TextBox();
            removeB = new Button();
            saveB = new Button();
            zoomUD = new NumericUpDown();
            DSB = new TextBox();
            messageB = new TextBox();
            setBaseB = new Button();
            PointB = new TrackBar();
            motionB = new Button();
            screenshotB = new Button();
            animationB = new Button();
            printoutB = new Button();
            texturelabel = new Label();
            rootpathbox = new TextBox();
            selectB = new Button();
            SpeedBar = new TrackBar();
            speedLabel = new Label();
            ResetTextureB = new Button();
            ForceMirrorButton = new Button();
            CheckB = new Button();
            ((System.ComponentModel.ISupportInitialize)zoomUD).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PointB).BeginInit();
            ((System.ComponentModel.ISupportInitialize)SpeedBar).BeginInit();
            SuspendLayout();
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 25;
            timer1.Tick += ticked;
            // 
            // loadB
            // 
            loadB.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            loadB.Location = new Point(444, 826);
            loadB.Name = "loadB";
            loadB.Size = new Size(75, 23);
            loadB.TabIndex = 0;
            loadB.Text = "load";
            loadB.UseVisualStyleBackColor = true;
            loadB.Click += loadB_Click;
            // 
            // textB
            // 
            textB.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            textB.Location = new Point(12, 826);
            textB.Name = "textB";
            textB.Size = new Size(416, 23);
            textB.TabIndex = 1;
            // 
            // removeB
            // 
            removeB.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            removeB.Location = new Point(606, 826);
            removeB.Name = "removeB";
            removeB.Size = new Size(75, 23);
            removeB.TabIndex = 2;
            removeB.Text = "remove";
            removeB.UseVisualStyleBackColor = true;
            removeB.Click += removeB_Click;
            // 
            // saveB
            // 
            saveB.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            saveB.Location = new Point(525, 826);
            saveB.Name = "saveB";
            saveB.Size = new Size(75, 23);
            saveB.TabIndex = 3;
            saveB.Text = "save";
            saveB.UseVisualStyleBackColor = true;
            saveB.Click += saveB_Click;
            // 
            // zoomUD
            // 
            zoomUD.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            zoomUD.DecimalPlaces = 1;
            zoomUD.Increment = new decimal(new int[] { 5, 0, 0, 65536 });
            zoomUD.Location = new Point(1510, 7);
            zoomUD.Margin = new Padding(2);
            zoomUD.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            zoomUD.Minimum = new decimal(new int[] { 1, 0, 0, 65536 });
            zoomUD.Name = "zoomUD";
            zoomUD.Size = new Size(66, 23);
            zoomUD.TabIndex = 4;
            zoomUD.TextAlign = HorizontalAlignment.Center;
            zoomUD.Value = new decimal(new int[] { 1, 0, 0, 0 });
            zoomUD.ValueChanged += zoomUD_ValueChanged;
            // 
            // DSB
            // 
            DSB.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            DSB.Location = new Point(1105, 52);
            DSB.Margin = new Padding(2);
            DSB.Multiline = true;
            DSB.Name = "DSB";
            DSB.ScrollBars = ScrollBars.Both;
            DSB.Size = new Size(459, 662);
            DSB.TabIndex = 5;
            DSB.WordWrap = false;
            DSB.TextChanged += DSB_TextChanged;
            DSB.KeyUp += DSB_KeyUp;
            // 
            // messageB
            // 
            messageB.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            messageB.Location = new Point(1105, 747);
            messageB.Margin = new Padding(2);
            messageB.Multiline = true;
            messageB.Name = "messageB";
            messageB.ScrollBars = ScrollBars.Both;
            messageB.Size = new Size(459, 109);
            messageB.TabIndex = 6;
            // 
            // setBaseB
            // 
            setBaseB.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            setBaseB.Location = new Point(1489, 719);
            setBaseB.Name = "setBaseB";
            setBaseB.Size = new Size(75, 23);
            setBaseB.TabIndex = 7;
            setBaseB.Text = "setBase";
            setBaseB.UseVisualStyleBackColor = true;
            setBaseB.Click += setBaseB_Click;
            // 
            // PointB
            // 
            PointB.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            PointB.Location = new Point(1369, 7);
            PointB.Margin = new Padding(2);
            PointB.Maximum = 100;
            PointB.Name = "PointB";
            PointB.Size = new Size(127, 45);
            PointB.TabIndex = 8;
            PointB.Value = 50;
            // 
            // motionB
            // 
            motionB.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            motionB.Location = new Point(687, 826);
            motionB.Name = "motionB";
            motionB.Size = new Size(75, 23);
            motionB.TabIndex = 9;
            motionB.Text = "motion";
            motionB.UseVisualStyleBackColor = true;
            motionB.Click += motionB_Click;
            // 
            // screenshotB
            // 
            screenshotB.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            screenshotB.Location = new Point(1105, 719);
            screenshotB.Name = "screenshotB";
            screenshotB.Size = new Size(75, 23);
            screenshotB.TabIndex = 10;
            screenshotB.Text = "screenshot";
            screenshotB.UseVisualStyleBackColor = true;
            screenshotB.Click += screenshotB_Click;
            // 
            // animationB
            // 
            animationB.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            animationB.Location = new Point(768, 825);
            animationB.Name = "animationB";
            animationB.Size = new Size(75, 23);
            animationB.TabIndex = 11;
            animationB.Text = "animation";
            animationB.UseVisualStyleBackColor = true;
            animationB.Click += animationB_Click;
            // 
            // printoutB
            // 
            printoutB.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            printoutB.Location = new Point(1235, 720);
            printoutB.Name = "printoutB";
            printoutB.Size = new Size(75, 23);
            printoutB.TabIndex = 12;
            printoutB.Text = "printout";
            printoutB.UseVisualStyleBackColor = true;
            printoutB.Click += printoutB_Click;
            // 
            // texturelabel
            // 
            texturelabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            texturelabel.AutoSize = true;
            texturelabel.Location = new Point(1191, 724);
            texturelabel.Name = "texturelabel";
            texturelabel.Size = new Size(38, 15);
            texturelabel.TabIndex = 13;
            texturelabel.Text = "label1";
            texturelabel.Click += label1_Click;
            // 
            // rootpathbox
            // 
            rootpathbox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            rootpathbox.Location = new Point(849, 825);
            rootpathbox.Name = "rootpathbox";
            rootpathbox.Size = new Size(246, 23);
            rootpathbox.TabIndex = 14;
            rootpathbox.TextChanged += textBox1_TextChanged;
            // 
            // selectB
            // 
            selectB.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            selectB.Location = new Point(12, 797);
            selectB.Name = "selectB";
            selectB.Size = new Size(75, 23);
            selectB.TabIndex = 15;
            selectB.Text = "select";
            selectB.UseVisualStyleBackColor = true;
            selectB.Click += selectB_Click;
            // 
            // SpeedBar
            // 
            SpeedBar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            SpeedBar.Location = new Point(1105, 3);
            SpeedBar.Margin = new Padding(2);
            SpeedBar.Maximum = 100;
            SpeedBar.Name = "SpeedBar";
            SpeedBar.Size = new Size(127, 45);
            SpeedBar.TabIndex = 16;
            SpeedBar.Value = 100;
            // 
            // speedLabel
            // 
            speedLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            speedLabel.AutoSize = true;
            speedLabel.Location = new Point(1237, 7);
            speedLabel.Name = "speedLabel";
            speedLabel.Size = new Size(39, 15);
            speedLabel.TabIndex = 17;
            speedLabel.Text = "Speed";
            // 
            // ResetTextureB
            // 
            ResetTextureB.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            ResetTextureB.Location = new Point(1316, 720);
            ResetTextureB.Name = "ResetTextureB";
            ResetTextureB.Size = new Size(75, 23);
            ResetTextureB.TabIndex = 18;
            ResetTextureB.Text = "ResetTexture";
            ResetTextureB.UseVisualStyleBackColor = true;
            ResetTextureB.Click += ResetTextureB_Click;
            // 
            // ForceMirrorButton
            // 
            ForceMirrorButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            ForceMirrorButton.Location = new Point(1408, 720);
            ForceMirrorButton.Name = "ForceMirrorButton";
            ForceMirrorButton.Size = new Size(75, 23);
            ForceMirrorButton.TabIndex = 19;
            ForceMirrorButton.Text = "ForceMirror";
            ForceMirrorButton.UseVisualStyleBackColor = true;
            ForceMirrorButton.Click += ForceMirrorButton_Click;
            // 
            // CheckB
            // 
            CheckB.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            CheckB.Font = new Font("Yu Gothic UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            CheckB.Location = new Point(1289, 7);
            CheckB.Name = "CheckB";
            CheckB.Size = new Size(75, 23);
            CheckB.TabIndex = 20;
            CheckB.Text = "Check";
            CheckB.UseVisualStyleBackColor = true;
            CheckB.Click += CheckB_Click;
            // 
            // Charamaker
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1584, 861);
            Controls.Add(CheckB);
            Controls.Add(ForceMirrorButton);
            Controls.Add(ResetTextureB);
            Controls.Add(speedLabel);
            Controls.Add(SpeedBar);
            Controls.Add(selectB);
            Controls.Add(rootpathbox);
            Controls.Add(texturelabel);
            Controls.Add(printoutB);
            Controls.Add(animationB);
            Controls.Add(screenshotB);
            Controls.Add(motionB);
            Controls.Add(PointB);
            Controls.Add(setBaseB);
            Controls.Add(messageB);
            Controls.Add(DSB);
            Controls.Add(zoomUD);
            Controls.Add(saveB);
            Controls.Add(removeB);
            Controls.Add(textB);
            Controls.Add(loadB);
            Margin = new Padding(2);
            Name = "Charamaker";
            Text = "Charamaker3Test";
            Load += Form1_Load;
            KeyDown += OnKeyDown;
            KeyUp += OnKeyUp;
            MouseDown += OnMouseDown;
            MouseUp += OnMouseUp;
            Resize += Resized;
            ((System.ComponentModel.ISupportInitialize)zoomUD).EndInit();
            ((System.ComponentModel.ISupportInitialize)PointB).EndInit();
            ((System.ComponentModel.ISupportInitialize)SpeedBar).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private Button loadB;
        private TextBox textB;
        private Button removeB;
        private Button saveB;
        private NumericUpDown zoomUD;
        private TextBox DSB;
        private TextBox messageB;
        private Button setBaseB;
        private Button motionB;
        private Button screenshotB;
        private Button animationB;
        private Button printoutB;
        private Label texturelabel;
        private TextBox rootpathbox;
        private Button selectB;
        private TrackBar SpeedBar;
        private Label speedLabel;
        private Button ResetTextureB;
        private Button ForceMirrorButton;
        private Button CheckB;
        public TrackBar PointB;
    }
}
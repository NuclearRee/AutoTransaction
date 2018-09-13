namespace AutoTransaction
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Code = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Condition = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Price = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CurrentPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.groupbox2 = new System.Windows.Forms.GroupBox();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.Start = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupbox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dataGridView1);
            this.groupBox1.Font = new System.Drawing.Font("宋体", 9F);
            this.groupBox1.Location = new System.Drawing.Point(5, 344);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(750, 226);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "预警列表";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Code,
            this.Condition,
            this.Time,
            this.Price,
            this.CurrentPrice});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.GridColor = System.Drawing.SystemColors.ButtonFace;
            this.dataGridView1.Location = new System.Drawing.Point(3, 21);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 27;
            this.dataGridView1.Size = new System.Drawing.Size(744, 202);
            this.dataGridView1.TabIndex = 0;
            // 
            // Code
            // 
            this.Code.DataPropertyName = "code";
            this.Code.HeaderText = "股票代码";
            this.Code.MinimumWidth = 180;
            this.Code.Name = "Code";
            this.Code.ReadOnly = true;
            this.Code.Width = 180;
            // 
            // Condition
            // 
            this.Condition.DataPropertyName = "condition";
            this.Condition.HeaderText = "预警条件";
            this.Condition.MinimumWidth = 120;
            this.Condition.Name = "Condition";
            this.Condition.ReadOnly = true;
            this.Condition.Width = 120;
            // 
            // Time
            // 
            this.Time.DataPropertyName = "time";
            this.Time.HeaderText = "预警时间";
            this.Time.MinimumWidth = 180;
            this.Time.Name = "Time";
            this.Time.ReadOnly = true;
            this.Time.Width = 180;
            // 
            // Price
            // 
            this.Price.DataPropertyName = "price";
            this.Price.HeaderText = "预警价格";
            this.Price.MinimumWidth = 120;
            this.Price.Name = "Price";
            this.Price.ReadOnly = true;
            this.Price.Width = 120;
            // 
            // CurrentPrice
            // 
            this.CurrentPrice.DataPropertyName = "nowprice";
            this.CurrentPrice.HeaderText = "现价/盈亏";
            this.CurrentPrice.MinimumWidth = 150;
            this.CurrentPrice.Name = "CurrentPrice";
            this.CurrentPrice.ReadOnly = true;
            this.CurrentPrice.Width = 150;
            // 
            // groupbox2
            // 
            this.groupbox2.Controls.Add(this.button5);
            this.groupbox2.Controls.Add(this.button4);
            this.groupbox2.Controls.Add(this.textBox1);
            this.groupbox2.Controls.Add(this.button3);
            this.groupbox2.Controls.Add(this.button2);
            this.groupbox2.Controls.Add(this.button1);
            this.groupbox2.Controls.Add(this.Start);
            this.groupbox2.Location = new System.Drawing.Point(5, 6);
            this.groupbox2.Name = "groupbox2";
            this.groupbox2.Size = new System.Drawing.Size(750, 332);
            this.groupbox2.TabIndex = 1;
            this.groupbox2.TabStop = false;
            this.groupbox2.Text = "运行及参数配置";
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(555, 178);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(180, 45);
            this.button5.TabIndex = 7;
            this.button5.Text = "预警检测";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(555, 127);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(180, 45);
            this.button4.TabIndex = 6;
            this.button4.Text = "持仓检测";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(7, 24);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(542, 301);
            this.textBox1.TabIndex = 5;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(555, 280);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(180, 45);
            this.button3.TabIndex = 4;
            this.button3.Text = "停止运行";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(555, 25);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(180, 45);
            this.button2.TabIndex = 1;
            this.button2.Text = "加载句柄";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(555, 76);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(180, 45);
            this.button1.TabIndex = 2;
            this.button1.Text = "参数配置";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Start
            // 
            this.Start.Location = new System.Drawing.Point(555, 229);
            this.Start.Name = "Start";
            this.Start.Size = new System.Drawing.Size(180, 45);
            this.Start.TabIndex = 3;
            this.Start.Text = "开始运行";
            this.Start.UseVisualStyleBackColor = true;
            this.Start.Click += new System.EventHandler(this.Start_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(766, 572);
            this.Controls.Add(this.groupbox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("宋体", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.Text = "自动下单";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupbox2.ResumeLayout(false);
            this.groupbox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.GroupBox groupbox2;
        private System.Windows.Forms.Button Start;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Code;
        private System.Windows.Forms.DataGridViewTextBoxColumn Condition;
        private System.Windows.Forms.DataGridViewTextBoxColumn Time;
        private System.Windows.Forms.DataGridViewTextBoxColumn Price;
        private System.Windows.Forms.DataGridViewTextBoxColumn CurrentPrice;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
    }
}


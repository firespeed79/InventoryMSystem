namespace InventoryMSystem
{
    partial class frmInventory
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmInventory));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dgvProductInfo = new System.Windows.Forms.DataGridView();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnQuit = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnScan = new System.Windows.Forms.Button();
            this.btnResult = new System.Windows.Forms.Button();
            this.cbLess = new System.Windows.Forms.CheckBox();
            this.cbEqual = new System.Windows.Forms.CheckBox();
            this.cbMore = new System.Windows.Forms.CheckBox();
            this.lblLess = new System.Windows.Forms.Label();
            this.lblEqual = new System.Windows.Forms.Label();
            this.lblMore = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dgvProductInfo);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(735, 458);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "盘点产品信息";
            // 
            // dgvProductInfo
            // 
            this.dgvProductInfo.AllowUserToAddRows = false;
            this.dgvProductInfo.AllowUserToDeleteRows = false;
            this.dgvProductInfo.AllowUserToResizeRows = false;
            this.dgvProductInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProductInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvProductInfo.Location = new System.Drawing.Point(3, 17);
            this.dgvProductInfo.Name = "dgvProductInfo";
            this.dgvProductInfo.ReadOnly = true;
            this.dgvProductInfo.RowHeadersWidth = 20;
            this.dgvProductInfo.RowTemplate.Height = 23;
            this.dgvProductInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvProductInfo.Size = new System.Drawing.Size(729, 438);
            this.dgvProductInfo.TabIndex = 0;
            this.dgvProductInfo.TabStop = false;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(772, 81);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(93, 30);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "重新盘点";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnQuit
            // 
            this.btnQuit.Location = new System.Drawing.Point(772, 537);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(93, 30);
            this.btnQuit.TabIndex = 2;
            this.btnQuit.Text = "退出";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Location = new System.Drawing.Point(16, 505);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(934, 10);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            // 
            // btnScan
            // 
            this.btnScan.Location = new System.Drawing.Point(772, 27);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(93, 30);
            this.btnScan.TabIndex = 4;
            this.btnScan.Text = "开始盘点";
            this.btnScan.UseVisualStyleBackColor = true;
            this.btnScan.Click += new System.EventHandler(this.btnScan_Click);
            // 
            // btnResult
            // 
            this.btnResult.Location = new System.Drawing.Point(772, 135);
            this.btnResult.Name = "btnResult";
            this.btnResult.Size = new System.Drawing.Size(93, 30);
            this.btnResult.TabIndex = 5;
            this.btnResult.Text = "盘点结果";
            this.btnResult.UseVisualStyleBackColor = true;
            this.btnResult.Visible = false;
            this.btnResult.Click += new System.EventHandler(this.btnResult_Click);
            // 
            // cbLess
            // 
            this.cbLess.AutoSize = true;
            this.cbLess.Location = new System.Drawing.Point(25, 484);
            this.cbLess.Name = "cbLess";
            this.cbLess.Size = new System.Drawing.Size(48, 16);
            this.cbLess.TabIndex = 6;
            this.cbLess.Text = "盘亏";
            this.cbLess.UseVisualStyleBackColor = true;
            // 
            // cbEqual
            // 
            this.cbEqual.AutoSize = true;
            this.cbEqual.Checked = true;
            this.cbEqual.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbEqual.Location = new System.Drawing.Point(117, 484);
            this.cbEqual.Name = "cbEqual";
            this.cbEqual.Size = new System.Drawing.Size(72, 16);
            this.cbEqual.TabIndex = 6;
            this.cbEqual.Text = "账物相符";
            this.cbEqual.UseVisualStyleBackColor = true;
            // 
            // cbMore
            // 
            this.cbMore.AutoSize = true;
            this.cbMore.Checked = true;
            this.cbMore.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.cbMore.Location = new System.Drawing.Point(230, 484);
            this.cbMore.Name = "cbMore";
            this.cbMore.Size = new System.Drawing.Size(48, 16);
            this.cbMore.TabIndex = 6;
            this.cbMore.Text = "盘盈";
            this.cbMore.UseVisualStyleBackColor = true;
            // 
            // lblLess
            // 
            this.lblLess.AutoSize = true;
            this.lblLess.Location = new System.Drawing.Point(70, 485);
            this.lblLess.Name = "lblLess";
            this.lblLess.Size = new System.Drawing.Size(41, 12);
            this.lblLess.TabIndex = 7;
            this.lblLess.Text = "说明：";
            // 
            // lblEqual
            // 
            this.lblEqual.AutoSize = true;
            this.lblEqual.Location = new System.Drawing.Point(186, 486);
            this.lblEqual.Name = "lblEqual";
            this.lblEqual.Size = new System.Drawing.Size(41, 12);
            this.lblEqual.TabIndex = 7;
            this.lblEqual.Text = "说明：";
            // 
            // lblMore
            // 
            this.lblMore.AutoSize = true;
            this.lblMore.Location = new System.Drawing.Point(284, 485);
            this.lblMore.Name = "lblMore";
            this.lblMore.Size = new System.Drawing.Size(41, 12);
            this.lblMore.TabIndex = 7;
            this.lblMore.Text = "说明：";
            // 
            // frmInventory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(877, 579);
            this.Controls.Add(this.lblMore);
            this.Controls.Add(this.lblEqual);
            this.Controls.Add(this.lblLess);
            this.Controls.Add(this.cbMore);
            this.Controls.Add(this.cbEqual);
            this.Controls.Add(this.cbLess);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnResult);
            this.Controls.Add(this.btnScan);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnRefresh);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmInventory";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "库存盘点";
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductInfo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dgvProductInfo;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnScan;
        private System.Windows.Forms.Button btnResult;
        private System.Windows.Forms.CheckBox cbLess;
        private System.Windows.Forms.CheckBox cbEqual;
        private System.Windows.Forms.CheckBox cbMore;
        private System.Windows.Forms.Label lblLess;
        private System.Windows.Forms.Label lblEqual;
        private System.Windows.Forms.Label lblMore;
    }
}
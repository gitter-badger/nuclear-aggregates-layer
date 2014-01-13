namespace DoubleGis.Erm.BLCore.Reports.UI
{
	partial class ReportMakerForm
	{
		/// <summary>
		/// Требуется переменная конструктора.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Освободить все используемые ресурсы.
		/// </summary>
		/// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Код, автоматически созданный конструктором форм Windows

		/// <summary>
		/// Обязательный метод для поддержки конструктора - не изменяйте
		/// содержимое данного метода при помощи редактора кода.
		/// </summary>
		private void InitializeComponent()
		{
            this.btnExecute = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbLegalPersonPaymentsReport = new System.Windows.Forms.RadioButton();
            this.rbPlanningReport = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnCheckConnection = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDataBase = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSqlServer = new System.Windows.Forms.TextBox();
            this.rbAuthenticationNT = new System.Windows.Forms.RadioButton();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.pgParameters = new System.Windows.Forms.PropertyGrid();
            this.pgConnections = new System.Windows.Forms.PropertyGrid();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnExecute
            // 
            this.btnExecute.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExecute.Location = new System.Drawing.Point(12, 653);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(622, 49);
            this.btnExecute.TabIndex = 2;
            this.btnExecute.Text = "Сформировать отчет";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.rbLegalPersonPaymentsReport);
            this.groupBox1.Controls.Add(this.rbPlanningReport);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(197, 635);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Отчеты";
            // 
            // rbLegalPersonPaymentsReport
            // 
            this.rbLegalPersonPaymentsReport.AutoSize = true;
            this.rbLegalPersonPaymentsReport.Location = new System.Drawing.Point(6, 42);
            this.rbLegalPersonPaymentsReport.Name = "rbLegalPersonPaymentsReport";
            this.rbLegalPersonPaymentsReport.Size = new System.Drawing.Size(189, 17);
            this.rbLegalPersonPaymentsReport.TabIndex = 6;
            this.rbLegalPersonPaymentsReport.TabStop = true;
            this.rbLegalPersonPaymentsReport.Text = "Отчет по оплатам юр. лиц (ERM)";
            this.rbLegalPersonPaymentsReport.UseVisualStyleBackColor = true;
            this.rbLegalPersonPaymentsReport.CheckedChanged += new System.EventHandler(this.reportRadioButton_CheckedChanged);
            // 
            // rbPlanningReport
            // 
            this.rbPlanningReport.AutoSize = true;
            this.rbPlanningReport.Location = new System.Drawing.Point(6, 19);
            this.rbPlanningReport.Name = "rbPlanningReport";
            this.rbPlanningReport.Size = new System.Drawing.Size(179, 17);
            this.rbPlanningReport.TabIndex = 4;
            this.rbPlanningReport.TabStop = true;
            this.rbPlanningReport.Text = "Отчет по планированию (ERM)";
            this.rbPlanningReport.UseVisualStyleBackColor = true;
            this.rbPlanningReport.CheckedChanged += new System.EventHandler(this.reportRadioButton_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.btnCheckConnection);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.txtPassword);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.txtUserName);
            this.groupBox2.Controls.Add(this.radioButton1);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.txtDataBase);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.txtSqlServer);
            this.groupBox2.Controls.Add(this.rbAuthenticationNT);
            this.groupBox2.Location = new System.Drawing.Point(12, 290);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(197, 216);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Соединение";
            // 
            // btnCheckConnection
            // 
            this.btnCheckConnection.Location = new System.Drawing.Point(6, 173);
            this.btnCheckConnection.Name = "btnCheckConnection";
            this.btnCheckConnection.Size = new System.Drawing.Size(185, 37);
            this.btnCheckConnection.TabIndex = 10;
            this.btnCheckConnection.Text = "Проверить соединение";
            this.btnCheckConnection.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 150);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Password";
            // 
            // txtPassword
            // 
            this.txtPassword.Enabled = false;
            this.txtPassword.Location = new System.Drawing.Point(71, 147);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(116, 20);
            this.txtPassword.TabIndex = 8;
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 124);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "UserName";
            // 
            // txtUserName
            // 
            this.txtUserName.Enabled = false;
            this.txtUserName.Location = new System.Drawing.Point(71, 121);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(116, 20);
            this.txtUserName.TabIndex = 6;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(9, 94);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(148, 17);
            this.radioButton1.TabIndex = 5;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "SQL Server авторизация";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "DataBase";
            // 
            // txtDataBase
            // 
            this.txtDataBase.Location = new System.Drawing.Point(71, 45);
            this.txtDataBase.Name = "txtDataBase";
            this.txtDataBase.Size = new System.Drawing.Size(116, 20);
            this.txtDataBase.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Server";
            // 
            // txtSqlServer
            // 
            this.txtSqlServer.Location = new System.Drawing.Point(71, 19);
            this.txtSqlServer.Name = "txtSqlServer";
            this.txtSqlServer.Size = new System.Drawing.Size(116, 20);
            this.txtSqlServer.TabIndex = 1;
            this.txtSqlServer.Text = "localhost";
            this.txtSqlServer.TextChanged += new System.EventHandler(this.txtSqlServer_TextChanged);
            // 
            // rbAuthenticationNT
            // 
            this.rbAuthenticationNT.AutoSize = true;
            this.rbAuthenticationNT.Checked = true;
            this.rbAuthenticationNT.Location = new System.Drawing.Point(9, 71);
            this.rbAuthenticationNT.Name = "rbAuthenticationNT";
            this.rbAuthenticationNT.Size = new System.Drawing.Size(137, 17);
            this.rbAuthenticationNT.TabIndex = 0;
            this.rbAuthenticationNT.TabStop = true;
            this.rbAuthenticationNT.Text = "Windows авторизация";
            this.rbAuthenticationNT.UseVisualStyleBackColor = true;
            this.rbAuthenticationNT.CheckedChanged += new System.EventHandler(this.rbAuthenticationNT_CheckedChanged);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "xlsx";
            this.saveFileDialog.Filter = "Excel 2007 files|*.xlsx";
            this.saveFileDialog.SupportMultiDottedExtensions = true;
            this.saveFileDialog.Title = "Выберите папку для сохранения файла отчета";
            // 
            // pgParameters
            // 
            this.pgParameters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgParameters.HelpVisible = false;
            this.pgParameters.Location = new System.Drawing.Point(3, 16);
            this.pgParameters.Name = "pgParameters";
            this.pgParameters.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.pgParameters.Size = new System.Drawing.Size(410, 348);
            this.pgParameters.TabIndex = 8;
            // 
            // pgConnections
            // 
            this.pgConnections.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgConnections.HelpVisible = false;
            this.pgConnections.Location = new System.Drawing.Point(3, 16);
            this.pgConnections.Name = "pgConnections";
            this.pgConnections.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.pgConnections.Size = new System.Drawing.Size(413, 246);
            this.pgConnections.TabIndex = 9;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.pgConnections);
            this.groupBox3.Location = new System.Drawing.Point(215, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(419, 265);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Соединения";
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.pgParameters);
            this.groupBox4.Location = new System.Drawing.Point(218, 280);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(416, 367);
            this.groupBox4.TabIndex = 11;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Параметры";
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // ReportMakerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(646, 705);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnExecute);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(612, 387);
            this.Name = "ReportMakerForm";
            this.Text = "ReportMakerUI";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnExecute;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TextBox txtSqlServer;
		private System.Windows.Forms.RadioButton rbAuthenticationNT;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtDataBase;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtPassword;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtUserName;
		private System.Windows.Forms.RadioButton radioButton1;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.PropertyGrid pgParameters;
		private System.Windows.Forms.Button btnCheckConnection;
		private System.Windows.Forms.PropertyGrid pgConnections;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.RadioButton rbPlanningReport;
		internal System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.RadioButton rbLegalPersonPaymentsReport;
	}
}


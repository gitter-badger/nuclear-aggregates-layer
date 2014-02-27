using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace DoubleGis.Erm.BL.Reports.UI
{
    public partial class ReportMakerForm : Form
    {
        private IReport _report = null;
        private bool _isInProgress = false;
        private bool _forceStop = false;

        public ReportMakerForm()
        {
            InitializeComponent();
            Text = String.Format("{0} (версия сборки: v{1})", Text, typeof(IReport).Assembly.GetName().Version);
        }

        private void txtSqlServer_TextChanged(object sender, EventArgs e)
        {
        }

        private void rbAuthenticationNT_CheckedChanged(object sender, EventArgs e)
        {
            txtUserName.Enabled = !rbAuthenticationNT.Checked;
            txtPassword.Enabled = !rbAuthenticationNT.Checked;
        }

        private bool IsInProgress
        {
            get { return _isInProgress; }
            set
            {
                _isInProgress = value;

                Enabled = !_isInProgress;
                UseWaitCursor = _isInProgress;
            }
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            if (_report == null) return;
            if (_report is SpreadsheetReportBase)
                saveFileDialog.FileName = ((SpreadsheetReportBase)_report).DefaultFileName;
            else
                saveFileDialog.FileName = _report.ReportName;

            if (saveFileDialog.ShowDialog() != DialogResult.OK) return;

            backgroundWorker.RunWorkerAsync();

            IsInProgress = true;
        }

        private void reportRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (rbPlanningReport.Checked)
                _report = new PlanningReport.PlanningReport();
            else if (rbLegalPersonPaymentsReport.Checked)
                _report = new LegalPersonPayments.LegalPersonPayments();
            else
                return;

            pgParameters.SelectedObject = new ProtectedDictionaryPropertyGridAdapter(_report.Parameters);
            pgConnections.SelectedObject = new ProtectedDictionaryPropertyGridAdapter(_report.Connections);
        }


        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            _report.SaveAs(saveFileDialog.FileName);
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled || _forceStop)
            {
                MessageBox.Show(@"Операция отменена пользователем");
            }
            else if (e.Error != null)
            {
                MessageBox.Show(String.Format("Произошла ошибка: {0}", e.Error.Message));
            }
            else
            {
                // The operation completed normally.
                string msg = String.Format("\"{0}\" успешно сохранен", _report.ReportName);
                MessageBox.Show(msg);
            }

            IsInProgress = false;
        }

    }
}

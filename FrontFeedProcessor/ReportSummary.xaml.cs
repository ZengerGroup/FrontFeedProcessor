namespace FrontFeedProcessor;

public partial class ReportSummary : ContentView
{
	public ReportSummary(ProcessReport report)
	{
		InitializeComponent();
		MailingSegmentData.Text = report.MailingSegment;
		ClassOfPostageData.Text = report.ClassOfPostage;
		ZGJobNumberData.Text = report.ZGJobNumber;
		ReportPathData.Text = report.ReportPath;
		WarningData.Text = report.OtherWarnings;
	}
}
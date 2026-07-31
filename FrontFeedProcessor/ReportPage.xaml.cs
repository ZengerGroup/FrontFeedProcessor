namespace FrontFeedProcessor;

public partial class ReportPage : ContentPage
{
	public ReportPage(ProcessReport report)
	{
		InitializeComponent();
		ReportView.Children.Add(new ReportSummary(report));
		for (int i = 0; i < report.BatchDetails.Count; i++) 
		{
			ReportView.Children.Add(new BatchSummary(report.BatchDetails[i]));
		}
	}
}
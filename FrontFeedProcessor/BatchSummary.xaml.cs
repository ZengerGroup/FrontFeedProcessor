namespace FrontFeedProcessor;

public partial class BatchSummary : ContentView
{
	public BatchSummary(Dictionary<string, string> batch)
	{
		InitializeComponent();
		DescriptorJobCodeData.Text = batch["Descriptor Job Code"];
		RecordCountData.Text = batch["Final Record Count"];
		DaysForSuppressionData.Text = batch["Days for Intrafile Suppression"];
		CriteriaData.Text = batch["Additional Suppression Criteria"];
		InfoData.Text = batch["Additional Info"];
		StateSelectionData.Text = batch["State Selection"];
		NetworkData.Text = batch["Network"];
	}
}
using System;
using System.Collections.Generic;
using System.Text;

namespace FrontFeedProcessor
{
    internal class BatchAssignment : Window
    {
        //gay shit bro-ccinnno
        private readonly TaskCompletionSource<List<JobBatch>> CompletionSource;

        public Task<List<JobBatch>> ResultTask => CompletionSource.Task;

        public BatchAssignment(Page rootPage, List<Row> rowsToProcess, string[] DecryptedFiles) : base(rootPage)
        {
            //User closes window by hitting the X.
            this.Destroying += (sender, e) =>
            {
                CompletionSource.TrySetResult(null);
            };
        }

        public void CloseWithResult(List<JobBatch> batches) 
        {
            //User closes the window by hitting 'Okay'.
            CompletionSource.TrySetResult(batches);
            Application.Current?.CloseWindow(this);
        }
    }
}

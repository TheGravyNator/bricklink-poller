using Google.Cloud.Firestore;

namespace BrickLinkPoller.Models
{
	[FirestoreData]
	public class BrickLinkOrder
	{
		[FirestoreProperty]
		public int Order_Id { get; set; }

		[FirestoreProperty]
		public string Store_Name { get; set; }

		[FirestoreProperty]
		public string Status { get; set; }

		[FirestoreProperty]
		public string? Tracking_Number { get; set; }
    }

	public class BrickLinkOrderResponse
	{
		public List<BrickLinkOrder> Data { get; set; }
	}

	public class BrickLinkOrderSingleResponse 
	{
		public BrickLinkOrder Data { get; set; }
    }
}

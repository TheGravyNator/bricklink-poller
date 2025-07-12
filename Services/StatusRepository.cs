using BrickLinkPoller.Models;
using Google.Cloud.Firestore;
using static Google.Cloud.Firestore.V1.StructuredQuery.Types;

namespace BrickLinkPoller.Services
{
	public class StatusRepository
	{
		Configuration config;
		FirestoreDb db;

		public StatusRepository()
		{
			config = new Configuration();

			db = FirestoreDb.Create(
				projectId: config.configuration["Firestore:ProjectName"]);	
		}

		public async Task<BrickLinkOrder?> GetOrder(int orderId)
		{
			var ordersCollection = db.Collection("orders");
			var docRef = ordersCollection.Document(orderId.ToString());

			var snapshot = await docRef.GetSnapshotAsync();

			if (snapshot.Exists)
			{
				return snapshot.ConvertTo<BrickLinkOrder>();
			}

			return null;
		}

		public async Task<List<BrickLinkOrder>?> GetOrders()
		{
			var ordersCollection = db.Collection("orders");

			var snapshot = await ordersCollection.GetSnapshotAsync();

			var orders = new List<BrickLinkOrder>();

			foreach (var document in snapshot.Documents)
			{
				var order = document.ConvertTo<BrickLinkOrder>();
				orders.Add(order);
			}

			if (orders.Any())
			{
				return orders;
			}

			return null;
		}

		public async Task WriteOrders(List<BrickLinkOrder> orders)
		{
			var ordersCollection = db.Collection("orders");

			foreach (var order in orders)
			{
				var docRef = ordersCollection.Document(order.Order_Id.ToString());
				await docRef.SetAsync(order);
			}
		}

		public async Task AddOrder(BrickLinkOrder order)
		{
			var docRef = db.Collection("orders").Document($"order.Order_Id");

			await docRef.SetAsync(order);
		}

		public async Task RemoveOrder(int orderId)
		{
			var ordersCollection = db.Collection("orders");
			var docRef = ordersCollection.Document(orderId.ToString());

			await docRef.DeleteAsync();
		}

		public async Task UpdateOrderStatus(BrickLinkOrder order)
		{
			var ordersCollection = db.Collection("orders");
			var docRef = ordersCollection.Document(order.Order_Id.ToString());

			await docRef.UpdateAsync("Status", order.Status);
		}
	}
}

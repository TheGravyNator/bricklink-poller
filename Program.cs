using BrickLinkPoller.Models;
using BrickLinkPoller.Services;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace BrickLinkPoller
{
	internal class Program
    {
        static BrickLinkConnection bricklink;
		static DiscordMessager messager;
        static StatusRepository statusRepository;

        static async Task Main(string[] args)
        {
			messager = new DiscordMessager();
            bricklink = new BrickLinkConnection();
            statusRepository = new StatusRepository();

            var orders = await bricklink.GetOrders();
            var newOrders = new List<BrickLinkOrder>();

            var message = new StringBuilder();

            if (orders != null)
            {
                var databaseOrders = await statusRepository.GetOrders();

                if (databaseOrders == null)
                {
                    await statusRepository.WriteOrders(orders.Where(o => o.Status != "COMPLETED").ToList());
                }
                else
                {
					foreach (var order in orders)
					{
						if (databaseOrders!.Select(dord => dord.Order_Id).Contains(order.Order_Id))
						{
							var databaseOrder = databaseOrders!.SingleOrDefault(dord => dord.Order_Id == order.Order_Id);
							if (databaseOrder != null)
							{
								if (order.Status == "COMPLETED")
								{
									await statusRepository.RemoveOrder(order.Order_Id);
									message.AppendLine($"Order [#{order.Order_Id}](<https://www.bricklink.com/orderDetail.asp?ID={order.Order_Id}>) from {order.Store_Name} has been completed!");
								}
								else if (databaseOrder.Status != order.Status)
								{
									var oldStatus = databaseOrder.Status;
									databaseOrder.Status = order.Status;
									await statusRepository.UpdateOrderStatus(databaseOrder);
									message.AppendLine($"Order [#{order.Order_Id}](<https://www.bricklink.com/orderDetail.asp?ID={order.Order_Id}>) from {order.Store_Name} has changed from {oldStatus} to {databaseOrder.Status}!");
									if (order.Tracking_Number != null)
									{
										message.AppendLine($"Order [#{order.Order_Id}](<https://www.bricklink.com/orderDetail.asp?ID={order.Order_Id}>)'s tracking number is {order.Tracking_Number}.");
									}
								}
							}
						}
						else
						{
							if (order.Status != "COMPLETED")
							{
								await statusRepository.AddOrder(order);
								message.AppendLine($"Order [#{order.Order_Id}](<https://www.bricklink.com/orderDetail.asp?ID={order.Order_Id}>) from {order.Store_Name} has been added!");
								if (order.Tracking_Number != null)
								{
									message.AppendLine($"Order [#{order.Order_Id}](<https://www.bricklink.com/orderDetail.asp?ID={order.Order_Id}>)'s tracking number is {order.Tracking_Number}.");
								}
							}
						}
					}
				}
            }

            if (!string.IsNullOrWhiteSpace(message.ToString()))
            {
				await messager.SendMessage(message.ToString());
			}

			Console.WriteLine(message.ToString());
        }
    }
}

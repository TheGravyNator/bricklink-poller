using BrickLinkPoller.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

public class BrickLinkOrderConverter : JsonConverter<BrickLinkOrder>
{
    public override BrickLinkOrder Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        var result = new BrickLinkOrder
        {
            Order_Id = root.GetProperty("order_id").GetInt32(),
            Store_Name = root.GetProperty("store_name").GetString() ?? "",
            Status = root.GetProperty("status").GetString() ?? ""
        };

        var shipping = root.GetProperty("shipping");
        if (shipping.TryGetProperty("tracking_no", out var trackingNumber) && trackingNumber.ValueKind != JsonValueKind.Null)
        {
            result.Tracking_Number = trackingNumber.GetString();
        }

        return result;
    }

    public override void Write(Utf8JsonWriter writer, BrickLinkOrder value, JsonSerializerOptions options)
    {
        throw new NotImplementedException("Writing not needed in this case.");
    }
}
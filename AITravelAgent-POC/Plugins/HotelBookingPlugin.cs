using System.ComponentModel;
using System.Text.Json;
using Microsoft.SemanticKernel;

public class HotelBookingPlugin
{
    private const string FilePath = "AITravelAgent-POC/Plugins/Hotels.json";
    private List<HotelModel> Hotels;

    public HotelBookingPlugin()
    {
        // Load Hotels from the file
        Hotels = LoadHotelsFromFile();
    }

    [KernelFunction("search_hotels")]
    [Description("Searches for available hotels based on the destination")]
    [return: Description("A list of available hotels")]
    public List<HotelModel> SearchHotels(string hotelname)
    {
        // Filter Hotels based on destination
        return Hotels.Where(Hotel =>
            Hotel.HotelName.Equals(hotelname, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    [KernelFunction("book_hotel")]
    [Description("Books a hotel based on the hotel ID provided")]
    [return: Description("Booking confirmation message")]
    public string BookHotel(int hotelId)
    {
        var hotel = Hotels.FirstOrDefault(f => f.Id == hotelId);
        if (hotel == null)
        {
            return "Hotel not found. Please provide a valid hotel ID.";
        }

        if (hotel.IsBooked)
        {
            return $"Hotel ID {hotelId} is already booked.";
        }

        hotel.IsBooked = true;
        SaveHotelsToFile();
        return $"Hotel booked successfully! Hotel: {hotel.HotelName}, CheckInDate: {hotel.CheckInDate}, CheckOutDate: {hotel.CheckOutDate}, Price: ${hotel.Price}.";
    }

    private void SaveHotelsToFile()
    {
        var json = JsonSerializer.Serialize(Hotels, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(FilePath, json);
    }

    private List<HotelModel> LoadHotelsFromFile()
    {
        if (File.Exists(FilePath))
        {
            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<HotelModel>>(json)!;
        }

        throw new FileNotFoundException($"The file '{FilePath}' was not found. Please provide a valid Hotels.json file.");
    }
}

// Hotel model
public class HotelModel
{
    public int Id { get; set; }
    public required string HotelName { get; set; }
    public required string CheckInDate { get; set; }
    public required string CheckOutDate { get; set; }
    public decimal Price { get; set; }
    public bool IsBooked { get; set; } = false; // Added to track booking status
}

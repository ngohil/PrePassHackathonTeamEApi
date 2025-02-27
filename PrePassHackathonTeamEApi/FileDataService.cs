using PrePassHackathonTeamEApi.Models;
using System.Text.Json;

namespace PrePassHackathonTeamEApi
{
    public class FileDataService
    {
        private readonly string _filePath = "data.json";


    // Read list of items from file
    public async Task<List<CalendarEvent>> ReadItemsAsync()
    {
        if (!File.Exists(_filePath))
            return new List<CalendarEvent>(); // Return an empty list if the file doesn't exist

        var content = await File.ReadAllTextAsync(_filePath);
        List<CalendarEvent> existingEvents = JsonSerializer.Deserialize<List<CalendarEvent>>(content) ?? new List<CalendarEvent>();

        return existingEvents;
    }

    // Write a list of items to the file (overwrite)
    public async Task WriteItemsAsync(List<CalendarEvent> items)
    {
        var jsonData = JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_filePath, jsonData);
    }

    // Append an item to the list
    public async Task AppendItemAsync(CalendarEvent newItem)
    {
        var items = await ReadItemsAsync();
        items.Add(newItem);
        await WriteItemsAsync(items);
    }    

    }
}

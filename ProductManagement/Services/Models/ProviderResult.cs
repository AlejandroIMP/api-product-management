using System.Text.Json;

namespace ProductManagement.Services.Models;

public record ProviderResult(bool Success, JsonElement? Result, bool IsTransient = false, string? ErrorMessage = null);


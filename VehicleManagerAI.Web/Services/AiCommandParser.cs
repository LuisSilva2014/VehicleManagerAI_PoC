using System.Text.Json;
using System.Text.RegularExpressions;
using VehicleManagerAI.Web.Models;

namespace VehicleManagerAI.Web.Services;

/// <summary>
/// Turns free-form model output into an <see cref="AiCommand"/>.
/// Isolated from HTTP so parsing can be unit-tested without Ollama.
/// </summary>
public static class AiCommandParser
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static AiCommand Parse(string? rawContent)
    {
        if (string.IsNullOrWhiteSpace(rawContent))
        {
            return Unknown("The AI model returned an empty response.", rawContent);
        }

        var json = ExtractJson(rawContent);
        try
        {
            var command = JsonSerializer.Deserialize<AiCommand>(json, JsonOptions);
            if (command is null)
            {
                return Unknown("The AI model returned JSON that could not be mapped to a command.", rawContent);
            }

            command.RawResponse = rawContent;
            command.Action = string.IsNullOrWhiteSpace(command.Action) ? "unknown" : command.Action.Trim();
            command.Make = Normalize(command.Make);
            command.Model = Normalize(command.Model);
            return command;
        }
        catch (JsonException)
        {
            return Unknown("The AI model did not return valid JSON. Try rephrasing the request.", rawContent);
        }
    }

    public static AiCommand Unknown(string explanation, string? rawResponse = null) => new()
    {
        Action = "unknown",
        Explanation = explanation,
        RawResponse = rawResponse
    };

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string ExtractJson(string rawContent)
    {
        var trimmed = rawContent.Trim();

        var fenced = Regex.Match(
            trimmed,
            @"```(?:json)?\s*(\{.*?\})\s*```",
            RegexOptions.Singleline | RegexOptions.IgnoreCase);
        if (fenced.Success)
        {
            return fenced.Groups[1].Value;
        }

        var start = trimmed.IndexOf('{');
        var end = trimmed.LastIndexOf('}');
        if (start >= 0 && end > start)
        {
            return trimmed[start..(end + 1)];
        }

        return trimmed;
    }
}

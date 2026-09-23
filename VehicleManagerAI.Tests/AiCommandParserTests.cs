using VehicleManagerAI.Web.Models;
using VehicleManagerAI.Web.Services;

namespace VehicleManagerAI.Tests;

public class AiCommandParserTests
{
    [Fact]
    public void Parse_ReadsCreateCommandFromJson()
    {
        var json = """{"action":"create","vehicleId":123,"make":"Toyota","model":"Corolla","year":2024,"explanation":"ok"}""";

        var command = AiCommandParser.Parse(json);

        Assert.Equal(AiAction.Create, command.GetAction());
        Assert.Equal(123, command.VehicleId);
        Assert.Equal("Toyota", command.Make);
        Assert.Equal("Corolla", command.Model);
        Assert.Equal(2024, command.Year);
    }

    [Fact]
    public void Parse_ExtractsJsonFromMarkdownFence()
    {
        var raw = """
            Sure.
            ```json
            {"action":"delete","vehicleId":123,"explanation":"removing"}
            ```
            """;

        var command = AiCommandParser.Parse(raw);

        Assert.Equal(AiAction.Delete, command.GetAction());
        Assert.Equal(123, command.VehicleId);
    }

    [Fact]
    public void Parse_ReturnsUnknownForInvalidJson()
    {
        var command = AiCommandParser.Parse("I cannot help with that.");

        Assert.Equal(AiAction.Unknown, command.GetAction());
        Assert.False(string.IsNullOrWhiteSpace(command.Explanation));
    }
}

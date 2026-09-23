using System.ComponentModel.DataAnnotations;

namespace VehicleManagerAI.Web.Models;

/// <summary>
/// The only business entity in this PoC. Keeping it small makes the AI command
/// contract easy to understand: the model only needs id, make, model, and year.
/// </summary>
public class Vehicle
{
    [Display(Name = "Vehicle ID")]
    [Range(1, int.MaxValue, ErrorMessage = "Id must be a positive number.")]
    public int Id { get; set; }

    [Required]
    [StringLength(60)]
    public string Make { get; set; } = string.Empty;

    [Required]
    [StringLength(60)]
    public string Model { get; set; } = string.Empty;

    [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100.")]
    public int Year { get; set; }

    public override string ToString() => $"#{Id} {Year} {Make} {Model}";
}

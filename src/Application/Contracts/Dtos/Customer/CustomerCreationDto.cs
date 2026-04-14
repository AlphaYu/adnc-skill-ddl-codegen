namespace Adnc.Skill.Test.Application.Contracts.Dtos.Customer;

/// <summary>
/// Represents the payload used to create a customer.
/// </summary>
public class CustomerCreationDto : InputDto
{
    /// <summary>
    /// Gets or sets the customer name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the customer phone.
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the customer is enabled.
    /// </summary>
    public bool Status { get; set; }
}

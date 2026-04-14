using System;

namespace Adnc.Skill.Test.Application.Contracts.Dtos.Customer;

/// <summary>
/// Represents a customer.
/// </summary>
[Serializable]
public class CustomerDto : CustomerCreationDto
{
    /// <summary>
    /// Gets or sets the customer ID.
    /// </summary>
    public long Id { get; set; }
}

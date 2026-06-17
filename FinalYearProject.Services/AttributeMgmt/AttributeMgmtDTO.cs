using FinalYearProject.Data.Domain.Entities.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinalYearProject.Services.AttributeMgmt;

public record AllAttributesResponse(
    Guid Id,
    string AttributeName,
    AttributeType AttributeType
);

public record AttributeDetailsResponse(
    Guid Id,
    string AttributeName,
    AttributeType AttributeType
);

public class AttributeMgmtRequest
{
    [Required]
    public string AttributeName { get; set; }

    [Required]
    public AttributeType AttributeType { get; set; }
}

public class AttributeParameters
{
    public string? Search { get; set; }
    public AttributeType? AttributeType { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
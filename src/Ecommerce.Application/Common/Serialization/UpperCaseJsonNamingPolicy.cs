using System.Text.Json;

namespace Ecommerce.Application.Common.Serialization;

public sealed class UpperCaseJsonNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name) => name.ToUpperInvariant();
}

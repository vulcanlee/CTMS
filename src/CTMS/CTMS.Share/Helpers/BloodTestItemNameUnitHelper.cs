namespace CTMS.Share.Helpers;

public static class BloodTestItemNameUnitHelper
{
    private static readonly HashSet<string> KnownUnits = new(StringComparer.OrdinalIgnoreCase)
    {
        "%",
        "mg/dL",
        "U/L",
        "g/dL",
        "mmol/L",
        "mg/L",
        "ng/mL",
        "ng/dL",
        "mIU/L",
        "10^3/μL",
        "10^6/μL",
        "fL",
        "pg",
    };

    private static readonly Dictionary<string, string> LegacyNameMap = new(StringComparer.Ordinal)
    {
        ["LDH"] = "乳酸脫氫酶 (LDH)",
    };

    public static string NormalizeItemName(string? itemName)
    {
        var (normalizedName, _) = Parse(itemName);
        return normalizedName;
    }

    public static (string 項目名稱, string 單位) Parse(string? itemName)
    {
        var normalizedItemName = itemName?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(normalizedItemName))
        {
            return (string.Empty, string.Empty);
        }

        var normalizedUnit = string.Empty;
        var match = System.Text.RegularExpressions.Regex.Match(
            normalizedItemName,
            @"^(?<name>.+?)(?<unit>\([^()]+\))$");
        if (match.Success)
        {
            var parenthesizedUnit = match.Groups["unit"].Value[1..^1].Trim();
            if (LooksLikeUnit(parenthesizedUnit))
            {
                normalizedItemName = match.Groups["name"].Value.TrimEnd();
                normalizedUnit = parenthesizedUnit;
            }
        }

        if (string.IsNullOrEmpty(normalizedUnit))
        {
            var lastWhitespaceIndex = normalizedItemName.LastIndexOf(' ');
            if (lastWhitespaceIndex > 0)
            {
                var unitCandidate = normalizedItemName[(lastWhitespaceIndex + 1)..].Trim();
                if (LooksLikeUnit(unitCandidate))
                {
                    normalizedItemName = normalizedItemName[..lastWhitespaceIndex].TrimEnd();
                    normalizedUnit = unitCandidate;
                }
            }
        }

        if (LegacyNameMap.TryGetValue(normalizedItemName, out var legacyMappedName))
        {
            normalizedItemName = legacyMappedName;
        }

        return (normalizedItemName, normalizedUnit);
    }

    private static bool LooksLikeUnit(string unitCandidate)
    {
        if (string.IsNullOrWhiteSpace(unitCandidate))
        {
            return false;
        }

        if (KnownUnits.Contains(unitCandidate))
        {
            return true;
        }

        return unitCandidate.Contains('%') ||
               unitCandidate.Contains('/') ||
               unitCandidate.Contains('^');
    }
}

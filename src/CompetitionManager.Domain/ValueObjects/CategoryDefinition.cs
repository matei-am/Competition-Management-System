namespace CompetitionManager.Domain.ValueObjects;

public sealed class CategoryDefinition
{
    public CategoryDefinition(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Category name is required.", nameof(name));
        }

        Name = name.Trim();
        Description = description?.Trim();
    }

    public string Name { get; }

    public string? Description { get; }
}
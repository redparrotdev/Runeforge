namespace Lore.Structure;

public abstract class DialogStep
{
    public string Id { get; protected set; } = Guid.NewGuid().ToString();
}

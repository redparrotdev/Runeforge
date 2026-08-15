namespace Lore.Structure;

public class RouteStep : DialogStep
{
    public IEnumerator<DialogStep> Route { get; protected set; }

    public RouteStep(IEnumerator<DialogStep> route)
    {
        Route = route;
    }
}

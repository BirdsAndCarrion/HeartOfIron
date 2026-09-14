
public class VisibilityQuery
{
    public MechController MechController;
    public bool isVisible;

    public VisibilityQuery(MechController MechController)
    {
        this.MechController = MechController;
        this.isVisible = false; // Default visibility status
    }
}

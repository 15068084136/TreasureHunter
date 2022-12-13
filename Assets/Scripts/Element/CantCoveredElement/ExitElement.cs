public class ExitElement : CantCoveredElement
{
    protected override void Awake()
    {
        base.Awake();
        elementContent = ElementContent.exit;
        LoadSprit(GameManager.Instance.exitSprit);
        name = "Exit";
    }

    public override void PlayerOnStand()
    {
        GameManager.Instance.SaveData();
    }
}

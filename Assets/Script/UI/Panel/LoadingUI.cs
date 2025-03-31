using cfUnityEngine;
using cfUnityEngine.UI.UGUI;

public class LoadingUI: UIPanel
{
    public const string PANEL_ID = nameof(LoadingUI);
    private string message;
    public override string id => PANEL_ID;

    public override void Bind(INamespaceScope scope)
    {
        base.Bind(scope);
        BindSubspace(scope, nameof(message), this);
        OnPropertyChanged(nameof(message), "Loading...");
    }
}

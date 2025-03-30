using cfUnityEngine;
using cfUnityEngine.UI.UGUI;

public class LoadingUI: UIPanel
{
    private string message;

    public override void Bind(INamespaceScope scope)
    {
        base.Bind(scope);
        BindSubspace(scope, nameof(message), this);
        OnPropertyChanged(nameof(message), "Loading...");
    }
}

namespace WikiExtractor.Maui.App.Controls
{
    /// <summary>
    /// Trigger action for fade-in animation
    /// </summary>
    public class FadeInAnimation : TriggerAction<VisualElement>
    {
        protected override async void Invoke(VisualElement sender)
        {
            if (sender == null) return;

            // Start with transparent
            sender.Opacity = 0;
            
            // Fade in over 500ms
            await sender.FadeTo(1, 500, Easing.CubicInOut);
        }
    }
}

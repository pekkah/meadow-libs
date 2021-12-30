using Chibi.Ui.MicroGraphics;
using Meadow.Foundation.Graphics;

namespace Chibi.Ui.Simple;

public class MainMenuScreen
{
    private readonly Renderable _renderable;

    private int _currentButtonId = 0;
    
    public MainMenuScreen()
    {
        _renderable = new VerticalLayout(
            new[]
            {
                new HorizontalLayout(
                    height: () => 16,
                    margin: () => new Margin(2, 2, 2, 2),
                    children: new[]
                    {
                        new Text(
                            () => $"{99.9:F1}"
                        ),
                        new Text(
                            () => "MENU",
                            () => TextAlignment.Center
                        ),
                        new Text(
                            () => $"{99.9:F1}",
                            () => TextAlignment.Right
                        )
                    }),
                new HorizontalLayout(
                    new[]
                    {
                        new IconButton(
                            icon: ()=> Icon.Play,
                            margin: () => CurrentButtonMargin(0),
                            text: () => "Button 1"
                        ),
                        new IconButton(
                            icon: ()=> Icon.Play,
                            margin: () => CurrentButtonMargin(1),
                            text: () => "Button 2"
                        ),
                        new IconButton(
                            icon: ()=> Icon.Settings,
                            margin: () => CurrentButtonMargin(2),
                            text: () => "Button 3"
                        )
                    })
            });
    }

    public void Render(RenderingContext context)
    {
        _renderable.Render(context);
    }

    private Margin CurrentButtonMargin(int buttonId)
    {
        return buttonId == _currentButtonId ? Margin.Zero : new Margin(0, 4, 0, 2);
    }
}
namespace Godot;

public static class ScreenUtils
{
    public static int ScreenWidth { get; private set; }
    public static int ScreenHeight { get; private set; }
    public static int MarginLeft { get; private set; } = 50;
    public static int MarginTop { get; private set; } = 50;
    public static int MarginRight { get; private set; }
    public static int MarginBottom { get; private set; }
    public static int XMax { get; private set; }
    public static int XMin { get; private set; }
    public static int YMax { get; private set; }
    public static int YMin { get; private set; }

    public static void Initialize(Viewport viewport)
    {
        Vector2 windowSize = viewport.GetVisibleRect().Size;
        ScreenWidth = (int)windowSize.X;
        ScreenHeight = (int)windowSize.Y;
        MarginRight = ScreenWidth - MarginLeft;
        MarginBottom = ScreenHeight - MarginTop;
        XMax = MarginRight;
        XMin = MarginLeft;
        YMax = MarginTop;
        YMin = MarginBottom;
    }
}
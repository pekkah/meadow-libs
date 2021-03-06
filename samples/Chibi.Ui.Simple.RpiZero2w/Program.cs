using Chibi.Ui.Simple.Screens;
using Meadow.Foundation.Displays.Ssd130x;
using Serilog.Sinks.SystemConsole.Themes;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(theme: AnsiConsoleTheme.Code)
    .MinimumLevel.Verbose()
    .Enrich.FromLogContext()
    .CreateLogger();

var logger = Log.ForContext<Program>();

var gpio = Peripherals.GPIOCHIP0;

// We require four 
using var leftButtonInput = gpio.GPIO21.ToInput(ResistorMode.InternalPullUp, InterruptMode.EdgeFalling, 1000);
using var rightButtonInput = gpio.GPIO20.ToInput(ResistorMode.InternalPullUp, InterruptMode.EdgeFalling, 1000);
using var displayBus = Peripherals.CreateI2C1();

logger.Information("Initialize controls");
using var leftButton = new PushButton(leftButtonInput);
using var rightButton = new PushButton(rightButtonInput);

// state variables
var exit = false;

logger.Information("Initialize graphics");
var display = new Ssd1306(displayBus)
{
    IgnoreOutOfBoundsPixels = true
};
var graphics = new MicroGraphics(display)
{
    PenColor = Color.White
};

var renderingContext = new RenderingContext(graphics)
{
    DefaultFont = new Font4x6()
};
var screen = new MainMenuScreen(display.Width, display.Height);

leftButton.Clicked += (_, _) =>
{
    screen.Left();
    logger.Information("Left");
};
rightButton.Clicked += (_, _) =>
{
    screen.Right();
    logger.Information("Right");
};

Console.CancelKeyPress += (_, _) => exit = true;

logger.Information("Main");
while (!exit)
{
    graphics.Clear();
    screen.Render(renderingContext);
    graphics.Show();
}
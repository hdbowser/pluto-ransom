using pluto;

PlutoExecutor executor = new();

var docummentDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Documents");
var desktopDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Desktop");
await executor.Run(docummentDir);
await executor.Run(desktopDir);

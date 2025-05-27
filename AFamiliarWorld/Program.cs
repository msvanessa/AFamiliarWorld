namespace AFamiliarWorld
{
    public class Program
    {
        public static void Main(string[] args) =>
            new AFamiliarWorld.Bot.AFamiliarWorld().RunAsync(args[0]).GetAwaiter().GetResult();
    }
    
}
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ExtensionMethods
{
    // Static class to define an extension method for the Game class.
    public static class MyExtensions
    {
        // Extension method used to make a deep clone of a game state. This
        // is useful in to get the child states for the Minimax algorithm.
        public static Game DeepClone<Game>(this Game game)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, game);
                ms.Position = 0;

                return (Game) formatter.Deserialize(ms);
            }
        }
    }
}
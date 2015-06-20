using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AwesomeLogger.Monitor.Tasks
{
    /// <summary>
    /// Fake method to disable Intellisense waring if caller not awated to parallel task
    /// </summary>
    public static class TaskExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NoWarning(this Task task)
        {
        }
    }
}
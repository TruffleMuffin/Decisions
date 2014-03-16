namespace Decisions.Example.Support
{
    /// <summary>
    /// A simple environment that maintains an internal counter for how many times its is constructed.
    /// </summary>
    public class SimpleCounterEnvironment
    {
        private static int counter = 0;

        public SimpleCounterEnvironment(bool inc)
        {
            if (inc) ++counter;
        }

        public const string ALIAS = "Counter";

        public int Counter()
        {
            return counter;
        }
    }
}
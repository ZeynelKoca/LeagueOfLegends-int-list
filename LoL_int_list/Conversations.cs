namespace Siskos_LOL_int_list
{
    internal abstract class Conversations
    {
        protected Conversations(string type)
        {
            Type = type;
        }

        public string Type { get; }
    }
}

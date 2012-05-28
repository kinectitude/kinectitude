
namespace MessagePassing.Public
{
    /// <summary>
    /// Used to receive information after a user or component event occurs.
    /// </summary>
    /// <param name="data">An array of generically-typed results from the event. The length is variable.</param>
    public delegate void MessageCallback(params object[] data);

    /// <summary>
    /// Interface provided to components so that they can publish messages and subscribe to outside ones.
    /// </summary>
    public interface IMessenger
    {
        void Publish(string id, params object[] data);
        void Subscribe(string id, MessageCallback callback, params object[] parameters);
    }
}

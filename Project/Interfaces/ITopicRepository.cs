using Project.Models;

namespace Project.Interfaces
{
    public interface ITopicRepository
    {
        ICollection<Topic> GetTopics();
        Topic GetTopic(int id);
        bool TopicExists(int id);
        bool CreateTopic(Topic topic);
        bool UpdateTopic(Topic topic);
        bool DeleteTopic(Topic topic);
        bool Save();
    }
}

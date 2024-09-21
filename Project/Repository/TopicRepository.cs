using System.Collections.Generic;
using System.Linq;
using Project.Data;
using Project.Interfaces;
using Project.Models;

namespace Project.Repositories
{
    public class TopicRepository : ITopicRepository
    {
        private readonly DataContext _context;

        public TopicRepository(DataContext context)
        {
            _context = context;
        }

        public ICollection<Topic> GetTopics()
        {
            return _context.Topics.ToList();
        }

        public Topic GetTopic(int id)
        {
            return _context.Topics.Find(id);
        }

        public bool TopicExists(int id)
        {
            return _context.Topics.Any(t => t.ID == id);
        }

        public bool CreateTopic(Topic topic)
        {
            _context.Topics.Add(topic);
            return Save();
        }

        public bool UpdateTopic(Topic topic)
        {
            _context.Topics.Update(topic);
            return Save();
        }

        public bool DeleteTopic(Topic topic)
        {
            _context.Topics.Remove(topic);
            return Save();
        }

        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }
    }
}

using System.IO;
using System.Xml.Serialization;

namespace cron.net.Utils.Serialization
{
    public class XmlSerializer<T> : ISerializer<T>
    {
        private readonly string _path;

        public XmlSerializer(string path)
        {
            _path = path;
        }

        public T Get()
        {
            using (var stream = new FileStream(_path, FileMode.Open))
            {
                return (T) new XmlSerializer(typeof(T)).Deserialize(stream);
            }
        }

        public void Set(T item)
        {
            using (var stream = new FileStream(_path, FileMode.Create))
            {
                new XmlSerializer(typeof(T)).Serialize(stream, item);
            }
        }
    }
}
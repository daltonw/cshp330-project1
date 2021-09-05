using System.Linq;

namespace MiniCstructor.Repository
{
    public interface IClassRepository
    {
        ClassModel[] Classes { get; }
    }

    public class ClassModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }

    public class ClassRepository : IClassRepository
    {
        public ClassModel[] Classes
        {
            get
            {
                return DatabaseAccessor.Instance.Class
                                       .Select(t => new ClassModel
                                       { 
                                           Id = t.ClassId, 
                                           Name = t.ClassName,
                                           Description = t.ClassDescription,
                                           Price = t.ClassPrice
                                       })
                                       .ToArray();
            }
        }
    }
}
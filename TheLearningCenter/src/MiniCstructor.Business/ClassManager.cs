using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MiniCstructor.Repository;

namespace MiniCstructor.Business
{
    public interface IClassManager
    {
        ClassModel[] Classes { get; }
    }

    public class ClassModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public ClassModel(int id, string name, string description, decimal price)
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
        }
    }

    public class ClassManager : IClassManager
    {
        private readonly IClassRepository classRepository;
        public ClassManager(IClassRepository classRepository)
        {
            this.classRepository = classRepository;
        }
        public ClassModel[] Classes
        {
            get
            {
                return classRepository.Classes
                                       .Select(t => new ClassModel(t.Id, t.Name, t.Description, t.Price))
                                       .ToArray();
            }

        }
    }
}

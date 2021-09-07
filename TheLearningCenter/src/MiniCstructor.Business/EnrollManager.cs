using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MiniCstructor.Repository;

namespace MiniCstructor.Business
{
    public interface IEnrollManager
    {
        EnrollModel Add(int userId, int classId);
        EnrollModel[] GetAll(int userId);
    }
    public class EnrollModel
    {
        public int UserId { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
    public class EnrollManager : IEnrollManager
    {
        private readonly IEnrollRepository enrollRepository;
        private readonly IClassRepository classRepository;
        public EnrollManager(IEnrollRepository enrollRepository, IClassRepository classRepository)
        {
            this.enrollRepository = enrollRepository;
            this.classRepository = classRepository;
        }
        public EnrollModel Add(int userId, int classId)
        {
            var addedClass = enrollRepository.Add(userId, classId);

            var getClass = classRepository.GetClass(classId);

            return new EnrollModel
            {
                UserId = addedClass.UserId,
                ClassId = addedClass.ClassId,
                ClassName = getClass.Name,
                Description = getClass.Description,
                Price = getClass.Price
            };
        }
        public EnrollModel[] GetAll(int userId)
        {
            var courses = enrollRepository.GetAll(userId)
                .Select(t =>
                {
                    var getClass = classRepository.GetClass(t.ClassId);
                    return new EnrollModel
                    {
                        UserId = t.UserId,
                        ClassId = t.ClassId,
                        ClassName = getClass.Name,
                        Description = getClass.Description,
                        Price = getClass.Price
                    };
                })
                .ToArray();

            return courses;

        }
    }
}

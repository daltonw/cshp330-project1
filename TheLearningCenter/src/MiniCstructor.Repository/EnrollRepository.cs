using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MiniCstructor.Repository
{
    public interface IEnrollRepository
    {
        EnrollModel Add(int userId, int classId);
        EnrollModel[] GetAll(int userId);

    }
    public class EnrollModel
    {
        public int UserId { get; set; }
        public int ClassId { get; set; }
    }
    public class EnrollRepository : IEnrollRepository
    {
        public EnrollModel Add(int userId, int classId)
        {
            var course = DatabaseAccessor.Instance.UserClass.Add(
                new MiniCstructor.Database.UserClass
                {
                    UserId = userId,
                    ClassId = classId
                });

            DatabaseAccessor.Instance.SaveChanges();

            return new EnrollModel
            {
                UserId = course.Entity.UserId,
                ClassId = course.Entity.ClassId
            };
        }
        public EnrollModel[] GetAll(int userId)
        {
            var courses = DatabaseAccessor.Instance.UserClass
                .Where(t => t.UserId == userId)
                .Select(t => new EnrollModel
                {
                    UserId = t.UserId,
                    ClassId = t.ClassId
                })
                .ToArray();

            return courses;
        }
    }
}

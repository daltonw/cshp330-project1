using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MiniCstructor.Repository
{
    public interface IUserRepository
    {
        UserModel LogIn(string email, string password);
        UserModel Register(string email, string password);
    }

    public class UserModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class UserRepository : IUserRepository
    {
        public UserModel LogIn(string email, string password)
        {
            var user = DatabaseAccessor.Instance.User
                                       .FirstOrDefault(t => t.UserEmail.ToLower() == email.ToLower()
                                            && t.UserPassword == password);

            if (user == null)
            {
                return null;
            }

            return new UserModel { Id = user.UserId, Name = user.UserEmail };
        }

        public UserModel Register(string email, string password)
        {
            var userFound = DatabaseAccessor.Instance.User
                .FirstOrDefault(t => t.UserEmail.ToLower() == email.ToLower());

            if (userFound != null)
            {
                return null;
            }
            var user = DatabaseAccessor.Instance.User
                .Add(new MiniCstructor.Database.User
                {
                    UserEmail = email,
                    UserPassword = password
                });

            DatabaseAccessor.Instance.SaveChanges();

            return new UserModel { Id = user.Entity.UserId, Name = user.Entity.UserEmail };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using MiniCstructor.Database;

namespace MiniCstructor.Repository
{
    class DatabaseAccessor
    {
        static DatabaseAccessor()
        {
            Instance = new MiniCstructorDBContext();
        }

        public static MiniCstructorDBContext Instance { get; private set; }
    }
}

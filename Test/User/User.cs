namespace Test
{
    public class User
    {
        public int Id { get; set; }        

        public virtual string Name { get; set; }

        public User()
        {            
        }

        public User(int id, string name)
        {            
            Id = id;
            Name = name;
        }
    }
}
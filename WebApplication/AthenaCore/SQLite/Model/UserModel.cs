using WebApplication.AthenaCore.SQLite.Model.Attributes;

namespace WebApplication.AthenaCore.SQLite.Model
{
    public class UserModel : BaseModel<UserModel>
    {
        [Column(ColumnFlags.PrimaryKey)]
        public int Id { get; set; }
        
        [Column(ColumnFlags.Required)]
        public string Username { get; set; }
        
        [Column(ColumnFlags.Required)]
        public string Name { get; set; }
        
        [Column(ColumnFlags.Required)]
        public string Password { get; set; }
    }
}
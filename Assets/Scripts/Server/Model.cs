using Realms;

public class GameModel : RealmObject
{

    [PrimaryKey]
    public string username { get; set; }
    public string password { get; set; }
    private int addr { get; set; }

    public GameModel() { }

    public GameModel(string username, string password, int addr)
    {
        this.username = username;
        this.password = password;
        this.addr = addr;
    }

}
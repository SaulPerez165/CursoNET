using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BankConsole;

public static class Storage{
    static string filePath = AppDomain.CurrentDomain.BaseDirectory + @"\users.json";

    public static void AddUser(User user){
        var listUsers = GetAllUsers();
        listUsers.Add(user);
        CreateJson(listUsers);
    }

    public static List<User> GetAllUsers(){
        string usersInFile = "";
        var listUsers = new List<User>();

        if(File.Exists(filePath))
            usersInFile = File.ReadAllText(filePath);

        var listObjects = JsonConvert.DeserializeObject<List<object>>(usersInFile);

        if(listObjects == null)
            return listUsers;

        foreach(object obj in listObjects){
            User newUser;
            JObject user = (JObject)obj;
            
            if(user.ContainsKey("TaxRegime"))
                newUser = user.ToObject<Client>();
            else
                newUser = user.ToObject<Employee>();

            listUsers.Add(newUser);
        }

        return listUsers;
    }

    public static void CreateJson(List<User> listUsers){
        JsonSerializerSettings settings = new JsonSerializerSettings { Formatting = Formatting.Indented };
        string json = JsonConvert.SerializeObject(listUsers, settings);
        File.WriteAllText(filePath, json);
    }

    public static string DeleteUser(int ID){
        var userToDelete = new User();
        
        var listUsers = GetAllUsers();

        //userToDelete = listUsers.Where(user => user.GetID() == ID).Single();

        foreach(User user in listUsers){
            if(user.GetID() == ID){
                userToDelete = user;
                break;
            }
        }

        if(userToDelete.GetID() != ID)
            return "Error";
        
        listUsers.Remove(userToDelete);
        CreateJson(listUsers);
        return "Success";
    }

    public static string GetFilePath(){
        return filePath;
    }
}
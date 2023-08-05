using BankConsole;
using System.Text.RegularExpressions;

char option;

Console.Clear();
Console.WriteLine("Quieres recibir un correo con la lista de todos los usuarios registrados? (S/s = Si, N/n = No): ");
option = Console.ReadKey().KeyChar;
if(option == 's' || option == 'S'){
    EmailService.SendMail();
    Console.Clear();
    Console.WriteLine("Se ha enviado un correo con la lista de todos los usuarios registrados.");
    Console.WriteLine("Presiona 'enter' para salir del programa...");
    while(Console.ReadKey().Key != ConsoleKey.Enter){}
}
else
    ShowMenu();

/*
if(args.Length != 0)
    EmailService.SendMail();
else
    ShowMenu();
*/

void ShowMenu(){

    do{
        Console.Clear();
        Console.WriteLine("Selecciona una opción:");
        Console.WriteLine("1 - Crear un Usuario nuevo.");
        Console.WriteLine("2 - Eliminar un Usuario existente.");
        Console.WriteLine("3 - Salir.\n");

        Console.WriteLine("Opción: ");
        option = Console.ReadKey().KeyChar;

        switch(option){
        case '1': CreateUser();
                  break;
        case '2': DeleteUser();
                  break;
        case '3': option = '0';
                  Console.Clear();
                  Environment.Exit(0);
                  break;
        default: option = '1';
                 Console.Clear();
                 Console.WriteLine("Ingresa una opción válida.");
                 Console.WriteLine("Presiona 'enter' para volver al menu...");
                 while(Console.ReadKey().Key != ConsoleKey.Enter){}
                 break;
    }
    }while(option != '0');
}

void CreateUser(){
    Console.Clear();
    Console.WriteLine("Ingresa la información del usuario:\n");

    int option = 0;
    var listUsers = Storage.GetAllUsers();
    int ID;
    
    do{
        do{
            Console.Write("ID: ");
            if(int.TryParse(Console.ReadLine(), out ID))
                if(ID > 0)
                    option = 1;
            if(option == 0)
                Console.WriteLine("El ID debe ser un número mayor que 0. Ingresa nuevamente el ID.");
        }while(option == 0);
        foreach(User user in listUsers)
            if(user.GetID() == ID){
                Console.WriteLine("El ID ya existe. Ingresa otro ID.");
                option = 0;
                break;
            }
    }while(option == 0);    

    Console.Write("Nombre: ");
    string name = Console.ReadLine();

    option = 0;
    string email;

    do{
        Console.Write("Email: ");
        email = Console.ReadLine();
        if(!Regex.IsMatch(email, @"^[^\s@]+@[^\s@]+\.[^\s@]+$"))
            Console.WriteLine("El email es inválido. Ingresa nuevamente el email.");
        else
            option = 1;
    }while(option == 0);
    
    option = 0;
    decimal balance;

    do{
        Console.Write("Saldo: ");
        if(decimal.TryParse(Console.ReadLine(), out balance))
            if(balance > 0)
                option = 1;
        if(option == 0)
            Console.WriteLine("El Saldo debe ser un número positivo y mayor que 0. Ingresa nuevamente el Saldo.");
    }while(option == 0);
    
    option = 0;
    char userType;

    do{
        Console.Write("Escribe 'c' si el usuario es Cliente, 'e' si es Empleado: ");
        userType = Console.ReadKey().KeyChar;
        if(userType != 'c' && userType != 'e')
            Console.WriteLine("\nSolo se puede ingresar 'c' o 'e'. Ingresa nuevamente el tipo de usuario.");
        else
            option = 1;
    }while(option == 0);

    User newUser;

    if(userType.Equals('c')){
        Console.Write("\nRegimen Fiscal: ");
        char taxRegime = Console.ReadKey().KeyChar;
        Console.Write("\n");
        newUser = new Client(ID, name, email, balance, taxRegime);
    } else {
        Console.Write("\nDepartamento: ");
        string department = Console.ReadLine();

        newUser = new Employee(ID, name, email, balance, department);
    }

    Storage.AddUser(newUser);

    Console.WriteLine("Usuario creado.");
    Console.WriteLine("Presiona 'enter' para volver al menu...");
    while(Console.ReadKey().Key != ConsoleKey.Enter){}
}

void DeleteUser(){

    int opcion = 0;
    var listUsers = Storage.GetAllUsers();

    if(listUsers.Count == 0){
        Console.Clear();
        Console.WriteLine("No hay usuarios existentes.");
        Console.WriteLine("Presiona 'enter' para volver al menu...");
        while(Console.ReadKey().Key != ConsoleKey.Enter){}
    } else
        do{
            int ID;
            do{
                Console.Clear();
                Console.Write("Ingresa el ID del usuario a eliminar: ");
                if(!int.TryParse(Console.ReadLine(), out ID)){
                    Console.Clear();
                    Console.WriteLine("El ID debe ser un número. Ingresa nuevamente el ID.");
                    Console.WriteLine("Presiona 'enter' para continuar...");
                    while(Console.ReadKey().Key != ConsoleKey.Enter){}
                } else
                    opcion = 1;
            }while(opcion == 0);

            Console.Clear();

            opcion = 0;
            string result = Storage.DeleteUser(ID);

            if(result.Equals("Success")){
                opcion = 1;
                Console.WriteLine("Usuario eliminado.");
                Console.WriteLine("Presiona 'enter' para volver al menu...");
                while(Console.ReadKey().Key != ConsoleKey.Enter){}
            }
            else{
                Console.WriteLine("El usuario no existe. Ingresa nuevamente el ID.");
                Console.WriteLine("Presiona 'enter' para continuar...");
                while(Console.ReadKey().Key != ConsoleKey.Enter){}
            }
        }while(opcion == 0);
}

/*
Client james = new Client(1, "James", "james@gmail.com", 4000, 'M');

james.SetBalance(2000);

Console.WriteLine(james.ShowData());

Employee pedro = new Employee(2, "Pedro", "pedro@gmail.com", 4000, "IT");

pedro.SetBalance(2000);

Console.WriteLine(pedro.ShowData());

Storage.AddUser(james);
Storage.AddUser(pedro);
*/
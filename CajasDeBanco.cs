Console.Clear();
char op;
int[] retiros = new int[10];
int cantidadRetiros = 0;
do{
    Console.WriteLine("------------------------Banco CDIS:------------------------");
    Console.WriteLine("1. Ingresar la cantidad de retiros hechos por los usuarios.");
    Console.WriteLine("2. Revisar la cantidad entregada de billetes y monedas.");
    Console.WriteLine("3. Salir del programa.\n");
    Console.WriteLine("Ingresa la opción:");
    op = Console.ReadKey().KeyChar;
    switch (op){
        case '1':
            ingresarRetiros(retiros, ref cantidadRetiros);
            break;
        case '2':
            entregarBilletes(retiros, cantidadRetiros);
            break;
        case '3':
            break;
        default:
            Console.Clear();
            Console.WriteLine("La opción ingresada no es válida, intenta con otra opción.");
            Console.WriteLine("Presiona 'enter' para continuar ...");
            while(Console.ReadKey().Key != ConsoleKey.Enter){}
            break;
    }
    Console.Clear();
}while(op != '3');
Console.WriteLine("Programa terminado con éxito!");
Console.WriteLine("Presiona 'enter' para salir ...");
while(Console.ReadKey().Key != ConsoleKey.Enter){}
Console.Clear();

void ingresarRetiros(int[] retiros, ref int cantidadRetiros){
    op = '0';
    while(op == '0'){
    Console.Clear();
    Console.WriteLine("¿Cuantos retiros se hicieron (máximo 10)?");
    if(int.TryParse(Console.ReadLine(), out cantidadRetiros))
        if(cantidadRetiros > 0 && cantidadRetiros <= 10)
            op = '1';
        if(op == '0'){
            Console.Clear();
            Console.WriteLine("La cantidad de retiros es inválida, intenta con otra cantidad.");
            Console.WriteLine("Presiona 'enter' para continuar ...");
            while(Console.ReadKey().Key != ConsoleKey.Enter){}
        }
    }
    for(int i = 0; i < cantidadRetiros; i++){
        op = '0';
        do{
            Console.WriteLine($"\nIngresa la cantidad del retiro #{i+1}:");
            if(int.TryParse(Console.ReadLine(), out retiros[i]))
                if(retiros[i] > 0 && retiros[i] <= 50000)
                    op = '1';
            if(op == '0'){
                Console.WriteLine("\nLa cantidad del retiro es inválida, intenta con otra cantidad.");
                Console.WriteLine("Presiona 'enter' para continuar ...");
                while(Console.ReadKey().Key != ConsoleKey.Enter){}
            }
        }while(op == '0');
    }
}

void entregarBilletes(int[] retiros, int cantidadRetiros){
    Console.Clear();
    for(int i = 0; i < cantidadRetiros; i++){
        int retiro = retiros[i];
        int billetes = 0;
        int monedas = 0;
        while(retiro >= 1){
            while(retiro >= 5){
                while(retiro >= 10){
                    while(retiro >= 20){
                        while(retiro >= 50){
                            while(retiro >= 100){
                                while(retiro >= 200){
                                    while(retiro >= 500){
                                        billetes++;;
                                        retiro-=500;
                                    }
                                    if(retiro >= 200){
                                        billetes++;;
                                        retiro-=200;
                                    }
                                }
                                if(retiro >= 100){
                                    billetes++;;
                                    retiro-=100;
                                }
                            }
                            if(retiro >= 50){
                                billetes++;;
                                retiro-=50;
                            }
                        }
                        if(retiro >= 20){
                            billetes++;;
                            retiro-=20;
                        }
                    }
                    if(retiro >= 10){
                        monedas++;;
                        retiro-=10;
                    }
                }
                if(retiro >= 5){
                    monedas++;;
                    retiro-=5;
                }
            }
            if(retiro >= 1){
                monedas++;;
                retiro-=1;
            }
        }
        Console.WriteLine($"Retiro #{i+1}:");
        Console.WriteLine($"Billetes entregados: {billetes}");
        Console.WriteLine($"Monedas entregadas: {monedas}\n");
    }
    Console.WriteLine("Presiona 'enter' para continuar ...");
    while(Console.ReadKey().Key != ConsoleKey.Enter){}
}
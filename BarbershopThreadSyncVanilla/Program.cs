using System;
using System.Threading;
class Program
{
    // Create a Random Number Generator
    static Random Rand = new Random();
    // Define the maximum number of customers and the maximum number of chairs.
    const int MaxCustomers = 25;
    const int NumChairs = 3;
    static Semaphore waitingRoom = new Semaphore(NumChairs, NumChairs);
    static Mutex barberReady = new Mutex();
    static Mutex custReady = new Mutex();
    static Mutex accesWR = new Mutex();
    static int currentCount = 0;
    
    static void Barber()
    {
        while (true)
        {
            Console.WriteLine("Б: Спит...\n");
            accesWR.WaitOne();
            custReady.WaitOne();
            barberReady.WaitOne();
            accesWR.ReleaseMutex();
            // Cutting hair for a random amount of time.
            Console.WriteLine("Б: Делает стрижку...\n");
            Thread.Sleep(Rand.Next(1, 3) * 1000);
            Console.WriteLine("Б: Закончил делать стрижку\n");
            custReady.ReleaseMutex();
            barberReady.ReleaseMutex();
            Thread.Sleep(3000);
        }        
    }
    static void Customer(Object number)
    {
        int Number = (int)number;
        Console.WriteLine("К{0}: Заходит в парикмахерскую...\n", Number);
        Thread.Sleep(Rand.Next(1, 5) * 1000);
        accesWR.WaitOne();
        Console.WriteLine("К{0}: Заходит в комнату ожидания...\n", Number);
        if (currentCount > 0)
        {
            waitingRoom.WaitOne();
            Console.WriteLine("К{0}: Занимает стул в комнате ожидания...\n", Number);
            currentCount++;
            accesWR.ReleaseMutex();            
            barberReady.WaitOne();
            custReady.WaitOne();
            accesWR.WaitOne();
            currentCount--;
            waitingRoom.Release();            
            accesWR.ReleaseMutex();            
        }
        else
        {            
            barberReady.WaitOne();
            custReady.WaitOne();            
            Console.WriteLine("К{0}: Будит парикмахера...\n", Number);            
        }
        barberReady.ReleaseMutex();
        custReady.ReleaseMutex();
        Console.WriteLine("К{0}: Покидает парикмахерскую...\n", Number);
    }
    static void Main()
    {
        Thread BarberThread = new Thread(Barber);
        BarberThread.Start();
        Thread[] Customers = new Thread[MaxCustomers];
        for (int i = 0; i < MaxCustomers; i++)
        {
            Customers[i] = new Thread(new ParameterizedThreadStart(Customer));
            Customers[i].Start(i);
            Thread.Sleep(3000);
        }
        Thread.Sleep(3000);
        for (int i = 0; i < MaxCustomers; i++)
        {
            Customers[i].Join();
        }        
        // Wait for the Barber's thread to finish before exiting.
        BarberThread.Join();
        Console.WriteLine("End of demonstration. Thanks for watching. This should always be the last line displayed.");
    }
}

        //static void Barber()
        //{
        //    while (true)
        //    {
        //        _custReady.WaitOne();
        //        Console.WriteLine("B: Зовет следующего клиента...");
        //        _accessWRSeats.WaitOne();
        //        numberOfFreeWRSeats += 1;
        //        _barberReady.ReleaseMutex();
        //        Console.WriteLine("B: Начинает свою работу...");
        //        _accessWRSeats.ReleaseMutex();
        //        Thread.Sleep(5000);
        //    }
        //}

        //static void Client(int i)
        //{
        //    Console.WriteLine("C{0}: Заходит в парикмахерскую...", i);
        //    _accessWRSeats.WaitOne();

        //    if (numberOfFreeWRSeats > 0)
        //    {
        //        Console.WriteLine("C{0}: Занимает стул в комнате ожидания...", i);
        //        numberOfFreeWRSeats -= 1;
        //        _custReady.Release();
        //        _accessWRSeats.ReleaseMutex();

        //        _barberReady.WaitOne();
        //        Console.WriteLine("C{0}: Подстриженный идет домой...", i);
        //    }
        //    else
        //    {
        //        Console.WriteLine("C{0}: Идет домой, т.к. нет свободных мест...", i);
        //        _accessWRSeats.ReleaseMutex();

        //    }
        //}
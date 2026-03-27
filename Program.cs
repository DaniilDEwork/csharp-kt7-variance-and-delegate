using System;
using System.Collections.Generic;

interface IConverter<in T, out U>
{
    U Convert(T value);
}

class StringToIntConverter : IConverter<string, int>
{
    public int Convert(string value)
    {
        return int.Parse(value);
    }
}

class ObjectToStringConverter : IConverter<object, string>
{
    public string Convert(object value)
    {
        if (value == null)
            return "";

        return value.ToString();
    }
}

class AnimalToStringConverter : IConverter<Animal, string>
{
    public string Convert(Animal value)
    {
        return value.Name;
    }
}

abstract class Animal
{
    public string Name { get; set; }

    public Animal(string name)
    {
        Name = name;
    }

    public abstract void SayHello();
}

class Dog : Animal
{
    public Dog(string name) : base(name)
    {
    }

    public override void SayHello()
    {
        Console.WriteLine("Привет, я собака " + Name);
    }
}

class Cat : Animal
{
    public Cat(string name) : base(name)
    {
    }

    public override void SayHello()
    {
        Console.WriteLine("Привет, я кошка " + Name);
    }
}

class Calculator
{
    public static double Add(double x, double y)
    {
        return x + y;
    }

    public static double Subtract(double x, double y)
    {
        return x - y;
    }

    public static double Multiply(double x, double y)
    {
        return x * y;
    }

    public static double Divide(double x, double y)
    {
        return x / y;
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("1. IConverter<T, U>");
        string[] numbers = { "10", "20", "30" };
        StringToIntConverter intConverter = new StringToIntConverter();
        int[] intResult = ConvertArray(numbers, intConverter);

        Console.WriteLine("StringToIntConverter:");
        PrintArray(intResult);

        object[] values = { 123, "hello", 45.6 };
        ObjectToStringConverter stringConverter = new ObjectToStringConverter();
        string[] stringResult = ConvertArray(values, stringConverter);

        Console.WriteLine("ObjectToStringConverter:");
        PrintArray(stringResult);

        Console.WriteLine("Ковариантность и контравариантность:");
        IConverter<string, object> converter1 = new ObjectToStringConverter();
        string[] words = { "one", "two", "three" };
        object[] wordsResult = ConvertArray(words, converter1);
        PrintArray(wordsResult);

        IConverter<Dog, object> converter2 = new AnimalToStringConverter();
        Dog[] dogs = { new Dog("Шарик"), new Dog("Бобик") };
        object[] dogNames = ConvertArray(dogs, converter2);
        PrintArray(dogNames);

        Console.WriteLine();
        Console.WriteLine("2. Animal и Action<Animal>");

        List<Animal> animals = new List<Animal>();

        Func<Animal> animal1 = CreateDog;
        Func<Animal> animal2 = CreateCat;

        animals.Add(animal1());
        animals.Add(animal2());
        animals.Add(new Dog("Рекс"));
        animals.Add(new Cat("Мурка"));

        Console.WriteLine("Обычный Action<Animal>:");
        ProcessAnimals(animals, SayHelloAnimal);

        Console.WriteLine("Контравариантность Action<Animal>:");
        Action<Animal> action = PrintObject;
        ProcessAnimals(animals, action);

        Console.WriteLine();
        Console.WriteLine("3. Calculator и Func<double, double, double>");
        Console.WriteLine("10 + 5 = " + Calculate(10, 5, Calculator.Add));
        Console.WriteLine("10 - 5 = " + Calculate(10, 5, Calculator.Subtract));
        Console.WriteLine("10 * 5 = " + Calculate(10, 5, Calculator.Multiply));
        Console.WriteLine("10 / 5 = " + Calculate(10, 5, Calculator.Divide));

        Console.ReadLine();
    }

    static U[] ConvertArray<T, U>(T[] values, IConverter<T, U> converter)
    {
        U[] result = new U[values.Length];

        for (int i = 0; i < values.Length; i++)
        {
            result[i] = converter.Convert(values[i]);
        }

        return result;
    }

    static void ProcessAnimals(List<Animal> animals, Action<Animal> action)
    {
        for (int i = 0; i < animals.Count; i++)
        {
            action(animals[i]);
        }
    }

    static double Calculate(double x, double y, Func<double, double, double> operation)
    {
        return operation(x, y);
    }

    static void PrintArray<T>(T[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            Console.WriteLine(array[i]);
        }

        Console.WriteLine();
    }

    static Dog CreateDog()
    {
        return new Dog("Тузик");
    }

    static Cat CreateCat()
    {
        return new Cat("Барсик");
    }

    static void SayHelloAnimal(Animal animal)
    {
        animal.SayHello();
    }

    static void PrintObject(object obj)
    {
        Animal animal = (Animal)obj;
        Console.WriteLine("Объект: " + animal.Name);
    }
}